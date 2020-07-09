using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChoicePhase.DeviceNet.Element;
using ChoicePhase.PlatformModel.Helper;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 节点状态，永磁控制器，同步控制器
    /// </summary>
    public class NodeStatus : ObservableObject
    {
        /// <summary>
        /// 节点状态更新事件
        /// </summary>
        public event EventHandler<StatusMessage> StatusUpdateEvent;


        /// <summary>
        /// MAC地址
        /// </summary>
        public byte Mac
        {
            get;
            private set;
        }

        /// <summary>
        /// 节点名称
        /// </summary>
        public String Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 同步配置字
        /// </summary>
        public byte SynConfigByte
        {
            get;
            set;
        }
        /// <summary>
        /// 延时时间1 us
        /// </summary>
        public ushort DelayTime1
        {
            get;
            set;
        }
        /// <summary>
        /// 延时时间2  us
        /// </summary>
        public ushort DelayTime2
        {
            get;
            set;
        }
        /// <summary>
        /// 分闸预制状态
        /// </summary>
        public bool ReadyOpenState
        {
            get;
            set;
        }
        /// <summary>
        /// 合闸预制状态
        /// </summary>
        public bool ReadyCloseState
        {
            get;
            set;
        }
        /// <summary>
        /// 同步合闸预制状态
        /// </summary>
        public bool SynReadyCloseState
        {
            get;
            set;
        }
        /// <summary>
        /// 分闸执行状态
        /// </summary>
        public bool ActionOpenState
        {
            get;
            set;
        }
        /// <summary>
        /// 合闸执行状态
        /// </summary>
        public bool ActionCloseState
        {
            get;
            set;
        }
        /// <summary>
        /// 同步合闸执行状态
        /// </summary>
        public bool SynActionCloseState
        {
            get;
            set;
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        public byte[] LastSendData
        {
            get;
            set;
        }

        /// <summary>
        /// 上一次是否在线
        /// </summary>
        public bool LastOnline
        {
            get;
            set;
        }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline
        {
            get;
            set;
        }

        /// <summary>
        /// 单相的合分位状态
        /// </summary>
        public StatusLoop PositStatus
        {
            get
            {
                if (statusLoopCollect != null)
                {
                    if (statusLoopCollect[0] == statusLoopCollect[1])
                    {
                        return statusLoopCollect[0];
                    }
                    else
                    {
                        return StatusLoop.Error;
                    }
                }
                return StatusLoop.Error;
            }
        }

        private StatusLoop[] statusLoopCollect;
        /// <summary>
        /// 合分状态1
        /// </summary>
        public StatusLoop[] StatusLoopCollect
        {
            get
            {
                return statusLoopCollect;
            }
            set
            {
                statusLoopCollect = value;
                RaisePropertyChanged("StatusLoopCollect");
               
            }
        }

        /// <summary>
        /// 整体储能状态,默认为2个回路.有一个More，整体为More，有一个为Null或Less，认为为Less
        /// </summary>
        public EnergyStatusLoop EnergyStatus
        {
            get
            {
                if (energyStatusLoopCollect != null)
                {
                    if (energyStatusLoopCollect[0] == energyStatusLoopCollect[1])
                    {
                        return energyStatusLoopCollect[0];
                    }
                    else
                    {
                        if ((energyStatusLoopCollect[0] == EnergyStatusLoop.More) ||
                            (energyStatusLoopCollect[1] == EnergyStatusLoop.More))
                        {
                            return EnergyStatusLoop.More;
                        }
                        return EnergyStatusLoop.Less;
                    }
                }
                return EnergyStatusLoop.Less;
            }
        
        }


        private EnergyStatusLoop[] energyStatusLoopCollect;
        /// <summary>
        /// 回路储能状态1
        /// </summary>
        public EnergyStatusLoop[] EnergyStatusLoopCollect
        {
            get
            {
                return energyStatusLoopCollect;
            }
            set
            {
                energyStatusLoopCollect = value;
                RaisePropertyChanged("EnergyStatusLoopCollect");
            }
        }
        /// <summary>
        /// 超时时间默认为3000 ms
        /// </summary>
        public int OverTimeMs
        {
            get;
            private set;
        }


        /// <summary>
        /// 超时定时器--循环判断是否有接收数据
        /// </summary>
        private OverTimeTimer overTimerCycle;

        /// <summary>
        /// 重新启动超时定时器
        /// </summary>
        public void ReStartOverTimer()
        {
            overTimerCycle.ReStartTimer();
        }
        /// <summary>
        /// 循环接收状态，超时处理
        /// </summary>
        private void CycleOverTime()
        {
            LastOnline = IsOnline;
            IsOnline = false;
            if (StatusUpdateEvent != null)
            {
                StatusUpdateEvent(this, new StatusMessage(false, this));
            }
        }

        /// <summary>
        /// 频率集合
        /// </summary>
        public EnergyStatusLoop[] FrequencyLoopCollect
        {
            get;
            set;
        }

        /// <summary>
        /// 电压集合
        /// </summary>
        public EnergyStatusLoop[] VoltageLoopCollect
        {
            set;
            get;
        }


        //电压频率
        public float Frequency;

        /// <summary>
        /// 更新同步状态
        /// </summary>
        /// <param name="statusByte"></param>
        public void UpdateSynStatus(byte[] statusByte)
        {
            //判断长度
            if (statusByte.Length <5)
            {
                return;
            }
            for (int k = 0; k < 4; k++)
            {
                VoltageLoopCollect[k] = (EnergyStatusLoop)((statusByte[2] >> (2 * k)) & (0x03));
            }
            Frequency = (float)((statusByte[3] + ((UInt16)statusByte[4] << 8)) * 0.001);
           
            if (Frequency < 49)
            {
                FrequencyLoopCollect[0] = EnergyStatusLoop.Less;
            }
            else if (Frequency < 51)
            {
                FrequencyLoopCollect[0] = EnergyStatusLoop.Normal;
            }
            else
            {
                FrequencyLoopCollect[0] = EnergyStatusLoop.More;
            }



            for (int k = 2; k < 4; k++)
            {
                FrequencyLoopCollect[k] = EnergyStatusLoop.Null;
            }
            LastOnline = IsOnline;
            IsOnline = true;      
            
            if (StatusUpdateEvent != null)
            {
                StatusUpdateEvent(this, new StatusMessage(IsOnline, this));
            }

            overTimerCycle.ReStartTimer();
        }
       
        /// <summary>
        /// 更新开关状态
        /// </summary>
        /// <param name="statusByte"></param>
        public void UpdateSwitchStatus(byte[] statusByte)
        {
            //判断长度
            if (statusByte.Length <6)
            {
                return;
            }         
            for( int k = 0; k < 4; k++)
            {
                StatusLoopCollect[k] = (StatusLoop)((statusByte[1] >> (2 * k)) & (0x03));
            }
            for (int k = 0; k < 4; k++)
            {
                EnergyStatusLoopCollect[k] = (EnergyStatusLoop)((statusByte[3] >> (2 * k)) & (0x03));
            }
            LastOnline = IsOnline;

            IsOnline = true;

            if (StatusUpdateEvent != null)
            {
                StatusUpdateEvent(this, new StatusMessage(IsOnline, this));
            }

            overTimerCycle.ReStartTimer();
        }

        /// <summary>
        /// 比较应答数据是否有效
        /// </summary>
        /// <param name="reciveData"></param>
        /// <returns></returns>
        public bool IsValid(byte[] reciveData)
        {
            if (LastSendData == null)
            {
                return false;
            }
            if (reciveData == null)
            {
                return false;
            }
            //if (reciveData.Length != LastSendData.Length)
            //{
            //    return false;
            //}
            //比较是否为应答
            if ((LastSendData[0] | 0x80) != reciveData[0] )
            {
                return false;
            }
            for (int i = 1; i < reciveData.Length; i++)
            {
                if (reciveData[i] != LastSendData[i])
                {
                    return false;
                }
            }
            return true;            
        }

        /// <summary>
        /// 复位指示状态
        /// </summary>
        public void ResetState()
        {
            ReadyOpenState = false;
            ReadyCloseState = false;
           
            ActionOpenState = false;
            ActionCloseState = false;

            SynReadyCloseState = false;
            SynActionCloseState = false;

           // LastOnline = true;
           // IsOnline = false;
        }

        /// <summary>
        /// 节点状态
        /// </summary>
        /// <param name="mac">MAC地址</param>
        /// <param name="name">名称</param>
        /// <param name="overMs">超时时间ms</param>
        public NodeStatus(byte mac, string name, int overMs)
        {
            Mac = mac;
            Name = name;

            StatusLoopCollect = new StatusLoop[4];
            for (int i = 0; i < StatusLoopCollect.Length; i++ )
            {
                StatusLoopCollect[i] = DataItemSet.StatusLoop.Null;
            }

            EnergyStatusLoopCollect = new EnergyStatusLoop[4];
            for (int i = 0; i < EnergyStatusLoopCollect.Length; i++)
            {
                EnergyStatusLoopCollect[i] = DataItemSet.EnergyStatusLoop.Null;
            }
            OverTimeMs = overMs;
            overTimerCycle = new OverTimeTimer(OverTimeMs, CycleOverTime);

            VoltageLoopCollect = new EnergyStatusLoop[4];
            for (int i = 0; i < VoltageLoopCollect.Length; i++)
            {
                VoltageLoopCollect[i] = DataItemSet.EnergyStatusLoop.Null;
            }

            FrequencyLoopCollect = new EnergyStatusLoop[4];
            for (int i = 0; i < FrequencyLoopCollect.Length; i++)
            {
                FrequencyLoopCollect[i] = DataItemSet.EnergyStatusLoop.Null;
            }

            SynConfigByte = 0x09;
            DelayTime1 = 50;
        }
    }

    /// <summary>
    /// 节点离线/在线信息
    /// </summary>
    public class StatusMessage : EventArgs
    {
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline
        {
            get;
            set;
        }
        /// <summary>
        /// 节点状态
        /// </summary>
        public NodeStatus Node
        {
            get;
            set;
        }


        /// <summary>
        /// 预制，执行消息
        /// </summary>
        /// <param name="mac">MAC</param>
        /// <param name="serverData">服务数据</param>
        public StatusMessage(bool isOnline, NodeStatus node)
        {
            IsOnline = isOnline;
            Node = node;
        }
    }

    /// <summary>
    /// 回路状态
    /// </summary>
    public enum StatusLoop
    {
        /// <summary>
        /// 空
        /// </summary>
        Null = 0,
        /// <summary>
        /// 合位
        /// </summary>
        Close = 2,
        /// <summary>
        /// 分位
        /// </summary>
        Open = 1,
        /// <summary>
        /// 故障
        /// </summary>
        Error = 3,
    }
    /// <summary>
    /// 储能/电压/频率状态
    /// </summary>
    public enum EnergyStatusLoop
    {
        /// <summary>
        /// 空
        /// </summary>
        Null = 0,

        /// <summary>
        /// 范围内
        Normal = 2,
        /// <summary>
        /// 小于下限
        /// </summary>
        Less = 1,
        /// <summary>
        /// 超过
        /// </summary>
        More = 3,
    }
}
