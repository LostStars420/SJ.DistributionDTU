using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.Modbus
{

    public class RTUFrame
    {
        
        private byte[] frame;
        /// <summary>
        /// 完整一帧数据
        /// </summary>
        public byte[] Frame
        {
             get { return frame; }
            set { frame = value; }
        }

        private byte address;
        public byte Address
        {
             get { return address; }
            set {address = value; }
        }

        private byte function;
        public byte Function
        {
             get { return function; }
            set { function = value; }
        }
        private byte[] framedata;
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

        private byte  dataLen;
        public byte DataLen
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
        public  RTUFrame(byte addr, byte funcode,
                        byte[] sendData, byte datalen)
        {
            //addrss(1) funcode(1) + bytecount(1) sendData(datalen) CRC(2)
            int len = 1 + 1 + 1 + datalen + 2;
            frame = new byte[len];

            this.dataLen = datalen;
            this.address = addr;
            this.function = (byte)funcode;
            this.framedata = new byte[datalen];
            for (int i = 0; i < datalen;  i++)
            {
                this.framedata[i] = sendData[i];
                frame[i + 3] = sendData[i];
            }
            frame[0] = addr;
            frame[1] = (byte)funcode;
            frame[2] = (byte)datalen;

            ushort crc = GenCRC.CRC16(frame, (ushort)(len -2));
            //ushort crc = (ushort)len;
            frame[len - 2] = (byte)(crc & 0xFF); //低8位
            frame[len - 1] = (byte)((crc & 0xFF00) >> 8);//高8位

            completeFlag = false;
            saveFlag = false;
        }
        public RTUFrame(byte addr, byte funcode,
                        byte[] sendData, byte datalen,  byte crcLi, byte crcHo)
        {
            //addrss(1) funcode(1) + bytecount(1) sendData(datalen) CRC(2)
            int len = 1 + 1 +1 + datalen + 2;
            frame = new byte[len];

            this.dataLen = datalen;
            this.address = addr;
            this.function = (byte)funcode;
            this.framedata = new byte[datalen];
            for (int i = 0; i < datalen; i++)
            {
                this.framedata[i] = sendData[i];
                frame[i + 3] = sendData[i];
            }
            frame[0] = addr;
            frame[1] = (byte)funcode;
            frame[2] = (byte)datalen;


            frame[len - 2] = crcLi; //低8位
            frame[len - 1] = crcHo;//高8位

            completeFlag = false;
        }
        /// <summary>
        /// 生成发送帧，适用于只有功能代码的类型.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="funcode"></param>
        public RTUFrame(byte addr, byte funcode)
        {
            //addrss(1) funcode(1) + bytecount(1) sendData(datalen) CRC(2)
            byte datalen = 0;
            int len = 1 + 1 +1 + datalen + 2;
            frame = new byte[len];

            this.dataLen = datalen;
            this.address = addr;
            this.function = (byte)funcode;
            this.framedata = new byte[datalen];
          
            frame[0] = addr;
            frame[1] = (byte)funcode;
            frame[2] = (byte)datalen;

            ushort crc = GenCRC.CRC16(frame, (ushort)(len - 2));
            //ushort crc = (ushort)len;
            frame[len - 2] = (byte)(crc & 0xFF); //低8位
            frame[len - 1] = (byte)((crc & 0xFF00) >> 8);//高8位

            completeFlag = false;
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
