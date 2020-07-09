using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NATUPNPLib;
using System.Net;
using System.Net.Sockets;
using Net.Element;



namespace Net.UPNP
{
 
    /// <summary>
    /// UnpnServer初始化，包含单个子网穿透功能
    /// </summary>
    public class UpnpServer
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
        public string hostName;
        /// <summary>
        /// 主机名称
        /// </summary>
        public string HostName
        {
            get { return hostName; }
        }
       /// <summary>
       /// 内网IPV4地址
       /// </summary>
        public IPAddress innerIpv4;
        /// <summary>
        /// 内网IPV4地址
        /// </summary>
        public IPAddress InerIPV4
        {
            get { return innerIpv4; }
        }
        /// <summary>
        /// 对外端口
        /// </summary>
        public int eport;
        /// <summary>
        /// 对内端口
        /// </summary>
        public int iport;

       /// <summary>
        /// NAT网络 创建COM类型
       /// </summary>
        private UPnPNAT upnpnat;
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
        public event EventHandler<NetDataArrivedEventArgs> LinkingMsg;
        /// <summary>
        /// 数据服务事件--字符串
        /// </summary>
        public event EventHandler<NetDataArrivedEventArgs> DataArrived;
        /// <summary>
        /// 异常处理事件
        /// </summary>
        public event EventHandler<NetDataArrivedEventArgs> ExceptionArrived; 
        /// <summary>
        /// 网络数据到达事件，字节形式
        /// </summary>
        public event EventHandler<NetDataArrayEventArgs> NetDataArrayArrived; 
        /// <summary>
        /// 服务运行标志
        /// </summary>
        private bool isRun = false;
        /// <summary>
        /// UpnpServer初始化
        /// </summary>
        /// <param name="veport">外部端口</param>
        /// <param name="viport">内部端口</param>
        public UpnpServer(int veport,int viport)
        {
            try
            {
                
                buffer = new byte[BufferSize];

                innerIpv4 = GetHostInerIP();//获取内网IP


                //UPnP绑定信息
                eport = veport;
                iport = viport;
                var description = "LINK测试";
                //创建COM类型
                upnpnat = new UPnPNAT();
                var mappings = upnpnat.StaticPortMappingCollection;

                //错误判断
                if (mappings == null)
                {
                    Console.WriteLine("没有检测到路由器，或者路由器不支持UPnP功能。");
                    return;
                }

                //添加之前的ipv4变量（内网IP），内部端口，和外部端口
                mappings.Add(eport, "TCP", iport, innerIpv4.ToString(), true, description);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //绑定内网IP和内部端口
                socket.Bind(new IPEndPoint(innerIpv4, iport));
                socket.Listen(1);

                AsyncCallback callBack = new AsyncCallback(AcceptFinalCallback);
                //成功连接
                socket.BeginAccept(callBack, socket);
            }
              catch (Exception ex)
            {
                this.ExceptionArrived(this, new NetDataArrivedEventArgs("UpnpServer:" + ex.Message));
            }
        }
      
