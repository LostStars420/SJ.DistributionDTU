using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using ChoicePhase.DeviceNet;
using ChoicePhase.DeviceNet.Element;
using ChoicePhase.DeviceNet.LogicApplyer;
using TransportProtocol.Comself;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.GetViewData;
using ChoicePhase.PlatformModel.Helper;
using Monitor.DASModel.GetViewData;
using TransportProtocol.DistributionControl;
using System.Net;
using System.Windows;
using System.Net.Sockets;

namespace ChoicePhase.PlatformModel
{

    /// <summary>
    /// PlatformModel服务
    /// </summary>
    public partial class PlatformModelServer
    {
        /// <summary>
        /// 获取监控数据
        /// </summary>
        public MonitorViewData MonitorData
        {
            private set;
            get;
        }
       

        /// <summary>
        /// 逻辑呈现UI
        /// </summary>
        public LogicalPresentation LogicalUI
        {
            private set;
            get;
        }


        /// <summary>
        /// 通讯服务
        /// </summary>
        public CommunicationServer CommServer
        {
            private set;
            get;
        }

        /// <summary>
        ///RTU处理服务
        /// </summary>
        public RtuServer RtuServer
        {
            private set;
            get;
        }

        /// <summary>
        /// DeviceNet服务
        /// </summary>
        public DeviceNetServer ControlNetServer
        {
            private set;
            get;
        }

        /// <summary>
        /// 站点信息列表
        /// </summary>
        public List<DefStationInformation> StationInformation
        {
            get;
            set;
        }


        /// <summary>
        /// 本地地址
        /// </summary>
        private ushort _localAddr;//readonly

        public ushort LocalAddr
        {
            get
            {
                return _localAddr;
            }
            set
            {
                _localAddr = value;
            }
        }

        /// <summary>
        /// 目的地址
        /// </summary>
        private readonly byte _destinateAddr;

        /// <summary>
        /// 本地udp端口
        /// </summary>
        public int UdpLocalPort
        {
            get;
            private set;
        }

        /// <summary>
        /// 远程Udp端口
        /// </summary>
        public  int UdpRemotePort
        {
            get;
            private set;
        }


        /// <summary>
        /// 维护端口Udp端口
        /// </summary>
        public int UdpMaintancePort
        {
            get;
            private set;
        }


        /// <summary>
        /// 常规功能码--用于DeviceNet数据下行，上位机到终端
        /// </summary>
        private readonly byte _downCode;

        /// <summary>
        /// 常规功能码--用于DeviceNet数据上行，终端到上位机
        /// </summary>
        private readonly byte _upCode;

        /// <summary>
        /// 超时定时器用于设备离线状态
        /// </summary>
        private OverTimeTimer _deviceOverTimer;

        /// <summary>
        /// 设备离线超时时间
        /// </summary>
        private int _DeviceOverTime;

        /// <summary>
        /// 任务调度器
        /// </summary>
        private readonly TaskScheduler syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        /// <summary>
        /// 初始化控制平台Model服务
        /// </summary>
        public PlatformModelServer()
        { 
            try
            {
                string path3 = System.IO.Directory.GetCurrentDirectory();
                UdpLocalPort = 5555;
                UdpRemotePort = 5555;
                UdpMaintancePort = 5500; 
                _DeviceOverTime = 7000;
                _destinateAddr = 0x00;
                _downCode = 0x55;
                _upCode = 0xAA;
                _multiFrameBuffer = new List<byte>();
                _lastIndex = 0;

                MonitorData = new MonitorViewData();

                //注释了也没什么关系
                UpadteConfigParameter();

                CommServer = new CommunicationServer(_destinateAddr);

                StationInformation = new List<DefStationInformation>();
                StationInformation.Add(new DefStationInformation(NodeAttribute.MacSynController, ((NodeAttribute.EnabitSelect & 0x01) == 0x01), "同步控制器"));
                StationInformation.Add(new DefStationInformation(NodeAttribute.MacPhaseA, ((NodeAttribute.EnabitSelect & 0x02) == 0x02), "A相"));
                StationInformation.Add(new DefStationInformation(NodeAttribute.MacPhaseB, ((NodeAttribute.EnabitSelect & 0x04) == 0x04), "B相"));
                StationInformation.Add(new DefStationInformation(NodeAttribute.MacPhaseC, ((NodeAttribute.EnabitSelect & 0x08) == 0x08), "C相"));


                ControlNetServer = new DeviceNetServer(PacketDevicetNetData, ExceptionDeal, StationInformation);
                ControlNetServer.PollingService.ArrtributesArrived += PollingService_ArrtributesArrived;
                ControlNetServer.PollingService.MultiFrameArrived += PollingService_MultiFrameArrived;
                ControlNetServer.PollingService.ReadyActionArrived += PollingService_ReadyActionArrived;
                ControlNetServer.PollingService.SubStationStatusChanged += PollingService_SubStationStatusChanged;
                ControlNetServer.PollingService.ErrorAckChanged += PollingService_ErrorAckChanged;
                ControlNetServer.PollingService.NormalStatusArrived += PollingService_NormalStatusArrived;
                ControlNetServer.StationArrived += ControlNetServer_StationArrived;

                LogicalUI = new LogicalPresentation(MonitorData.UpdateStatus);
                _deviceOverTimer = new OverTimeTimer(_DeviceOverTime, DeviceOverTimeDeal);
                
            }
            catch(Exception ex)
            {               
                Common.LogTrace.CLog.LogError(ex.StackTrace);
            }

        }

