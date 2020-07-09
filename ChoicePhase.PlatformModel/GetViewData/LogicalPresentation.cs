using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ChoicePhase.DeviceNet.Element;
using ChoicePhase.DeviceNet.LogicApplyer;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.Helper;

namespace ChoicePhase.PlatformModel.GetViewData
{
    /// <summary>
    /// 逻辑呈现， 合分闸指示灯状态，
    /// </summary>
    public class LogicalPresentation : ObservableObject
    {
        private StatusBarMessage statusBar;
        public StatusBarMessage StatusBar
        {
            get
            {
                return statusBar;
            }
            set
            {
                statusBar = value;
                RaisePropertyChanged("StatusBar");
            }
        }

        /// <summary>
        /// 三相指示灯
        /// </summary>
        public IndicatorLight IndicatorLightABC
        {
            get;
            set;
        }

        /// <summary>
        /// 用户控件使能控制
        /// </summary>
        public EnableControl UserControlEnable
        {
            set;
            get;
        }

        /// <summary>
        /// 节点状态
        /// </summary>
        public ObservableCollection<NodeStatus> NodeStatusList
        {
            private set;
            get;
        }

        /// <summary>
        /// 同步相角选择
        /// </summary>
        public PhaseChoice SynPhaseChoice
        {
            private set;
            get;
        }

        /// <summary>
        /// 配置参数
        /// </summary>
        public ConfigParameter NodeParameter
        {
            private set;
            get;
        }


        private byte _onlineBit;
        /// <summary>
        /// 在线位 bit0-同步控制器 bit1-A相 bit2-B相 bit3-C相
        /// </summary>
        public byte OnlineBit
        {
            get
            {
                return _onlineBit;
            }
            private set
            {
                _onlineBit = value;
                if (_onlineBit == NodeAttribute.EnabitSelect)//A,B,C使能控制
                {
                    UserControlEnable.ControlEnable = true;
                }
                else
                {
                    UserControlEnable.ControlEnable = false;
                }
            }
        }

        /// <summary>
        /// 用于循环测试
        /// </summary>
        public LoopTest TestParameter
        {
            get;
            set;
        }

    


        #region 更新参数
        /// <summary>
        /// 主要负责更新上位机设定参数，时序脉冲信息
        /// </summary>
        /// <param name="message"></param>
        public void UpdatePramter(StatusChangeMessage message)
        {
            if (message.MAC == NodeAttribute.MacSynController)
            {
                //针对时序脉冲，由永磁控制器代替发送
                if (message.Data.Length >= 2)
                {
                    if (message.Data[0] == (byte)CommandIdentify.SynTimeSequence)
                    {
                        if(message.Data[1] == 0xA5)
                        {
                            UpdateStatus("时序脉冲到达，发送同步执行指令。");
                        }
                    }
                }

                if (message.Data.Length < 4)
                {
                    return;
                }
                //是否为SetOne应答
                if (message.Data[0] == ((byte)CommandIdentify.MasterParameterSetOne | 0x80))
                {
                    var time = ((ushort)message.Data[3] << 8) | (message.Data[2]);
                    if (message.Data[1] == 0x0E)//A相
                    {
                        UpdateStatus(string.Format("更新A相合闸时间为{0}us", time));
                    }
                    if (message.Data[1] == 0x0F)//B相
                    {
                        UpdateStatus(string.Format("更新B相合闸时间为{0}us", time));
                    }
                    if (message.Data[1] == 0x10)//C相
                    {
                        UpdateStatus(string.Format("更新C相合闸时间为{0}us", time));
                    }

                    if (message.Data.Length < 6)
                    {
                        return;
                    }
                    var fl = new byte[4];
                    Array.Copy(message.Data, 2, fl, 0, 4);
                    ShortFloating sf = new ShortFloating(fl);
                    if (message.Data[1] == 0x11)
                    {
                        UpdateStatus(string.Format("更新A相补偿时间为{0}us", sf.Value));
                    }
                    if (message.Data[1] == 0x12)
                    {
                        UpdateStatus(string.Format("更新B相补偿时间为{0}us", sf.Value));
                    }
                    if (message.Data[1] == 0x13)
                    {
                        UpdateStatus(string.Format("更新C相补偿时间为{0}us", sf.Value));
                    }
                }


            }
        }

        #endregion


        #region 同步控制器 A/B/C 离线/在线

