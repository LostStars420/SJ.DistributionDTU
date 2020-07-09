using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoicePhase.DeviceNet;
using ChoicePhase.DeviceNet.Element;
using ChoicePhase.DeviceNet.LogicApplyer;
using TransportProtocol.Comself;
using ChoicePhase.PlatformModel.GetViewData;
using ChoicePhase.PlatformModel.Helper;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common.LogTrace;

namespace UdpDebugMonitor.Model
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
        private readonly ushort _localAddr;

        public ushort LocalAddr
        {
            get
            {
                return _localAddr;
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
        public int UdpRemotePort
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
                var ipArray = Dns.GetHostAddresses(Dns.GetHostName());
                var localIp = ipArray.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                _localAddr = (ushort)(localIp.GetAddressBytes()[2] << 8 | localIp.GetAddressBytes()[3]);
                UdpLocalPort = 5556;

                UdpRemotePort = 5555;
                UdpMaintancePort = 5500;
              //  _DeviceOverTime = 7000;
                _destinateAddr = 0x00;
                _downCode = 0x55;
                _upCode = 0xAA;


                CommServer = new CommunicationServer(_destinateAddr, UdpLocalPort, ExceptionDeal);
                CommServer.CommonServer.SerialDataArrived += CommonServer_SerialDataArrived;
                CommServer.UdpServer.DataArrived += UdpServer_DataArrived;

                LogicalUI = new LogicalPresentation(MonitorData.UpdateStatus);

            }
            catch (Exception ex)
            {
                CLog.LogError(ex.StackTrace);
            }

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
        /// 通讯数据到达
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CommonServer_SerialDataArrived(object sender, ChoicePhase.PlatformModel.Communication.SerialDataEvent e)
        {
            RtuServer.AddBuffer(e.SerialData);
            CommServer.RawReciveMessage += ByteToString(e.SerialData, 0, e.SerialData.Length);
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
        /// 异常消息处理，此处用于显示
        /// </summary>
        /// <param name="ex"></param>
        void ExceptionDeal(Exception ex)
        {
            //MonitorData.UpadeExceptionMessage(ex.Message);
            return;
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
        /// 数组转化为字符串数组
        /// </summary>
        /// <param name="data">数据数组</param>
        /// <param name="start">开始索引</param>
        /// <param name="len">转换长度</param>
        /// <returns>转换后的字符串</returns>
        private string ByteToString(byte[] data, int start, int len)
        {
            StringBuilder strBuild = new StringBuilder(len * 3 + 10);
            for (int i = start; i < start + len; i++)
            {
                strBuild.AppendFormat("{0:X2} ", data[i]);
            }
            return strBuild.ToString();
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
            if ((_modelServer == null) || flag)
            {
                _modelServer = new PlatformModelServer();
                return _modelServer;
            }
            else
            {
                return _modelServer;
            }
        }
    }
}
