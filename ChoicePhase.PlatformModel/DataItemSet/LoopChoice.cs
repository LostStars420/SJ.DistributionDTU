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
    /// <summary>
    /// 回路选择
    /// </summary>
    public class LoopChoice : ObservableObject
    {
        #region 永磁控制器控制
        private byte _macAddress = NodeAttribute.MacPhaseA;

        /// <summary>
        /// MAC ID
        /// </summary>
        public byte MacID
        {
            get
            {
                return _macAddress;
            }
        }

        public string MacAddress
        {
            get
            {
                return _macAddress.ToString("X2");
            }
            set
            {
                byte.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out  _macAddress);
                RaisePropertyChanged("MacAddress");
            }
        }

        private int selectActionIndex = 0;

        public int SelectActionIndex
        {
            get
            {
                return selectActionIndex;
            }
            set
            {
                selectActionIndex = value;
                RaisePropertyChanged("SelectActionIndex");
            }
        }

        private ObservableCollection<string> _actionSelect;

        public ObservableCollection<string> ActionSelect
        {
            get
            {
                return _actionSelect;
            }
        }
        /// <summary>
        /// 三相选择字
        /// </summary>
        private byte _phaseSelectByte = 0;
        public bool CheckPhaseA
        {
            set
            {
                if (value)
                {
                    _phaseSelectByte |= 0x01;
                }
                else
                {
                    _phaseSelectByte &= 0xFE;
                }
                RaisePropertyChanged("CheckPhaseA");
            }
            get
            {
                var state = ((_phaseSelectByte & 0x01) == 0x01) ? (true) : (false);
                return state;
            }
        }

        public bool CheckPhaseB
        {
            set
            {
                if (value)
                {
                    _phaseSelectByte |= 0x02;
                }
                else
                {
                    _phaseSelectByte &= 0xFC;
                }
                RaisePropertyChanged("CheckPhaseB");
            }
            get
            {
                var state = ((_phaseSelectByte & 0x02) == 0x02) ? (true) : (false);
                return state;
            }
        }

        public bool CheckPhaseC
        {
            set
            {
                if (value)
                {
                    _phaseSelectByte |= 0x04;
                }
                else
                {
                    _phaseSelectByte &= 0xFB;
                }
                RaisePropertyChanged("CheckPhaseC");
            }
            get
            {
                var state = ((_phaseSelectByte & 0x04) == 0x04) ? (true) : (false);
                return state;
            }
        }
        /// <summary>
        /// 合闸分闸控制选择的回路
        /// </summary>
        private byte _circleByte = 0;
        public bool CircleI
        {
            set
            {
                if (value)
                {
                    _circleByte |= 0x01;
                }
                else
                {
                    _circleByte &= 0xFE;
                }
                RaisePropertyChanged("CircleI");
            }
            get
            {
                var state = ((_circleByte & 0x01) == 0x01) ? (true) : (false);
                return state;
            }
        }

        public bool CircleII
        {
            set
            {
                if (value)
                {
                    _circleByte |= 0x02;
                }
                else
                {
                    _circleByte &= 0xFD;
                }
                RaisePropertyChanged("CircleII");
            }
            get
            {
                var state = ((_circleByte & 0x02) == 0x02) ? (true) : (false);
                return state;
            }
        }

        public bool CircleIII
        {
            set
            {
                if (value)
                {
                    _circleByte |= 0x04;
                }
                else
                {
                    _circleByte &= 0xFB;
                }
                RaisePropertyChanged("CircleIII");
            }
            get
            {
                var state = ((_circleByte & 0x04) == 0x04) ? (true) : (false);
                return state;
            }
        }
        /// <summary>
        /// 回路选择字
        /// </summary>
        private byte _circleSelectByte = 0;

        private ObservableCollection<string> _loopSelect;

        public ObservableCollection<string> LoopSelect
        {
            get
            {
                return _loopSelect;
            }
        }

        private int _loopIndexI = 0;

        public int LoopIndexI
        {
            get
            {
                return _loopIndexI;
            }
            set
            {
                _loopIndexI = value;
                RaisePropertyChanged("LoopIndexI");
            }
        }
        private int _loopIndexII = 0;

        public int LoopIndexII
        {
            get
            {
                return _loopIndexII;
            }
            set
            {
                _loopIndexII = value;
                RaisePropertyChanged("LoopIndexII");
            }
        }
        private int _loopIndexIII = 0;

        public int LoopIndexIII
        {
            get
            {
                return _loopIndexIII;
            }
            set
            {
                _loopIndexIII = value;
                RaisePropertyChanged("LoopIndexIII");
            }
        }
        /// <summary>
        /// 合分闸动作时间
        /// </summary>
        private int _actionTime = 50;
        public string ActionTime
        {
            set
            {
                int time;
                if (int.TryParse(value, out time))
                {
                    if ((time >= 10) && (time <= 100))
                    {
                        _actionTime = time;
                    }
                }
                RaisePropertyChanged("ActionTime");
            }
            get
            {
                return _actionTime.ToString();
            }
        }

        #endregion

        #region 偏移时间
        /// <summary>
        /// 偏移时间 I
        /// </summary>
        private int _offsetTimeI = 0;

        public string OffsetTimeI
        {
            set
            {
                int time;
                if (int.TryParse(value, out time))
                {
                    if ((time >= 0) && (time <= 65535))
                    {
                        _offsetTimeI = time;
                    }
                }
                RaisePropertyChanged("OffsetTimeI");
            }
            get
            {
                return _offsetTimeI.ToString();
            }
        }
        /// <summary>
        /// 偏移时间 II
        /// </summary>
        private int _offsetTimeII = 0;
        public string OffsetTimeII
        {
            set
            {
                int time;
                if (int.TryParse(value, out time))
                {
                    if ((time >= 0) && (time <= 65535))
                    {
                        _offsetTimeII = time;
                    }
                }
                RaisePropertyChanged("OffsetTimeII");
            }
            get
            {
                return _offsetTimeII.ToString();
            }
        }
        /// <summary>
        /// 偏移时间 III
        /// </summary>
        private int _offsetTimeIII = 0;

        public string OffsetTimeIII
        {
            set
            {
                int time;
                if (int.TryParse(value, out time))
                {
                    if ((time >= 0) && (time <= 65535))
                    {
                        _offsetTimeIII = time;
                    }
                }
                RaisePropertyChanged("OffsetTimeIII");
            }
            get
            {
                return _offsetTimeIII.ToString();
            }
        }


        public Action<string> ExecuteOpearateCommandDelegate;

        public RelayCommand<string> OpearateCommand { get; private set; }

        private void ExecuteOpearateCommand(string obj)
        {
            if (ExecuteOpearateCommandDelegate != null)
            {
                ExecuteOpearateCommandDelegate(obj);
            }
        }



        /// <summary>
        /// 获取同步合闸预制命令
        /// </summary>  
        /// <returns></returns>
        public byte[] GetSynReadyCommand()
        {
            try
            {
                var data = new int[3];
                int i = 0;
                _circleSelectByte = (byte)(LoopIndexI);
                if (LoopIndexI == 0)
                {
                    throw new ArgumentNullException("你没有选择任何回路!");
                }

                if (LoopIndexII != 0)
                {
                    data[i++] = _offsetTimeI;
                    _circleSelectByte = (byte)(_circleSelectByte | ((LoopIndexII) << 2));

                    if (LoopIndexIII != 0)
                    {
                        data[i++] = _offsetTimeII;
                        _circleSelectByte = (byte)(_circleSelectByte | ((LoopIndexIII) << 4));
                    }
                }
                var byteArray = new byte[i * 2];
                for (int k = 0; k < i; k++)
                {
                    byteArray[2 * k] = (byte)(data[k] & 0x00FF);
                    byteArray[2 * k + 1] = (byte)(data[k] >> 8);
                }

                var command = new byte[2 + 6];
                command[0] = (byte)CommandIdentify.SyncReadyClose;
                command[1] = _circleSelectByte;
                Array.Copy(byteArray, 0, command, 2, 2 * i);

                return command;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取预制命令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] GetReadyCommand()
        {
            CommandIdentify cmd;
            if (SelectActionIndex == 0)//合闸预制
            {
                cmd = CommandIdentify.ReadyClose;
            }
            else if (SelectActionIndex == 1)//分闸预制
            {
                cmd = CommandIdentify.ReadyOpen;
            }
            else
            {
                return null;
            }
            var command = new byte[] { (byte)cmd, _circleByte, (byte)_actionTime };
            return command;
        }

        /// <summary>
        /// 获取执行命令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] GetExecuteCommand()
        {
            CommandIdentify cmd;
            if (SelectActionIndex == 0)//合闸执行
            {
                cmd = CommandIdentify.CloseAction;
            }
            else if (SelectActionIndex == 1)//分闸执行
            {
                cmd = CommandIdentify.OpenAction;
            }
            else
            {
                return null;
            }
            var command = new byte[] { (byte)cmd, _circleByte, (byte)_actionTime };
            return command;

        }


        public LoopChoice()
        {
            _actionSelect = new ObservableCollection<string>();
            _actionSelect.Add("合闸");
            _actionSelect.Add("分闸");

            _loopSelect = new ObservableCollection<string>();
            _loopSelect.Add("未选择");
            _loopSelect.Add("回路I");
            _loopSelect.Add("回路II");
            _loopSelect.Add("回路III");

            OpearateCommand = new RelayCommand<string>(ExecuteOpearateCommand);
        }


        #endregion

    }
}
