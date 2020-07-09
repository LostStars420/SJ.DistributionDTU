using System;
using System.Collections.Generic;
using System.Threading;
using Common.LogTrace;
using System.Runtime.InteropServices;

namespace TransportProtocol.Comself
{
    /// <summary>     
    /// 开辟独立线程负责数据的提取与转换；采用双重缓冲，保证数据的完整性。
    ///   在开辟线程中，对帧进行校验提取，对提取帧以事件的形式发出。
    /// </summary>
    public class RtuServer
    {
        /// <summary>
        /// 地址
        /// </summary>
        private ushort _addrss;

        /// <summary>
        /// 超时时间 ms
        /// </summary>
        private int _overTime;

        /// <summary>
        /// 第一级缓冲
        /// </summary>
        private List<byte> _dataFirstBuffer;
        /// <summary>
        /// 第二级缓冲
        /// </summary>
        private List<byte> _dataSecoundBuffer;

        /// <summary>
        /// 读取线程
        /// </summary>
        private Thread readThread;


        private ProtocolAnylast _anylast;

        /// <summary>
        /// 获取一个完整的RTU帧
        /// </summary>
        public event EventHandler<RtuFrameArgs> RtuFrameArrived;
        /// <summary>
        /// 获取一个完整的RTU帧
        /// </summary>
        public event EventHandler<FrameRtuArgs> FrameRtuArrived;

        /// <summary>
        /// 发送数据委托
        /// </summary>
        private Func<byte[], bool> _sendDataDelegate;

        /// <summary>
        /// 发送数据委托
        /// </summary>
        private Func<byte[], UInt32, int, bool> _udpSendDataDelegate;

        /// <summary>
        /// 远程端口
        /// </summary>
        public int RemotePort
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为UDP模式
        /// </summary>
        public bool IsUdpMode
        {
            get;
            private set;
        }

        /// <summary>
        /// 异常处理委托
        /// </summary>
        private Action<Exception> ExceptionDelegate;

        /// <summary>
        /// 接收RTU帧服务
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="overTime">超时时间</param>
        public RtuServer(ushort addr, int overTime, Func<byte[], bool> sendDataDelegate, Action<Exception> exceptionDelegate)
        {
            _addrss = addr;
            _overTime = overTime;
            _dataFirstBuffer = new List<byte>();
            _dataSecoundBuffer = new List<byte>();
            _sendDataDelegate = sendDataDelegate;

            _anylast = new ProtocolAnylast(_addrss);

            ExceptionDelegate = exceptionDelegate;

            readThread = new Thread(ReciveThread);
            readThread.Priority = ThreadPriority.Normal;
            readThread.Name = "ReciveRtuServer";
            readThread.Start();
        }

        public RtuServer(ushort addr, int overTime, Func<byte[], bool> sendDataDelegate, Func<byte[], UInt32, int, bool> udpSendDataDelegate, int port, Action<Exception> exceptionDelegate
            ) : this(addr, overTime, sendDataDelegate, exceptionDelegate)
        {
            _udpSendDataDelegate = udpSendDataDelegate;
            RemotePort = port;
            IsUdpMode = true;
        }

        /// <summary>
        /// 关闭RTU服务
        /// </summary>
        public void Close()
        {
            readThread.Join(100);
            readThread.Abort();
        }

        /// <summary>
        /// 拷贝数据添加到缓冲中
        /// </summary>
        /// <param name="data">缓冲数据数组</param>
        public void AddBuffer(byte[] data)
        {
            lock (_dataFirstBuffer)
            {
                var array = (byte[])data.Clone();//重点测试
                _dataFirstBuffer.AddRange(array);
            }
        }

        /// <summary>
        /// 发送帧，UDP,串口帧，两种兼容
        /// </summary>
        /// <param name="frame">帧数据</param>
        /// <param name="id">id号</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public bool SendFrame(RTUFrame frame, uint id, int port)
        {
            if (_udpSendDataDelegate != null && IsUdpMode)
            {
                return _udpSendDataDelegate(frame.Frame, id, port);
            }
            else if (_sendDataDelegate != null && (!IsUdpMode))
            {
                return _sendDataDelegate(frame.Frame);
            }
            return false;
        }

        /// <summary>
        /// 发送帧数据
        /// </summary>
        /// <param name="frame"></param>
        public bool SendFrame(RTUFrame frame)
        {
            if (_sendDataDelegate != null && (!IsUdpMode))
            {
                return _sendDataDelegate(frame.Frame);
            }

            if (_udpSendDataDelegate != null && IsUdpMode)
            {
                return _udpSendDataDelegate(frame.Frame, frame.Address, RemotePort);
            }
            return false;
        }

        private void ReciveThread()
        {
            DateTime timeStart = DateTime.Now;

            try
            {
                do
                {
                    lock (_dataFirstBuffer)
                    {
                        if (_dataFirstBuffer.Count > 0)
                        {
                            _dataSecoundBuffer.AddRange(_dataFirstBuffer);//添加到新数据中
                            _dataFirstBuffer.Clear();//清空第一缓冲区
                        }
                    }

                    if (_dataSecoundBuffer.Count == 0)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    FrameRtu rtu = new FrameRtu();
                    bool result = false;
                    int usedlen = 0;
                    var inData = _dataSecoundBuffer.ToArray();
                    do
                    {

                        result = _anylast.Deal(inData, ref usedlen, ref rtu, _addrss);
                        if (result)
                        {
                            FrameRtuArrived?.Invoke(this, new FrameRtuArgs(rtu));
                        }
                        inData = new byte[0];
                        _dataSecoundBuffer.RemoveRange(0, usedlen);
                    }
                    while (result);//若为true，重新进入，有可能存在多帧情况

                    Thread.Sleep(5);
                } while (true);
            }
            catch (ThreadAbortException ex)
            {
                Thread.ResetAbort();
                CLog.LogWarning("串口接收进程::" + ex.Message);
            }
            catch (Exception ex)
            {
                CLog.LogError("串口接收进程::" + ex.Message);

            }
            finally
            {

            }
        }
    }


}