        /// <summary>
        /// 更新节点在线状态
        /// </summary>
        /// <param name="index"></param>
        private void UpdateNodeOnlineState(byte mac)
        {
            byte index = 0;

            if (NodeAttribute.MacSynController == mac)
            {
                index = 0;
                StatusBar.SetSyn(true, "同步控制器在线");
            }
            else if (NodeAttribute.MacPhaseA == mac)
            {
                index = 1;
                StatusBar.SetPhaseA(true, "A相在线");
                //此模式下，ABC联动
                if (NodeAttribute.SingleThreeMode)
                {
                    StatusBar.SetPhaseB(true, "B相在线");
                    StatusBar.SetPhaseC(true, "C相在线");
                }

            }
            else if (NodeAttribute.MacPhaseB == mac)
            {
                index = 2;
                StatusBar.SetPhaseB(true, "B相在线");
            }
            else if (NodeAttribute.MacPhaseC == mac)
            {
                index = 3;
                StatusBar.SetPhaseC(true, "C相在线");
            }
            else
            {
                return;
            }
            var node = GetNdoe(mac);
            node.ReStartOverTimer();
            node.IsOnline = true;
            node.LastOnline = true;

            OnlineBit = SetBit(OnlineBit, index);
        }


        /// <summary>
        /// 同步控制器 离线/在线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SynController_StatusUpdateEvent(object sender, DataItemSet.StatusMessage e)
        {
            if (e.IsOnline)
            {
                for (int i = 0; i < 1; i++)
                {
                    if (NodeStatusList[0].VoltageLoopCollect[i] != EnergyStatusLoop.Normal)
                    {
                        UpdateStatus("系统电压:" + NodeStatusList[0].VoltageLoopCollect[i].ToString());
                    }
                }
                if (NodeStatusList[0].FrequencyLoopCollect[0] != EnergyStatusLoop.Normal)
                {
                    UpdateStatus("系统频率:" + NodeStatusList[0].FrequencyLoopCollect[0].ToString());
                    NodeAttribute.Period = (int)(1000000 / NodeStatusList[0].Frequency);
                }

                if (!e.Node.LastOnline)
                {
                    StatusBar.SetSyn(true, "同步控制器在线");
                    UpdateStatus("同步控制器恢复在线");
                }
                OnlineBit = SetBit(OnlineBit, 0);
            }
            else
            {
                var comment = "同步控制超时离线";
                StatusBar.SetSyn(false, comment);
                UpdateStatus(comment);
                OnlineBit = ClearBit(OnlineBit, 0);

            }
        }

