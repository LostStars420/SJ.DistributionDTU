using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Common.LogTrace;

namespace ChoicePhase.Modbus
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
        private byte _addrss;

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
        /// <summary>
        /// 获取一个完整的RTU帧
        /// </summary>
        public event EventHandler<RtuFrameArgs> RtuFrameArrived;


        /// <summary>
        /// 发送数据委托
        /// </summary>
        private Func<byte[], bool> _sendDataDelegate;

        /// <summary>
        /// 异常处理委托
        /// </summary>
        private Action<Exception> ExceptionDelegate;

        /// <summary>
        /// 接收RTU帧服务
        /// </summary>
        /// <param name="addr">地址</param>
        /// <param name="overTime">超时时间</param>
        public RtuServer(byte addr, int overTime, Func<byte[], bool> sendDataDelegate, Action<Exception> exceptionDelegate)
        {
            _addrss = addr;
            _overTime = overTime;
            _dataFirstBuffer = new List<byte>();
            _dataSecoundBuffer = new List<byte>();
            _sendDataDelegate = sendDataDelegate;


            ExceptionDelegate = exceptionDelegate;

            readThread = new Thread(ReciveThread);
            readThread.Priority = ThreadPriority.Normal;
            readThread.Name = "ReciveRtuServer";
            readThread.Start();
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
        /// 发送帧数据
        /// </summary>
        /// <param name="frame"></param>
        public bool  SendFrame(RTUFrame frame)
        {
            return (_sendDataDelegate(frame.Frame));           
        }
  

        /// <summary>
        /// 接收线程
        /// </summary>
        private void ReciveThread()
        {
            DateTime timeStart = DateTime.Now;
            int step = 0;
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
                    
                    if(step == 1)
                    {
                        var diff = (DateTime.Now - timeStart).TotalMilliseconds;
                        if (diff > _overTime)//判断是否超时
                        {
                            //超时丢弃处理
                            _dataSecoundBuffer.Clear();
                            step = 0;
                            if (_dataSecoundBuffer.Count > 0)
                            {
                                _dataSecoundBuffer.RemoveAt(0);//移除丢弃
                            }
                            CLog.LogWarning("接收超时，移除第一个重新开始判断。");
                        }

                    }
                    if (_dataSecoundBuffer.Count == 0)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    switch (step)
                    {
                        case 0:
                            {
                                //判断长度是否满足要求
                                if (_dataSecoundBuffer.Count < 5)
                                {
                                    continue;
                                }

                                //判断地址是否相同
                                if (_dataSecoundBuffer[0] != _addrss)
                                {
                                    _dataSecoundBuffer.RemoveAt(0);//移除丢弃
                                    continue;
                                }
                                timeStart = DateTime.Now;//开始设置时间,超时计算
                                step = 1;
                                break;
                            }
                        case 1:
                            {

                                //判读是否达到有效的数据长度
                                if (_dataSecoundBuffer.Count < _dataSecoundBuffer[2] + 5)
                                {
                                    continue;
                                }
                                int allLen = _dataSecoundBuffer[2] + 5;
                                ushort crc = GenCRC.CumulativeSumCalculate(_dataSecoundBuffer.ToArray(), 0, (ushort)(_dataSecoundBuffer[2] + 3));
                                var low = (byte)(crc & 0xFF); //低8位
                                if (low != _dataSecoundBuffer[allLen - 2])
                                {
                                    CLog.LogWarning("接收累加和校验不正确，移除第一个重新判断。");
                                    step = 0;
                                    _dataSecoundBuffer.RemoveAt(0);
                                    continue;
                                }

                                var hig = (byte)((crc & 0xFF00) >> 8);//高8位
                                if (hig != _dataSecoundBuffer[allLen - 1])
                                {
                                    CLog.LogWarning("接收累加和校验不正确，高8bit，移除第一个重新判断。");
                                    step = 0;
                                    _dataSecoundBuffer.RemoveAt(0);
                                    continue;
                                }
                                if (RtuFrameArrived != null)
                                {
                                    var temp = new byte[_dataSecoundBuffer[2]];
                                    _dataSecoundBuffer.CopyTo(3, temp, 0, _dataSecoundBuffer[2]);
                                    var frame = new RTUFrame(_dataSecoundBuffer[0], _dataSecoundBuffer[1], temp,
                                        _dataSecoundBuffer[2], low, hig);
                                    //产生RTU生成事件。
                                    RtuFrameArrived(this, new RtuFrameArgs(frame));
                                    //移除处理后的数据
                                    _dataSecoundBuffer.RemoveRange(0, allLen);

                                }

                                step = 0;
                                continue; ;
                            }
                    }
                    //进行CRC校验

                }while (true);
            }
            catch (ThreadAbortException ex)
            {
                Thread.ResetAbort();
                CLog.LogWarning("串口接收进程::" + ex.Message);
                
            }
            catch (Exception ex)
            {                
                CLog.LogError("串口接收进程::" + ex.Message);
                ExceptionDelegate(ex);
            }
        }


    }
}
