using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Net;
using GalaSoft.MvvmLight.Command;
using ChoicePhase.PlatformModel.Helper;
using System.Collections.ObjectModel;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 开关合分位
    /// </summary>
    public enum SwitchState :byte
    {
        NULL = 0,
        SWITCH_CLOSE = 1, //合位
        SWITCH_OPEN = 2, //分位   
    }

    /// <summary>
    ///开关类型描述
    /// </summary>
    public enum SwitchType : byte
    {
        SWITCH_TYPE_LOAD = 0x10,//负荷   
        SWITCH_TYPE_LOAD_POWER, //负荷--电源开关
        SWITCH_TYPE_LOAD_BRANCH,  //负荷--分支开关
        SWITCH_TYPE_LOAD_SECTION,  //负荷--分段开关
        SWITCH_TYPE_LOAD_BUS,  //负荷--母联开关
        SWITCH_TYPE_LOAD_INTERCONNECTION, //负荷联络开关

        SWITCH_TYPE_BREAKER = 0x40,//断路器
        SWITCH_TYPE_BREAKER_POWER, //断路器--电源开关
        SWITCH_TYPE_BREAKER_BRANCH,  //断路器--分支开关
        SWITCH_TYPE_BREAKER_SECTION,  //断路器--分段开关
        SWITCH_TYPE_BREAKER_BUS,  //断路器--母联开关
        SWITCH_TYPE_BREAKER_INTERCONNECTION, //断路器--联络开关
    };


    /// <summary>
    /// 动作信息
    /// </summary>
    public enum OperateType
    {
        OPERATE_NULL = 0, //空
        OPERATE_OPEN = 1, //分
        OPERATE_CLOSE = 2, //合
        OPERATE_REJECT_CLOSE = 11, //拒合
        OPERATE_REJECT_OPEN = 12, //拒分
    }
    /// <summary>
    /// 超时类型
    /// </summary>
    public enum OverTimeType
    {
        OVER_TIME_NULL = 0,
        OVER_TIME_REMOVAL = 1, //切除超时
        OVER_TIME_GATHER = 2, //收集超时	
    }

    /// <summary>
    /// 结果信息
    /// </summary>
    public enum ResultType
    {
        RESULT_TYPE_NULL = 0, //NULL
        RESULT_SUCCESS = 1, //成功
        RESULT_FAILURE = 3, //失败
    }

    /// <summary>
    /// 故障状态
    /// </summary>
    public enum FaultState
    {
        FAULT_YES = 0x01, //有故障
        FAULT_NO = 0x02, //无故障
        FAULT_INCOME_LOSS = 0x11, //进线失压故障
    }

    /// <summary>
    /// 转供电代码
    /// </summary>
    public enum TransferCode
    {
        TRANSFER_NULL = 0, //空
        TRANSFER_REMAINDER_CAP = 1, //剩余容量
        TRANSFER_SET = 2, //启动转供电
        TRANSFER_CANCER = 3, //不进行转供电
    };
 
    /// <summary>
    /// 属性参数包含可以应用于设定与参数读取
    /// </summary>
    public class SwitchProperty : ObservableObject
    {
        /// <summary>
        /// 超时类型
        /// </summary>
        public enum OverTimeType
        {
            OVER_TIME_NULL = 0,
            OVER_TIME_REMOVAL = 1, //切除超时
            OVER_TIME_GATHER = 2, //收集超时
        };

        private ObservableCollection<string> _typeSelectionList = new ObservableCollection<string>();
        /// <summary>
        /// 类型下拉列表
        /// </summary>
        public ObservableCollection<string> TypeSelectionList
        {
            get
            {
                return _typeSelectionList;
            }
            set
            {
                _typeSelectionList = value;
                RaisePropertyChanged("TypeSelectionList");
            }
         }


        private string _selectedSwitchType;
        /// <summary>
        /// 选择的类型
        /// </summary>
        public string SelectedSwitchType
        {
            get
            {
                return _selectedSwitchType;
            }
            set
            {
                _selectedSwitchType = value;
                _type = GetType(_selectedSwitchType);
                RaisePropertyChanged("Type");
                RaisePropertyChanged("SelectedSwitchType");
            }
        }


        private ObservableCollection<string> _stateSelectionList = new ObservableCollection<string>();
        /// <summary>
        /// 状态下拉列表
        /// </summary>
        public ObservableCollection<string> StateSelectionList
        {
            get
            {
                return _stateSelectionList;
            }
            set
            {
                _stateSelectionList = value;
                RaisePropertyChanged("StateSelectionList");
            }
        }


        private string _selectedState;
        /// <summary>
        /// 选择的状态
        /// </summary>
        public string SelectedState
        {
            get
            {
                return _selectedState;
            }
            set
            {
                _selectedState = value;
                _state = GetState(_selectedState);
                RaisePropertyChanged("SelectedState");
            }
        }

        private int _configID;
        /// <summary>
        /// 配置号
        /// </summary>
        public int ConfigID
        {
            get
            {
                return _configID;
            }
            set
            {
                _configID = value;
                RaisePropertyChanged("ConfigID");
            }
        }

        private byte[] ipArray;
        /// <summary>
        /// IP数组形式
        /// </summary>
        public byte[] IpArray
        {
            get
            {
                return ipArray;
            }
        }


        private string _IP;
        /// <summary>
        /// ID
        /// </summary>
        public string IP
        {
            get
            {
                return _IP;
            }
            set
            {
                byte[] array;
                if (TypeConvert.IpTryToInt(value, out array))
                {
                    _IP = value;//value
                    ipArray = array;
                    RaisePropertyChanged("IP");
                }
                else
                {
                    RaisePropertyChanged("IP");
                }
            }
        }

      //  private ObservableCollection<byte> _testType = new ObservableCollection<byte>();

        /// <summary>
        /// 类型
        /// </summary>
        public string Test
        {
            get
            {
                return "test";
            }
            set
            {
                RaisePropertyChanged("Test");
            }
        }

        private int _type;
        /// <summary>
        /// 类型
        /// </summary>
        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                RaisePropertyChanged("Type");
            }
        }

        private int _state;
        /// <summary>
        /// 状态
        /// </summary>
        public int State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                RaisePropertyChanged("State");
                RaisePropertyChanged("SwitchState");
            }
        }
       
        public string SwitchState
        {
            get
            {
                switch (_state)
                {
                    case 0x01:
                        {
                            return "合位";
                        }
                    case 0x02:
                        {
                            return "分位";
                        }
                    default:
                        {
                            return "未定义";
                        }
                }
            }
        }

        private int _neighboorNum;
        /// <summary>
        /// 邻居数
        /// </summary>
        public int NeighboorNum
        {
            get
            {
                return _neighboorNum;
            }
            set
            {
                _neighboorNum = value;
                RaisePropertyChanged("NeighboorNum");
            }
        }

        private byte[] _neighboorArray;
        public byte[] NeighboorArray
        {
            get
            {
                return _neighboorArray;
            }
        }

        private string _neighboorCollect;
        /// <summary>
        /// 邻居合集
        /// </summary>
        public string NeighboorCollect
        {
            get
            {
                return _neighboorCollect;
            }
            set
            {
                byte[] index;
                if (TypeConvert.SplitStrToIndex(value, out index))
                {
                    if (index.Length == NeighboorNum)
                    {
                        _neighboorCollect = value;
                        RaisePropertyChanged("NeighboorCollect");
                        Comment = "Index:";
                        foreach (var m in index)
                        {
                            Comment += " " + m.ToString();
                        }
                        _neighboorArray = index;
                        RaisePropertyChanged(() => NeighboorArray);
                    }
                    else
                    {
                        Comment = "输入数量不匹配";
                    } 
                }
                else
                {
                    Comment = "邻居合集，格式不正确";
                }
            }
        }
 
        private float _capacity;
        /// <summary>
        /// 容量
        /// </summary>
        public float Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                _capacity = value;
                RaisePropertyChanged("Capacity");
            }
        }
 
        private  string _comment;
        /// <summary>
        /// 说明内容
        /// </summary>
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }
       
        private string _timeStamp;
        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
                RaisePropertyChanged("TimeStamp");
            }

        }

        private static Dictionary<OverTimeType, string> _overTimeCollect;

        private OverTimeType overTime;
        /// <summary>
        /// 超时值
        /// </summary>
        public int OverTimeValue
        {
            set
            {
                if (Enum.IsDefined(typeof(OverTimeType), value))
                {
                    overTime = (OverTimeType)value;
                    RaisePropertyChanged("OverTime");
                }
            }
        }

        /// <summary>
        /// 超时类型
        /// </summary>
        public string OverTime
        {      
            get
            {
                string str; 
                var state = _overTimeCollect.TryGetValue(overTime, out str);
                if (state)
                {
                    return str;
                }
                else
                {
                    return "未知";
                }
            }
        }

        #region 新加的M以及N
        private string m;
        /// <summary>
        /// M侧邻居
        /// </summary>
        public string M
        {
            get
            {
                return m;
            }
            set
            {
                m = value;
                RaisePropertyChanged("M");
            }
        }


        private int mCount;
        /// <summary>
        /// m侧邻居的数量
        /// </summary>
        public int MCount
        {
            get
            {
                return mCount;
            }
            set
            {
                mCount = value;
                var newNeiCollect = _neighboorCollect.Split(',');
                if(mCount == 0)
                {
                    m = "";
                }
                else if (mCount > _neighboorNum)
                {
                    m = "数量超界";
                }
                //else if ((mCount + nCount) > _neighboorNum)
                //{
                //    n = "两侧数量大于邻居总数量";
                //    m = "两侧数量大于邻居总数量";
                //    RaisePropertyChanged("M");
                //    RaisePropertyChanged("N");
                //}
                else
                {
                    m = _neighboorCollect.Substring(_neighboorCollect.Length - mCount - (mCount - 1), mCount + (mCount -1));
                }
                RaisePropertyChanged("M");
                RaisePropertyChanged("MCount");
            }
        }

        private string mType;
        /// <summary>
        /// M侧邻居类型
        /// </summary>
        public string MType
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
                RaisePropertyChanged("MType");
            }
        }

        private string n;
        /// <summary>
        /// N侧邻居
        /// </summary>
        public string N
        {
            get
            {
                return n;
            }
            set
            {
                n = value;
                RaisePropertyChanged("N");
            }
        }

        private int nCount;
        /// <summary>
        /// n侧邻居的数量
        /// </summary>
        public int NCount
        {
            get
            {
                return nCount;
            }
            set
            {
                nCount = value;
                if(nCount == 0)
                {
                    n = "";
                }
                else if (nCount > _neighboorNum)
                {
                    n = "数量超界";
                }
                //else if((mCount + nCount) > _neighboorNum)
                //{
                //    n = "两侧数量大于邻居总数量";
                //    m = "两侧数量大于邻居总数量";
                //    RaisePropertyChanged("M");
                //    RaisePropertyChanged("N");
                //}
                else
                {
                    n = _neighboorCollect.Substring(0, nCount + (nCount - 1));
                }
                RaisePropertyChanged("N");
                RaisePropertyChanged("NCount");
            }
        }

        private string nType;
        /// <summary>
        /// N侧邻居类型
        /// </summary>
        public string NType
        {
            get
            {
                return nType;
            }
            set
            {
                nType = value;
                RaisePropertyChanged("NType");
            }
        }

        private string addr;
        /// <summary>
        /// 配置STU1/CPU1的
        /// </summary>
        public string Addr
        {
            get
            {
                return addr;
            }
            set
            {
                addr = value;
                RaisePropertyChanged("Addr");
            }
        }


        private string gocb;
        /// <summary>
        /// 现在不用了，原来是邻居
        /// </summary>
        public string Gocb
        {
            get
            {
                return gocb;
            }
            set
            {
                gocb = value;
                RaisePropertyChanged("gocb");
            }
        }

        private string macAddr;
        /// <summary>
        /// MAC地址
        /// </summary>
        public string MACAddress
        {
            get
            {
                return macAddr;
            }
            set
            {
                macAddr = value;
                RaisePropertyChanged("MACAddress");
            }
        }

        private int appid;
        /// <summary>
        /// APPID值
        /// </summary>
        public int APPID
        {
            get
            {
                return appid;
            }
            set
            {
                appid = value;
                RaisePropertyChanged("");
            }
        }

        private int switchID;
        /// <summary>
        /// 开关ID，现在在列表中也不显示了
        /// </summary>
        public int SwitchID
        {
            get
            {
                return switchID;
            }
            set
            {
                switchID = value;
                RaisePropertyChanged("LocalAddr");
            }
        }

        private int maxTime;
        /// <summary>
        /// 最大时间
        /// </summary>
        public int MaxTime
        {
            get
            {
                return maxTime;
            }
            set
            {
                maxTime = value;
                RaisePropertyChanged("MaxTime");
            }
        }

        #endregion

        /// <summary>
        /// 根据类型获取类型的字节形式
        /// </summary>
        /// <param name="switchType"></param>
        /// <returns></returns>
        public byte GetType(string switchType)
        {
            switch(switchType)
            {
                case "负荷开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_LOAD;
                    }
                case "负荷--电源开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_LOAD_POWER;
                    }
                case "负荷--分支开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_LOAD_BRANCH;
                    }
                case "负荷--联络开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_LOAD_INTERCONNECTION;
                    }
                case "负荷--母联开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_LOAD_BUS;
                    }
                case "断路器":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_BREAKER;
                    }
                case "断路器--电源开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_BREAKER_POWER;
                    }
                case "断路器--分支开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_BREAKER_BRANCH;
                    }
                case "断路器--分段开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_BREAKER_SECTION;
                    }
                case "断路器--联络开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_BREAKER_INTERCONNECTION;
                    }
                case "断路器--母联开关":
                    {
                        return (byte)SwitchType.SWITCH_TYPE_BREAKER_BUS;
                    }
                default:
                    {
                        return 0x00;
                    }
            }
        }

        /// <summary>
        /// 根据状态描述获取合分位状态（state）
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public int GetState(string switchState)
        {
            switch(switchState)
            {
                case "合位":
                    {
                        return 0x01;
                    }
                case "分位":
                    {
                        return 0x02;
                    }
                default:
                    {
                        return 0x00;
                    }
            }
        }

        public string GetSwitchState(byte state) 
        {
            switch(state)
            {
                case 0x01:
                    {
                        return "合位";
                    }
                case 0x02:
                    {
                        return "分位";
                    }
                case 0x00:
                    {
                        return "未定义";
                    }
                default:
                    {
                        return "未定义";
                    }
            }
        }

        public string GetSwitchType(byte type)
        {
            var enumType = (SwitchType)type;
            switch (enumType)
            {
                case SwitchType.SWITCH_TYPE_LOAD:
                    {
                        return "负荷开关";
                    }
                case SwitchType.SWITCH_TYPE_LOAD_POWER:
                    {
                        return "负荷--电源开关";
                    }
                case SwitchType.SWITCH_TYPE_LOAD_BRANCH:
                    {
                        return "负荷--分支开关";
                    }
                case SwitchType.SWITCH_TYPE_LOAD_INTERCONNECTION:
                    {
                        return "负荷--联络开关";
                    }
                case SwitchType.SWITCH_TYPE_LOAD_BUS:
                    {
                        return "负荷--母联开关";
                    }
                case SwitchType.SWITCH_TYPE_BREAKER:
                    {
                        return "断路器";
                    }
                case SwitchType.SWITCH_TYPE_BREAKER_POWER:
                    {
                        return "断路器--电源开关";
                    }
                case SwitchType.SWITCH_TYPE_BREAKER_BRANCH:
                    {
                        return "断路器--分支开关";
                    }
                case SwitchType.SWITCH_TYPE_BREAKER_SECTION:
                    {
                        return "断路器--分段开关";
                    }
                case SwitchType.SWITCH_TYPE_BREAKER_INTERCONNECTION:
                    {
                        return "断路器--联络开关";
                    }
                case SwitchType.SWITCH_TYPE_BREAKER_BUS:
                    {
                        return "断路器--母联开关";
                    }
                default:
                    {
                        return "未定义";
                    }
            }

        }

        private static Dictionary<int, string> _faultStateDictionary;

        private int _faultStateValue;

        private int FaultStateValue
        {
            set
            {
                _faultStateValue = value;
                RaisePropertyChanged("FaultState");
            }
        }

        public string FaultState
        {
            get
            {
                string str;
                var state = _faultStateDictionary.TryGetValue(_faultStateValue, out str);
                if (state)
                {
                    return str;
                }
                else
                {
                    return "未知";
                }
            }
        }

        private static Dictionary<int, string> _operateDictionary;

        private int _operateValue = 0;

        private int OperateValue
        {
            set
            {
                _operateValue = value;
                RaisePropertyChanged("Operate");
            }

        }

        public string Operate
        {
            get
            {
                string str;

                var state = _operateDictionary.TryGetValue(_operateValue, out str);
                if (state)
                {
                    return str;
                }
                else
                {
                    return "未知";
                }
            }
        }
        private int _index = 0;
        /// <summary>
        /// 选择的状态
        /// </summary>
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
                RaisePropertyChanged("Index");
            }
        }
        public void Update(byte[] data, int offset)
        {
            FaultStateValue = data[offset];
            State = data[offset + 1];
            OperateValue = data[offset + 2];
            OverTimeValue = data[offset + 3];
        }


        public RelayCommand<string> SelectedTypeChangedCommand { get; private set; }
        /// <summary>
        /// 状态改变触发事件
        /// </summary>
        /// <param name="changedValue"></param>
        void ExecuteSelectedTypeChangedCommand(string changedValue)
        {
            _selectedSwitchType = changedValue;
            _type = GetType(_selectedSwitchType);
            RaisePropertyChanged("Type");
            RaisePropertyChanged("SelectedSwitchType");
        }

        public RelayCommand<string> SelectedStateChangedCommand { get; private set; }
        /// <summary>
        /// 类型改变触发事件
        /// </summary>
        /// <param name="changedValue"></param>
        void ExecuteSelectedStateChangedCommand(string changedValue)
        {
            _selectedState = changedValue;
            _state = GetState(_selectedState);
            RaisePropertyChanged("State");
            RaisePropertyChanged("SelectedState");
        }

        /// <summary>
        /// 属性值
        /// </summary>
        public SwitchProperty(int configid, string ip, int type, int state, int neighboorNum, string neighboorCollect, float capacity,
            string m,int mCount, string mtype,string n, int nCount, string ntype, string addr, string gocb, string macAddress,
            int appid, int switchID, int maxTime)
        {
            _configID = configid;
            IP = ip;
            Type = type;
            _state = state;
            _neighboorNum = neighboorNum;
            NeighboorCollect = neighboorCollect;
            _capacity = capacity;
            M = m;
            N = n;
            MType = mtype;
            NType = ntype;
            MCount = mCount;
            NCount = nCount;
            Addr = addr;
            Gocb = gocb;
            MACAddress = macAddress;
            APPID = appid;
            SwitchID = switchID;
            MaxTime = maxTime;
            _timeStamp = "历史数据载入";

            _overTimeCollect = new Dictionary<OverTimeType, string>();
            _overTimeCollect.Add(OverTimeType.OVER_TIME_NULL, "空");
            _overTimeCollect.Add(OverTimeType.OVER_TIME_GATHER, "收集超时");
            _overTimeCollect.Add(OverTimeType.OVER_TIME_REMOVAL, "切除超时");

            _operateDictionary = new Dictionary<int, string>();
            _operateDictionary.Add(0, "空");
            _operateDictionary.Add(1, "分闸");
            _operateDictionary.Add(2, "合闸");
            _operateDictionary.Add(11, "拒合");
            _operateDictionary.Add(12, "拒分");

            _faultStateDictionary = new Dictionary<int, string>();
            _faultStateDictionary.Add(0, "未知");
            _faultStateDictionary.Add(1, "有故障");
            _faultStateDictionary.Add(2, "没有故障");

            OverTimeValue = 0;
            OperateValue = 0;
            FaultStateValue = 0;

            //类型选择列表
            _typeSelectionList.Add("负荷");
            _typeSelectionList.Add("负荷--电源开关");
            _typeSelectionList.Add("负荷--分支开关");
            _typeSelectionList.Add("负荷--分段开关");
            _typeSelectionList.Add("负荷--母联开关");
            _typeSelectionList.Add("负荷联络开关");
            _typeSelectionList.Add("断路器");
            _typeSelectionList.Add("断路器--电源开关");
            _typeSelectionList.Add("断路器--分支开关");
            _typeSelectionList.Add("断路器--分段开关");
            _typeSelectionList.Add("断路器--母联开关");
            _typeSelectionList.Add("断路器--联络开关");

            //状态选择列表
            _stateSelectionList.Add("合位");
            _stateSelectionList.Add("分位");
            //状态类型描述和状态类型一致
            _selectedSwitchType = GetSwitchType((byte)((SwitchType)_type));
            _selectedState = GetSwitchState((byte)_state);

           SelectedTypeChangedCommand = new RelayCommand<string>(ExecuteSelectedTypeChangedCommand);
            SelectedStateChangedCommand = new RelayCommand<string>(ExecuteSelectedStateChangedCommand);
        }
    }
}
