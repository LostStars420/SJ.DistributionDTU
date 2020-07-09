using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChoicePhase.PlatformModel.GetViewData;
using ChoicePhase.PlatformModel.Helper;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    public class ConfigParameter : ObservableObject
    {
        public RelayCommand<string> OperateCommand { get; private set; }

        private string _tips = "未修改";
        public string Tips
        {
            get
            {
                return _tips;
            }
            set
            {
                _tips = value;
                TipColor = "Red";
                RaisePropertyChanged("Tips");
            }
        }

        public string _tipColor = "Black";
        /// <summary>
        /// 
        /// </summary>
        public string TipColor
        {
            get
            {
                return _tipColor;
            }
            set
            {
                _tipColor = value;
                RaisePropertyChanged("TipColor");
            }
        }


        /// <summary>
        /// 使能bit bit0-同步控制器 bit1-A相 bit2-B相 bit3-C相
        /// </summary>
        public byte EnabitSelect
        {
            get
            {
                return NodeAttribute.EnabitSelect;
            }
            set
            {
                NodeAttribute.EnabitSelect = value;
                RaisePropertyChanged("EnabitSelect");
                Tips = "值已改变";
            }
        }

        public bool IsEnableSyncontroller
        {
            get
            {
                return ((EnabitSelect & 0x01) == 0x01);                
            }
            set
            {
                if(value)
                {
                    EnabitSelect = SetBit(EnabitSelect, 0);
                }
                else
                {
                    EnabitSelect = ClearBit(EnabitSelect, 0);
                }
            }
        }
        public bool IsEnablePhaseA
        {
            get
            {
                return ((EnabitSelect & 0x02) == 0x02);
            }
            set
            {
                if (value)
                {
                    EnabitSelect = SetBit(EnabitSelect, 1);
                }
                else
                {
                    EnabitSelect = ClearBit(EnabitSelect, 1);
                }

            }
        }

        public bool IsEnablePhaseB
        {
            get
            {
                return ((EnabitSelect & 0x04) == 0x04);
            }
            set
            {
                if (value)
                {
                    EnabitSelect = SetBit(EnabitSelect, 2);
                }
                else
                {
                    EnabitSelect = ClearBit(EnabitSelect, 2);
                }

            }
        }
        public bool IsEnablePhaseC
        {
            get
            {
                return ((EnabitSelect & 0x08) == 0x08);
            }
            set
            {
                if (value)
                {
                    EnabitSelect = SetBit(EnabitSelect, 3);
                }
                else
                {
                    EnabitSelect = ClearBit(EnabitSelect, 3);
                }
            }
        }
        /// <summary>
        /// 同步合闸操作超时时间ms
        /// </summary>
        public int SynCloseActionOverTime
        {
            get
            {
                return NodeAttribute.SynCloseActionOverTime;
            }
            set
            {
                NodeAttribute.SynCloseActionOverTime = value;
                RaisePropertyChanged("SynCloseActionOverTime");
                Tips = "值已改变";
            }
        }

        /// <summary>
        /// 同步合闸操作超时时间ms
        /// </summary>
        public int OpenActionOverTime
        {
            get
            {
                return NodeAttribute.OpenActionOverTime;
            }
            set
            {
                NodeAttribute.OpenActionOverTime = value;
                RaisePropertyChanged("OpenActionOverTime");
                Tips = "值已改变";
            }
        }
        /// <summary>
        /// 合闸操作超时时间ms
        /// </summary>
        public  int CloseActionOverTime
        {
            get
            {
                return NodeAttribute.CloseActionOverTime;
            }
            set
            {
                NodeAttribute.CloseActionOverTime = value;
                RaisePropertyChanged("CloseActionOverTime");
                Tips = "值已改变";
            }
        }
        
        /// <summary>
        /// 合闸通电时间
        /// </summary>
        public  byte ClosePowerOnTime
        {
            get
            {
                return NodeAttribute.ClosePowerOnTime;
            }
            set
            {
                NodeAttribute.ClosePowerOnTime = value;
                RaisePropertyChanged("ClosePowerOnTime");
                Tips = "值已改变";
            }
        }

        

        /// <summary>
        /// 当前周期
        /// </summary>
        public ushort Period
        {
            get
            {
                return (ushort)NodeAttribute.Period;
            }
            set
            {
                var old = Period;
                NodeAttribute.Period = value;
                RaisePropertyChanged("CloseTimeB");
                Tips = string.Format("周期由[{0}]变成[{1}]us.", old, Period);
            }
        }

        /// <summary>
        /// 分闸通电时间
        /// </summary>
        public  byte OpenPowerOnTime
        {
            get
            {
                return NodeAttribute.OpenPowerOnTime;
            }
            set
            {
                NodeAttribute.OpenPowerOnTime = value;
                RaisePropertyChanged("OpenPowerOnTime");
                Tips = "值已改变";
            }
        }


        /// <summary>
        /// 工作模式
        /// </summary>
        public  int WorkMode
        {
            get
            {
                return NodeAttribute.WorkMode;
            }
            set
            {
                NodeAttribute.WorkMode = value;
                RaisePropertyChanged("WorkMode");
                Tips = "值已改变";
            }
        }


        /// <summary>
        /// 将其中deq位设置1，[0,7]
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="seq">位数</param>
        /// <returns>置位后的数据</returns>
        private byte SetBit(byte value, byte seq)
        {
            return (byte)(value | (1 << seq));
        }

        /// <summary>
        /// 将其中deq位设清0，[0,7]
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="seq">位数</param>
        /// <returns>清除相应位后的数据</returns>
        private byte ClearBit(byte value, byte seq)
        {
            return (byte)(value & (~(1 << seq)));
        }

        public string _userName = NodeAttribute.CurrentUser.Name;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        /// <summary>
        /// 密钥暂存
        /// </summary>
        private string _operatOldPassword = "";
        private string _operatNewPassword = "";
        private string _operatAckPassword = "";
        private string _loginOldPassword = "";
        private string _loginNewPassword = "";
        private string _loginAckPassword = "";

        /// <summary>
        /// 更新密钥
        /// </summary>
        /// <param name="obj">item1-属性名称，item2-密码</param>
        public void UpadeOptionViewPassword(Tuple<string, string> obj)
        {
            switch (obj.Item1)
            {
                case "loginOldPassword":
                    {
                        _loginOldPassword = obj.Item2;
                        break;
                    }
                case "loginNewPassword":
                    {
                        _loginNewPassword = obj.Item2;
                        break;
                    }
                case "loginAckPassword":
                    {
                        _loginAckPassword = obj.Item2;
                        break;
                    }
                case "operatOldPassword":
                    {
                        _operatOldPassword = obj.Item2;
                        break;
                    }
                case "operatNewPassword":
                    {
                        _operatNewPassword = obj.Item2;
                        break;
                    }
                case "operatAckPassword":
                    {
                        _operatAckPassword = obj.Item2;
                        break;
                    }
            }
        }

        /// <summary>
        /// 登陆密码设置展开
        /// </summary>
        private bool isExpander = true;
        public bool ExpanderLogin
        {
            get
            {
                return isExpander;
            }
            set
            {
                isExpander = value;
                if (isExpander)
                {
                    isExpanderOperate = false;
                }
                RaisePropertyChanged("ExpanderLogin");
                RaisePropertyChanged("ExpanderOperate");
            }
        }

        private bool isExpanderOperate = false;
        /// <summary>
        /// 操作密码设置展开
        /// </summary>
        public bool ExpanderOperate
        {
            get
            {
                return isExpanderOperate;
            }
            set
            {
                isExpanderOperate = value;
                if (isExpanderOperate)
                {
                    isExpander = false;
                }
                RaisePropertyChanged("ExpanderLogin");
                RaisePropertyChanged("ExpanderOperate");
            }
        }

        private void ExecuteOperateCommand(string obj)
        {
            switch(obj)
            {
                case "Save":
                    {
                        XMLOperate.WriteLastConfigRecod();
                        Tips = "保存设置";
                        break;
                    }
                case "Read":
                    {
                        XMLOperate.ReadLastConfigRecod();
                        RaisePropertyChanged("EnabitSelect");
                        RaisePropertyChanged("SynCloseActionOverTime");
                        RaisePropertyChanged("OpenActionOverTime");
                        RaisePropertyChanged("CloseActionOverTime");
                        RaisePropertyChanged("ClosePowerOnTime");
                        RaisePropertyChanged("OpenPowerOnTime");
                        RaisePropertyChanged("WorkMode");
                        Tips = "重新载入设置";
                        break;
                    }
                case "AckChangeLoginPassword"://确认登陆密码
                    {
                        if (NodeAttribute.PasswordI != _loginOldPassword)
                        {
                            Tips = "原始密码错误";
                            TipColor = "Red";
                            return;
                        }
                        if (_loginNewPassword != _loginAckPassword)
                        {
                            Tips = "设置密码不一致错误";
                            TipColor = "Red";
                            return;
                        }
                        NodeAttribute.CurrentUser.PasswordI = _loginNewPassword;
                        Tips = "修改成功";
                        TipColor = "Green";
                        XMLOperate.WriteUserRecod(NodeAttribute.CurrentUser);

                        break;
                    }
                case "AckChangeOperatPassword"://确认操作密码
                    {
                        if (NodeAttribute.PasswordII != _operatOldPassword)
                        {
                            Tips = "原始密码错误";
                            TipColor = "Red";
                            return;
                        }
                        if (_operatNewPassword != _operatAckPassword)
                        {
                            Tips = "设置密码不一致错误";
                            TipColor = "Red";
                            return;
                        }
                        NodeAttribute.CurrentUser.PasswordII = _operatNewPassword;
                        Tips = "修改成功";
                        TipColor = "Green";
                        XMLOperate.WriteUserRecod(NodeAttribute.CurrentUser);
                        break;
                    }
            }
        }

     



        public ConfigParameter()
        {
            OperateCommand = new RelayCommand<string>(ExecuteOperateCommand);
          
        }

    }
}
