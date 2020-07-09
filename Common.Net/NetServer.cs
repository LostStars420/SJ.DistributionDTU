using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Net.Element;



namespace Net
{
 
    /// <summary>
    /// UnpnServer初始化，包含单个子网穿透功能
    /// </summary>
    public class NetServer
    {
        /// <summary>
        /// 缓冲大小
        /// </summary>
        private const int BufferSize = 8192;
        /// <summary>
        /// 缓冲块
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// 主机名称
        /// </summary>
        private string hostName;
        /// <summary>
        /// 主机名称
        /// </summary>
        public string HostName
        {
            get { return hostName; }
        }
       /// <summary>
       /// IPV4地址
       /// </summary>
        private IPAddress innerIpv4;
        /// <summary>
        /// IPV4地址
        /// </summary>
        public IPAddress InerIPV4
        {
            get { return innerIpv4; }
        }
        /// <summary>
        /// 网络端口
        /// </summary>
        private int eport;
    
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// 客户端套接字
        /// </summary>
        private Socket client;
        /// <summary>
        /// 网络流服务
        /// </summary>
        private NetworkStream streamToServer;
        /// <summary>
        /// 连接信息事件
        /// </summary>
        public event EventHandler<NetLinkMessagEvent> LinkingEventMsg;
        /// <summary>
        /// 异常处理事件
        /// </summary>
        public event EventHandler<NetExcptionEventArgs> ExceptionEventArrived; 
        /// <summary>
        /// 网络数据到达事件，字节形式
        /// </summary>
        public event EventHandler<NetDataArrayEventArgs> NetDataEventArrived; 
        /// <summary>
        /// 服务运行标志
        /// </summary>
        private bool isRun = false;
       
        /// <summary>
        /// 网络运行标志
        /// </summary>
        public bool IsRun
        {
            get
            {
                return isRun;
            }
        }
       
        /// <summary>
        /// NetServer初始化,作为服务器端
        /// </summary>
        public NetServer()
        {
            try
            {

                buffer = new byte[BufferSize];
               
            }
            catch (Exception ex)
            {                
                MakeExceptionEvent(ex, "NetServer");
            }
        }
     
        /// <summary>
        /// 启动网络
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        /// <returns>false-启动失败，true--正常初始化</returns>
        public bool StartServer(IPAddress ip, int port)
        {
            try
            {
                //检测是否运行
                if(isRun)
                {
                    throw new Exception("服务正在运行禁止重复启动");
                   
                }                
                innerIpv4 = ip;
                eport = port;                
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //绑定IP和端口
                socket.Bind(new IPEndPoint(innerIpv4, eport));
                socket.Listen(1);
                MakeLinkMessageEvent("开始监听", NetState.StartLink);
                AsyncCallback callBack = new AsyncCallback(AcceptFinalCallback);
                //成功连接
                socket.BeginAccept(callBack, socket);
                return true;
            }
            catch (Exception ex)
            {
                MakeExceptionEvent(ex, "StartServer");
                return false;
            }
        }
        /// <summary>
        /// 是否在线
        /// </summary>
        /// <returns></returns>
        bool IsOnline()
        {
            return !((client.Poll(1000, SelectMode.SelectRead) && (client.Available == 0)) || !client.Connected);
        }
   

