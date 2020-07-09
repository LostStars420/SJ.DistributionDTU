using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChoicePhase.PlatformModel.GetViewData;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 指示灯
    /// </summary>
    public class IndicatorLight : ObservableObject
    {
        private static string redLed = @"../Pictures/red.png";
        private static string greenLed = @"../Pictures/green.png";
        private static string yellowLed = @"../Pictures/yellow.png";
        private static string offLed = @"../Pictures/off.jpg";

        #region 指示灯操作

        public void PhaseA_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (NodeAttribute.SingleThreeMode)
            {
                if (e.IsOnline)
                {
                    UpdatePositionStatusSingleThree(e.Node.StatusLoopCollect, "A");
                    UpdateEnergyStatusSingleThree(e.Node.EnergyStatusLoopCollect, "A");
                }
                else
                {
                    OverTimeCycleASingleThree();
                }
            }
            else
            {
                if (e.IsOnline)
                {
                    UpdatePositionStatus(e.Node.StatusLoopCollect, "A");
                    UpdateEnergyStatus(e.Node.EnergyStatusLoopCollect, "A");
                }
                else
                {
                    OverTimeCycleA();
                }
            }
        }
        public void PhaseB_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                UpdatePositionStatus(e.Node.StatusLoopCollect, "B");
                UpdateEnergyStatus(e.Node.EnergyStatusLoopCollect, "B");
            }
            else
            {
                OverTimeCycleB();
            }
        }
        public void PhaseC_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                UpdatePositionStatus(e.Node.StatusLoopCollect, "C");
                UpdateEnergyStatus(e.Node.EnergyStatusLoopCollect, "C");
            }
            else
            {
                OverTimeCycleC();
            }
        }

        void OverTimeCycleA()
        {
            LedCloseA1 = offLed;
            LedOpenA1 = offLed;
            LedEneryA1 = offLed;
            LedCloseA2 = offLed;
            LedOpenA2 = offLed;
            LedEneryA2 = offLed;

            LedCloseA = offLed;
            LedOpenA = offLed;
            LedErrorA = redLed;
            LedEneryA = offLed;
        }

        /// <summary>
        /// SingleThree 超时三相控制
        /// </summary>
        void OverTimeCycleASingleThree()
        {
            LedCloseA1 = offLed;
            LedOpenA1 = offLed;
            LedErrorA1 = redLed;
            LedEneryA1 = offLed;

            LedCloseA2 = offLed;
            LedOpenA2 = offLed;
            LedErrorA2 = redLed;
            LedEneryA2 = offLed;

            LedCloseA3 = offLed;
            LedOpenA3 = offLed;
            LedErrorA3 = redLed;
            LedEneryA3 = offLed;           
        }

        void OverTimeCycleB()
        {
            LedCloseB1 = offLed;
            LedOpenB1 = offLed;
            LedEneryB1 = offLed;
            LedCloseB2 = offLed;
            LedOpenB2 = offLed;
            LedEneryB2 = offLed;

            LedCloseB = offLed;
            LedOpenB = offLed;
            LedErrorB = redLed;
            LedEneryB = offLed;

        }
        void OverTimeCycleC()
        {
            LedCloseC1 = offLed;
            LedOpenC1 = offLed;
            LedEneryC1 = offLed;
            LedCloseC2 = offLed;
            LedOpenC2 = offLed;
            LedEneryC2 = offLed;

            LedCloseC = offLed;
            LedOpenC = offLed;
            LedErrorC = redLed;
            LedEneryC = offLed;

        }

        void UpdatePositionStatus(StatusLoop[] status, string ph)
        {
            var statusLoop = status[0];

            //针对双路
            if (status[0] == status[1])
            {
                UpdateWholeLedStatus(status[0], ph);
            }
            else
            {
                //认为为故障状态
                // LedCloseA = offLed;
                //LedOpenA = offLed;
                //LedErrorA = redLed;
                SetLed("LedClose" + ph, offLed);
                SetLed("LedOpen" + ph, offLed);
                SetLed("LedError" + ph, offLed);

            }
            statusLoop = status[0];
            UpdateLedStatus(status[0], ph + "1");

            statusLoop = status[1];
            UpdateLedStatus(status[1], ph + "2");

        }
        void UpdateEnergyStatus(EnergyStatusLoop[] status, string ph)
        {
            if (status[0] == status[1])
            {
                UpdateEnerggLedStatus(status[0], ph);
            }
            else
            {
                SetLed("LedEnery" + ph, offLed);
            }

            UpdateEnerggLedStatus(status[0], ph + "1");
            UpdateEnerggLedStatus(status[1], ph + "2");

        }

        void UpdatePositionStatusSingleThree(StatusLoop[] status, string ph)
        {
            Action<StatusLoop, string> act = (loop, led) =>
                {
                    if ((loop== StatusLoop.Error) || (loop== StatusLoop.Null))
                    {
                        SetLed(led, redLed);
                    }
                    else
                    {
                        SetLed(led, offLed);
                    }
                };

            UpdateLedStatus(status[0], ph + "1");
            act(status[0], "LedErrorA1");
            UpdateLedStatus(status[1], ph + "2");
            act(status[1], "LedErrorA2");
            UpdateLedStatus(status[2], ph + "3");
            act(status[2], "LedErrorA3"); 
        }

        void UpdateEnergyStatusSingleThree(EnergyStatusLoop[] status, string ph)
        {
            UpdateEnerggLedStatus(status[0], ph + "1");
            UpdateEnerggLedStatus(status[1], ph + "2");
            UpdateEnerggLedStatus(status[2], ph + "3");      
        }

        /// <summary>
        /// 更新单只 储能LED状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ph"></param>
        void UpdateEnerggLedStatus(EnergyStatusLoop status, string ph)
        {
            string led = "LedEnery" + ph;

            switch (status)
            {
                case EnergyStatusLoop.Less: //欠压
                    {
                        SetLed(led, yellowLed);
                        break;
                    }
                case EnergyStatusLoop.Normal: //正常范围
                    {
                        SetLed(led, greenLed);
                        break;
                    }
                case EnergyStatusLoop.More: //过压或为空
                    {
                        SetLed(led, redLed);
                        break;
                    }
                case EnergyStatusLoop.Null:
                    {
                        SetLed(led, offLed);
                        break;
                    }
            }

        }
        /// <summary>
        /// 针对多个机构，更新整体的合位状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ph"></param>
        void UpdateWholeLedStatus(StatusLoop status, string ph)
        {
            string close = "LedClose" + ph;
            string open = "LedOpen" + ph;
            string error = "LedError" + ph;

            switch (status)
            {
                case StatusLoop.Null:
                case StatusLoop.Error:
                    {
                        SetLed(close, offLed);
                        SetLed(open, offLed);
                        SetLed(error, redLed);
                        break;
                    }
                case StatusLoop.Close:
                    {
                        SetLed(close, redLed);
                        SetLed(open, offLed);
                        SetLed(error, offLed);
                        break;
                    }
                case StatusLoop.Open:
                    {
                        SetLed(close, offLed);
                        SetLed(open, greenLed);
                        SetLed(error, offLed);
                        break;
                    }

            }
        }
        /// <summary>
        /// 更新合分位状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ph"></param>
        void UpdateLedStatus(StatusLoop status, string ph)
        {
            string close = "LedClose" + ph;
            string open = "LedOpen" + ph;

            switch (status)
            {
                case StatusLoop.Null:
                case StatusLoop.Error:
                    {
                        SetLed(close, offLed);
                        SetLed(open, offLed);

                        break;
                    }
                case StatusLoop.Close:
                    {
                        SetLed(close, redLed);
                        SetLed(open, offLed);

                        break;
                    }
                case StatusLoop.Open:
                    {
                        SetLed(close, offLed);
                        SetLed(open, greenLed);

                        break;
                    }
            }
        }

        /// <summary>
        /// 设置LED
        /// </summary>
        /// <param name="led">指定等号</param>
        /// <param name="state">led状态</param>
        void SetLed(string led, string state)
        {
            switch (led)
            {
                case "LedCloseA1":
                    {
                        LedCloseA1 = state;
                        break;
                    }
                case "LedCloseA2":
                    {
                        LedCloseA2 = state;
                        break;
                    }
                case "LedCloseA3":
                    {
                        LedCloseA3 = state;
                        break;
                    }
                case "LedOpenA1":
                    {
                        LedOpenA1 = state;
                        break;
                    }
                case "LedOpenA2":
                    {
                        LedOpenA2 = state;
                        break;
                    }
                case "LedOpenA3":
                    {
                        LedOpenA3 = state;
                        break;
                    }
                case "LedCloseA":
                    {
                        LedCloseA = state;
                        break;
                    }
                case "LedOpenA":
                    {
                        LedOpenA = state;
                        break;
                    }
                case "LedErrorA":
                    {
                        LedErrorA = state;
                        break;
                    }
                case "LedErrorA1":
                    {
                        LedErrorA1 = state;
                        break;
                    }
                case "LedErrorA2":
                    {
                        LedErrorA2 = state;
                        break;
                    }
                case "LedErrorA3":
                    {
                        LedErrorA3 = state;
                        break;
                    }
                case "LedEneryA":
                    {
                        LedEneryA = state;
                        break;
                    }
                case "LedEneryA1":
                    {
                        LedEneryA1 = state;
                        break;
                    }
                case "LedEneryA2":
                    {
                        LedEneryA2 = state;
                        break;
                    }
                case "LedEneryA3":
                    {
                        LedEneryA3 = state;
                        break;
                    }
                case "LedCloseB1":
                    {
                        LedCloseB1 = state;
                        break;
                    }
                case "LedCloseB2":
                    {
                        LedCloseB2 = state;
                        break;
                    }
                case "LedOpenB1":
                    {
                        LedOpenB1 = state;
                        break;
                    }
                case "LedOpenB2":
                    {
                        LedOpenB2 = state;
                        break;
                    }
                case "LedCloseB":
                    {
                        LedCloseB = state;
                        break;
                    }
                case "LedOpenB":
                    {
                        LedOpenB = state;
                        break;
                    }
                case "LedErrorB":
                    {
                        LedErrorB = state;
                        break;
                    }
                case "LedEneryB":
                    {
                        LedEneryB = state;
                        break;
                    }
                case "LedEneryB1":
                    {
                        LedEneryB1 = state;
                        break;
                    }
                case "LedEneryB2":
                    {
                        LedEneryB2 = state;
                        break;
                    }
                case "LedCloseC1":
                    {
                        LedCloseC1 = state;
                        break;
                    }
                case "LedCloseC2":
                    {
                        LedCloseC2 = state;
                        break;
                    }
                case "LedOpenC1":
                    {
                        LedOpenC1 = state;
                        break;
                    }
                case "LedOpenC2":
                    {
                        LedOpenC2 = state;
                        break;
                    }
                case "LedCloseC":
                    {
                        LedCloseC = state;
                        break;
                    }
                case "LedOpenC":
                    {
                        LedOpenC = state;
                        break;
                    }
                case "LedErrorC":
                    {
                        LedErrorC = state;
                        break;
                    }
                case "LedEneryC":
                    {
                        LedEneryC = state;
                        break;
                    }
                case "LedEneryC1":
                    {
                        LedEneryC1 = state;
                        break;
                    }
                case "LedEneryC2":
                    {
                        LedEneryC2 = state;
                        break;
                    }
                default:
                    {
                        throw new Exception("没有指示灯");
                    }

            }

            
        }
       






        #endregion


        #region 状态指示灯 A相


        private string ledCloseA1 = offLed;
        /// <summary>
        /// 合闸指示A1
        /// </summary>
        public String LedCloseA1
        {
            get
            {
                return ledCloseA1;
            }
            set
            {
                ledCloseA1 = value;
                RaisePropertyChanged("LedCloseA1");
            }
        }
        private string ledCloseA2 = offLed;
        /// <summary>
        /// 合闸指示A2
        /// </summary>
        public String LedCloseA2
        {
            get
            {
                return ledCloseA2;
            }
            set
            {
                ledCloseA2 = value;
                RaisePropertyChanged("LedCloseA2");
            }
        }

        private string ledCloseA3 = offLed;
        /// <summary>
        /// 合闸指示A3
        /// </summary>
        public String LedCloseA3
        {
            get
            {
                return ledCloseA3;
            }
            set
            {
                ledCloseA3 = value;
                RaisePropertyChanged("LedCloseA3");
            }
        }

        private string ledOpenA1 = offLed;
        /// <summary>
        /// 分闸指示A1
        /// </summary>
        public String LedOpenA1
        {
            get
            {
                return ledOpenA1;
            }
            set
            {
                ledOpenA1 = value;
                RaisePropertyChanged("LedOpenA1");
            }
        }
        private string ledOpenA2 = offLed;
        /// <summary>
        /// 分闸指示A2
        /// </summary>
        public String LedOpenA2
        {
            get
            {
                return ledOpenA2;
            }
            set
            {
                ledOpenA2 = value;
                RaisePropertyChanged("LedOpenA2");
            }
        }

        private string ledOpenA3 = offLed;
        /// <summary>
        /// 分闸指示A3
        /// </summary>
        public String LedOpenA3
        {
            get
            {
                return ledOpenA3;
            }
            set
            {
                ledOpenA3 = value;
                RaisePropertyChanged("LedOpenA3");
            }
        }

        private string ledCloseA = offLed;
        /// <summary>
        /// 总合闸指示
        /// </summary>
        public String LedCloseA
        {
            get
            {
                return ledCloseA;
            }
            set
            {
                ledCloseA = value;
                RaisePropertyChanged("LedCloseA");
            }
        }


        private string ledOpenA = offLed;
        /// <summary>
        /// 总分闸指示A1
        /// </summary>
        public String LedOpenA
        {
            get
            {
                return ledOpenA;
            }
            set
            {
                ledOpenA = value;
                RaisePropertyChanged("LedOpenA");
            }
        }

        private string ledErrorA = offLed;
        /// <summary>
        /// 故障指示A
        /// </summary>
        public String LedErrorA
        {
            get
            {
                return ledErrorA;
            }
            set
            {
                ledErrorA = value;
                RaisePropertyChanged("LedErrorA");
            }
        }
        private string ledErrorA1 = offLed;
        /// <summary>
        /// 故障指示A1
        /// </summary>
        public String LedErrorA1
        {
            get
            {
                return ledErrorA1;
            }
            set
            {
                ledErrorA1 = value;
                RaisePropertyChanged("LedErrorA1");
            }
        }
        private string ledErrorA2 = offLed;
        /// <summary>
        /// 故障指示A1
        /// </summary>
        public String LedErrorA2
        {
            get
            {
                return ledErrorA2;
            }
            set
            {
                ledErrorA2 = value;
                RaisePropertyChanged("LedErrorA2");
            }
        }
        private string ledErrorA3 = offLed;
        /// <summary>
        /// 故障指示A1
        /// </summary>
        public String LedErrorA3
        {
            get
            {
                return ledErrorA3;
            }
            set
            {
                ledErrorA3 = value;
                RaisePropertyChanged("LedErrorA3");
            }
        }
        private string ledEneryA = offLed;
        public String LedEneryA
        {
            get
            {
                return ledEneryA;
            }
            set
            {
                ledEneryA = value;
                RaisePropertyChanged("LedEneryA");
            }
        }
        private string ledEneryA1 = offLed;
        public String LedEneryA1
        {
            get
            {
                return ledEneryA1;
            }
            set
            {
                ledEneryA1 = value;
                RaisePropertyChanged("LedEneryA1");
            }
        }
        private string ledEneryA2 = offLed;
        public String LedEneryA2
        {
            get
            {
                return ledEneryA2;
            }
            set
            {
                ledEneryA2 = value;
                RaisePropertyChanged("LedEneryA2");
            }
        }
        private string ledEneryA3 = offLed;
        public String LedEneryA3
        {
            get
            {
                return ledEneryA3;
            }
            set
            {
                ledEneryA3 = value;
                RaisePropertyChanged("LedEneryA3");
            }
        }
        #endregion
        #region 状态指示灯 B相


        private string ledCloseB1 = offLed;
        /// <summary>
        /// 合闸指示B1
        /// </summary>
        public String LedCloseB1
        {
            get
            {
                return ledCloseB1;
            }
            set
            {
                ledCloseB1 = value;
                RaisePropertyChanged("LedCloseB1");
            }
        }
        private string ledCloseB2 = offLed;
        /// <summary>
        /// 合闸指示B2
        /// </summary>
        public String LedCloseB2
        {
            get
            {
                return ledCloseB2;
            }
            set
            {
                ledCloseB2 = value;
                RaisePropertyChanged("LedCloseB2");
            }
        }


        private string ledOpenB1 = offLed;
        /// <summary>
        /// 分闸指示B1
        /// </summary>
        public String LedOpenB1
        {
            get
            {
                return ledOpenB1;
            }
            set
            {
                ledOpenB1 = value;
                RaisePropertyChanged("LedOpenB1");
            }
        }
        private string ledOpenB2 = offLed;
        /// <summary>
        /// 分闸指示B2
        /// </summary>
        public String LedOpenB2
        {
            get
            {
                return ledOpenB2;
            }
            set
            {
                ledOpenB2 = value;
                RaisePropertyChanged("LedOpenB2");
            }
        }



        private string ledCloseB = offLed;
        /// <summary>
        /// 总合闸指示
        /// </summary>
        public String LedCloseB
        {
            get
            {
                return ledCloseB;
            }
            set
            {
                ledCloseB = value;
                RaisePropertyChanged("LedCloseB");
            }
        }


        private string ledOpenB = offLed;
        /// <summary>
        /// 总分闸指示B1
        /// </summary>
        public String LedOpenB
        {
            get
            {
                return ledOpenB;
            }
            set
            {
                ledOpenB = value;
                RaisePropertyChanged("LedOpenB");
            }
        }

        private string ledErrorB = offLed;
        /// <summary>
        /// 故障指示B
        /// </summary>
        public String LedErrorB
        {
            get
            {
                return ledErrorB;
            }
            set
            {
                ledErrorB = value;
                RaisePropertyChanged("LedErrorB");
            }
        }
        private string ledEneryB = offLed;
        public String LedEneryB
        {
            get
            {
                return ledEneryB;
            }
            set
            {
                ledEneryB = value;
                RaisePropertyChanged("LedEneryB");
            }
        }
        private string ledEneryB1 = offLed;
        public String LedEneryB1
        {
            get
            {
                return ledEneryB1;
            }
            set
            {
                ledEneryB1 = value;
                RaisePropertyChanged("LedEneryB1");
            }
        }
        private string ledEneryB2 = offLed;
        public String LedEneryB2
        {
            get
            {
                return ledEneryB2;
            }
            set
            {
                ledEneryB2 = value;
                RaisePropertyChanged("LedEneryB2");
            }
        }

        #endregion

        #region 状态指示灯 C相


        private string ledCloseC1 = offLed;
        /// <summary>
        /// 合闸指示C1
        /// </summary>
        public String LedCloseC1
        {
            get
            {
                return ledCloseC1;
            }
            set
            {
                ledCloseC1 = value;
                RaisePropertyChanged("LedCloseC1");
            }
        }
        private string ledCloseC2 = offLed;
        /// <summary>
        /// 合闸指示C2
        /// </summary>
        public String LedCloseC2
        {
            get
            {
                return ledCloseC2;
            }
            set
            {
                ledCloseC2 = value;
                RaisePropertyChanged("LedCloseC2");
            }
        }


        private string ledOpenC1 = offLed;
        /// <summary>
        /// 分闸指示C1
        /// </summary>
        public String LedOpenC1
        {
            get
            {
                return ledOpenC1;
            }
            set
            {
                ledOpenC1 = value;
                RaisePropertyChanged("LedOpenC1");
            }
        }
        private string ledOpenC2 = offLed;
        /// <summary>
        /// 分闸指示C2
        /// </summary>
        public String LedOpenC2
        {
            get
            {
                return ledOpenC2;
            }
            set
            {
                ledOpenC2 = value;
                RaisePropertyChanged("LedOpenC2");
            }
        }



        private string ledCloseC = offLed;
        /// <summary>
        /// 总合闸指示
        /// </summary>
        public String LedCloseC
        {
            get
            {
                return ledCloseC;
            }
            set
            {
                ledCloseC = value;
                RaisePropertyChanged("LedCloseC");
            }
        }


        private string ledOpenC = offLed;
        /// <summary>
        /// 总分闸指示C1
        /// </summary>
        public String LedOpenC
        {
            get
            {
                return ledOpenC;
            }
            set
            {
                ledOpenC = value;
                RaisePropertyChanged("LedOpenC");
            }
        }

        private string ledErrorC = offLed;
        /// <summary>
        /// 故障指示C
        /// </summary>
        public String LedErrorC
        {
            get
            {
                return ledErrorC;
            }
            set
            {
                ledErrorC = value;
                RaisePropertyChanged("LedErrorC");
            }
        }
        private string ledEneryC = offLed;
        public String LedEneryC
        {
            get
            {
                return ledEneryC;
            }
            set
            {
                ledEneryC = value;
                RaisePropertyChanged("LedEneryC");
            }
        }
        private string ledEneryC1 = offLed;
        public String LedEneryC1
        {
            get
            {
                return ledEneryC1;
            }
            set
            {
                ledEneryC1 = value;
                RaisePropertyChanged("LedEneryC1");
            }
        }
        private string ledEneryC2 = offLed;
        public String LedEneryC2
        {
            get
            {
                return ledEneryC2;
            }
            set
            {
                ledEneryC2 = value;
                RaisePropertyChanged("LedEneryC2");
            }
        }

        #endregion



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


        private const string Hidden = "Hidden";
        private const string Collapsed = "Collapsed";
        private const string Visible = "Visible";

        public IndicatorLight()
        {
            if (NodeAttribute.NormalMode)
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
