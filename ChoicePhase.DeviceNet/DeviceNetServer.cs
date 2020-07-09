using System;
using System.Collections.Generic;
using System.Threading;
using ChoicePhase.DeviceNet.Element;
using ChoicePhase.DeviceNet.LogicApplyer;

namespace ChoicePhase.DeviceNet
{

    /// <summary>
    /// DeviceNetServer
    /// </summary>
    public class DeviceNetServer
    {
        // private CodeDictionary _codeDictionary;

       /// <summary>
       /// 逻辑地址 MAC
       /// </summary>
        public byte LocalMac
        {
            get;
            set;
        }

        /// <summary>
        /// 子站站点信息列表
        /// </summary>
        private List<DefStationInformation> StationInformation;

        /// <summary>
        /// 标识符，索引字典
        /// </summary>
        private CodeDictionary _codeDictionary;

        /// <summary>
        /// 站点连接更新事件
        /// </summary>
        public event EventHandler<StationEventArgs> StationArrived;

        /// <summary>
        /// 主线程，用于循环发送建立连接的信息
        /// </summary>
        private Thread ThreadWork;

        /// <summary>
        /// 发送委托
        /// </summary>
        Func<byte[], bool> SendDelegate;

       /// <summary>
       /// 主站轮询，从站应答服务。 IO报文 GROUP1_POLL_STATUS_CYCLER_ACK
       /// </summary>
        public StationPollingService PollingService;


        /// <summary>
        /// 是否为连接激活状态
        /// </summary>
        public bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// 异常处理委托
        /// </summary>
        private Action<Exception> ExceptionDelegate;
        public DeviceNetServer(Func<byte[], bool> sendDelegate, Action<Exception> exceptionDelegate, List<DefStationInformation> stationList)
        {
            LocalMac = 0x02;

            StationInformation = stationList;

            ExceptionDelegate = exceptionDelegate;
            PollingService = new StationPollingService(ExceptionDelegate);


            List<Identifier> groupList = new List<Identifier>();
            groupList.Add(new Identifier("GROUP1_STATUS_CYCLE_ACK", 13));
            groupList.Add(new Identifier("GROUP1_BIT_STROKE", 14));
            groupList.Add(new Identifier("GROUP1_POLL_STATUS_CYCLER_ACK", 15));

            groupList.Add(new Identifier("GROUP2_BIT_STROBE", 0));
            groupList.Add(new Identifier("GROUP2_POLL", 1));
            groupList.Add(new Identifier("GROUP2_STATUS_POLL", 2));
            groupList.Add(new Identifier("GROUP2_VISIBLE_UCN", 3));

            groupList.Add(new Identifier("GROUP2_VSILBLE", 4));
            groupList.Add(new Identifier("GROUP2_POLL_STATUS_CYCLE", 5));
            groupList.Add(new Identifier("GROUP2_VSILBLE_ONLY2", 6));
            groupList.Add(new Identifier("GROUP2_REPEAT_MACID", 7));

           var serverCode = new List<Identifier>();
           serverCode.Add(new Identifier("SVC_AllOCATE_MASTER_SlAVE_CONNECTION_SET", 0x4B));
           serverCode.Add(new Identifier("SVC_RELEASE_GROUP2_IDENTIFIER_SET", 0x4C));
          
           serverCode.Add(new Identifier("SVC_ERROR_RESPONSE", 0x14));
           serverCode.Add(new Identifier("SVC_SET_ATTRIBUTE_SINGLE", 0x10));
           serverCode.Add(new Identifier("SVC_GET_ATTRIBUTE_SINGLE", 0x0E));

            //初始化
           var errorCode = new List<Identifier>();
            _codeDictionary = new CodeDictionary(groupList,  serverCode, errorCode);

           SendDelegate = sendDelegate;

           ThreadWork = new Thread(MainWorkThread);
           ThreadWork.Name = "MainWorkThread";
           ThreadWork.Priority = ThreadPriority.Normal;
           //ThreadWork.Start();
        }

