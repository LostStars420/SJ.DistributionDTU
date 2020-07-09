using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using ChoicePhase.PlatformModel;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.GetViewData;
using ChoicePhase.PlatformModel.Helper;
using TransportProtocol.DistributionControl;

namespace ChoicePhase.ControlPlatform
{
    public class FaultTest
    {
        private LoopTest loopTest;

        private PlatformModelServer modelServer;
        private readonly ushort _downAddress;
        private readonly ushort _localAddress;
        private ushort _destAddress = 0xAA;
        private SwitchPropertyIndex _attributeIndex = SwitchPropertyIndex.SwitchList;
        private SwitchPropertyIndex _faultStatusIndex = SwitchPropertyIndex.FaultStatus;

        private ushort _remoteID = 0;
        private ushort _remotePort = 0;
        Thread thread;

        public FaultTest(PlatformModelServer modelServer)
        {
            this.modelServer = modelServer;
            _downAddress = modelServer.CommServer.DownAddress;
            _localAddress = modelServer.LocalAddr;
            _remoteID = _downAddress;
            _remotePort = (ushort)modelServer.UdpRemotePort;
            OperateCmd = modelServer.MonitorData.OperateCmd;
            loopTest = modelServer.LogicalUI.TestParameter;

            UserData = modelServer.MonitorData.ReadAttribute(_attributeIndex, false);
            FaultStatus = modelServer.MonitorData.ReadFaultStatus(_faultStatusIndex, false);

            allState = new List<Dictionary<uint, uint>>();
            openCloseCmd = new List<string>();

            //获取故障状态
            GetFaultStatus();

            //设置合分闸状态命令
            SetCloseOpenCmd();
        }

        private ObservableCollection<SwitchProperty> _userData;
        /// <summary>
        /// 用户信息数据
        /// </summary>
        public ObservableCollection<SwitchProperty> UserData
        {
            get
            {
                return _userData;
            }
            set
            {
                _userData = value;
            }
        }

        private ObservableCollection<FaultStatus> _faultStatus;
        /// <summary>
        /// 从数据获取的故障信息
        /// </summary>
        public ObservableCollection<FaultStatus> FaultStatus
        {
            get { return _faultStatus; }
            set
            {
                _faultStatus = value;
            }
        }

        private List<Dictionary<uint, uint>> allState;
        /// <summary>
        /// 故障状态信息
        /// </summary>
        public List<Dictionary<uint,uint>> AllState
        {
            get
            {
                return allState;
            }
            set
            {
                allState = value;
            }
        }

        private ObservableCollection<string> _operateCmd;
        /// <summary>
        /// 指令信息
        /// </summary>
        public ObservableCollection<string> OperateCmd
        {
            get
            {
                return _operateCmd;
            }
            set
            {
                _operateCmd = value;
            }
        }

        private List<String> openCloseCmd;

        public List<String> OpenCloseCmd
        {
            get
            {
                return openCloseCmd;
            }
            set
            {
                openCloseCmd = value;
            }
        }

        private ObservableCollection<StationInformation> _userDataTest;
        /// <summary>
        /// 测试合分闸的数据数据
        /// </summary>
        public ObservableCollection<StationInformation> UserDataTest
        {
            get { return _userDataTest; }
            set
            {
                _userDataTest = value;
            }
        }

