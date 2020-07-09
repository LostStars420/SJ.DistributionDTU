using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;


namespace ChoicePhase.PlatformModel.Communication
{

    /// <summary>
    /// 串口服务
    /// </summary>
    public class SerialPortServer
    {
        private SerialPort serialPort; //串口实例
        private bool portState = false; //串口状态
                                                    
        /// <summary>
        /// 串口数据到达
        /// </summary>
        public event EventHandler<SerialDataEvent> SerialDataArrived;//事件

        /// <summary>
        /// 获取串口状态
        /// </summary>
        public bool CommState
        {
            get { return portState; }
        }

        /// <summary>
        /// 获取串口
        /// </summary>
        public SerialPort SerialPort
        {
            get { return serialPort; }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close()
        {
            if (serialPort != null)
            {

                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }

            }
            portState = false;                      
        }

        void InitSerialPort()
        {
            serialPort = new SerialPort();

            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;

            serialPort.DtrEnable = true;
            serialPort.ReadBufferSize = 2 * 20 * 1024;//默认值4096

            serialPort.ReceivedBytesThreshold = 1;
            serialPort.DataReceived += serialPort_DataReceived;
        }

        /// <summary>
        /// 串口接收进程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (serialPort.BytesToRead != 0)
                {
                    int len = serialPort.BytesToRead;
                    var data = new byte[len];
                    serialPort.Read(data, 0, len);
                    this.SerialDataArrived(serialPort, new SerialDataEvent(data));
                }                                          
            }
            catch (Exception ex)
            {
                if (ExceptionDealDelegate != null)
                {
                    ExceptionDealDelegate(ex);
                }               
                Trace.WriteLine("串口接收进程::" + ex.Message);
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns>true-打开成功，false-失败</returns>
        public bool Open()
        {
            try
            {
                if (!portState)
                {
                    serialPort.Open();                  
                    portState = true;                                       
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Open(string port, int baud, int dataBit,  System.IO.Ports.Parity parity, StopBits stopBit)
        {
            try
            {
                serialPort.PortName = port;
                serialPort.BaudRate = baud;
                serialPort.DataBits = dataBit;
                serialPort.Parity = parity;
                serialPort.StopBits = stopBit;
                Open();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
      
        /// <summary>
        /// 串口中心初始化
        /// </summary>
        public SerialPortServer()
        {
            InitSerialPort();
        }

        /// <summary>
        /// 异常信息处理委托
        /// </summary>
        public Action<Exception> ExceptionDealDelegate
        {
            get;
            set;
        }


       
        /// <summary>
        /// 发送带有长度标识的字节数组
        /// </summary>
        /// <param name="sendData">字节数组</param>
        /// <param name="len">发送的数组长度</param>
        public void SendMessage(byte[] sendData, int len)
        {
            serialPort.Write(sendData, 0, len);
        }

        /// <summary>
        /// 发送数据--字节数组
        /// </summary>
        /// <param name="sendData">字节数组</param>
        public void SendMessage(byte[] sendData)
        {
            serialPort.Write(sendData, 0, sendData.Length);
        }

        /// <summary>
        /// 发送数据-自付出
        /// </summary>
        /// <param name="msg">字符串</param>
        public void SendMessage(string msg)
        {
            byte[] temp = Encoding.Unicode.GetBytes(msg); // 获得缓存        
            serialPort.Write(temp, 0, temp.Length);
        }
    }
}
