using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using Distribution.station;

namespace TransportProtocol.Comself
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TagFrameRtu 
    {
        public UInt16 address; //地址
        public UInt16 destAddress; //源地址地址
        public byte funcode; //功能代码
        public UInt16 datalen; //数据长度
        public byte[] pData; //指向数据指针
        public byte[] pValidData; //指向发指向有效数据部分
        public bool completeFlag; //0-未完成 
    };


    /* Struct definitions */
    [StructLayout(LayoutKind.Sequential)]
    public struct ExceptionRecord
    {
        public byte has_id;
        public UInt32 id;
        public byte has_time;
        public UInt32 time;
        public byte has_functionName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] functionName;
        public byte has_line;
        public UInt32 line;
        public byte has_code;
        public UInt32 code;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct LogRecord
    {
        public byte exception_count;
        public ExceptionRecord[] exception;
    }
   

    public struct FrameRtu
    {
        public uint Address; //地址
        public uint DestAddress; //源地址地址
        public byte Funcode; //功能代码        
        public byte[] ValidData; //指向发指向有效数据部分   
    };


    /// <summary>
    /// 协议解析处理
    /// </summary>
    public class ProtocolAnylast
    {
        private const byte START_CHAR1 = 0x65; //启动字符
        private const byte START_CHAR2 = 0xA6;
        private const byte START_CHAR3 = 0xA6;
        private const byte START_CHAR4 = 0x65;
        private const byte endChar = 0x1A;    //结束字符
        private const uint BROADCAST_ADDRESS = 0x0AFF; //广播地址，后两字段，根据需要修改
        private const int RECIVE_FRAME_MAX = 2048;
        private const uint FRAME_END_LEN = 4;
        private const uint FRAME_CRC_OFFSET = 11;
        private const uint ERROR_VOLUM = 0x19;
        private const uint ERROR_NULL_PTR = 0x11;
        private const int FRAME_SOURCE_INDEX = 6;
        private const int FRAME_DEST_INDEX = 8;
        private const int FRAME_FUNCODE_INDEX = 10;
        private const int FRAME_VALID_INDEX = 11;
        private const int END_CHAR = 0x1A;
        private const int FRAME_MIN_LEN = 14;

        private static int _runCount = 0;
        private TagFrameRtu reciveRtu;
        private int step = 0;

        #region
        public int DataProtocolAnylast(ref int step, int reciveIndex,ref TagFrameRtu receRtu,List<byte> reviceFrame,UInt16 ReceRtuAddress)
        {
            var result = false;
            var list = reviceFrame;
            if (list == null)
            {
                return (int)ERROR_NULL_PTR;
            }
            if (list.Count > 0)
            {
                switch (step)
                {
                    case 0:
                        receRtu.completeFlag = false;
                        if (list.Count < FRAME_FUNCODE_INDEX)
                        {
                            return list.Count;
                        }
                        if(list.Count > FRAME_FUNCODE_INDEX)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                            list.RemoveAt(0);
                            break;
                        } 
                        var dataL = list[FRAME_SOURCE_INDEX];
                        var dataH = list[FRAME_SOURCE_INDEX + 1];
                        var adress = (dataH << 8 | dataL);
                        receRtu.address = (ushort)adress;
                        dataL = list[FRAME_DEST_INDEX];
                        dataH = list[FRAME_DEST_INDEX + 1];
                        adress = (dataH << 8 | dataL);
                        receRtu.destAddress = (ushort)adress;
                        if ((list[0] == START_CHAR1) &&
                            (list[1] == START_CHAR2) &&
                            (list[4] == START_CHAR3) &&
                            (list[5] == START_CHAR4) &&
                            (ReceRtuAddress == adress) || (BROADCAST_ADDRESS == adress) ||  (ReceRtuAddress == 0))
                        {
                            dataL = list[2];
                            dataH = list[3];
                            receRtu.datalen = (ushort)(dataH << 8 | dataL);
                            if(receRtu.datalen <= RECIVE_FRAME_MAX)//
                            {
                                step = 1;
                            }
                        }
                        else
                        {
                            list.RemoveAt(0); 
                        }
                        break;
                    case 1:
                        if (list.Count >= receRtu.datalen + FRAME_END_LEN)
                        {
                            if ((END_CHAR != list[receRtu.datalen + FRAME_MIN_LEN - 1]))
                            {
                                list.RemoveAt(0);
                            }
                            else
                            {
                                int count = (int)(receRtu.datalen + FRAME_CRC_OFFSET);
                                var frameIntrix = new byte[count];
                                reviceFrame.CopyTo(0, frameIntrix, 0, count);
                                var crc = GenCRC.CRC16(frameIntrix, (ushort)count);
                                var crcL = reviceFrame[count];
                                var crcH = reviceFrame[count + 1];
                                if (crc == ((uint)crcH << 8 | crcL))
                                {
                                    receRtu.pData = new byte[receRtu.datalen + FRAME_MIN_LEN];
                                    list.CopyTo(0, receRtu.pData, 0, receRtu.datalen + FRAME_MIN_LEN);
                                    receRtu.pValidData = new byte[receRtu.datalen];
                                    list.CopyTo(FRAME_VALID_INDEX, receRtu.pValidData, 0, receRtu.datalen);
                                    receRtu.funcode = reviceFrame[FRAME_FUNCODE_INDEX];// 功能码                          
                                    receRtu.completeFlag = true;
                                    //处理完一帧数据后删除
                                    list.RemoveRange(0, receRtu.datalen + FRAME_MIN_LEN); 
                                }
                                else
                                {
                                    list.RemoveAt(0);//丢弃一个
                                }
                            }
                            step = 0; 
                        }
                        break;
                    default:
                        step = 0;
                        reciveIndex = 0;
                        break;
                }
            }
            return list.Count;
        }

        /// <summary>
        /// </summary>
        /// <param name="inData">收到的数据</param>
        /// <param name="inLen">收到的数据长度</param>
        /// <param name="usedLen">使用的数据长度</param>
        /// <param name="rtu">解析之后的rtu</param>
        /// <param name="address">解析之后rtu的地址</param>
        /// <returns></returns>
        public byte  ProtocolAnylastDeal(byte[] inData, int inLen, ref int usedLen, ref TagFrameRtu rtu,UInt16 address)
        {
            var handle = new List<byte>();
            var i = 0;
            var resideLen = 0;
            var lastResideLen = 0;
            for (i = 0; i < inLen; i++)
            {
                handle.Add(inData[i]); 
            }
            usedLen = i;
            do
            {
                rtu.completeFlag = false;
                resideLen = DataProtocolAnylast(ref step, 3,ref rtu, handle,address);
                if(rtu.completeFlag)
                {
                    return 0;
                }
                //若相等，则退出
                if (lastResideLen == resideLen)
                {
                    break;
                }
                lastResideLen = resideLen;
            } while (resideLen > 0);
            return 0;
        }
        #endregion

        public bool Deal(byte[] indata, ref int usedLen, ref FrameRtu rtu, UInt16 address)
        {
            try
            {
                int inLen = (ushort)indata.Length;
                int outLen = 0;
                byte resut = ProtocolAnylastDeal(indata, inLen, ref outLen, ref reciveRtu,address);
                usedLen = outLen;
                if (resut != 0)
                { 
                    throw new Exception("C DLL处理异常.Code:" + resut.ToString());
                }

                if (reciveRtu.completeFlag == true)
                {
                    rtu.Address = reciveRtu.address;
                    rtu.DestAddress = reciveRtu.destAddress;
                    rtu.Funcode = reciveRtu.funcode;
                    rtu.ValidData = new byte[reciveRtu.datalen];
                    for (int i = 0; i < reciveRtu.datalen; i++)
                    {
                        rtu.ValidData[i] = reciveRtu.pValidData[i];
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public ProtocolAnylast(UInt16 addres)
        {
            if (_runCount > 1)//TODO：可以升级为动态加载,以便支持多实例
            {
                throw new Exception("ProtocolAnylast 只能有一个实例");
            }
        }

        public static byte[] Serialize(StationMessage model)
        {
            try
            {
                //涉及格式转换，需要用到流，将二进制序列化到流中  
                using (MemoryStream ms = new MemoryStream())
                {
                    //使用ProtoBuf工具的序列化方法  
                    ProtoBuf.Serializer.Serialize<StationMessage>(ms, model);
                    //定义二级制数组，保存序列化后的结果  
                    byte[] result = new byte[ms.Length];
                    //将流的位置设为0，起始点  
                    ms.Position = 0;
                    //将流中的内容读取到二进制数组中  
                    ms.Read(result, 0, result.Length);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StationMessage DeSerialize(byte[] msg)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //将消息写入流中  
                    ms.Write(msg, 0, msg.Length);
                    //将流的位置归0  
                    ms.Position = 0;
                    //使用工具反序列化对象  
                    StationMessage result = ProtoBuf.Serializer.Deserialize<StationMessage>(ms);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
