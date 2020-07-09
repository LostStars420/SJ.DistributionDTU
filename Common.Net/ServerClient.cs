using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Net.Element;


namespace Net
{
    public class ServerClient
    {
        private const int BufferSize = 8192;
        private byte[] buffer;
        private TcpClient client;
        private NetworkStream streamToServer;
        //private string msg = "Welcome To !";

        public event EventHandler<NetDataArrivedEventArgs> DataArrived; //数据到达
        public event EventHandler<NetDataArrivedEventArgs> ExceptionArrived; //数据到达
        public event EventHandler<NetDataArrivedEventArgs> LinkingMsg; //链接消息
        public event EventHandler<NetDataArrayEventArgs> NetDataArrayArrived; //网络数据字节形式
        private bool isRun = false;
        public ServerClient(IPAddress ip, int port)//"localhost", 8500
        {
            buffer = new byte[BufferSize];
            client = new TcpClient();
           // client.Connect(ip, port); // 与服务器连接
            AsyncCallback callBack = new AsyncCallback(ReadComplete);
            client.BeginConnect(ip, port, ConnectFinal, client);
        }

        bool IsOnline()
        {
            return !((client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0)) || !client.Client.Connected);
        }
        void InitNet()
        {
            streamToServer = client.GetStream();

            //回调读函数
            lock (streamToServer)
            {
                AsyncCallback callBack = new AsyncCallback(ReadComplete);
                streamToServer.BeginRead(buffer, 0, BufferSize, callBack, null);
            }
            isRun = true;
        }

        public TcpClient Client
        {
            get
            {
                return client;
    
            } 
        }
        
        public void SendMessage(string msg)
        {
            msg = String.Format("[length={0}]{1}", msg.Length, msg);
            byte[] temp = Encoding.Unicode.GetBytes(msg); // 获得缓存        
            streamToServer.Write(temp, 0, temp.Length); // 发往服务器     
            streamToServer.Flush();
        }

        //public void SendMessage()
        //{
        //    SendMessage(this.msg);
        //}

        private void ConnectFinal(IAsyncResult ar)
        {
            try
            {
                var ct = (TcpClient)ar.AsyncState;
                ct.EndConnect(ar);
                streamToServer = client.GetStream();

                lock (streamToServer)
                {
                    AsyncCallback callBack = new AsyncCallback(ReadComplete);
                    streamToServer.BeginRead(buffer, 0, BufferSize, callBack, null);
                }

                string strLocal = client.Client.LocalEndPoint.ToString();
                string remoteLocal = client.Client.RemoteEndPoint.ToString();
                string str = strLocal + "--->" + remoteLocal + "\n";
                this.LinkingMsg(this, new NetDataArrivedEventArgs(str));

                isRun = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                this.ExceptionArrived(this, new NetDataArrivedEventArgs(e.Message));
            }
            finally
            {

            }
        }
        // 读取完成时的回调方法
        private void ReadComplete(IAsyncResult ar)
        {
            int bytesRead;
            try
            {
                if (!isRun)
                {
                    return;
                }
                //检测网络是否正常
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

                this.NetDataArrayArrived(this, new NetDataArrayEventArgs(buffer, bytesRead));

                string msg = Encoding.Unicode.GetString(buffer, 0, bytesRead);

                lock (this.DataArrived)
                {
                    this.DataArrived(this, new NetDataArrivedEventArgs(msg));
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
                this.ExceptionArrived(this, new NetDataArrivedEventArgs(ex.Message));
                // throw ex;
            }
        }
       
        public void Stop()
        {
            isRun = false;
            if (streamToServer != null)
                    streamToServer.Dispose();
            if (client != null)
            {
                client.Close();
            }
                
        }
    }
}
