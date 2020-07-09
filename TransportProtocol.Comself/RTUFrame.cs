using System;

namespace TransportProtocol.Comself
{
    /// <summary>
    /// 自定义帧
    /// </summary>

    public class RTUFrame
    {
        private const byte startChar1 = 0x65; //启动字符
        private const byte startChar2 = 0xA6;
        private const byte startChar3 = 0xA6;
        private const byte startChar4 = 0x65;
        private const byte endChar = 0x1A; //结束字符

        private byte[] frame;
        /// <summary>
        /// 完整一帧数据
        /// </summary>
        public byte[] Frame
        {
             get { return frame; }
            set { frame = value; }
        }
        private ushort _sourceAddress;
        /// <summary>
        /// 16bit地址
        /// </summary>
        public ushort SourceAddress
        {
            get { return _sourceAddress; }
            set { _sourceAddress = value; }
        }


        private ushort address;
        /// <summary>
        /// 16bit地址
        /// </summary>
        public ushort Address
        {
             get { return address; }
            set {address = value; }
        }

        private byte function;
        /// <summary>
        /// 功能码
        /// </summary>
        public byte Function
        {
             get { return function; }
            set { function = value; }
        }
        private byte[] framedata;
        /// <summary>
        /// 帧里数据部分长度
        /// </summary>
        public byte[] FrameData
        {
            get { return framedata; }
            set { framedata = value; }

        }


        private bool completeFlag;
        public bool CompleteFlag
        {
            get { return completeFlag; }
            set { completeFlag = value; }
        }

        private ushort dataLen;
        /// <summary>
        /// 数据长度
        /// </summary>
        public ushort DataLen
        {
            get { return dataLen; }
            set { dataLen = value; }
        }

        //存储完成标志
        private bool saveFlag;
        public bool SaveFlag
        {
            get { return saveFlag; }
            set { saveFlag = value; }
        }

        /// <summary>
        /// 帧的时间标志
        /// </summary>
        public DateTime Time;
        public  RTUFrame(ushort sourceAddre, ushort addr, byte funcode,
                        byte[] sendData, ushort datalen)
        {
            //启动字符2+ 数据长度2 + 启动字符2 + 源头地址2 + 地址2 + 功能码1+ 数据n+ 校验2+结束符1
            int len = 14 + datalen;
            frame = new byte[len];

            this.dataLen = datalen;
            this.address = addr;
            this.function = (byte)funcode;
            this.framedata = new byte[datalen];

            //数据长度为0，则跳过
            if (datalen != 0)
            {
                Array.Copy(sendData, this.framedata, datalen);
                Array.Copy(sendData, 0, frame, 11, datalen);
            }
            int index = 0;
            frame[index++] = startChar1;
            frame[index++] = startChar2;
            frame[index++] = (byte)datalen;
            frame[index++] = (byte)(datalen >> 8);
            frame[index++] = startChar3;
            frame[index++] = startChar4;
            frame[index++] = (byte)sourceAddre;
            frame[index++] = (byte)(sourceAddre >> 8);
            frame[index++] = (byte)addr;
            frame[index++] = (byte)(addr >> 8);
            frame[index++] = funcode;
            ushort crc = GenCRC.CRC16(this.frame, (ushort)(len -3));
            //ushort crc = (ushort)len;
            frame[len - 3] = (byte)(crc & 0xFF); //低8位
            frame[len - 2] = (byte)((crc & 0xFF00) >> 8);//高8位
            frame[len - 1] = endChar;

            completeFlag = false;
            saveFlag = false;
        }
        public RTUFrame(ushort addr, byte funcode,
                        byte[] sendData, ushort datalen) : this(0, addr, funcode, sendData, datalen)
        {

        }
        /// <summary>
        /// 生成发送帧，适用于只有功能代码的类型.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="funcode"></param>
        public RTUFrame(ushort addr, byte funcode) : this(addr, funcode, null, 0)
        {
           
        }
        private RTUFrame()
        {

        }

        private ErrorCodeEnum errorFlag;
        public ErrorCodeEnum ErrorFlag
        {
            get { return errorFlag; }
            set { errorFlag = value; }
        }      
    }
}
