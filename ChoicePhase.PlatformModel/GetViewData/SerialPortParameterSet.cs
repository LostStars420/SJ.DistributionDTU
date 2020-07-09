using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using ChoicePhase.PlatformModel.Communication;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.Helper;

namespace Monitor.DASModel.GetViewData
{
    /// <summary>
    /// 串口属性
    /// </summary>
    public class SerialPortParameterSet : ObservableObject
    {
        private int selectedIndexCommonPort;

        public int SelectedIndexCommonPort
        {
            get
            {
                return selectedIndexCommonPort;
            }
            set
            {
                selectedIndexCommonPort = value;
                RaisePropertyChanged("SelectedIndexCommonPort");
            }
        }

        private int selectedIndexBaud;
        public int SelectedIndexBaud
        {
            get
            {
                return selectedIndexBaud;
            }
            set
            {
                selectedIndexBaud = value;
                RaisePropertyChanged("SelectedIndexBaud");
            }
        }

        private int selectedIndexDataBit;

        public int SelectedIndexDataBit
        {
            get
            {
                return selectedIndexDataBit;
            }
            set
            {
                selectedIndexDataBit = value;
                RaisePropertyChanged("SelectedIndexDataBit");
            }
        }


        private int selectedIndexStopBit;
        public int SelectedIndexStopBit
        {
            get
            {
                return selectedIndexStopBit;
            }
            set
            {
                selectedIndexStopBit = value;
                RaisePropertyChanged("SelectedIndexStopBit");
            }
        }

        private int selectedIndexParity;

        public int SelectedIndexParity
        {
            get
            {
                return selectedIndexParity;
            }
            set
            {
                selectedIndexParity = value;
                RaisePropertyChanged("SelectedIndexParity");
            }
        
        
        
        }



        /// <summary>
        /// 波特率
        /// </summary>
        public ObservableCollection<SerialPortParamer<int>> Baud
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据位
        /// </summary>
        public ObservableCollection<SerialPortParamer<int>> DataBit
        {
            get;
            private set;
        }

        /// <summary>
        /// 校验位
        /// </summary>
        public ObservableCollection<SerialPortParamer<Parity>> ParityBit
        {
            get;
            private set;
        }



        /// <summary>
        /// 停止位
        /// </summary>
        public ObservableCollection<SerialPortParamer<StopBits>> StopBit
        {
            get;
            private set;
        }



        /// <summary>
        /// 串口号
        /// </summary>
        public ObservableCollection<SerialPortParamer<String>> CommonPort
        {
            get;
            private set;
        }


        /// <summary>
        /// 最后一次记录
        /// </summary>
        public SerialPortAttribute LastRecord;


        /// <summary>
        /// 获取端口属性
        /// </summary>
        /// <returns></returns>
        public SerialPortAttribute GetSelectedPortAttribute()
        {
            return new SerialPortAttribute(CommonPort[SelectedIndexCommonPort].Paramer,
                Baud[SelectedIndexBaud].Paramer, DataBit[SelectedIndexDataBit].Paramer,
                                ParityBit[SelectedIndexParity].Paramer, StopBit[SelectedIndexStopBit].Paramer);
        }


        /// <summary>
        /// 获取索引，通过ToString() 进行比较
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="select">选择值</param>
        /// <returns>如果有返回相应索引，否则返回0</returns>
        public int GetIndex<T>(ObservableCollection<SerialPortParamer<T>> list, T select)
            
        {
           for(int i = 0; i <　list.Count; i++)
           {
               if (list[i].Paramer.ToString() == select.ToString())
               {
                   return i;
               }
           }
           return 0;
        }
        /// <summary>
        /// 保存最新的串口记录
        /// </summary>
        public void SaveLastPortRecod()
        {
            XMLOperate.WriteLastPortRecod(GetSelectedPortAttribute());
        }

        

        /// <summary>
        /// 初始化一个串口参数合集
        /// </summary>
        public SerialPortParameterSet()
        {
            try
            {
                Baud = new ObservableCollection<SerialPortParamer<int>>();
                Baud.Add(new SerialPortParamer<int>(1200));
                Baud.Add(new SerialPortParamer<int>(2400));
                Baud.Add(new SerialPortParamer<int>(4800));
                Baud.Add(new SerialPortParamer<int>(9600));
                Baud.Add(new SerialPortParamer<int>(14400));
                Baud.Add(new SerialPortParamer<int>(28800));
                Baud.Add(new SerialPortParamer<int>(38400));
                Baud.Add(new SerialPortParamer<int>(56000));
                Baud.Add(new SerialPortParamer<int>(57600));
                Baud.Add(new SerialPortParamer<int>(115200));


                DataBit = new ObservableCollection<SerialPortParamer<int>>();
                DataBit.Add(new SerialPortParamer<int>(5));
                DataBit.Add(new SerialPortParamer<int>(6));
                DataBit.Add(new SerialPortParamer<int>(7));
                DataBit.Add(new SerialPortParamer<int>(8));

                ParityBit = new ObservableCollection<SerialPortParamer<Parity>>();
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.Even));
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.Odd));
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.Mark));
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.Space));
                ParityBit.Add(new SerialPortParamer<Parity>(Parity.None));

                StopBit = new ObservableCollection<SerialPortParamer<StopBits>>();
                StopBit.Add(new SerialPortParamer<StopBits>(StopBits.One));
                StopBit.Add(new SerialPortParamer<StopBits>(StopBits.OnePointFive));
                StopBit.Add(new SerialPortParamer<StopBits>(StopBits.Two));


                CommonPort = new ObservableCollection<SerialPortParamer<string>>();
                foreach (string s in SerialPort.GetPortNames())
                {
                    CommonPort.Add(new SerialPortParamer<string>(s));
                }

                LastRecord = XMLOperate.ReadLastPortRecod();

                if (LastRecord != null)
                {
                    SelectedIndexCommonPort = GetIndex<string>(CommonPort,
                        LastRecord.CommonPort);
                    SelectedIndexBaud = GetIndex<int>(Baud,
                        LastRecord.Baud);
                    SelectedIndexDataBit = GetIndex<int>(DataBit,
                        LastRecord.DataBit);
                    SelectedIndexStopBit = GetIndex<StopBits>(StopBit,
                        LastRecord.StopBit);
                    SelectedIndexParity = GetIndex<Parity>(ParityBit,
                        LastRecord.ParityBit);
                  
                }

            }
            catch(Exception ex)
            {
                Common.LogTrace.CLog.LogError(ex.StackTrace);
            }
        }

       
    }
}