        /// <summary>
        /// 停止连接轮询服务
        /// </summary>
        public void StopLinkServer()
        {
            foreach (var m in StationInformation)
            {
                m.Enable = false;
                m.State = 0;
                m.OldState = 0;
                m.Online = false;
                m.Complete = false;
                m.Step = NetStep.Start;
            }
            IsActive = false;

        }

        /// <summary>
        /// 重启连接服务
        /// </summary>
        public void RestartLinkServer()
        {
            foreach (var m in StationInformation)
            {                
                m.State = 0;
                m.OldState = 0;
                m.Online = false;
                m.Complete = false;
                m.Step = NetStep.Start;
                m.Enable = m.InitEnable;//避免多次使能启动
            }
            IsActive = true;
        }

        /// <summary>
        /// 关闭DeviceNet服务
        /// </summary>
        public void Close()
        {
            if (ThreadWork.IsAlive)
            {
                ThreadWork.Join(100);
                ThreadWork.Abort();
            }
        }

        /// <summary>
        /// 发送CAN报文
        /// </summary>
        /// <param name="can">CAN</param>
        public void SendData(CanMessage can)
        {
            var data = GetCanData(can);
            SendDelegate(data);
        }

        /// <summary>
        /// 获取CAN数据
        /// </summary>
        /// <param name="can">CAN消息</param>
        /// <returns>对应的字节数据</returns>
        private byte[] GetCanData(CanMessage can)
        {
            var data = new byte[can.DataLen + 2];
            data[0] = (byte)can.ID;
            data[1] = (byte)(can.ID >> 8);
            Array.Copy(can.Data, 0, data, 2, can.DataLen);
            return data;
        }