        /// <summary>
        /// 更新异常状态信息
        /// </summary>
        /// <param name="index">相索引</param>
        void UpadeErrorStatusChange(int index)
        {

            string phase = "";
            switch (index)
            {
                case NodeAttribute.IndexPhaseA:
                    {

                        phase = "A";
                        break;
                    }
                case NodeAttribute.IndexPhaseB:
                    {
                        phase = "B";
                        break;
                    }
                case NodeAttribute.IndexPhaseC:
                    {
                        phase = "C";
                        break;
                    }
                default:
                    {
                        return;
                    }
            }


            //三机构情况
            if (NodeAttribute.SingleThreeMode)
            {
                for (int i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        case 0:
                            {
                                phase = "A";
                                break;
                            }
                        case 1:
                            {
                                phase = "B";
                                break;
                            }
                        case 2:
                            {
                                phase = "C";
                                break;
                            }
                    }
                    if (NodeStatusList[index].EnergyStatusLoopCollect[i] != EnergyStatusLoop.Normal)
                    {
                        UpdateStatus(phase + "相:" + NodeStatusList[1].EnergyStatusLoopCollect[i].ToString());
                    }
                    if ((NodeStatusList[index].StatusLoopCollect[i] == StatusLoop.Error) ||
                        (NodeStatusList[index].StatusLoopCollect[i] == StatusLoop.Null))
                    {
                        UpdateStatus(phase + "相:" + NodeStatusList[index].FrequencyLoopCollect[i].ToString());
                    }
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (NodeStatusList[index].EnergyStatusLoopCollect[i] != EnergyStatusLoop.Normal)
                    {
                        UpdateStatus(string.Format("{0}相回路{1}:", phase, i) + NodeStatusList[1].EnergyStatusLoopCollect[i].ToString());
                    }
                    if ((NodeStatusList[index].StatusLoopCollect[i] == StatusLoop.Error) ||
                        (NodeStatusList[index].StatusLoopCollect[i] == StatusLoop.Null))
                    {
                        UpdateStatus(string.Format("{0}相回路{1}:", phase, i) + NodeStatusList[index].FrequencyLoopCollect[i].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// A相状态更新事件 离线/在线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PhaseA_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                //说明最后一次为非在线状态
                if (!e.Node.LastOnline)
                {
                    StatusBar.SetPhaseA(true, "A相在线");
                    UpdateStatus("A相在线1");

                    //此模式下，ABC联动
                    if (NodeAttribute.SingleThreeMode)
                    {
                        StatusBar.SetPhaseB(true, "B相在线");
                        UpdateStatus("B相在线1");
                        StatusBar.SetPhaseC(true, "C相在线");
                        UpdateStatus("C相在线1");
                    }
                }

                UpadeErrorStatusChange(NodeAttribute.IndexPhaseA);

                OnlineBit = SetBit(OnlineBit, NodeAttribute.IndexPhaseA);


            }
            else
            {
                StatusBar.SetPhaseA(false, "");
                UpdateStatus("A相超时离线");
                OnlineBit = ClearBit(OnlineBit, NodeAttribute.IndexPhaseA);

                //此模式下，ABC联动
                if (NodeAttribute.SingleThreeMode)
                {
                    StatusBar.SetPhaseB(false, "");
                    UpdateStatus("B相超时离线");

                    StatusBar.SetPhaseC(false, "");
                    UpdateStatus("C相超时离线");

                }
            }

            if (NodeAttribute.SingleThreeMode)
            {
                SetControlButtonStateSigleThree();
            }
            else
            {
                SetControlButtonState();
            }

        }

        /// <summary>
        /// B相状态更新事件 离线/在线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PhaseB_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                //说明最后一次为非在线状态
                if (!e.Node.LastOnline)
                {
                    StatusBar.SetPhaseB(true, "B相在线");
                    UpdateStatus("B相在线1");
                }
                OnlineBit = SetBit(OnlineBit, NodeAttribute.IndexPhaseB);
                UpadeErrorStatusChange(NodeAttribute.IndexPhaseB);
            }
            else
            {
                StatusBar.SetPhaseB(false, "");
                UpdateStatus("B相超时离线");
                OnlineBit = ClearBit(OnlineBit, NodeAttribute.IndexPhaseB);
            }

            SetControlButtonState();
        }

        /// <summary>
        /// C相状态更新事件 离线/在线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PhaseC_StatusUpdateEvent(object sender, StatusMessage e)
        {
            if (e.IsOnline)
            {
                //说明最后一次为非在线状态
                if (!e.Node.LastOnline)
                {
                    StatusBar.SetPhaseC(true, "C相在线");
                    UpdateStatus("C相在线1");
                }
                OnlineBit = SetBit(OnlineBit, NodeAttribute.IndexPhaseC);
                UpadeErrorStatusChange(NodeAttribute.IndexPhaseC);
            }
            else
            {
                StatusBar.SetPhaseC(false, "");
                UpdateStatus("C相超时离线");
                OnlineBit = ClearBit(OnlineBit, NodeAttribute.IndexPhaseC);
            }
            SetControlButtonState();
        }
        #endregion


