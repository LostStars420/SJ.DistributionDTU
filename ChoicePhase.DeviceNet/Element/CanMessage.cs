using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.DeviceNet.Element
{
    /// <summary>
    /// CAN消息
    /// </summary>
    public class CanMessage
    {
        /// <summary>
        /// ID
        /// </summary>
        public UInt16 ID
        {
            get;
            set;
        }

        public byte[] Data
        {
            get;
            set;
        }
        /// <summary>
        /// 数据长度
        /// </summary>
        public byte DataLen
        {
            get;
            set;
        }

         


        /// <summary>
        /// 初始化CAN帧
        /// </summary>
        /// <param name="id">ID标识符</param>
        /// <param name="data">发送的数据</param>
        /// <param name="start">开始索引</param>
        /// <param name="len">数据长度</param>
        public CanMessage(UInt16 id, byte[] data, int start, int len)
        {
            if((len > 8) || (len == 0))
            {
                throw new ArgumentOutOfRangeException("数据长度应大于0，小于等于8");
            }
            ID = id;
            Data = new byte[len];
            Array.Copy(data, start, Data, 0, len);
            DataLen = (byte)len;
        }

    
 
    }
}