        /// <summary>
        /// UDP 通讯数据到达
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UdpServer_DataArrived(object sender, Net.UDP.UdpDataEventArgs e)
        {
            RtuServer.AddBuffer(e.DataArray);
            CommServer.RawReciveMessage += ByteToString(e.DataArray, 0, e.DataArray.Length);
        }


        /// <summary>
        /// 其它功能码统一处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollingService_NormalStatusArrived(object sender, StatusChangeMessage e)
        {
            LogicalUI.UpdatePramter(e);
        }

        /// <summary>
        /// 子站主动错误应答事件,信息栏显示主动应答错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollingService_ErrorAckChanged(object sender, StatusChangeMessage e)
        {
            MonitorData.UpadeExceptionMessage(GetErrorComment(e.MAC, e.Data));
        }

        /// <summary>
        /// 更新站点连接信息，建立连接等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlNetServer_StationArrived(object sender, StationEventArgs e)
        {
            Task.Factory.StartNew(() => LogicalUI.StationLinkChanged(e.Station),
                    new System.Threading.CancellationTokenSource().Token, TaskCreationOptions.None, syncContextTaskScheduler).Wait();           
        }
        
        /// <summary>
        /// 从站状态改变信息,循环报告信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollingService_SubStationStatusChanged(object sender, StatusChangeMessage e)
        {
            LogicalUI.UpdateTickMesasge(e.MAC, e.Data);            
        }

        /// <summary>
        /// 预制分闸，合闸，同步设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PollingService_ReadyActionArrived(object sender, StatusChangeMessage e)
        {
            LogicalUI.UpdateNodeReadyActionStatus(e.MAC, e.Data);
        }


     


        /// <summary>
        /// 从站上传属性值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PollingService_ArrtributesArrived(object sender, ArrtributesEventArgs e)
        {
            MonitorData.UpdateAttributeData(e.MAC, e.ID, e.AttributeByte);
        }



        /// <summary>
        /// 发送RTU数据包，并更新到信息栏显示
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>true--正常发送，false--端口关闭</returns>
        private bool SendRtuData(byte[] data)
        {
            if(CommServer.CommonServer.CommState)
            {
                CommServer.CommonServer.SendMessage(data);

                var rawDataStr = ByteToString(data, 0, data.Length);
                CommServer.UpadteSendLinkMessage(rawDataStr);
                CommServer.RawSendMessage += rawDataStr;
            }
            return CommServer.CommonServer.CommState;            
        }
        /// <summary>
        /// 发送UDP RTU数据包，并更新到信息栏显示
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>true--正常发送，false--端口关闭</returns>
        private bool UdpSendRtuData(byte[] data, UInt32 id, int port)
        {
             if (CommServer.UdpServer.IsRun)
             {
                var ip = new IPAddress(new byte[] { (byte)(id >> 24), (byte)(id >> 16), (byte)(id >> 8), (byte)id });
                CommServer.UdpServer.Send(ip, port, data);
                var rawDataStr = ByteToString(data, 0, data.Length);
                CommServer.UpadteSendLinkMessage(rawDataStr);
                CommServer.RawSendMessage += rawDataStr;
             }
            return CommServer.CommonServer.CommState;
        }
        /// <summary>
        /// 打包DevicetNet数据为RTU数据并发送。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool PacketDevicetNetData(byte[] data)
        {
            var frame = new RTUFrame(_destinateAddr, _downCode, data, (byte)data.Length);
            SendRtuData(frame.Frame);
            return true;
        }
        /// <summary>
        /// 数据初步解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RtuServer_FrameRtuArrived(object sender, FrameRtuArgs e)
        {
            try
            {
                var rawStr = ByteToString(e.Rtu.ValidData, 0, e.Rtu.ValidData.Length);
                var str = string.Format("FROM：{0:X}, TO:{1:X}, FUNCODE:{2:X}, ValidData:{3:X}", e.Rtu.Address, e.Rtu.DestAddress, e.Rtu.Funcode, rawStr);
                CommServer.UpadteReciveLinkMessage(str);

                switch ((ControlCode)e.Rtu.Funcode)
                {
                    case ControlCode.LOOP_STATUS:
                    case ControlCode.STATUS_MESSAGE:
                        {
                            MonitorData.UpdateSwitchProperty(e.Rtu.ValidData);
                            break;
                        }
                    case ControlCode.NANOPB_TYPE:
                        {
                            ParseNanopbMessage(e.Rtu.ValidData);
                            break;
                        }
                        
                    default:
                        {
                            break;
                        }

                }
            }
            catch (System.Exception ex)
            {
                ExceptionDeal(ex);
            }
            
        }

