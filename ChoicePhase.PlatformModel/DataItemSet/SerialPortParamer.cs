using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 串口参数，数据位，波特率，奇偶校验，停止位
    /// </summary>
    public class SerialPortParamer<T>
    {
        T _paramer;
        /// <summary>
        /// 获取串口参数
        /// </summary>
        public T Paramer
        {
            get
            {
                return _paramer;
            }
        }

        /// <summary>
        /// 初始化一个串口参数
        /// </summary>
        public SerialPortParamer(T paramer)
        {
            _paramer = paramer;
        }
    }

}