        /// <summary>
        /// sever收到连接回调
        /// </summary>
        /// <param name="ar">异步状态</param>
        private void AcceptFinalCallback(IAsyncResult ar)
        {
            try
            {
                Socket skServer = (Socket)ar.AsyncState;
                client = skServer.EndAccept(ar);
                streamToServer = new NetworkStream(client);

                lock (streamToServer)
                {
                    AsyncCallback callBack = new AsyncCallback(ReadComplete);
                    streamToServer.BeginRead(buffer, 0, BufferSize, callBack, null);
                }
                string strLocal = client.LocalEndPoint.ToString();
                string remoteLocal = client.RemoteEndPoint.ToString();
                string str =  strLocal + "--->" + remoteLocal + "\n";
                MakeLinkMessageEvent(str, NetState.EstablishLink);
                isRun = true;
                
            }
            catch(Exception ex)
            {               
                MakeExceptionEvent(ex, "AcceptFinalCallback");
            }
            
            
        }
        /// <summary>
        /// 发送带有长度标识的字节数组
        /// </summary>
        /// <param name="sendData">字节数组</param>
        /// <param name="len">发送的数组长度</param>
        public void SendMessage(byte[] sendData, int len)
        {
            try
            {
                streamToServer.Write(sendData, 0, len); // 发往服务器   
            }
            catch (Exception ex)
            {                               
                MakeExceptionEvent(ex, "SendMessage(byte[] sendData, int len)");
            }
        }
        /// <summary>
        /// 发送数据--字节数组
        /// </summary>
        /// <param name="sendData">字节数组</param>
        public void SendMessage(byte[] sendData)
        {
            try
            {
                streamToServer.Write(sendData, 0, sendData.Length); // 发往服务器     
                

            }
            catch (Exception ex)
            {
                MakeExceptionEvent(ex, "SendMessage(byte[] sendData)");                
            }
        }
        /// <summary>
        /// 发送数据-自付出
        /// </summary>
        /// <param name="msg">字符串</param>
        public void SendMessage(string msg)
        {
            try
            {
                  byte[] temp = Encoding.Unicode.GetBytes(msg); // 获得缓存        
                  streamToServer.Write(temp, 0, temp.Length); // 发往服务器                   
            }
            catch(Exception ex)
            {
                MakeExceptionEvent(ex, "SendMessage(string msg)");                            
            }
        }
        /// <summary>
        /// 读完成回调
        /// </summary>
        /// <param name="ar">异步状态</param>
        private void ReadComplete(IAsyncResult ar)
        {
            int bytesRead = 0;
            try
            {
                if (!isRun)
                {
                    return;
                }
                if (!IsOnline())
                {
                    MakeLinkMessageEvent("网络连接不畅,已经停止连接!", NetState.StartLink);                  
                }
                lock (streamToServer)
                {                  
                   bytesRead = streamToServer.EndRead(ar);
                    
                }
                if (bytesRead == 0) 
                    throw new Exception("读取到 0 字节");
                
                     MakeNetDataEvent(buffer, bytesRead);              
               
                Array.Clear(buffer, 0, buffer.Length); // 清空缓存，避免脏读
                lock (streamToServer)
                {
                    AsyncCallback callBack = new AsyncCallback(ReadComplete);
                    streamToServer.BeginRead(buffer, 0, BufferSize, callBack, null);
                }
            }
            catch (Exception ex)
            {
                if (streamToServer != null)
                    streamToServer.Dispose();
                client.Close();
                isRun = false;               
                 MakeExceptionEvent(ex, "ReadComplete");
            }
        }
        /// <summary>
        /// 获取内网地址IP
        /// </summary>
        /// <returns>内网IP</returns>
          private IPAddress GetHostInerIP()
          {
              hostName = Dns.GetHostName(); //获取主机名称
              //从当前Host Name解析IP地址，筛选IPv4地址是本机的内网IP地址。
              var addrlist = Dns.GetHostEntry(hostName).AddressList;
              IPAddress ipv4 = null;
              foreach (var m in addrlist)
              {
                  if (m.AddressFamily == AddressFamily.InterNetwork)
                  {
                      ipv4 = m;
                      break;
                  }
              }
              return ipv4;
          }
         /// <summary>
         /// 停止连接
         /// </summary>
          public void Stop()
          {              
              isRun = false;
              if (streamToServer != null)
                    streamToServer.Dispose();
              if (client != null)
              {
                  
                  client.Close();
                  client.Dispose();
                  client = null;
                 
              }
              if (socket != null)
              {
                  socket.Close();
                  socket.Dispose();
                  socket = null;
              }
             
          }
          /// <summary>
          /// 产生网络数据到达事件
          /// </summary>
          private void MakeLinkMessageEvent(string message, NetState state)
          {
              if (LinkingEventMsg != null)
              {
                  LinkingEventMsg(this, new NetLinkMessagEvent(message, state));
              }
          }
          /// <summary>
          /// 产生网络数据到达事件
          /// </summary>
          private void MakeNetDataEvent(byte[] data, int len)
          {
              if (NetDataEventArrived != null)
              {
                  lock (NetDataEventArrived)
                  {
                      NetDataEventArrived(this, new NetDataArrayEventArgs(data, len));
                  }
              }
          }
          /// <summary>
          /// 产生网络异常事件
          /// </summary>
          /// <param name="ex"></param>
          /// <param name="comment"></param>
          private void MakeExceptionEvent(Exception ex, string comment)
          {
              if (ExceptionEventArrived != null)
              {
                  ExceptionEventArrived(this, new NetExcptionEventArgs(ex, comment));
              }
          }

          
    }
}
