using GalaSoft.MvvmLight;
using System;
using System.Net;
using ChoicePhase.PlatformModel.Communication;
using Net.UDP;

namespace UdpDebugMonitor.Model
{
    public class CommunicationServer : ObservableObject
    {
        /// <summary>
        /// 下位机地址
        /// </summary>
        public byte DownAddress
        {
            private set;
            get;
        }

        /// <summary>
        /// 通讯服务
        /// </summary>
        private SerialPortServer _commonServer;

        /// <summary>
        /// 通讯服务
        /// </summary>
        public SerialPortServer CommonServer
        {
            get
            {
                return _commonServer;
            }
        }

        /// <summary>
        /// UDP网络服务
        /// </summary>
        private UdpClientManager _udpServer;

        /// <summary>
        ///  UDP网络服务
        /// </summary>
        public UdpClientManager UdpServer
        {
            get
            {
                return _udpServer;
            }
        }

        private UdpClientManager _udpMonitorServer;
        public UdpClientManager UdpMonitorServer
        {
            get
            {
                return _udpMonitorServer;
            }
            set
            {
                _udpMonitorServer = value;
            }
        }

        public void OpenUdpMonitor(IPEndPoint endpoint, Action<Exception> exa)
        {
            if (_udpMonitorServer != null)
            { 
                _udpMonitorServer.Close();
            }
            _udpMonitorServer = new UdpClientManager(endpoint.Address, endpoint.Port, exa);

        }

        public void CloseUdpMonitor()
        {
            if (_udpMonitorServer != null)
            {
                _udpMonitorServer.Close();
            }
            _udpMonitorServer = null;
        }

        private string linkMessage = "";

        /// <summary>
        /// 连接信息
        /// </summary>
        public string LinkMessage
        {
            get
            {
                return linkMessage;
            }
            set
            {
                linkMessage = value;
                RaisePropertyChanged("LinkMessage");
                if (linkMessage.Length > 5000)
                {
                    linkMessage = "";
                }
            }
        }
        /// <summary>
        /// 更新发送连接信息
        /// </summary>
        /// <param name="message"></param>
        public void UpadteSendLinkMessage(string message)
        {
            LinkMessage += string.Format("{0} {1:#####}: ", DateTime.Now.ToLongTimeString(), "Send");
            LinkMessage += message + "\n";
        }

        /// <summary>
        /// 更新接收连接信息
        /// </summary>
        /// <param name="message"></param>
        public void UpadteReciveLinkMessage(string message)
        {
            LinkMessage += string.Format("{0} {1:#####}: ", DateTime.Now.ToLongTimeString(), "Recive");
            LinkMessage += message + "\n";
        }

        private string rawReciveMessage = "";

        /// <summary>
        /// 原始接收数据
        /// </summary>
        public string RawReciveMessage
        {
            get
            {
                return rawReciveMessage;
            }
            set
            {
                if (rawReciveMessage.Length > 5000)
                {
                    rawReciveMessage = "";
                }
                rawReciveMessage = value;
                RaisePropertyChanged("RawReciveMessage");
            }
        }

        private string rawSendMessage = "";

        /// <summary>
        /// 原始发送数据
        /// </summary>
        public string RawSendMessage
        {
            get
            {
                return rawSendMessage;
            }
            set
            {
                if (rawReciveMessage.Length > 5000)
                {
                    rawReciveMessage = "";
                }
                rawSendMessage = value;

                RaisePropertyChanged("RawSendMessage");

            }
        }

        public void Close()
        {
            CommonServer.Close();
            UdpServer.Close();
            CloseUdpMonitor();
        }

        /// <summary>
        /// 初始化通讯服务
        /// </summary>
        public CommunicationServer(byte downAddress)
        {
            _commonServer = new SerialPortServer();

            DownAddress = downAddress;

        }
        /// <summary>
        /// 初始化通讯服务
        /// </summary>
        public CommunicationServer(byte downAddress, int port, Action<Exception> actionException) : this(downAddress)
        {

            _udpServer = new UdpClientManager(port, actionException);

        }
    }

}