        /// <summary>
        /// 启动开始测试线程
        /// </summary>
        public void StartThread()
        {
            GetFaultStatus();
            thread = new Thread(new ThreadStart(Test));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 取消测试
        /// </summary>
        public void CancleThread()
        {
            thread.Join(100);
            thread.Abort();
        }

        /// <summary>
        /// 测试故障点
        /// </summary>
        private void Test()
        {
            string setFaultIp = null;
            string cancleFaultIp = null;
            loopTest.UpadeTip("更新监控数据");
            UpdateData();
            UpdateData();
            Thread.Sleep(1000);
            for (int i = 1; i <= allState.Count - 1; i++)
            {
                loopTest.UpadeTip("故障点F" + FaultStatus[i].FaultType);
                int j = GetFaultIp(out setFaultIp, out cancleFaultIp, FaultStatus[i].FaultType);

                loopTest.UpadeTip("取消故障");
                for (int m = 1; m < openCloseCmd.Count; m += 2)
                {
                    OpenOrCloseFault(cancleFaultIp, openCloseCmd[m]);
                } 
                Thread.Sleep(100);

                //复归
                Dictionary<uint, uint> result = null;
                do
                {
                    Revert();
                    Thread.Sleep(1000);
                    UpdateData();
                    Thread.Sleep(1000);
                    result = CheckStatus(allState[0]);
                    if (result.Count != 0)
                    {
                        uint resultValue;
                        foreach (uint key in result.Keys)
                        {
                            result.TryGetValue(key, out resultValue);
                            loopTest.UpadeTip(key + ": " + resultValue);
                        }
                        loopTest.UpadeTip("故障隔离前状态：" + false);
                    }
                    else
                    {
                        loopTest.UpadeTip("故障隔离前状态：" + true);
                    }
                } while (result.Count != 0);
                Thread.Sleep(100);
                Thread.Sleep(30000);
                
                String[] ipCollect = new string[5];
                int k = 0;
                foreach (var key in allState[i].Keys)
                {
                    uint state;
                    var isGetSuccess = allState[i].TryGetValue(key, out state);
                    if (isGetSuccess && state.ToString().Contains("3"))
                    {
                        var currentIP = key.ToString();
                        currentIP = currentIP.Substring(8);
                        if (currentIP[0].Equals('8'))
                        {
                            loopTest.UpadeTip("设置拒动K" + currentIP[1]);
                            currentIP = "192.168.10." + currentIP[1];
                        }
                        if (currentIP[0].Equals('9'))
                        {
                            loopTest.UpadeTip("设置拒动K1" + currentIP[1]);
                            currentIP = "192.168.10." + "1" + currentIP[1];
                        }
                        SetRefuseAction(currentIP, 4);
                        ipCollect[k++] = currentIP;
                    }
                    Thread.Sleep(1000);
                }
                   
                loopTest.UpadeTip("故障点投入");
                //故障点投入
                OpenOrCloseFault(setFaultIp, openCloseCmd[j - 2]);
                Thread.Sleep(2000);
                loopTest.UpadeTip("取消故障");
                OpenOrCloseFault(cancleFaultIp, openCloseCmd[j - 1]);
                //检查故障投入后的和分闸状态
                Thread.Sleep(20000);
                UpdateData();
                Thread.Sleep(1000);

                var behindResult = CheckStatus(allState[i]);
                if (behindResult.Count == 0)
                {
                    loopTest.UpadeTip("动作后：状态符合要求");
                }
                else
                { 
                    uint resultValue;
                    foreach (uint key in behindResult.Keys)
                    {
                        behindResult.TryGetValue(key, out resultValue);
                        loopTest.UpadeTip(key + ": " + resultValue);
                    }
                    loopTest.UpadeTip("动作后：不符合");
                }
                Thread.Sleep(1000);
                loopTest.UpadeTip("取消拒动");
                foreach(var currentIp in ipCollect)
                {
                    if(currentIp != null)
                    {
                        SetRefuseAction(currentIp, 5);
                    }
                }
                Thread.Sleep(1000);
                //}             
            }
        }


        /// <summary>
        /// 设置合分闸状态命令
        /// </summary>
        private void SetCloseOpenCmd()
        {
            for (int i = 1; i < 7; i++)
            {
                openCloseCmd.Add("K" + i + "合闸");
                openCloseCmd.Add("K" + i + "分闸");
            }
        }

        /// <summary>
        /// 获取故障状态
        /// </summary>
        private void GetFaultStatus()
        {
            FaultStatus = modelServer.MonitorData.ReadFaultStatus(_faultStatusIndex, false);
            allState.RemoveRange(0, allState.Count);
            for (int i = 0; i < FaultStatus.Count; i++)
            {
                var currFaultStatus = new Dictionary<uint, uint>();
                currFaultStatus.Add((uint)0xC0A80A01, (uint)FaultStatus[i].K1);
                currFaultStatus.Add((uint)0xC0A80A02, (uint)FaultStatus[i].K2);
                currFaultStatus.Add((uint)0xC0A80A03, (uint)FaultStatus[i].K3);
                currFaultStatus.Add((uint)0xC0A80A04, (uint)FaultStatus[i].K4);
                currFaultStatus.Add((uint)0xC0A80A05, (uint)FaultStatus[i].K5);
                currFaultStatus.Add((uint)0xC0A80A06, (uint)FaultStatus[i].K6);
                currFaultStatus.Add((uint)0xC0A80A07, (uint)FaultStatus[i].K7);
                currFaultStatus.Add((uint)0xC0A80A08, (uint)FaultStatus[i].K8);

                currFaultStatus.Add((uint)0xC0A80A09, (uint)FaultStatus[i].K9);
                currFaultStatus.Add((uint)0xC0A80A0A, (uint)FaultStatus[i].K10);
                currFaultStatus.Add((uint)0xC0A80A0B, (uint)FaultStatus[i].K11);
                currFaultStatus.Add((uint)0xC0A80A0C, (uint)FaultStatus[i].K12);
                currFaultStatus.Add((uint)0xC0A80A0D, (uint)FaultStatus[i].K13);
                currFaultStatus.Add((uint)0xC0A80A0E, (uint)FaultStatus[i].K14);
                currFaultStatus.Add((uint)0xC0A80A0F, (uint)FaultStatus[i].K15);
                currFaultStatus.Add((uint)0xC0A80A10, (uint)FaultStatus[i].K16);

                currFaultStatus.Add((uint)0xC0A80A11, (uint)FaultStatus[i].K17);
                currFaultStatus.Add((uint)0xC0A80A12, (uint)FaultStatus[i].K18);
                currFaultStatus.Add((uint)0xC0A80A13, (uint)FaultStatus[i].K19);
                currFaultStatus.Add((uint)0xC0A80A14, (uint)FaultStatus[i].K20);
                currFaultStatus.Add((uint)0xC0A80A15, (uint)FaultStatus[i].K21);
                currFaultStatus.Add((uint)0xC0A80A16, (uint)FaultStatus[i].K22);
                currFaultStatus.Add((uint)0xC0A80A17, (uint)FaultStatus[i].K23);
                currFaultStatus.Add((uint)0xC0A80A18, (uint)FaultStatus[i].K24);

                currFaultStatus.Add((uint)0xC0A80A19, (uint)FaultStatus[i].K25);
                currFaultStatus.Add((uint)0xC0A80A1A, (uint)FaultStatus[i].K26);
                currFaultStatus.Add((uint)0xC0A80A1B, (uint)FaultStatus[i].K27);
                currFaultStatus.Add((uint)0xC0A80A1C, (uint)FaultStatus[i].K28);
                currFaultStatus.Add((uint)0xC0A80A1D, (uint)FaultStatus[i].K29);
                currFaultStatus.Add((uint)0xC0A80A1E, (uint)FaultStatus[i].K30);
                currFaultStatus.Add((uint)0xC0A80A1F, (uint)FaultStatus[i].K31);
                currFaultStatus.Add((uint)0xC0A80A20, (uint)FaultStatus[i].K32);
                var count = currFaultStatus.Count;
                for(int j = FaultStatus[i].ValidRow; j < count; j++)
                {
                    currFaultStatus.Remove((uint)(0xC0A80A00 +j +1));
                }
                allState.Add(currFaultStatus);
            }
        }


        private int GetFaultIp(out string setFaultIp, out string cancleFaultIp, int index)
        {
            int j = 0;
            setFaultIp = null;
            cancleFaultIp = null;
            if (index >= 1 && index <= 6)
            {
                int tmp = index;
                setFaultIp = "192.168.10.91";
                cancleFaultIp = "192.168.10.91";
                j = 2 * tmp;
            }
            else
            {
                if (index >= 7 && index <= 12)
                {
                    int tmp = index - 6;
                    setFaultIp = "192.168.10.92";
                    cancleFaultIp = "192.168.10.92";
                    j = 2 * tmp;
                }
                else
                {
                    if (index >= 13 && index <= 18)
                    {
                        int tmp = index - 12;
                        setFaultIp = "192.168.10.93";
                        cancleFaultIp = "192.168.10.93";
                        j = 2 * tmp;
                    }
                }
            }
            return j;
        }

        /// <summary>
        /// 故障投入或退出
        /// </summary>
        /// <param name="ipStr">故障投入的IP地址</param>
        /// <param name="cmd">指令名称：合闸或者分闸</param>
        private void OpenOrCloseFault(string ipStr, string cmd)
        {
            byte value;
            bool result = modelServer.MonitorData.OperateCollect.TryGetValue(cmd, out value);
            if ((!result))
            {
                return;
            }
            int offset = 0;
            byte[] cmdData = new byte[1 * 5 + 1];
            cmdData[offset++] = (byte)1;

            byte[] ip = new byte[] { 192, 168, 10, 1 };
            TypeConvert.IpTryToInt(ipStr, out ip);
            Array.Reverse(ip);
            Array.Copy(ip, 0, cmdData, offset, 4);
            offset += 4;
            cmdData[offset++] = value;

            var frame = new TransportProtocol.Comself.RTUFrame(_localAddress, _destAddress,
              (byte)ControlCode.SWITCH_OPERATE, cmdData, (ushort)cmdData.Length);
            ip[0] = 0xFF;//广播
            var adress = new IPAddress(ip);
            modelServer.RtuServer.SendFrame(frame, (uint)adress.Address, _remotePort);
        }

        /// <summary>
        /// 故障复归
        /// </summary>
        private void Revert()
        {
            loopTest.UpadeTip("复归");
            SendRevertCommand();
            Thread.Sleep(1000);
            UpdateData();
            Thread.Sleep(15000);
            StatusRevert();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// 发送复归命令
        /// </summary>
        private void SendRevertCommand()
        {
            byte value;
            bool result = modelServer.MonitorData.OperateCollect.TryGetValue(OperateCmd[3], out value);
            int offset = 0;
            byte[] cmdData = new byte[allState[0].Count * 5 + 1];
            cmdData[offset++] = (byte)allState[0].Count;

            byte[] ip = new byte[] { 192, 168, 10, 1 };
            for (int i = 0; i < allState[0].Count; i++)
            {
                ip = (byte[])UserData[i].IpArray.Clone();
                Array.Reverse(ip);
                Array.Copy(ip, 0, cmdData, offset, 4);
                offset += 4;
                cmdData[offset++] = value;
            }

            var frame = new TransportProtocol.Comself.RTUFrame(_localAddress, _destAddress,
              (byte)ControlCode.SWITCH_OPERATE, cmdData, (ushort)cmdData.Length);
            ip[0] = 0xFF;//广播
            var adress = new IPAddress(ip);
            modelServer.RtuServer.SendFrame(frame, (uint)adress.Address, _remotePort);
        }


        /// <summary>
        /// 将和分闸状态调成一致
        /// </summary>
        private void StatusRevert()
        {
            var checkResult = CheckStatus(allState[0]);
            if (checkResult.Count != 0)
            {
                byte value = 1;
                bool result;
                foreach (uint key in checkResult.Keys)
                {
                    uint dicValue;
                    checkResult.TryGetValue(key, out dicValue);
                    if (dicValue == 1)
                    {
                        result = modelServer.MonitorData.OperateCollect.TryGetValue(OperateCmd[0], out value);
                    }
                    if (dicValue == 2)
                    {
                        result = modelServer.MonitorData.OperateCollect.TryGetValue(OperateCmd[1], out value);
                    }

                    int offset = 0;
                    byte[] cmdData = new byte[1 * 5 + 1];
                    cmdData[offset++] = (byte)1;

                    byte[] ip = new byte[] { 192, 168, 10, 1 };
                    String ipStr = key.ToString();
                    ipStr = ipStr.Substring(8);
                    if(ipStr[0].Equals('8'))
                    {
                        ipStr = "192.168.10." + ipStr[1];
                    }
                    if (ipStr[0].Equals('9'))
                    {
                        ipStr = "192.168.10." + "1" + ipStr[1];
                    }
                    
                    TypeConvert.IpTryToInt(ipStr, out ip);
                    Array.Reverse(ip);
                    Array.Copy(ip, 0, cmdData, offset, 4);
                    offset += 4;
                    cmdData[offset++] = value;

                    var frame = new TransportProtocol.Comself.RTUFrame(_localAddress, _destAddress,
                      (byte)ControlCode.SWITCH_OPERATE, cmdData, (ushort)cmdData.Length);
                    ip[0] = 0xFF;//广播
                    var adress = new IPAddress(ip);
                    modelServer.RtuServer.SendFrame(frame, (uint)adress.Address, _remotePort);
                    Thread.Sleep(800);
                }
            }
        }

        private void SetRefuseAction(String ipStr,int index)
        {
            byte value;
            bool result = modelServer.MonitorData.OperateCollect.TryGetValue(OperateCmd[index], out value);
            int offset = 0;
            byte[] cmdData = new byte[1 * 5 + 1];
            cmdData[offset++] = (byte)1;

            byte[] ip = new byte[] { 192, 168, 10, 1 };

            TypeConvert.IpTryToInt(ipStr, out ip);
            Array.Reverse(ip);
            Array.Copy(ip, 0, cmdData, offset, 4);
            offset += 4;
            cmdData[offset++] = value;


            var frame = new TransportProtocol.Comself.RTUFrame(_localAddress, _destAddress,
              (byte)ControlCode.SWITCH_OPERATE, cmdData, (ushort)cmdData.Length);
            ip[0] = 0xFF;//广播
            var adress = new IPAddress(ip);
            modelServer.RtuServer.SendFrame(frame, (uint)adress.Address, _remotePort);
        }

        /// <summary>
        /// 发送更新数据指令
        /// </summary>
        private void UpdateData()
        {
            var switchList = modelServer.MonitorData.ReadAttribute(SwitchPropertyIndex.SwitchList, false);
            foreach (var m in switchList)
            {
                var data = new byte[] { (byte)NanopbType.NANOPB_GET_STATION };
                var frame = new TransportProtocol.Comself.RTUFrame(modelServer.LocalAddr, modelServer.CommServer.DownAddress, (byte)ControlCode.NANOPB_TYPE, data, (ushort)data.Length);
                uint address = (uint)(TypeConvert.IpToInt(m.IP));
                modelServer.RtuServer.SendFrame(frame, address, (ushort)modelServer.UdpRemotePort);//固定为获取串口
                System.Threading.Thread.Sleep(100);
            }
        }


        /// <summary>
        /// 合分闸状态检查
        /// </summary>
        /// <returns></returns>
        private Dictionary<uint, uint> CheckStatus(Dictionary<uint, uint> status)
        {
            loopTest.UpadeTip("检查分合闸状态");
            var unConsist = new Dictionary<uint, uint>();

            UserDataTest = modelServer.MonitorData.StationObservable;
            for (int i = 0; i < UserDataTest.Count; i++)
            {
                uint value;
                var result = status.TryGetValue(UserDataTest[i].ID, out value);
                if (result)
                {
                    if (value.ToString().Contains("3"))
                    {
                        var valueStr = value.ToString().Substring(0, 1);
                        var isGetSuccess = uint.TryParse(valueStr, out value);
                    }
                    else
                    {
                        if (value != (uint)UserDataTest[i].State)
                        {
                            unConsist.Add(UserDataTest[i].ID, value);
                        }
                    }
                   
                }
                else
                {
                    continue;
                }
            }
            return unConsist;
        }
    }
}