        string LogToString(ExceptionRecord record)
        {
            var sb = new StringBuilder();
            var ascii = new ASCIIEncoding();
            var name = ascii.GetString(record.functionName);
            sb.AppendFormat("time:{0}, id: {1}, code:{2}, name:{3}, line:{4}\n", record.time, record.id, record.code, name, record.line);
            return sb.ToString();
        }

        void ParseNanopbMessage(byte[] validData)
        {
            var list = new List<ExceptionRecord>();
            LogRecord log = new LogRecord();
            log.exception = new ExceptionRecord[32];
            switch ((NanopbType)validData[0])
            {
                case NanopbType.STATION_MESSAGE:
                    {
                        int len = validData[1] + validData[2] * 256;
                        var data = new byte[len];
                        Array.Copy(validData, 3, data, 0, data.Length);
                        var message = ProtocolAnylast.DeSerialize(data);
                        MonitorData.UpadteStationObservable(message);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            
        }
        /// <summary>
        /// 通讯数据到达
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CommonServer_SerialDataArrived(object sender, Communication.SerialDataEvent e)
        {
            RtuServer.AddBuffer(e.SerialData);
            CommServer.RawReciveMessage += ByteToString(e.SerialData, 0, e.SerialData.Length);                        
        }
        /// <summary>
        /// 异常消息处理，此处用于显示
        /// </summary>
        /// <param name="ex"></param>
        void ExceptionDeal(Exception ex)
        {         
            MonitorData.UpadeExceptionMessage(ex.Message);            
        }


        public void RTUConnect(IPAddress ip,ushort localaddr)
        {
            CommServer.UdpServerInit(ip, UdpLocalPort, ExceptionDeal);
            CommServer.CommonServer.SerialDataArrived += CommonServer_SerialDataArrived;
            CommServer.UdpServer.DataArrived += UdpServer_DataArrived;

            RtuServer = new RtuServer(localaddr, 500, null, UdpSendRtuData,UdpRemotePort, ExceptionDeal);
            RtuServer.FrameRtuArrived += RtuServer_FrameRtuArrived;
        }

        public void RTUClose()
        {
            CommServer.Close();
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Close()
        {
            CommServer.Close();
            if (RtuServer != null)
            {
                RtuServer.Close();
            }
            if (ControlNetServer != null)
            {
                ControlNetServer.Close();
            }
            Environment.Exit(0);
        }


        private static PlatformModelServer _modelServer; 


        /// <summary>
        /// 获取Model服务
        /// </summary>
        /// <returns>Model Server</returns>
        public static PlatformModelServer GetServer()
        {
            return GetServer(false);
        }


        /// <summary>
        /// 获取Model服务
        /// </summary>
        /// <param name="flag">ture-重新新建， false--采用默认</param>
        /// <returns>Model Server</returns>
        public static PlatformModelServer GetServer(bool flag)
        {
            if((_modelServer == null) || flag)
            {
                _modelServer =  new PlatformModelServer();
                return _modelServer;
            }
            else
            {
                return _modelServer;
            }
        }
    }


    
}
