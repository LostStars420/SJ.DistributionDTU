using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ChoicePhase.DeviceNet.LogicApplyer;
using ChoicePhase.PlatformModel.GetViewData;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    public class PhaseChoice : ObservableObject
    {
        public const string PhaseNull = "未选择"; 
        /// <summary>
        /// A相
        /// </summary>
        public  const string PhaseA = "A相";  
        /// <summary>
        /// B相
        /// </summary>
        public const string PhaseB= "B相"; 
        /// <summary>
        /// C相
        /// </summary>
        public const string PhaseC = "C相";


        
        //Visibility="Collapsed" Visibility="Collapsed" Visibility="Hidden" Visibility="Visible"

        private const string Hidden = "Hidden";
        private const string Collapsed = "Collapsed";
        private const string Visible = "Visible";

        private List<ObservableCollection<string>> _phaseSelectList;

        private ObservableCollection<string> _phaseSelectI;

        public ObservableCollection<string> PhaseSelectI
        {
            get
            {
                return _phaseSelectI;
            }
        }

        private ObservableCollection<string> _phaseSelectII;

        public ObservableCollection<string> PhaseSelectII
        {
            get
            {
                return _phaseSelectII;
            }
        }
        private ObservableCollection<string> _phaseSelectIII;

        public ObservableCollection<string> PhaseSelectIII
        {
            get
            {
                return _phaseSelectIII;
            }
        }

        private string _phaseItemI ;

        public string PhaseItemI
        {
            get
            {
                if (_phaseItemI == null)
                {
                    _phaseItemI = PhaseNull;
                }
                return _phaseItemI;
            }
            set
            {
                _phaseItemI = value;
                SelectedItemChanged(_phaseItemI, 0);
                RaisePropertyChanged("PhaseItemI");
                UpdateAngleVisble();
            }
        }
        private string _phaseItemII ;

        public string PhaseItemII
        {
            get
            {
                if (_phaseItemII == null)
                {
                    _phaseItemII = PhaseNull;
                }
                return _phaseItemII;
            }
            set
            {
                _phaseItemII = value;
                SelectedItemChanged(_phaseItemII, 1);
                RaisePropertyChanged("PhaseItemII");
                UpdateAngleVisble();
            }
        }
        private string _phaseItemIII;

        public string PhaseItemIII
        {
            get
            {
                if (_phaseItemIII == null)
                {
                    _phaseItemIII = PhaseNull;
                }
                return _phaseItemIII;
            }
            set
            {
                _phaseItemIII = value;
                RaisePropertyChanged("PhaseItemIII");
                UpdateAngleVisble();
            }
        }

        /// <summary>
        /// 角度 I
        /// </summary>
        private double _angleI = 0;

        public string AngleI
        {
            set
            {
                CalAngle(value, out _angleI);
                RaisePropertyChanged("AngleI");
                AdjusetAngle(_angleI, 1);
            }
            get
            {
                return _angleI.ToString("f1");
            }
        }

        /// <summary>
        /// 角度 II
        /// </summary>
        private double _angleII = 0;
        public string AngleII
        {
            set
            {
                CalAngle(value, out _angleII);
                RaisePropertyChanged("AngleII");
                AdjusetAngle(_angleII, 2);
            }
            get
            {
                return _angleII.ToString("f1");
            }
        }
        /// <summary>
        /// 角度 III
        /// </summary>
        private double _angleIII = 0;

        public string AngleIII
        {
            set
            {
                CalAngle(value, out _angleIII);
                RaisePropertyChanged("AngleIII");
                AdjusetAngle(_angleIII, 3);
            }
            get
            {
                return _angleIII.ToString("f1");
            }
        }

        //Visibility="Collapsed" Visibility="Collapsed" Visibility="Hidden" Visibility="Visible"

        private string _angleVisibleI;
        /// <summary>
        /// 相角一的可见性
        /// </summary>
        public string AngleVisibleI
        {
            get
            {
                return _angleVisibleI;
            }
            set
            {
                _angleVisibleI = value;
                RaisePropertyChanged("AngleVisibleI");
            }
        }

        private string _angleVisibleII;
        /// <summary>
        /// 相角二的可见性
        /// </summary>
        public string AngleVisibleII
        {
            get
            {
                return _angleVisibleII;
            }
            set
            {
                _angleVisibleII = value;
                RaisePropertyChanged("AngleVisibleII");
            }
        }
        private string _angleVisibleIII;
        /// <summary>
        /// 相角三的可见性
        /// </summary>
        public string AngleVisibleIII
        {
            get
            {
                return _angleVisibleIII;
            }
            set
            {
                _angleVisibleIII = value;
                RaisePropertyChanged("AngleVisibleIII");
            }
        }
        /// <summary>
        /// 更新相角可见性
        /// </summary>
        public void UpdateAngleVisble()
        {
            if ((PhaseItemI != null ) && (PhaseItemI != PhaseNull ))
            {
                AngleVisibleI = Visible;
            }
            else
            {
                AngleVisibleI = Collapsed;
            }
            if ((PhaseItemII != null) && (PhaseItemII != PhaseNull))
            {
                AngleVisibleII = Visible;
            }
            else
            {
                AngleVisibleII = Collapsed;
            }
            if ((PhaseItemIII != null) && (PhaseItemIII != PhaseNull))
            {
                AngleVisibleIII = Visible;
            }
            else
            {
                AngleVisibleIII = Collapsed;
            }
        }

        private void SelectedItemChanged(string item, int index)
        {
            switch(index)
            {
                case 0:
                    {
                        switch (item)
                        {
                            case PhaseNull:
                                {
                                    InitphaseSelect(_phaseSelectList[1]);
                                    InitphaseSelect(_phaseSelectList[2]);                                    
                                    RemoveItem(PhaseA, 1);
                                    RemoveItem(PhaseB, 1);
                                    RemoveItem(PhaseC, 1);
                                    PhaseItemII = PhaseNull;
                                    PhaseItemIII = PhaseNull;
                                    break;
                                }
                            case PhaseA:
                            case PhaseB:
                            case PhaseC:
                                {
                                    InitphaseSelect(_phaseSelectList[1]);                                    
                                    _phaseSelectList[2].Clear();
                                    _phaseSelectList[2].Add(PhaseNull);
                                    RemoveItem(item, 1);
                                    break;
                                }                                
                        }
                        break;
                    }
                case 1:
                    {
                        switch (item)
                        {
                            case PhaseNull:
                                {
                                    InitphaseSelect(_phaseSelectList[2]);
                                    RemoveItem(PhaseA, 2);
                                    RemoveItem(PhaseB, 2);
                                    RemoveItem(PhaseC, 2);                                  
                                    PhaseItemIII = PhaseNull;                                    
                                    break;  
                                }
                            case PhaseA:
                            case PhaseB:
                            case PhaseC:
                                {
                                    InitphaseSelect(_phaseSelectList[2]);//重新初始化
                                    RemoveItem(PhaseItemI, 2);//删去I组选择的
                                    RemoveItem(item, 2);//删去II组选择的
                                    break;
                                }
                        }
                        break;
                    }
            
            }
            if (PhaseItemI == null)
            {
                PhaseItemI = PhaseNull;
            }
            if (PhaseItemII == null)
            {
                PhaseItemII = PhaseNull;
            }
            if (PhaseItemIII == null)
            {
                PhaseItemIII = PhaseNull;
            }

            RaisePropertyChanged("PhaseItemI");
            RaisePropertyChanged("PhaseItemII");
            RaisePropertyChanged("PhaseItemIII");            
        }

        /// <summary>
        /// 移除开始索引后，具有的Item项目
        /// </summary>
        /// <param name="item"></param>
        /// <param name="start"></param>
        private void RemoveItem(string item, int start)
        {
            for(int i = start; i <  _phaseSelectList.Count; i++)
            {               
                _phaseSelectList[i].Remove(item);
            }

        }

        /// <summary>
        /// 获取相代号
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private byte GetByteCode(string item)
        {
            if (item == null)
            {
                return 0;
            }
            switch(item)
            {
                case PhaseA:
                    {
                        return 1;
                    }
                case PhaseB:
                    {
                        return 2;
                    }
                case PhaseC:
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
        /// 获取选择项是否使能 1-A相 2-B 3-C。。。。
        /// </summary>
        public bool GetPhaseEnable(int phase)
        {
            
                var configByte = ConfigByte;
                if (configByte == 0)
                {
                    return false;
                }
                for (int i = 0; i < 3; i++)
                {
                    var byteBit = (byte)((configByte >> (2 * i)) & 0x03);
                    if (byteBit == phase)
                    {
                        return true;
                    }
                }
                return false;            
        }

        /// <summary>
        /// 获取同步相角描述
        /// </summary>
        /// <returns></returns>
        public string GetActionAngleDescrible()
        {
            StringBuilder strBuild = new StringBuilder(32);
            
            if ((PhaseItemI != null) && (PhaseItemI!= PhaseNull))
            {
                strBuild.AppendFormat(" {0}:{1}° ", PhaseItemI, _angleI);
            }
            if ((PhaseItemII != null) && (PhaseItemII != PhaseNull))
            {
                strBuild.AppendFormat("{0}:{1}° ", PhaseItemII, _angleI);
            }
            if ((PhaseItemIII != null) && (PhaseItemIII != PhaseNull))
            {
                strBuild.AppendFormat("{0}:{1}° ", PhaseItemIII, _angleI);
            }
            return strBuild.ToString();
        }


        /// <summary>
        /// 获取配置字
        /// </summary>
        /// <returns></returns>
        public byte ConfigByte
        {
            get
            {
                byte byte1 =  (byte)(0x03 & GetByteCode(PhaseItemI));
                byte byte2 =  (byte)(0x03 & GetByteCode(PhaseItemII));
                byte byte3 =  (byte)(0x03 & GetByteCode(PhaseItemIII));
                return (byte)((byte3<<4)|(byte2<<2)|(byte1));
            }
        }

        /// <summary>
        /// 获取角度参数集合
        /// </summary>
        public double[] GetAngleSet()
        {        
                var collect = new List<double>();
                byte byte1 = (byte)(0x03 & GetByteCode(PhaseItemI));
                if (byte1 != 0)
                {
                    collect.Add(_angleI);
                }
                else
                {
                    return collect.ToArray();
                }


                byte byte2 = (byte)(0x03 & GetByteCode(PhaseItemII));
                if (byte2 != 0)
                {
                    collect.Add(_angleII);
                }
                else
                {
                    return collect.ToArray();
                }

                byte byte3 = (byte)(0x03 & GetByteCode(PhaseItemIII));
                if (byte3 != 0)
                {
                    collect.Add(_angleIII);
                }
                else
                {
                    return collect.ToArray();
                }
                return collect.ToArray();
            
        }

        /// <summary>
        /// 获取同步命令控制字
        /// </summary>
        /// <returns></returns>
        public byte[] GetSynCommand(CommandIdentify cmdID)
        {
            
            if (ConfigByte != 0)
            {
                var angle = GetAngleSet();
                var cmd = new byte[2 + 2*angle.Length];

                cmd[0] = (byte)cmdID;
                cmd[1] = ConfigByte;
                for (int i = 0; i < angle.Length; i++)
                {
                     UInt16 tris = (ushort)(angle[i] / 360 * 65536);//转化为以65536为为基准的归一化值
                     cmd[2 * i + 2] = (byte)(tris & 0x00FF);
                     cmd[2 * i + 3] = (byte)(tris >> 8);
                }
                return cmd;
            }
            return null;
        }

        /// <summary>
        /// 获取同步命令控制字,永磁同步Loop命令
        /// </summary>
        /// <returns></returns>
        public byte[] GetSynCommandLoop(CommandIdentify cmdID, float period)
        {

            if (ConfigByte != 0)
            {
                var angle = GetAngleSet();
                var cmd = new byte[2 + 2 * (angle.Length - 1)];

                cmd[0] = (byte)cmdID;
                cmd[1] = ConfigByte;
                for (int i = 1; i < angle.Length; i++)
                {
                    UInt16 tris = (ushort)((angle[i] - angle[i - 1]) / 360 * period);//转化为以65536为为基准的归一化值
                    cmd[2 * i ] = (byte)(tris & 0x00FF);
                    cmd[2 * i + 1] = (byte)(tris >> 8);
                }
                return cmd;
            }
            return null;
        }


        #region 单开关三级相角获取

        
        /// <summary>
        /// 获取同步命令控制字,永磁同步Loop命令
        /// 首先将合闸相角时间减去合闸时间CloseTime，得到起始时间StartTime。
        /// 找取起始时间最小值min，其作为同步动作的选择项，并选择其作为同步合闸相角设定。
        /// 将其它值减去最小值作为偏移。按从小到到达进行排列。
        /// </summary>
        /// <returns>同步控制命令，第一个元素：同步控制器命令，第二个元素：永磁同步命令</returns>
        public Tuple<byte[], byte[]> GetSynCommandLoop(CommandIdentify cmdIDSyn,CommandIdentify cmdID, List<ActionPhase> actionPhase)
        {
            if (ConfigByte != 0)
            {

                List<ActionPhase> ActionSeq = new List<ActionPhase>();
                Action<string, double> upadateState = (item, angle) =>
                    {
                        byte phaseIndex = (byte)(0x03 & GetByteCode(item));
                        if (phaseIndex != 0)
                        {
                            actionPhase[phaseIndex - 1].Index = (byte)(phaseIndex - 1) ;
                            actionPhase[phaseIndex - 1].Angle = angle;
                            actionPhase[phaseIndex - 1].Enable = true;
                            ActionSeq.Add(actionPhase[phaseIndex - 1]);
                        }                                       
                    };             
                upadateState(PhaseItemI, _angleI);
                upadateState(PhaseItemII, _angleII);
                upadateState(PhaseItemIII, _angleIII);


                //按顺序排列，Start最小值，最前面
                Comparison<ActionPhase> compare = (b1, b2)=>
                {
                    if(b1.StartTime < b2.StartTime)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                };

                ActionSeq.Sort(compare);

                //同步控制器指令
                var cmdSyn = new byte[2 + 2];
                cmdSyn[0] = (byte)cmdIDSyn;
                cmdSyn[1] = ActionSeq[0].Phase;
                UInt16 tris = (ushort)(ActionSeq[0].Angle / 360 * 65536);//转化为以65536为为基准的归一化值
                cmdSyn[2] = (byte)(tris & 0x00FF);
                cmdSyn[3] = (byte)(tris >> 8);

                //永磁同步控制器指令

                var cmd = new byte[2 + 2 * (ActionSeq.Count - 1)];
                byte config = ActionSeq[0].Phase;               

                for (int i = 1; i < ActionSeq.Count; i++)
                {
                    config = (byte)((ActionSeq[i].Phase << (2 * i)) | config);
                    tris = (ushort)(ActionSeq[i].StartTime - ActionSeq[i-1].StartTime);
                    cmd[2 * i] = (byte)(tris & 0x00FF);
                    cmd[2 * i + 1] = (byte)(tris >> 8);
                }                              
                cmd[0] = (byte)cmdID;
                cmd[1] = config;

                return new Tuple<byte[], byte[]>(cmdSyn, cmd);
            }
            return null;
        }


        #endregion


        /// <summary>
        /// 将字符串转化为，浮点数分辨率为0.5
        /// "1.23"转化为1，"1.43"转化为1.5,"1.83"转化为2.0
        /// </summary>
        /// <param name="str">字符串</param>

        private void CalAngle(string str, out double angle)
        {

            if (double.TryParse(str, out angle))
            {
                angle = (angle + 360) % 360; //必须在360度之内
                double remain = angle - (UInt32)angle;
                if (remain <= 0.3)
                {
                    angle = (double)((UInt32)angle);
                }
                else if (remain < 0.7)
                {
                    angle = (double)((UInt32)angle) + 0.5;
                }
                else
                {
                    angle = (double)((UInt32)angle) + 1;
                }
            }
        }

        /// <summary>
        /// 调整角度，其他设置的角度
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        private void AdjusetAngle(double value, int index)
        {
            
            switch (index)
            {
                case 1:
                    {
                        if(value > _angleII)//大于调整为相等
                        {
                            AngleII = AngleI;
                            if (value > _angleIII)//大于调整为相等
                            {
                                AngleIII = AngleI;
                            }                            
                        }
                        break;
                    }
                case 2:
                    {

                        if (value < _angleI) //小于调整为相等
                        {
                            AngleI = AngleII;
                        }
                        else if (value > _angleIII) //大于调整为相等
                        {
                            AngleIII = AngleII;
                        }
                        break;
                    }
                case 3:
                    {
                        if (value < _angleI)
                        {
                            AngleI = AngleIII;
                            AngleII = AngleIII;
                        }
                        else if (value < _angleII)
                        {
                            AngleII = AngleIII;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }




        /// <summary>
        ///  ActionCommand委托
        /// </summary>
        public Action<string> SynCommandDelegate
        {
            get;
            set;
        }


        public RelayCommand<string> SynCommand { get; private set; }

        void ExecuteSynCommand(string str)
         {
             if (SynCommandDelegate != null)
            {
                SynCommandDelegate(str);
            }
         }

        


        public void InitphaseSelect(ObservableCollection<string> select)
        {
            select.Clear();
            select.Add(PhaseNull);
            select.Add(PhaseA);
            select.Add(PhaseB);
            select.Add(PhaseC);
        }
        /// <summary>
        /// 初始化同步相角选择
        /// </summary>
        public PhaseChoice()
        {
            try
            {
                _phaseSelectI = new ObservableCollection<string>();
                InitphaseSelect(_phaseSelectI);

                _phaseSelectII = new ObservableCollection<string>();
                InitphaseSelect(_phaseSelectII);

                _phaseSelectIII = new ObservableCollection<string>();
                InitphaseSelect(_phaseSelectIII);

                _phaseSelectList = new List<ObservableCollection<string>>();
                _phaseSelectList.Add(_phaseSelectI);
                _phaseSelectList.Add(_phaseSelectII);
                _phaseSelectList.Add(_phaseSelectIII);

                SynCommand = new RelayCommand<string>(ExecuteSynCommand);

                PhaseItemI = PhaseNull;
                UpdateAngleVisble();

                if(NodeAttribute.SingleMode)
                {
                    PhaseItemI = PhaseA;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
