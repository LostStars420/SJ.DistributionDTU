using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Net
{
  
    public class RemoteClient
    {
        private TcpClient client;
        private NetworkStream streamToClient;
        private const int BufferSize = 8192;
        private byte[] buffer;
        private RequestHandler handler;

        public event EventHandler<NetDataArrivedEventArgs> DataArrived;

        public RemoteClient(TcpClient client)
        {
            this.client = client;
            // 获得流
            streamToClient = client.GetStream();
            buffer = new byte[BufferSize];
            // 设置 RequestHandler
            handler = new RequestHandler();
            // 在构造函数中就开始准备读取
            AsyncCallback callBack = new AsyncCallback(ReadComplete);
            streamToClient.BeginRead(buffer, 0, BufferSize, callBack, null); 
            //DataArrived = new EventHandler<NetDataArrivedEventArgs>();
        }

        public TcpClient Client
        {
            get
            {
                return client;

            }
        }

       
        // 在读取完成时进行回调
        private void ReadComplete(IAsyncResult ar)
        {
            int bytesRead = 0;
            try
            {
                lock (streamToClient)
                {
                    bytesRead = streamToClient.EndRead(ar);                   
                }
                if (bytesRead == 0) throw new Exception("读取到 0 字节");
                string msg = Encoding.Unicode.GetString(buffer, 0, bytesRead);
                Array.Clear(buffer, 0, buffer.Length); // 清空缓存，避免脏读
                string[] msgArray = handler.GetActualString(msg); // 获取实际的字符串
                // 遍历获得到的字符串
                foreach (string m in msgArray)
                {
                    //Console.WriteLine("Received: {0}", m); 
                    //产生事件发送出去
                    lock (this.DataArrived)
                    {
                        this.DataArrived(this, new NetDataArrivedEventArgs(msg));
                    } 
                }
                // 再次调用 BeginRead()，完成时调用自身，形成无限循环
                lock (streamToClient)
                {
                    AsyncCallback callBack = new AsyncCallback(ReadComplete);
                    streamToClient.BeginRead(buffer, 0, BufferSize, callBack, null);
                }
            }
            catch (Exception ex)
            {
                if (streamToClient != null)
                    streamToClient.Dispose();
                client.Close();// 捕获异常时退出程序
                Console.WriteLine(ex.Message);
            }
        }
        public void SendMessage(string msg)
        {
            msg = String.Format("[length={0}]{1}", msg.Length, msg);

            byte[] temp = Encoding.Unicode.GetBytes(msg); // 获得缓存
            try
            {
                streamToClient.Write(temp, 0, temp.Length); // 发往服务器
                streamToClient.Flush();
               
            }
            catch (Exception ex)
            {
                throw ex;
                
            }
        }
        public void Stop()
        {
            if (streamToClient != null)
                streamToClient.Dispose();
            client.Close();// 捕获异常时退出程序
           
        }
    }
}