        /// <summary>
        /// UpnpServer初始化
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        public UpnpServer(IPAddress ip, int port)
        {
            try
            {

                buffer = new byte[BufferSize];
                innerIpv4 = ip;

                eport = port;
              

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //绑定内网IP和内部端口
                socket.Bind(new IPEndPoint(innerIpv4, eport));
                socket.Listen(1);

                AsyncCallback callBack = new AsyncCallback(AcceptFinalCallback);
                //成功连接
                socket.BeginAccept(callBack, socket);
            }
            catch (Exception ex)
            {
                this.ExceptionArrived(this, new NetDataArrivedEventArgs("UpnpServer:" + ex.Message));
            }
        }
        /// <summary>
        /// UpnpServer 初始化
        /// </summary>
        /// <param name="ip">Ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="state">状态控制</param>
        public UpnpServer(IPAddress ip, int port, bool state)
        {
            try
            {

                buffer = new byte[BufferSize];
                innerIpv4 = ip;
                eport = port;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint end= new IPEndPoint(ip, port);
                AsyncCallback callBack = new AsyncCallback(ConnectedFinalCallback);
                //成功连接
                socket.BeginConnect(end, callBack, socket);
            }
            catch (Exception ex)
            {
                this.ExceptionArrived(this, new NetDataArrivedEventArgs("Client:" + ex.Message));
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
        /// 连接完成回调函数
        /// </summary>
        /// <param name="ar">异步操作状态</param>
         private void ConnectedFinalCallback(IAsyncResult ar)
         {
            client = (Socket)ar.AsyncState;
            try
            {
                client.EndConnect(ar);
                streamToServer = new NetworkStream(client);
            
                lock (streamToServer)
                {
                    AsyncCallback callBack = new AsyncCallback(ReadComplete);
                    streamToServer.BeginRead(buffer, 0, BufferSize, callBack, null);
                }
                string strLocal = client.LocalEndPoint.ToString();
                string remoteLocal = client.RemoteEndPoint.ToString();
                string str = strLocal + "--->" + remoteLocal + "\n";
                this.LinkingMsg(this, new NetDataArrivedEventArgs(str));

                isRun = true;


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                if (this.ExceptionArrived != null)
                {
                    this.ExceptionArrived(this, new NetDataArrivedEventArgs("ConnectedFinalCallback:" + e.Message));
                }
            }
            finally
            {

            }
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
               // string msg = "===Server: Welcome!===";
               // msg = String.Format("[length={0}]{1}", msg.Length, msg);
                //byte[] temp = Encoding.Unicode.GetBytes(msg); // 获得缓存 

               // streamToServer.Write(temp, 0, temp.Length);
               // streamToServer.Flush();
  
                lock (streamToServer)
                {
                    AsyncCallback callBack = new AsyncCallback(ReadComplete);
                    streamToServer.BeginRead(buffer, 0, BufferSize, callBack, null);
                }
                string strLocal = client.LocalEndPoint.ToString();
                string remoteLocal = client.RemoteEndPoint.ToString();
                string str =  strLocal + "--->" + remoteLocal + "\n";
                this.LinkingMsg(this, new NetDataArrivedEventArgs(str));
                
                isRun = true;
                
            }
            catch(Exception ex)
            {
                this.ExceptionArrived(this, new NetDataArrivedEventArgs("AcceptFinalCallback:" + ex.Message));
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
                this.ExceptionArrived(this, new NetDataArrivedEventArgs("SendMessage:" + ex.Message));
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
                this.ExceptionArrived(this, new NetDataArrivedEventArgs("SendMessage:" + ex.Message));
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

                  msg = String.Format("[length={0}]{1}", msg.Length, msg);
                  byte[] temp = Encoding.Unicode.GetBytes(msg); // 获得缓存        
                  streamToServer.Write(temp, 0, temp.Length); // 发往服务器     

                
            }
            catch(Exception ex)
            {
                this.ExceptionArrived(this, new NetDataArrivedEventArgs("SendMessage:" + ex.Message));
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
                    this.ExceptionArrived(this, new NetDataArrivedEventArgs("网络连接不畅,已经停止连接!"));
                    return;
                }
                lock (streamToServer)
                {
                  
                   bytesRead = streamToServer.EndRead(ar);
                    
                }
                if (bytesRead == 0) throw new Exception("读取到 0 字节");

                lock (this.NetDataArrayArrived)
                {
                    this.NetDataArrayArrived(this, new NetDataArrayEventArgs(buffer, bytesRead));
                }
                
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
                this.ExceptionArrived(this, new NetDataArrivedEventArgs("ReadComplete:" + ex.Message));

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
              if (LinkingMsg !=null)
              {
                 // LinkingMsg.
              }
          }

          
    }
}
