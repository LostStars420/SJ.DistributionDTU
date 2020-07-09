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
    /// <summary>
    /// 空间使能控制
    /// </summary>
    public class EnableControl : ObservableObject
    {
        private const string Hidden = "Hidden";
        private const string Collapsed = "Collapsed";
        private const string Visible = "Visible";

        private string _visibleNormal;
        /// <summary>
        /// 普通模式
        /// </summary>
        public string VisibleNormal
        {
            get
            {
                return _visibleNormal;
            }
            set
            {
                _visibleNormal = value;
                RaisePropertyChanged("VisibleNormal");
            }
        }

        private string _visibleSingle;
        /// <summary>
        /// 单相模式
        /// </summary>
        public string VisibleSingle
        {
            get
            {
                return _visibleSingle;
            }
            set
            {
                _visibleSingle = value;
                RaisePropertyChanged("VisibleSingle");
            }
        }

        private string _visibleSingleThree;
        /// <summary>
        /// 单开关，三相模式
        /// </summary>
        public string VisibleSingleThree
        {
            get
            {
                return _visibleSingleThree;
            }
            set
            {
                _visibleSingleThree = value;
                RaisePropertyChanged("VisibleSingleThree");
            }
        }
     
        /// <summary>
        /// 总控制使能
        /// </summary>
        private bool _controlEnable;
        public  bool ControlEnable
        {
            get
            {
                return _controlEnable;
            }
            set
            {
                _controlEnable = value;
                RaisePropertyChanged("ControlEnable");
            }
        }

        /// <summary>
        /// 开关操作
        /// </summary>
        private bool _switchOperateEnable;
        public bool SwitchOperateEnable
        {
            get
            {
                return _switchOperateEnable;
            }
            set
            {
                _switchOperateEnable = value;
                RaisePropertyChanged("SwitchOperateEnable");
            }
        }

        private bool _synCloseReady;
        /// <summary>
        /// 同步合闸预制
        /// </summary>
        public bool SynCloseReady
        {
            get
            {
                return _synCloseReady;
            }
            set
            {
                _synCloseReady = value;
                RaisePropertyChanged("SynCloseReady");
            }
        }

        private bool _synCloseAction;
        /// <summary>
        /// 同步合闸执行
        /// </summary>
        public bool SynCloseAction
        {
            get
            {
                return _synCloseAction;
            }
            set
            {
                _synCloseAction = value;
                RaisePropertyChanged("SynCloseAction");
            }
        }

        private bool _closeReady;
        /// <summary>
        /// 合闸预制
        /// </summary>
        public bool CloseReady
        {
            get
            {
                return _closeReady;
            }
            set
            {
                _closeReady = value;
                RaisePropertyChanged("CloseReady");
            }
        }

        private bool _closeAction;
        /// <summary>
        /// 合闸执行
        /// </summary>
        public bool CloseAction
        {
            get
            {
                return _closeAction;
            }
            set
            {
                _closeAction = value;
                RaisePropertyChanged("CloseAction");
            }
        }

        private bool _openReady;
        /// <summary>
        ///分闸预制
        /// </summary>
        public bool OpenReady
        {
            get
            {
                return _openReady;
            }
            set
            {
                _openReady = value;
                RaisePropertyChanged("OpenReady");
            }
        }

        private bool _openAction;
        /// <summary>
        /// 分闸执行
        /// </summary>
        public bool OpenAction
        {
            get
            {
                return _openAction;
            }
            set
            {
                _openAction = value;
                RaisePropertyChanged("OpenAction");
            }
        }


        private bool _closeReadyA;
        /// <summary>
        /// 合闸预制
        /// </summary>
        public bool CloseReadyA
        {
            get
            {
                return _closeReadyA;
            }
            set
            {
                _closeReadyA = value;
                RaisePropertyChanged("CloseReadyA");
            }
        }

        private bool _closeActionA;
        /// <summary>
        /// 合闸执行
        /// </summary>
        public bool CloseActionA
        {
            get
            {
                return _closeActionA;
            }
            set
            {
                _closeActionA = value;
                RaisePropertyChanged("CloseActionA");
            }
        }

        private bool _openReadyA;
        /// <summary>
        ///分闸预制
        /// </summary>
        public bool OpenReadyA
        {
            get
            {
                return _openReadyA;
            }
            set
            {
                _openReadyA = value;
                RaisePropertyChanged("OpenReadyA");
            }
        }

        private bool _openActionA;
        /// <summary>
        /// 分闸执行
        /// </summary>
        public bool OpenActionA
        {
            get
            {
                return _openActionA;
            }
            set
            {
                _openActionA = value;
                RaisePropertyChanged("OpenActionA");
            }
        }


        
        private bool _closeReadyB;
        /// <summary>
        /// 合闸预制
        /// </summary>
        public bool CloseReadyB
        {
            get
            {
                return _closeReadyB;
            }
            set
            {
                _closeReadyB = value;
                RaisePropertyChanged("CloseReadyB");
            }
        }

        private bool _closeActionB;
        /// <summary>
        /// 合闸执行
        /// </summary>
        public bool CloseActionB
        {
            get
            {
                return _closeActionB;
            }
            set
            {
                _closeActionB = value;
                RaisePropertyChanged("CloseActionB");
            }
        }

        private bool _openReadyB;
        /// <summary>
        ///分闸预制
        /// </summary>
        public bool OpenReadyB
        {
            get
            {
                return _openReadyB;
            }
            set
            {
                _openReadyB = value;
                RaisePropertyChanged("OpenReadyB");
            }
        }

        private bool _openActionB;
        /// <summary>
        /// 分闸执行
        /// </summary>
        public bool OpenActionB
        {
            get
            {
                return _openActionB;
            }
            set
            {
                _openActionB = value;
                RaisePropertyChanged("OpenActionB");
            }
        }


        private bool _closeReadyC;
        /// <summary>
        /// 合闸预制
        /// </summary>
        public bool CloseReadyC
        {
            get
            {
                return _closeReadyC;
            }
            set
            {
                _closeReadyC = value;
                RaisePropertyChanged("CloseReadyC");
            }
        }

        private bool _closeActionC;
        /// <summary>
        /// 合闸执行
        /// </summary>
        public bool CloseActionC
        {
            get
            {
                return _closeActionC;
            }
            set
            {
                _closeActionC = value;
                RaisePropertyChanged("CloseActionC");
            }
        }

        private bool _openReadyC;
        /// <summary>
        ///分闸预制
        /// </summary>
        public bool OpenReadyC
        {
            get
            {
                return _openReadyC;
            }
            set
            {
                _openReadyC = value;
                RaisePropertyChanged("OpenReadyC");
            }
        }

        private bool _openActionC;
        /// <summary>
        /// 分闸执行
        /// </summary>
        public bool OpenActionC
        {
            get
            {
                return _openActionC;
            }
            set
            {
                _openActionC = value;
                RaisePropertyChanged("OpenActionC");
            }
        }

        /// <summary>
        ///操作A
        /// </summary>
        public bool OperateA
        {
            get;
            set;
        }
        /// <summary>
        ///操作B 
        /// </summary>
        public bool OperateB
        {
            get;
            set;
        }
        /// <summary>
        ///操作C 
        /// </summary>
        public bool OperateC
        {
            get;
            set;
        }
        /// <summary>
        ///操作ABC 
        /// </summary>
        public bool OperateABC
        {
            get;
            set;
        }
        /// <summary>
        /// 同步操作
        /// </summary>
        public bool OperateSyn
        {
            get;
            set;
        }

        /// <summary>
        /// 返回当前是否有正在操作的状态
        /// </summary>
        public bool OperateState
        {
            get
            {
                return OperateABC | OperateA | OperateB | OperateC;
            }
           
        }

        /// <summary>
        /// 选择使能按钮,若果相应操作位使能，则跳过设置。由其他机制保证合闸预制正确。
        /// </summary>
        /// <param name="selectButton"></param>
        public void ChooiceEnableButton(UInt16 selectButton)
        {
            //int item = (0x00FF & (selectButton >> 8)); //项目0-选相控制，1-3，A-C
           // int closeOpen = (0x00FF & (selectButton)); //1-合位 2-分位

            switch (selectButton)
            {
                case 0x100 | 0x0001://A-分位， 使能合闸
                    {
                        if (!OperateA)
                        {
                            CloseReadyA = true;
                            CloseActionA = false;
                        } 

                        OpenActionA =  false;
                        OpenReadyA  =  false;
                        break;
                    }
                case 0x100 | 0x0002://A-合位， 使能分闸
                    {
                        if (!OperateA)
                        {
                            OpenReadyA = true;
                            OpenActionA = false;                            
                        }
                        CloseActionA = false;
                        CloseReadyA = false;
                        break;
                    }
                case 0x100 | 0x003:
                case 0x100 | 0x000:
                    {
                        OpenActionA = false;
                        OpenReadyA = false;
                        CloseActionA = false;
                        CloseReadyA = false;
                        break;
                    }
                case 0x200 | 0x0001://B-分位， 使能合闸
                    {
                        if (!OperateB)
                        {
                            CloseReadyB = true;
                            CloseActionB = false;
                        }
                        OpenActionB = false;
                        OpenReadyB = false;                        
                        break;
                    }
                case 0x200 | 0x0002://B-合位， 使能分闸
                    {
                        if (!OperateB)
                        {
                            OpenReadyB = true;
                            OpenActionB = false;                            
                        }
                        CloseActionB = false;
                        CloseReadyB = false;
                        break;
                    }
                case 0x200 | 0x003:
                case 0x200 | 0x000:
                    {
                        OpenActionB = false;
                        OpenReadyB = false;
                        CloseActionB = false;
                        CloseReadyB = false;
                        break;
                    }
                case 0x300 | 0x0001://C-分位， 使能合闸
                    {
                        if (!OperateC)
                        {
                            CloseActionC = false;
                            CloseReadyC = true;
                        }
                        OpenActionC = false;
                        OpenReadyC = false;
                        
                        break;
                    }
                case 0x300 | 0x0002://C-合位， 使能分闸
                    {
                        if (!OperateC)
                        {
                            OpenActionC = false;
                            OpenReadyC = true;
                        }
                        CloseActionC = false;
                        CloseReadyC = false;
                        break;
                    }
                case 0x300 | 0x003:
                case 0x300 | 0x000:
                    {
                        OpenActionC = false;
                        OpenReadyC = false;
                        CloseActionC = false;
                        CloseReadyC = false;
                        break;
                    }
            }
            //全是合闸则使能总合闸
            if (CloseReadyA && CloseReadyB && CloseReadyC)
            {
                if (!OperateABC)
                {
                    CloseReady = true;
                    CloseAction = false;
                }
                OpenReady = false;
                OpenAction = false;


                //同步按钮
                //if (!OperateSyn)
                //{
                //    SynCloseReady = true;
                //    SynCloseAction = false;
                //}                
                
            }
            else
            {
                CloseReady = false;
                CloseAction = false;
            }
            //全是分闸则使能总分闸
            if (OpenReadyA && OpenReadyB && OpenReadyC)
            {
                if (!OperateABC)
                {
                    OpenReady = true;
                    OpenAction = false;
                }
                CloseReady = false;
                CloseAction = false;
                //同步按钮
                //SynCloseReady = false;
                //SynCloseAction = false;
                
            }
            else
            {
                OpenReady = false;
                OpenAction = false;
            }
        }

        /// <summary>
        /// 设置同步按钮使能状态
        /// </summary>
        /// <param name="selectButton"></param>
        public void SetSynControlButtonState(byte configByte)
        {
            if (configByte == 0)
            {
                //同步按钮
                SynCloseReady = false;
                SynCloseAction = false;
                return;
            }

            bool sate = true;

            for (int i = 0; i < 3; i++)
            {
                var byteBit = (byte)((configByte >> (2 * i)) & 0x03);
                switch (byteBit)
                {
                    case 1:
                        {
                            sate =sate && CloseReadyA;                            
                            continue;
                        }
                    case 2:
                        {
                            sate = sate && CloseReadyB;
                            continue;                            
                        }
                    case 3:
                        {
                            sate = sate && CloseReadyC;
                            continue;
                        }
                    default:
                        {                            
                            break;
                        }
                }
                break;
            }
            //全是合闸则使能总合闸
            if (sate)
            {
                //同步按钮
                if (!OperateSyn)
                {
                    SynCloseReady = true;
                    SynCloseAction = false;
                }
            }
            else
            {
                //同步按钮
                SynCloseReady = false;
                SynCloseAction = false;
            }

        }





        public Action<string> ExecuteReadyCommandDelegate
        {
            get;
            set;
        }


        public RelayCommand<string> ReadyActionCommand { get; private set; }

        void ExecuteReadyCommand(string str)
        {
            try
            {
               if(ExecuteReadyCommandDelegate != null)
               {
                   ExecuteReadyCommandDelegate(str);
               }
            }
            catch (Exception ex)
            {
                //TODO:错误处理
            }
        }

        /// <summary>
        /// 超时状态定时器 A相
        /// </summary>
        public OverTimeTimer OverTimerReadyActionA
        {
            get;
            set;
        }
        /// <summary>
        /// 超时状态定时器 B相
        /// </summary>
        public OverTimeTimer OverTimerReadyActionB
        {
            get;
            set;
        }
        /// <summary>
        /// 超时状态定时器 C相
        /// </summary>
        public OverTimeTimer OverTimerReadyActionC
        {
            get;
            set;
        }
        /// <summary>
        /// 超时状态定时器 C相
        /// </summary>
        public OverTimeTimer OverTimerReadyActionABC
        {
            get;
            set;
        }

        /// <summary>
        /// 同步超时定时器
        /// </summary>
        public OverTimeTimer OverTimerReadyActionSyn
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化使能控制
        /// </summary>
        public EnableControl()
        {
            _controlEnable = false;
            _switchOperateEnable = false;
            _synCloseReady = false;
            _synCloseAction = false;
            _closeReady = false;
            _closeAction = false;
            _openReady = false;
            _openAction = false;
            OperateA = false;
            OperateB = false;
            OperateC = false;
            OperateABC = false;
            OperateSyn = false;
            

            ReadyActionCommand = new RelayCommand<string>(ExecuteReadyCommand);

            if(NodeAttribute.NormalMode || NodeAttribute.SingleThreeMode)
            {
                VisibleNormal = Visible;
            }
            else
            {
                VisibleNormal = Collapsed;
            }

            if (NodeAttribute.SingleMode)
            {
                VisibleSingle = Visible;
            }
            else
            {
                VisibleSingle = Collapsed;
            }

            if (NodeAttribute.SingleThreeMode)
            {
                VisibleSingleThree = Visible;
            }
            else
            {
                VisibleSingleThree = Collapsed;
            }
        }
    }
}
