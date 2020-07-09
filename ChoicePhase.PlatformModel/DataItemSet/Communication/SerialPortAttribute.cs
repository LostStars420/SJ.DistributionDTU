using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ChoicePhase.PlatformModel.Communication
{
    /// <summary>
    /// 串口参数
    /// </summary>
    public class SerialPortAttribute
    {
        /// <summary>
        /// 公共串口号
        /// </summary>
        private int _commonPortNum;
        /// <summary>
        /// 串口号
        /// </summary>
        public string CommonPort
        {
            get
            {
                return "COM" + _commonPortNum.ToString();
            }

            set
            {
                var num = value.ToUpper().Remove(0, 3);
                int.TryParse(num, out _commonPortNum);      
            }
        }

        public int CommonPortNum
        {
            get
            {
                return _commonPortNum;
            }
            set
            {
                _commonPortNum = value;
            }
        }


        /// <summary>
        /// 波特率
        /// </summary>

        public int Baud
        {
            get;
            set;
        }
        /// <summary>
        /// 数据位
        /// </summary>

        public int DataBit
        {
            get;
            set;
        }

        /// <summary>
        /// 奇偶校验位
        /// </summary>

        public Parity ParityBit
        {
            get;
            set;
        }

        public int ParityBitMark
        {
            get
            {
                return GetParityIndex(ParityBit);
            }
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBit
        {
            get;
            set;
        }

        /// <summary>
        /// 停止位索引mark
        /// </summary>
        public int StopBitMark
        {
            get
            {
                return GetStopBitIndex(StopBit);
            }
        }

        /// <summary>
        /// 根据代号Mark获取就bit
        /// </summary>
        /// <param name="parityBit"></param>
        /// <returns></returns>
        private Parity GetParityBit(int parityIndex)
        {
            switch (parityIndex)
            {
                case 0:
                    {
                        return Parity.None;                       
                    }
                case 1:
                    {
                        return Parity.Odd;                       
                    }
                case 2:
                    {
                        return Parity.Even;                        
                    }
                case 3:
                    {
                        return Parity.Space;                        
                    }
                case 5:
                    {
                        return Parity.Mark;                       
                    }
                default:
                    {
                        return Parity.None; 
                    }
            }
        }

        /// <summary>
        /// 根据ParityBit获取索引
        /// </summary>
        /// <param name="parityBit"></param>
        /// <returns></returns>
        private int GetParityIndex(Parity parityBit)
        {
            switch (parityBit)
            {
                case Parity.None:
                    {
                        return 0;
                    }
                case Parity.Odd:
                    {
                        return 1;
                    }
                case Parity.Even:
                    {
                        return 2;
                    }
                case Parity.Space:
                    {
                        return 3;
                    }
                case Parity.Mark:
                    {
                        return 4;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        /// <summary>
        /// 根据索引mark获取StopBits
        /// </summary>
        /// <param name="stopIndex"></param>
        /// <returns></returns>
        private StopBits GetStopBit(int stopIndex)
        {
            switch (stopIndex)
            {

                case 0:
                    {
                        return  StopBits.None;                       
                    }

                case 1:
                    {
                        return  StopBits.One;                        
                    }
                case 2:
                    {
                        return  StopBits.Two;                       
                    }
                case 3:
                    {
                        return  StopBits.OnePointFive;                      
                    }
                default:
                    {
                        return  StopBits.One;                       
                    }
            }
        }

        /// <summary>
        /// 根据停止位获取索引
        /// </summary>
        /// <param name="stopBit"></param>
        /// <returns></returns>
        private int  GetStopBitIndex(StopBits stopBit)
        {
            switch (stopBit)
            {

                case StopBits.None:
                    {
                        return 0;
                    }

                case StopBits.One:
                    {
                        return 1;
                    }
                case StopBits.Two:
                    {
                        return 2;
                    }
                case StopBits.OnePointFive:
                    {
                        return 3;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }
        /// <summary>
        /// 串口属性初始化
        /// </summary>
        /// <param name="comNum"></param>
        /// <param name="baud"></param>
        /// <param name="databit"></param>
        /// <param name="parityBit"></param>
        /// <param name="stopbit"></param>
        public SerialPortAttribute(int comNum, int baud, int databit, int parityBit, int stopbit)
        {
            string comstr = "COM" + comNum.ToString();
            //计算当前计算机的端口名
            var ports = SerialPort.GetPortNames();
            bool isExist = false;
            foreach (var m in ports)
            {
                if (comstr == m)
                {
                    isExist = true;
                }
            }
            if (!isExist)
            {
                //throw new ArgumentNullException("指定的串口号，不存在");
                if (ports.Length != 0)
                {
                   comstr = ports[0];
                }
            }
            CommonPort = comstr;

            if (baud < 0)
            {
                throw new ArgumentNullException("波特率不能为负");
            }
            Baud = baud;

            if (databit < 0)
            {
                throw new ArgumentNullException("波特率不能为负");
            }
            DataBit = databit;

            switch (parityBit)
            {
                case 0:
                    {
                        ParityBit = Parity.None;
                        break;
                    }
                case 1:
                    {
                        ParityBit = Parity.Odd;
                        break;
                    }
                case 2:
                    {
                        ParityBit = Parity.Even;
                        break;
                    }
                case 3:
                    {
                        ParityBit = Parity.Space;
                        break;
                    }
                case 5:
                    {
                        ParityBit = Parity.Mark;
                        break;
                    }
                default:
                    {
                        ParityBit = Parity.None;
                        break;
                    }
            }

            switch (stopbit)
            {

                case 0:
                    {
                        StopBit = StopBits.None;
                        break;
                    }

                case 1:
                    {
                        StopBit = StopBits.One;
                        break;
                    }
                case 2:
                    {
                        StopBit = StopBits.Two;
                        break;
                    }
                case 3:
                    {
                        StopBit = StopBits.OnePointFive;
                        break;
                    }
                default:
                    {
                        StopBit = StopBits.One;
                        break;
                    }
            }
        }

        /// <summary>
        /// 串口属性初始化
        /// </summary>
        /// <param name="comNum"></param>
        /// <param name="baud"></param>
        /// <param name="databit"></param>
        /// <param name="parityBit"></param>
        /// <param name="stopbit"></param>

        public SerialPortAttribute(string comNum, int baud, int databit, Parity parityBit, StopBits stopbit)
        {
            CommonPort = comNum;
            Baud = baud;
            DataBit = databit;
            ParityBit = parityBit;
            StopBit = stopbit;
        }
    }
}
