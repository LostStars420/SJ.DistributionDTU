using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.PlatformModel.Communication
{
    /// <summary>
    /// 串口数据
    /// </summary>
    public class SerialDataEvent :EventArgs
    {
        /// <summary>
        /// 字节数据
        /// </summary>
        public byte[] SerialData;

        /// <summary>
        /// 串口数据
        /// </summary>
        /// <param name="serialData">串口数据</param>
        public SerialDataEvent(byte[] serialData)
        {
            SerialData = serialData;
        }

    }
}
