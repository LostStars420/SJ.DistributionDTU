using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Net.UDP
{
    public class UdpClientManager
    {
        private UdpClient _udpClient;
        /// <summary>
        /// 是否允许知识
        /// </summary>
        private bool _isRun;

        public bool IsRun
        {
            get
            {
                return _isRun;
            }
        }

        /// <summary>
        /// 接收线程
        /// </summary>
        private Thread _receiveThread;

        /// <summary>
        /// 串口数据到达
        /// </summary>
        public event EventHandler<UdpDataEventArgs> DataArrived;//事件

        /// <summary>
        /// 异常信息处理委托
        /// </summary>
        public Action<Exception> ExceptionDealDelegate
        {
            get;
            private set;
        }

        /// <summary>
        /// 发送到指定的IP和端口
        /// </summary>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
        /// <param name="data">数据</param>
        /// <returns>成功 true</returns>
        public bool Send(IPAddress ip, int port, byte[] data)
        {
            try
            {
                IPEndPoint remoteIpEndPoint = new IPEndPoint(ip, port);
                int len = 0;

                len = _udpClient.Send(data, data.Length, remoteIpEndPoint);

                return (data.Length == len);
               
            }
            catch(Exception ex)
            {
                throw ex;
            }           
        }


        private void ReceiveData()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _isRun = true;
            while (_isRun)
            {
                try
                {
                    // 关闭receiveUdpClient时此时会产生异常
                    byte[] receiveBytes = _udpClient.Receive(ref remoteIpEndPoint);

                    if(receiveBytes.Length != 0)
                    {
                        this.DataArrived(this, new UdpDataEventArgs((receiveBytes), remoteIpEndPoint));
                    }                   
                }
                catch (Exception e)
                {
                    ExceptionDealDelegate?.Invoke(e);
                    break;
                }
                Thread.Sleep(10);
            }
        }
        /// <summary>
        /// 关闭udp
        /// </summary>
        public void Close()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
                _isRun = false;
            }
           
        }

        /// <summary>
        /// 初始化udpClient管理器
        /// </summary>
        /// <param name="ip">地址</param>
        /// <param name="port">端口</param>
        public UdpClientManager(IPAddress ip, int port, Action<Exception> exceptionDealDelegate)
        {
           
            try
            {
              
                ExceptionDealDelegate = exceptionDealDelegate;
                _udpClient = new UdpClient(new IPEndPoint(ip, port));
                _receiveThread = new Thread(ReceiveData);
                _receiveThread.Start();
               
            }
            catch (Exception e)
            {
                ExceptionDealDelegate?.Invoke(e);
                Console.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// 初始化udpClient管理器
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="exceptionDealDelegate">异常处理</param>
        public UdpClientManager(int port, Action<Exception> exceptionDealDelegate)
        {

            try
            {              
                ExceptionDealDelegate = exceptionDealDelegate;
              
                var ipArray = Dns.GetHostAddresses(Dns.GetHostName());
                var localIp = ipArray.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                //var localIp=
                var ipEndport = new IPEndPoint(localIp, port);
                _udpClient = new UdpClient(ipEndport);
                _receiveThread = new Thread(ReceiveData);
                _receiveThread.Start();

            }
            catch (Exception e)
            {
                ExceptionDealDelegate?.Invoke(e);
                Console.WriteLine(e.ToString());
            }
        }
    }

    public class UdpDataEventArgs : EventArgs
    {
        /// <summary>
        /// 数据部分
        /// </summary>
        public byte[] DataArray
        {
            get;
            private set;
        }
        /// <summary>
        /// 远程IP信息
        /// </summary>
        public IPEndPoint RemoteIpEndPoint
        {
            get;
            private set;
        }
        
        /// <summary>
        /// UDP数据到达
        /// </summary>
        /// <param name="dataArray"></param>
        /// <param name="remoteIpEndPoint"></param>
        public UdpDataEventArgs(byte[] dataArray, IPEndPoint remoteIpEndPoint)
        {
            DataArray = new byte[dataArray.Length];
            dataArray.CopyTo(DataArray, 0);
            RemoteIpEndPoint = new IPEndPoint(remoteIpEndPoint.Address, remoteIpEndPoint.Port);
        }
    }
}