        #region 操作按钮
        /// <summary>
        /// 设置用户控件使能
        /// </summary>
        /// <param name="mac">mac地址</param>
        /// <param name="cmd">应答命令</param>
        private void SetUserControlEnable(byte mac, CommandIdentify cmd)
        {


            if (NodeAttribute.MacSynController == mac)//同步控制器                    
            {
                switch (cmd)
                {
                    //执行合闸
                    case CommandIdentify.SyncOrchestratorCloseAction:
                        {
                            break;
                        }
                    //预备合闸
                    case CommandIdentify.SyncOrchestratorReadyClose:
                        {

                            break;
                        }
                }

            }
            else if (NodeAttribute.MacPhaseA == mac) //A相  
            {
                switch (cmd)
                {
                    case CommandIdentify.CloseAction://合闸执行
                        {
                            UserControlEnable.CloseReadyA = true;
                            UserControlEnable.CloseActionA = false;

                            //停止响应的超时定时器
                            if (UserControlEnable.OverTimerReadyActionA != null)
                            {
                                UserControlEnable.OverTimerReadyActionA.StopTimer();
                            }
                            UserControlEnable.OperateA = false;
                            break;
                        }
                    case CommandIdentify.ReadyClose: // 合闸预制
                        {
                            UserControlEnable.CloseReadyA = false;
                            UserControlEnable.CloseActionA = true;


                            break;
                        }
                    case CommandIdentify.ReadyOpen:  //分闸预制                   
                        {
                            UserControlEnable.OpenReadyA = false;
                            UserControlEnable.OpenActionA = true;
                            break;
                        }
                    case CommandIdentify.OpenAction: //分闸执行
                        {
                            UserControlEnable.OpenReadyA = true;
                            UserControlEnable.OpenActionA = false;

                            //停止响应的超时定时器
                            if (UserControlEnable.OverTimerReadyActionA != null)
                            {
                                UserControlEnable.OverTimerReadyActionA.StopTimer();
                            }
                            UserControlEnable.OperateA = false;
                            break;
                        }

                    case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                        {

                            break;
                        }

                }


            }
            else if (NodeAttribute.MacPhaseB == mac)//B相 
            {
                switch (cmd)
                {
                    case CommandIdentify.CloseAction://合闸执行
                        {
                            UserControlEnable.CloseReadyB = true;
                            UserControlEnable.CloseActionB = false;

                            //停止响应的超时定时器
                            if (UserControlEnable.OverTimerReadyActionB != null)
                            {
                                UserControlEnable.OverTimerReadyActionB.StopTimer();
                            }
                            UserControlEnable.OperateB = false;
                            break;
                        }
                    case CommandIdentify.ReadyClose: // 合闸预制
                        {
                            UserControlEnable.CloseReadyB = false;
                            UserControlEnable.CloseActionB = true;


                            break;
                        }
                    case CommandIdentify.ReadyOpen:  //分闸预制                   
                        {
                            UserControlEnable.OpenReadyB = false;
                            UserControlEnable.OpenActionB = true;
                            break;
                        }
                    case CommandIdentify.OpenAction: //分闸执行
                        {
                            UserControlEnable.OpenReadyB = true;
                            UserControlEnable.OpenActionB = false;

                            //停止响应的超时定时器
                            if (UserControlEnable.OverTimerReadyActionB != null)
                            {
                                UserControlEnable.OverTimerReadyActionB.StopTimer();
                            }
                            UserControlEnable.OperateB = false;

                            break;
                        }

                    case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                        {

                            break;
                        }

                }

            }
            else if (NodeAttribute.MacPhaseC == mac) //C相
            {

                switch (cmd)
                {
                    case CommandIdentify.CloseAction://合闸执行
                        {
                            UserControlEnable.CloseReadyC = true;
                            UserControlEnable.CloseActionC = false;

                            //停止响应的超时定时器
                            if (UserControlEnable.OverTimerReadyActionC != null)
                            {
                                UserControlEnable.OverTimerReadyActionC.StopTimer();
                            }
                            UserControlEnable.OperateC = false;
                            break;
                        }
                    case CommandIdentify.ReadyClose: // 合闸预制
                        {
                            UserControlEnable.CloseReadyC = false;
                            UserControlEnable.CloseActionC = true;
                            break;
                        }
                    case CommandIdentify.ReadyOpen:  //分闸预制                   
                        {
                            UserControlEnable.OpenReadyC = false;
                            UserControlEnable.OpenActionC = true;
                            break;
                        }
                    case CommandIdentify.OpenAction: //分闸执行
                        {
                            UserControlEnable.OpenReadyC = true;
                            UserControlEnable.OpenActionC = false;

                            //停止响应的超时定时器
                            if (UserControlEnable.OverTimerReadyActionC != null)
                            {
                                UserControlEnable.OverTimerReadyActionC.StopTimer();
                            }
                            UserControlEnable.OperateC = false;
                            break;
                        }

                    case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                        {

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 更新设置控件状态
        /// </summary>
        public void SetControlButtonState()
        {
            for (int i = 1; i < 4; i++)
            {
                //电能正常，且属于合位，使能分闸按钮
                if (NodeStatusList[i].EnergyStatus == EnergyStatusLoop.Normal)
                {
                    UserControlEnable.ChooiceEnableButton((UInt16)((i << 8) | ((byte)NodeStatusList[i].PositStatus)));
                    UserControlEnable.SetSynControlButtonState(SynPhaseChoice.ConfigByte);
                }
                else
                {
                    UserControlEnable.ChooiceEnableButton((UInt16)((i << 8)));//关闭所有
                    UserControlEnable.SetSynControlButtonState(SynPhaseChoice.ConfigByte);
                }
            }
        }
        /// <summary>
        /// 针对单相模式的三相控制
        /// </summary>
        public void SetControlButtonStateSigleThree()
        {
            for (int i = 0; i < 3; i++)
            {
                //电能正常，且属于合位，使能分闸按钮
                if (NodeStatusList[NodeAttribute.IndexPhaseA].EnergyStatusLoopCollect[i] == EnergyStatusLoop.Normal)
                {
                    UserControlEnable.ChooiceEnableButton((UInt16)(((i + 1) << 8) | ((byte)NodeStatusList[NodeAttribute.IndexPhaseA].StatusLoopCollect[i])));
                    UserControlEnable.SetSynControlButtonState(SynPhaseChoice.ConfigByte);
                }
                else
                {
                    UserControlEnable.ChooiceEnableButton((UInt16)(((i + 1) << 8)));//关闭所有
                    UserControlEnable.SetSynControlButtonState(SynPhaseChoice.ConfigByte);
                }
            }
        }

        #endregion


        #region 合分闸预制执行状态
        /// <summary>
        /// 合闸预制状态
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="data"></param>
        public void UpdateNodeReadyActionStatus(byte mac, byte[] data)
        {
            //首先判断是否启用SingleThreeMode
            if (NodeAttribute.SingleThreeMode)
            {
                UpdateNodeStatusSingleThree(mac, data);
                return;
            }

            var node = GetNdoe(mac);
            if (node == null)
            {
                return;
            }
            UpdateNodeOnlineState(mac);
            //if (!node.IsValid(data))
            //{
            //    UpdateStatus(node.Mac.ToString("X2") + "应答数据无效！");
            //    return;
            //}
            var cmd = (CommandIdentify)(data[0] & 0x7F);

            node.ResetState();
            if (NodeAttribute.MacSynController == mac)//同步控制器                    
            {
                switch (cmd)
                {
                    //执行合闸
                    case CommandIdentify.SyncOrchestratorCloseAction:
                        {
                            node.SynActionCloseState = true;
                            node.SynReadyCloseState = false;
                            UpdateStatus("同步控制器，执行合闸");
                            break;
                        }
                    //预备合闸
                    case CommandIdentify.SyncOrchestratorReadyClose:
                        {
                            node.SynReadyCloseState = true;
                            node.SynActionCloseState = false;
                            UpdateStatus("同步控制器，预制合闸");
                            break;
                        }
                }
            }
            else if ((NodeAttribute.MacPhaseA == mac) || (NodeAttribute.MacPhaseB == mac) || (NodeAttribute.MacPhaseC == mac)) //A相                  
            {

                switch (cmd)
                {
                    case CommandIdentify.CloseAction://合闸执行
                        {
                            node.ActionCloseState = true;
                            node.ReadyCloseState = false;
                            UpdateStatus(mac, "合闸执行");
                            break;
                        }
                    case CommandIdentify.OpenAction: //分闸执行
                        {
                            node.ActionOpenState = true;
                            node.ReadyOpenState = false;
                            UpdateStatus(mac, "分闸执行");
                            break;
                        }
                    case CommandIdentify.ReadyClose: // 合闸预制
                        {
                            node.ReadyCloseState = true;
                            node.ActionCloseState = false;
                            UpdateStatus(mac, "合闸预制");

                            break;
                        }
                    case CommandIdentify.ReadyOpen:  //分闸预制                   
                        {
                            node.ReadyOpenState = true;
                            node.ActionOpenState = false;
                            UpdateStatus(mac, "分闸预制");
                            break;
                        }
                    case CommandIdentify.SyncReadyClose:  //同步合闸预制 
                        {
                            UpdateStatus(mac, "同步合闸预制");
                            node.SynReadyCloseState = true;

                            break;
                        }

                }

            }

            //设置用户控件使能状态
            //不是ABC 一起动模式

            if (!UserControlEnable.OperateABC)
            {
                SetUserControlEnable(mac, cmd);

            }
            else
            {
                UpdateWholeOperate();
                return;
            }
            //同步合闸模式
            UpdateOperateSyn();
        }
        public void UpdateWholeOperate()
        {
            //ABC一起动模式
            //均为合闸预制模式
            if (NodeStatusList[1].ReadyCloseState && NodeStatusList[2].ReadyCloseState && NodeStatusList[3].ReadyCloseState)
            {
                UserControlEnable.CloseReady = false;
                UserControlEnable.CloseAction = true;

            }
            else if (NodeStatusList[1].ActionCloseState && NodeStatusList[2].ActionCloseState && NodeStatusList[3].ActionCloseState)
            {
                UserControlEnable.CloseReady = true;
                UserControlEnable.CloseAction = false;
                UserControlEnable.OverTimerReadyActionABC.StopTimer();
                UserControlEnable.OperateABC = false;
            }

            //均为分闸预制模式
            else if (NodeStatusList[1].ReadyOpenState && NodeStatusList[2].ReadyOpenState && NodeStatusList[3].ReadyOpenState)
            {
                UserControlEnable.OpenReady = false;
                UserControlEnable.OpenAction = true;
            }
            else if (NodeStatusList[1].ActionOpenState && NodeStatusList[2].ActionOpenState && NodeStatusList[3].ActionOpenState)
            {
                UserControlEnable.OpenReady = true;
                UserControlEnable.OpenAction = false;
                UserControlEnable.OverTimerReadyActionABC.StopTimer();
                UserControlEnable.OperateABC = false;
            }
        }
        /// <summary>
        /// 同步合闸模式更新
        /// </summary>
        public void UpdateOperateSyn()
        {
            if (UserControlEnable.OperateSyn)
            {

                bool state = true;
                //根据选择的
                if (SynPhaseChoice.GetPhaseEnable(1))
                {
                    state = state && NodeStatusList[1].SynReadyCloseState;
                }
                if (SynPhaseChoice.GetPhaseEnable(2))
                {
                    state = state && NodeStatusList[2].SynReadyCloseState;
                }
                if (SynPhaseChoice.GetPhaseEnable(3))
                {
                    state = state && NodeStatusList[3].SynReadyCloseState;
                }
                state = state && NodeStatusList[0].SynReadyCloseState;

                if (state)
                {
                    UserControlEnable.SynCloseAction = true;
                    UserControlEnable.SynCloseReady = false;
                }



                if (NodeStatusList[0].SynActionCloseState)
                {
                    UserControlEnable.SynCloseAction = false;
                    UserControlEnable.SynCloseReady = true;

                    UserControlEnable.OverTimerReadyActionSyn.StopTimer();
                    UserControlEnable.OperateSyn = false;
                }

            }
        }
        /// <summary>
        /// 合闸预制状态, 单开关三相
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="data"></param>
        public void UpdateNodeStatusSingleThree(byte mac, byte[] data)
        {
            var node = GetNdoe(mac);
            if (node == null)
            {
                return;
            }
            UpdateNodeOnlineState(mac);
            //if (!node.IsValid(data))
            //{
            //    UpdateStatus(node.Mac.ToString("X2") + "应答数据无效！");
            //    return;
            //}
            var cmd = (CommandIdentify)(data[0] & 0x7F);

            node.ResetState();
            if (NodeAttribute.MacSynController == mac)//同步控制器                    
            {
                switch (cmd)
                {
                    //执行合闸
                    case CommandIdentify.SyncOrchestratorCloseAction:
                        {
                            node.SynActionCloseState = true;
                            node.SynReadyCloseState = false;
                            UpdateStatus("同步控制器，执行合闸");
                            break;
                        }
                    //预备合闸
                    case CommandIdentify.SyncOrchestratorReadyClose:
                        {
                            node.SynReadyCloseState = true;
                            node.SynActionCloseState = false;
                            UpdateStatus("同步控制器，预制合闸");
                            break;
                        }
                }
            }
            else if (NodeAttribute.MacPhaseA == mac) //A相                  
            {
                var loop = data[1];
                for (int k = 0; k < 3; k++)
                {
                    switch (k)
                    {
                        case 0:
                            {
                                if ((loop & NodeAttribute.LoopI) == NodeAttribute.LoopI)
                                {
                                    node = NodeStatusList[1];
                                }

                                else
                                {
                                    continue;
                                }
                                break;
                            }
                        case 1:
                            {
                                if ((loop & NodeAttribute.LoopII) == NodeAttribute.LoopII)
                                {
                                    node = NodeStatusList[2];
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            }
                        case 2:
                            {
                                if ((loop & NodeAttribute.LoopIII) == NodeAttribute.LoopIII)
                                {
                                    node = NodeStatusList[3];
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            }
                        default:
                            {
                                throw new Exception("UpdateNodeStatusSingleThree,未定义相");
                            }

                    }



                    switch (cmd)
                    {
                        case CommandIdentify.CloseAction://合闸执行
                            {
                                node.ActionCloseState = true;
                                node.ReadyCloseState = false;
                                UpdateStatusSingleThree(loop, "合闸执行");
                                break;
                            }
                        case CommandIdentify.OpenAction: //分闸执行
                            {
                                node.ActionOpenState = true;
                                node.ReadyOpenState = false;
                                UpdateStatusSingleThree(loop, "分闸执行");
                                break;
                            }
                        case CommandIdentify.ReadyClose: // 合闸预制
                            {
                                node.ReadyCloseState = true;
                                node.ActionCloseState = false;
                                UpdateStatusSingleThree(loop, "合闸预制");

                                break;
                            }
                        case CommandIdentify.ReadyOpen:  //分闸预制                   
                            {
                                node.ReadyOpenState = true;
                                node.ActionOpenState = false;
                                UpdateStatusSingleThree(loop, "分闸预制");
                                break;
                            }


                    }

                }
                if (cmd == CommandIdentify.SyncReadyClose)  //同步合闸预制 
                {
                    bool finshed = false;
                    for (int i = 0; i < 3; i++)
                    {
                        var phase = (loop >> (2 * i)) & 0x03;
                        finshed = false;
                        switch (phase)
                        {
                            case 1:
                                {
                                    UpdateStatus("A相同步合闸预制");
                                    NodeStatusList[NodeAttribute.IndexPhaseA].SynReadyCloseState = true;
                                    break;
                                }
                            case 2:
                                {
                                    UpdateStatus("B相同步合闸预制");
                                    NodeStatusList[NodeAttribute.IndexPhaseB].SynReadyCloseState = true;
                                    break;
                                }
                            case 3:
                                {
                                    UpdateStatus("C相同步合闸预制");
                                    NodeStatusList[NodeAttribute.IndexPhaseC].SynReadyCloseState = true;
                                    break;
                                }
                            default:
                                {
                                    UpdateStatus("未知相，同步合闸预制");
                                    finshed = true;
                                    break;
                                }
                        }
                        if (finshed)
                        {
                            break;
                        }
                    }


                }


            }

            //设置用户控件使能状态
            //不是ABC 一起动模式

            if (!UserControlEnable.OperateABC)
            {
                var loop = data[1];
                for (int k = 0; k < 3; k++)
                {
                    switch (k)
                    {
                        case 0:
                            {
                                if ((loop & NodeAttribute.LoopI) == NodeAttribute.LoopI)
                                {
                                    node = NodeStatusList[1];
                                }
                                break;
                            }
                        case 1:
                            {
                                if ((loop & NodeAttribute.LoopII) == NodeAttribute.LoopII)
                                {
                                    node = NodeStatusList[2];
                                }
                                break;
                            }
                        case 2:
                            {
                                if ((loop & NodeAttribute.LoopIII) == NodeAttribute.LoopIII)
                                {
                                    node = NodeStatusList[3];
                                }
                                break;
                            }
                        default:
                            {
                                throw new Exception("UpdateNodeStatusSingleThree,未定义相");
                            }

                    }
                    SetUserControlEnable(node.Mac, cmd);
                }



            }
            else
            {
                UpdateWholeOperate();
                return;
            }
            //同步合闸模式
            UpdateOperateSyn();
        }
        #endregion

        /// <summary>
        /// 获根据MAC地址获取节点状态
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public NodeStatus GetNdoe(byte mac)
        {
            foreach (var m in NodeStatusList)
            {
                if (m.Mac == mac)
                {
                    return m;
                }
            }
            return null;
        }
        /// <summary>
        /// 更新节点状态改变信息，循环心跳报告等
        /// </summary>
        /// <param name="mac">MAC ID</param>
        /// <param name="data">数据</param>
        internal void UpdateTickMesasge(byte mac, byte[] data)
        {
            var node = GetNdoe(mac);
            if (node == null)
            {
                return;
            }


            if (NodeAttribute.MacSynController == mac)
            {
                node.UpdateSynStatus(data);

            }
            else if ((NodeAttribute.MacPhaseA == mac) ||
                (NodeAttribute.MacPhaseB == mac) ||
                (NodeAttribute.MacPhaseC == mac))
            {
                node.UpdateSwitchStatus(data);

            }
        }


        #region 更新状态栏信息

        void UpdateStationSingleStatus(byte mac, NetStep step)
        {
            Action<bool, string> action;
            string comment = "";



            if (NodeAttribute.MacSynController == mac)
            {
                action = statusBar.SetSyn;
                comment = "同步控制器";

            }
            else if (NodeAttribute.MacPhaseA == mac)
            {
                action = statusBar.SetPhaseA;
                comment = "A相";

            }
            else if (NodeAttribute.MacPhaseB == mac)
            {
                action = statusBar.SetPhaseB;
                comment = "B相";

            }
            else if (NodeAttribute.MacPhaseC == mac)
            {
                action = statusBar.SetPhaseC;
                comment = "C相";

            }
            else
            {
                return;
            }

            switch (step)
            {
                case NetStep.Start:
                    {
                        comment += "启动连接";
                        action(false, comment);
                        break;
                    }
                case NetStep.Linking:
                    {
                        comment += "建立显示连接";
                        action(true, comment);
                        break;
                    }
                case NetStep.StatusChange:
                    {
                        comment += "建立状态变化连接";
                        action(true, comment);
                        break;
                    }
                case NetStep.Cycle:
                    {
                        comment += "在线";
                        action(true, comment);
                        break;
                    }
                default:
                    {
                        return;
                    }

            }

            UpdateStatus(comment);
        }
        /// <summary>
        /// 更新站点信息
        /// </summary>
        /// <param name="defStationInformation"></param>
        internal void StationLinkChanged(DeviceNet.Element.DefStationInformation defStationInformation)
        {
            UpdateStationSingleStatus(defStationInformation.MacID, defStationInformation.Step);
            //ABC联动
            if (NodeAttribute.SingleThreeMode)
            {
                if (defStationInformation.MacID == NodeAttribute.MacPhaseA)
                {
                    UpdateStationSingleStatus(NodeAttribute.MacPhaseB, defStationInformation.Step);
                    UpdateStationSingleStatus(NodeAttribute.MacPhaseC, defStationInformation.Step);
                }
            }
        }
        #endregion


        private Action<string> UpdateStatusDelegate;

        /// <summary>
        /// 更新状态信息
        /// </summary>
        /// <param name="des"></param>
        public void UpdateStatus(string des)
        {
            if (UpdateStatusDelegate != null)
            {
                UpdateStatusDelegate(des);
            }
        }
        public void UpdateStatus(byte mac, string des)
        {

            if (NodeAttribute.MacPhaseA == mac)
            {
                UpdateStatus("A相:" + des);
            }
            else if (NodeAttribute.MacPhaseB == mac)
            {
                UpdateStatus("B相:" + des);
            }
            else if (NodeAttribute.MacPhaseC == mac)
            {
                UpdateStatus("C相:" + des);
            }
            else
            {
                UpdateStatus(des);
            }

        }
        public void UpdateStatusSingleThree(byte loop, string des)
        {

            if (loop == NodeAttribute.LoopI)
            {
                UpdateStatus("A相:" + des);
            }
            else if (loop == NodeAttribute.LoopII)
            {
                UpdateStatus("B相:" + des);
            }
            else if (loop == NodeAttribute.LoopIII)
            {
                UpdateStatus("C相:" + des);
            }
            else
            {
                UpdateStatus(des);
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

        /// <summary>
        /// 初始化逻辑呈现
        /// </summary>
        public LogicalPresentation(Action<string> updateDelegate)
        {
            try
            {
                IndicatorLightABC = new IndicatorLight();
                NodeStatusList = new ObservableCollection<NodeStatus>();
                NodeStatusList.Add(new NodeStatus(NodeAttribute.MacSynController, "同步控制器", 7000));
                NodeStatusList.Add(new NodeStatus(NodeAttribute.MacPhaseA, "A相控制器", 7000));
                NodeStatusList.Add(new NodeStatus(NodeAttribute.MacPhaseB, "B相控制器", 7000));
                NodeStatusList.Add(new NodeStatus(NodeAttribute.MacPhaseC, "C相控制器", 7000));

                NodeStatusList[0].StatusUpdateEvent += SynController_StatusUpdateEvent;
                NodeStatusList[1].StatusUpdateEvent += PhaseA_StatusUpdateEvent;
                NodeStatusList[2].StatusUpdateEvent += PhaseB_StatusUpdateEvent;
                NodeStatusList[3].StatusUpdateEvent += PhaseC_StatusUpdateEvent;
                //ABC指示灯
                NodeStatusList[1].StatusUpdateEvent += IndicatorLightABC.PhaseA_StatusUpdateEvent;
                NodeStatusList[2].StatusUpdateEvent += IndicatorLightABC.PhaseB_StatusUpdateEvent;
                NodeStatusList[3].StatusUpdateEvent += IndicatorLightABC.PhaseC_StatusUpdateEvent;

                StatusBar = new StatusBarMessage(NodeAttribute.CurrentUser.Name);

                _onlineBit = 0;

                UserControlEnable = new EnableControl();

                UpdateStatusDelegate = updateDelegate;

                SynPhaseChoice = new PhaseChoice();

                TestParameter = new LoopTest();
                NodeParameter = new ConfigParameter();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