        /// <summary>
        /// 生成CAN UnconnectVisibleRequestMessageOnlyGroup2 报文, 现主要用于建立连接使用。
        /// </summary>
        /// <param name="destMAC">目的地址</param>
        /// <param name="serverCode">服务代码</param>
        /// <param name="config">配置号</param>
        /// <returns>CanMessage</returns>
        public CanMessage MakeUnconnectVisibleRequestMessageOnlyGroup2(byte destMAC, string server, byte config)
        {            
            var id = DeviceNetID.MakeGroupIDTwo((byte)CodeDictionary.GroupFunctionCode["GROUP2_VSILBLE_ONLY2"], destMAC);
            byte[] data = new byte[6];
            data[0] = (byte)(LocalMac & 0x3F);
            data[1] = (byte)CodeDictionary.ServerCode[server];
            data[2] = 3;
            data[3] = 1;
            data[4] = config;
            data[5] = (byte)(LocalMac & 0x3F);
            CanMessage can = new CanMessage(id, data, 0, data.Length);
            return can;
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="connectType">连接类型</param>
        public void EstablishConnection(DefStationInformation station, byte connectType)
        {
            try
            {
                var can = MakeUnconnectVisibleRequestMessageOnlyGroup2(station.MacID,
                "SVC_AllOCATE_MASTER_SlAVE_CONNECTION_SET", connectType);
                station.SendMessage = can;
                station.WaitFlag = true;
                SendData(can);
            }
            catch (Exception ex)
            {
                Common.LogTrace.CLog.LogError(ex);
                ExceptionDelegate(ex);
            }

        }

        /// <summary>
        /// 主站发送命令或者数据，通过主站轮询命令，自定义协议
        /// </summary>
        /// <param name="destMAC">目的地址</param>
        /// <param name="data">数据</param>
        /// <param name="start">其实索引</param>
        /// <param name="len">长度</param>
        public void MasterSendCommand(byte destMAC, byte[] data, int start, int len)
        {
            var can = MakeIOMessage(destMAC, data, start, len);            
            SendData(can);
        }




        /// <summary>
        /// 生成主站IO报文(GROUP2_POLL_STATUS_CYCLE)消息
        /// </summary>
        /// <param name="destMAC"> 目的MAC地址</param>
        /// <param name="data"> 数据指针</param>
        /// <param name="start">开始索引</param>
        /// <param name="len">数据长度</param>
        /// <returns>生成的CAN消息</returns>
        public CanMessage MakeIOMessage(byte destMAC, byte[] data, int start, int len)
        {
            if (len <= 8)
            {
                var id = DeviceNetID.MakeGroupIDTwo((byte)CodeDictionary.GroupFunctionCode["GROUP2_POLL_STATUS_CYCLE"], destMAC);
                CanMessage can = new CanMessage(id, data, start, len);
                return can;
            }
            else
            {
                throw new Exception("MakeIOMessage 超出范围");
            }
        }

        /// <summary>
        /// 建立连接信息
        /// </summary>
        /// <param name="station">站点信息</param>
        public void NormalEstablishConnectionTask(DefStationInformation station)
        {
            try
            {
                if (!station.Enable)
                {
                    return;
                }

                Thread.Sleep((int)station.DelayTime); //自定义的延时时间
                if (station.Complete)
                {
                    station.OverTimeCount = 0;
                }
                else
                {
                    station.OverTimeCount++;
                }
                station.Complete = false;

                switch (station.Step)
                {
                    case NetStep.Start:                 
                        {
                            byte cmd = (byte)LinkConnectType.VisibleMessage 
                                |  (byte)LinkConnectType.StatusChange | (byte)LinkConnectType.CycleInquiry;
                            EstablishConnection(station, cmd); //建立显示连接                           

                            break;
                        }  
                    case NetStep.Cycle:
                        {
                            break;
                        }
                }              
            }
            catch(Exception ex)
            {
                ExceptionDelegate(ex);
            }

        }
        /// <summary>
        /// 轮询建立状态 
        /// </summary>
        public void MainWorkThread()
        {
            try
            {
                do
                {
                    foreach (var m in StationInformation)
                    {
                        NormalEstablishConnectionTask(m);
                    }
                    Thread.Sleep(100);//保证最小间隔时间。
                } while (true);
            }
            catch (ThreadAbortException ex)
            {
                Thread.ResetAbort();
                Common.LogTrace.CLog.LogWarning("DeviceNet::" + ex.Message);                
            }
            catch (Exception ex)
            {
                Common.LogTrace.CLog.LogError("DeviceNet::" + ex.Message);
                ExceptionDelegate(ex);
            }
           
        }

        /// <summary>
        /// 接收处理中心
        /// </summary>
        /// <param name="message">消息</param>
        public void ReciveCenter(CanMessage message)
        {
            try
            {
                DeviceNetID netID = new DeviceNetID(message.ID);
                var num = netID.GetGroupNumber();
                switch (num)
                {
                    case 1://Group1
                        {

                            GroupOneDeal(message, netID);
                            break;
                        }
                    case 2://Group2
                        {
                            GroupTwoDeal(message, netID);
                            break;
                        }
                   
                }
            }
            catch (Exception ex)
            {
                ExceptionDelegate(ex);
            }
        }
        #region GROUP 接收处理
        /// <summary>
        /// Group1接收处理
        /// </summary>
        /// <param name="message"></param>
        /// <param name="netID"></param>
        public void GroupOneDeal(CanMessage message, DeviceNetID netID)
        {
            var function = netID.GetFunctionCode();
            
            if (CodeDictionary.GroupFunctionCode["GROUP1_POLL_STATUS_CYCLER_ACK"] == function)
            {
                //问答式
                SlaveStationPollingAckService(message, netID);
            }
            else if (CodeDictionary.GroupFunctionCode["GROUP1_STATUS_CYCLE_ACK"] == function)
            {
                //主动上送信息
                SlaveStationStatusChangeService(message, netID);
            }
            else
            {
                throw new Exception("Group1未实现功能码");
            }

        }
        /// <summary>
        /// Group2 接收处理。处理MAC响应，建立连接服务，时间脉冲序列。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="netID"></param>
        public void GroupTwoDeal(CanMessage message, DeviceNetID netID)
        {
            var function = netID.GetFunctionCode();
            if (CodeDictionary.GroupFunctionCode["GROUP2_VISIBLE_UCN"] == function)
            {
                if (message.DataLen >= 2)
                {
                    SlaveStationVisibleMsgService(message, netID);
                }
            }
            else if (CodeDictionary.GroupFunctionCode["GROUP2_REPEAT_MACID"] == function)
            {
                if (netID.GetMAC() == LocalMac)
                {
                    ResponseMACID(0x80); 
                }
                else
                {
                    RestartEstablishLink((byte)netID.GetMAC());
                }
            }
            else if (CodeDictionary.GroupFunctionCode["GROUP2_POLL_STATUS_CYCLE"] == function)
            {
                PollingService.ServerTimeSequence(message.Data,(byte) netID.GetMAC());
            }
            else
            {
                throw new Exception("Group2未实现功能码");
            }

        }

       



        /// <summary>
        /// 通过MAC地址，搜索是否包含所在列表
        /// </summary>
        /// <param name="mac">mac地址</param>
        /// <returns>站点信息，null--表示不存在</returns>
        private DefStationInformation GetStationPoint(byte mac)
        {
            foreach (var m in StationInformation)
            {
                if (m.MacID == mac)
                {
                    return m;
                }
            }
            return null;
        }
        #endregion

        #region 响应MAC重复
        /// <summary>
        /// 响应MAC重复
        /// </summary>
        /// <param name="config"></param>
        private void ResponseMACID(byte config)
        {
            int i = 0;
            var id = DeviceNetID.MakeGroupIDTwo(
                (byte)CodeDictionary.GroupFunctionCode["GROUP2_REPEAT_MACID"], LocalMac);
            var data = new byte[7];
            data[i++] = config;
            data[i++] = 0x01;//临时数据
            data[i++] = 0x02;
            data[i++] = 0x03;
            data[i++] = 0x11;
            data[i++] = 0x12;
            data[i++] = 0x13;
            var can = new CanMessage(id, data, 0, i);
            SendData(can);
        }

        #endregion

        #region 从站循环状态改变服务 从站状态改变服务
        /// <summary>
        /// 主站轮询，从站应答服务
        /// </summary>
        /// <param name="message">CAN消息</param>
        /// <param name="netID">CAN ID标识</param>
        void SlaveStationPollingAckService(CanMessage message, DeviceNetID netID)
        {
            var station = GetStationPoint((byte)netID.GetMAC());

            if (station != null)
            {
                //检测是否已经建立循环连接--用于功能码读取与应答，读写通过此进行。
                if ((station.State & (byte)LinkConnectType.CycleInquiry) == (byte)LinkConnectType.CycleInquiry)
                {
                    PollingService.Server(message.Data, (byte)netID.GetMAC());
                }
            }


        }
        /// <summary>
        /// 从站状态改变服务
        /// </summary>
        /// <param name="message">CAN消息</param>
        /// <param name="netID">CAN ID标识</param>
        void SlaveStationStatusChangeService(CanMessage message, DeviceNetID netID)
        {
            var station = GetStationPoint((byte)netID.GetMAC());

            if (station != null)
            {
                //检测是否已经建立循环连接
                if ((station.State & (byte)LinkConnectType.CycleInquiry) == (byte)LinkConnectType.CycleInquiry)
                {
                    PollingService.StatusChangeServer(message.Data, (byte)netID.GetMAC());
                }
            }


        }
        #endregion

        #region 从站显示信息服务
        /// <summary>
        /// 从站显示信息服务
        /// </summary>
        /// <param name="message">CAN消息</param>
        /// <param name="netID">ID</param>
        void SlaveStationVisibleMsgService(CanMessage message, DeviceNetID netID)
        {
            try
            {
                var station = GetStationPoint((byte)netID.GetMAC());
                if (station == null)
                {
                    //报错
                    return;
                }
                if (!station.WaitFlag)//判断是否为等待
                {
                    return;
                }
                if (netID.GetMAC() != station.MacID) //判断是目的MAC与本机是否一致
                {
                    return;
                }
                if ((message.Data[0] & 0x3F) != LocalMac) //判断是否为同一个源MAC
                {
                    return;
                }               
                //判断服务代码是否是对应的应答代码,
                if ((station.SendMessage.Data[1] | 0x80) != message.Data[1])
                {
                    return;
                }
                //错误响应代码
                if (station.SendMessage.Data[1] == (0x80 |CodeDictionary.ServerCode["SVC_ERROR_RESPONSE"]))
                {
                    return;
                }
                if (message.DataLen > 10) //长度过长
                {
                    return;
                }
                station.ReciveMessage =  message;
                var serverCode = message.Data[1] & 0x7F;
                var ackConfig = message.Data[2];
                if (serverCode == CodeDictionary.ServerCode["SVC_AllOCATE_MASTER_SlAVE_CONNECTION_SET"])
                {

                    byte cmd = (byte)LinkConnectType.CycleInquiry | 
                        (byte)LinkConnectType.StatusChange|(byte)LinkConnectType.VisibleMessage;
                    //建立连接
                    if (ackConfig == cmd)
                    {
                        station.State |= cmd;
                        station.Step = NetStep.Cycle;
                    }                    
                    else
                    {
                        throw new Exception("未识别的建立连接服务");
                    }

                    if (StationArrived != null)
                    {
                        StationArrived(this, new StationEventArgs(station));
                    }
                }
                else if (serverCode == CodeDictionary.ServerCode["SVC_RELEASE_GROUP2_IDENTIFIER_SET"])
                {
                    //配置连接字
                    station.State = (byte)(station.State & (message.Data[5] ^ 0xFF));
                }
                else if (serverCode == CodeDictionary.ServerCode["SVC_GET_ATTRIBUTE_SINGLE"])
                {
                    //设置单个属性
                }
                else if (serverCode == CodeDictionary.ServerCode["SVC_SET_ATTRIBUTE_SINGLE"])
                {

                    //发送设置消息
                }
            }
            catch(Exception ex)
            {
                ExceptionDelegate(ex);
            }
        }
        #endregion 

        /// <summary>
        /// 重新建立连接
        /// </summary>
        /// <param name="mac">MAC地址</param>
        private void RestartEstablishLink(byte mac)
        {
            var station = GetStationPoint(mac);
            if (station != null)
            {
                station.Step = NetStep.Start;
                station.Complete = true;
                station.StartTime = 0;
                station.DelayTime = 300;
                station.EndTime = 0;
                station.OverTimeCount = 0;
                station.Online = false;
                station.State = 0;
            }
        }
    }

    /// <summary>
    /// 站信息
    /// </summary>
    public class StationEventArgs: EventArgs
    {
        public DefStationInformation Station;

        

        public StationEventArgs(DefStationInformation station)
        {
            Station = station;
        }
    }


    /// <summary>
    /// 连接类型
    /// </summary>
    public enum LinkConnectType
    {
        /// <summary>
        /// 状态改变
        /// </summary>
        StatusChange = 0x10,

        /// <summary>
        /// 位选通
        /// </summary>
        BitStroke = 0x04,
        /// <summary>
        /// 位轮询 IO轮询
        /// </summary>
        CycleInquiry = 0x02,
        /// <summary>
        ///  显示信息连接
        /// </summary>
        VisibleMessage = 0x01,


    }

}
