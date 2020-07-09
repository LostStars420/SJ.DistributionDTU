using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.Modbus
{

    public class ReciveRtuFrame
    {
        private Tuple<bool, DateTime> reciveFlagTuple;
        public Tuple<bool, DateTime> ReciveFlagTuple
        {
            get { return reciveFlagTuple; }
            set { reciveFlagTuple = value;}
        }

        /// <summary>
        /// 从字节流中判断是否为相应帧
        /// </summary>
        /// <param name="reciveData">接收的字节流</param>
        /// <param name="sendFrame">发送的帧数据</param>
        /// <returns></returns>
        public bool JudgeResponseFrame(List<byte> reciveData, RTUFrame sendFrame,RTUFrame reciveFrame)
        {
            //首先判断这一帧是否已经完成响应
            //if (!sendFrame.CompleteFlag)
            { //测试注销
                //判断 查询地址 与 发送地址 是否一致
                if (reciveData[0] != sendFrame.Address)
                    return false;

                if (reciveData[1] != sendFrame.Function)
                    return false;
                int len = reciveData[2]; //获取数据字节数

                //再紧接着进行CRC校验
                ushort crc = GenCRC.CRC16(reciveData.ToArray(), (ushort)(len + 3));
                
                var low = (byte)(crc & 0xFF); //低8位
                if (low != reciveData[2 + len - 1])
                {
                    return false;
                }

                var hig = (byte)(crc & 0xFF00 >> 8);//高8位
                if (low != reciveData[2 + len ])
                {
                    return false;
                }
                List<byte> tmp = new List<byte>();
                tmp.AddRange(reciveData);
                tmp.RemoveRange(0, 3); //去除头
                tmp.RemoveRange(len, 2); //去除尾巴
                reciveFrame = new RTUFrame(reciveData[0], reciveData[1], tmp.ToArray(), (byte)len);

                sendFrame.CompleteFlag = true; //防止重复响应
                ReciveFrame.Time = DateTime.Now;
                return true;
            }
            //else
            //{
            //    return false;
            //}
            
        }

        //适应一个一个字节的接收
        private byte GetByte()
        {
            if (ReciveAbyte != null)
            {
                return ReciveAbyte();
            }
            
            return 0x00;
        }
        public  Func<byte>  ReciveAbyte;
        public RTUFrame ReciveFrame;
        public Func<int> ReciveBuffer;
        public byte JudgeAddress;
        //private DateTime recivedTime;


        /// <summary>
        /// 获取字节数据，并进帧平判断
        /// </summary>
        /// <param name="sendFrame">帧</param>
        /// <returns>true--完整的一帧， false--不是完整一帧</returns>
        public bool JudgeGetByte(RTUFrame sendFrame)
        {
            //首先判断这一帧是否已经完成响应
          //  if (!sendFrame.CompleteFlag)
            {
                //判断 查询地址 与 发送地址 是否一致
              //  if (GetByte() != sendFrame.Address)
              //      return false;
                //放宽范围
                byte addr = GetByte();
                if (!(addr == JudgeAddress))
                {
                    return false;
                }
                
                reciveFlagTuple = new Tuple<bool, DateTime>(true, DateTime.Now);

                //if (GetByte() != sendFrame.Function)
                //    return false;
                var fun = GetByte();
               
                int len = GetByte(); //获取数据字节数

                byte[] array = new byte[len + 3];
                
                //array[0] = sendFrame.Address;
                array[0] = addr;
                //array[1] = sendFrame.Function;
                array[1] = fun;
                array[2] = (byte)len;
                for (int i = 0; i < len; i++ )
                {
                    array[i + 3] = GetByte();
                }

                ushort crc = GenCRC.CRC16(array, (ushort)(len + 3));
                //ushort crc = (ushort)(len + 5);
                byte crcl = (byte)(crc & 0x00FF);
                byte crch = (byte)(crc & 0xFF00 >> 8);
                var low = (byte)(crc & 0xFF); //低8位
                var getbyte = GetByte();
                if (crcl != getbyte)
                {
                    return false;
                }
               
                var hig = (byte)(crc & 0xFF00 >> 8);//高8位
                getbyte = GetByte();
                if (crch != getbyte)
                {
                    return false;
                }
                List<byte> tmp = new List<byte>();
                tmp.AddRange(array);
                tmp.RemoveRange(0, 3); //去除头

                ReciveFrame = new RTUFrame(array[0], array[1], tmp.ToArray(),
                    (byte)len, low, hig);
                reciveFlagTuple = new Tuple<bool,DateTime>(false, DateTime.Now);
                sendFrame.CompleteFlag = true; //防止重复响应
                ReciveFrame.Time = DateTime.Now;
                return true;

            }
          //  else
          //  {
          //      return false;
          //  }
        }


        /// <summary>
        ///附加错误代码的字节检测测试
        /// </summary>
        /// <param name="sendFrame">接收</param>
        /// <returns>错误代码</returns>
        public ErrorCodeEnum JudgeGetByteErrorCode(RTUFrame sendFrame)
        {
            //首先判断这一帧是否已经完成响应
            //  if (!sendFrame.CompleteFlag)
            {
                //判断 查询地址 与 发送地址 是否一致
                //  if (GetByte() != sendFrame.Address)
                //      return false;
                //放宽范围
                byte addr = GetByte();
                if (!(addr == JudgeAddress))
                {
                    return ErrorCodeEnum.ErrorAddress;
                }

                reciveFlagTuple = new Tuple<bool, DateTime>(true, DateTime.Now);

                //if (GetByte() != sendFrame.Function)
                //    return false;
                var fun = GetByte();

                int len = GetByte(); //获取数据字节数

                byte[] array = new byte[len + 3];

                //array[0] = sendFrame.Address;
                array[0] = addr;
                //array[1] = sendFrame.Function;
                array[1] = fun;
                array[2] = (byte)len;
                for (int i = 0; i < len; i++)
                {
                    array[i + 3] = GetByte();
                }

                ushort crc = GenCRC.CRC16(array, (ushort)(len + 3));
                //ushort crc = (ushort)(len + 5);
                byte crcl = (byte)(crc & 0x00FF);
                byte crch = (byte)((crc & 0xFF00) >> 8);
                var low = (byte)(crc & 0xFF); //低8位
                var getbyte = GetByte();
                if (crcl != getbyte)
                {
                    return ErrorCodeEnum.ErrorCRCL;
                }

                var hig = (byte)(crc & 0xFF00 >> 8);//高8位
                getbyte = GetByte();
                if (crch != getbyte)
                {
                    return ErrorCodeEnum.ErrorCRCH;
                }
                List<byte> tmp = new List<byte>();
                tmp.AddRange(array);
                tmp.RemoveRange(0, 3); //去除头

                ReciveFrame = new RTUFrame(array[0], array[1], tmp.ToArray(),
                    (byte)len, low, hig);
                reciveFlagTuple = new Tuple<bool, DateTime>(false, DateTime.Now);
                sendFrame.CompleteFlag = true; //防止重复响应

                ReciveFrame.Time = DateTime.Now;
                return ErrorCodeEnum.Correct;

            }
            //  else
            //  {
            //      return false;
            //  }
        }
       

        /// <summary>
        /// 对队列数据进行判断
        /// </summary>
        /// <param name="sendFrame"></param>
        /// <param name="quneRecive"></param>
        /// <returns></returns>
        public  int  JudgeQuneData(RTUFrame sendFrame, Queue<byte> quneRecive, int stepQune)
        {
            try
            {
                while (true)
                {
                    switch (stepQune)
                    {

                        //判断数据长度是否大于等于5 (完整帧最小长度)
                        case 0:
                            {
                                if (quneRecive.Count >= 5)
                                {
                                    stepQune = 1;
                                    continue;
                                }
                                return 0;
                            }
                        //判断是否为地址码
                        case 1:
                            {
                                //出队

                                var read = quneRecive.Dequeue();

                                //为地址码,则读取功能码与长度
                                if (read == JudgeAddress)
                                {
                                    sendFrame.Address = read;  //设置地址码
                                    sendFrame.Function = quneRecive.Dequeue(); //设置功能码
                                    sendFrame.DataLen = quneRecive.Dequeue(); //设置数据长度

                                    stepQune = 2;
                                   
                                    continue;
                                }
                                else
                                {//不为地址码，则丢弃
                                    stepQune = 0;
                                    continue;
                                }

                            }
                        case 2: //判断接收数据是否大于等于数据长度加校准长度
                            {
                                if (quneRecive.Count >= sendFrame.DataLen + 2)
                                {
                                    if (judgeDataPart(sendFrame, quneRecive))
                                    {
                                        sendFrame.Time = DateTime.Now;
                                        return 3; //完成标志
                                    }
                                    else
                                    {
                                        return 0; //重新进行验证
                                    }
                                }
                                else
                                {
                                    //数据长度不足够返回当前步骤
                                    return 2;
                                }

                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                
            }
            catch (System.InvalidOperationException ex)
            {
                return stepQune;
            }
        

        }

        /// <summary>
        /// 判断数据是否满足一帧要求
        /// </summary>
        /// <param name="sendFrame">需要存储的数据帧</param>
        /// <param name="quneRecive">出队参数</param>
        /// <returns>true-符合帧要求，false--不符合</returns>

        private bool judgeDataPart(RTUFrame sendFrame, Queue<byte> quneRecive)
        {            
            //队列数据长度大于已知数据长度
            if (quneRecive.Count >= sendFrame.DataLen + 2)
            {
                   sendFrame.Frame = new byte[sendFrame.DataLen + 5];
                   sendFrame.FrameData = new byte[sendFrame.DataLen];
                   sendFrame.Frame[0] = sendFrame.Address;
                   sendFrame.Frame[1] = sendFrame.Function;
                   sendFrame.Frame[2] = sendFrame.DataLen;

                   //获取数据
                   for (int i = 0; i < sendFrame.DataLen; i++)
                   {
                       sendFrame.Frame[i + 3] = quneRecive.Dequeue(); 
                       sendFrame.FrameData[i] =  sendFrame.Frame[i + 3] ;
                   }
                  //可先校验
                   var cl = quneRecive.Dequeue(); //获取校验位低8bit
                   var ch = quneRecive.Dequeue();  //获取校验位高8bit
                   sendFrame.Frame[sendFrame.DataLen + 3] = cl;
                   sendFrame.Frame[sendFrame.DataLen + 4] = ch;

                   ushort crc = GenCRC.CRC16(sendFrame.Frame, (ushort)(sendFrame.DataLen + 3));
                   byte crcl = (byte)(crc & 0x00FF);
                   byte crch = (byte)((crc & 0xFF00) >> 8);
                   if (crcl != cl)
                   {
                       return false;
                   }
                   if (crch != ch)
                   {
                       return false;
                   }

                   sendFrame.SaveFlag = true;
                   sendFrame.ErrorFlag = ErrorCodeEnum.Correct;

                   return true;
            }
            else
            {
                return false;
            }
        
        }
        /// <summary>
        /// 对于租数据进行判读
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="rtuf"></param>
        /// <returns></returns>
        public static ErrorCodeEnum JudgeFrame(byte[] frame,out RTUFrame rtuf, byte reciveAddress)
        {
            //数据长度不符合最小长度
            rtuf = new RTUFrame(reciveAddress, 0);
            if (frame.Length < 5)
            {
                
                return ErrorCodeEnum.ErrorLength;
            }
            if (!(frame[0] == reciveAddress))
            {
                return ErrorCodeEnum.ErrorAddress;
            }

            var fun = frame[1];  //长度数 
            int len = frame[2]; //获取数据字节数
            if (frame.Length < len + 5)
            {
                return ErrorCodeEnum.ErrorLength;
            }

            ushort crc = GenCRC.CRC16(frame, (ushort)(len + 3));
   
            byte crcl = (byte)(crc & 0x00FF);
            byte crch = (byte)(crc & 0xFF00 >> 8);
            var low = (byte)(crc & 0xFF); //低8位
            var getbyte = frame[len+3];
            if (crcl != getbyte)
            {
                return ErrorCodeEnum.ErrorCRCL;
            }

            var hig = (byte)(crc & 0xFF00 >> 8);//高8位
            getbyte = frame[len + 4];
            if (crch != getbyte)
            {
                return ErrorCodeEnum.ErrorCRCH;
            }
            List<byte> tmp = new List<byte>();
            tmp.AddRange(frame);
            tmp.RemoveRange(0, 3); //去除头

            rtuf = new RTUFrame(frame[0], frame[1], tmp.ToArray(),
                (byte)len, low, hig);
            rtuf.Time = DateTime.Now;
            return ErrorCodeEnum.Correct;
        }
    }
}
