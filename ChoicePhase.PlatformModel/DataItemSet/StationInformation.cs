using Distribution.station;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using static ChoicePhase.PlatformModel.DataItemSet.SwitchProperty;
using System.Windows;

namespace ChoicePhase.PlatformModel.DataItemSet
{

    
    /// <summary>
    /// 站点信息
    /// </summary>
    public class StationInformation : ObservableObject
    {
        private StationMessage _message;//更新的站点信息

        private node_property _currentNode;



        #region node_property

        public uint IdOwn
        {
            get
            {
                return _message.id_own;
            }
            set
            {
                _message.id_own = value;
                RaisePropertyChanged("ID");
            }
        }

        /// <summary>
        /// id
        /// /// </summary>
        public uint ID
        {
            get
            {
                return _currentNode.id;
            }
            set
            {
                _currentNode.id = value;
                RaisePropertyChanged("ID");
            }
        }

        /// <summary>
        /// 开关类型
        /// </summary>
        public SwitchType Type
        {
            get
            {
                return (SwitchType)_currentNode.type;
            }
            set
            {
                _currentNode.type = (uint)value;
                RaisePropertyChanged("Type");
            }
        }

        /// <summary>
        /// 开关状态        
        /// </summary>
        public SwitchState State
        {
            get
            {
                return (SwitchState)_currentNode.state;
            }
            set
            {
                _currentNode.state = (uint)value;
                RaisePropertyChanged("State");
            }
        }

        /// <summary>
        /// 邻居数量     
        /// </summary>
        public uint NeighbourCount
        {
            get
            {
                return (uint)_currentNode.neighbourCollect.Count;
            }
        }

        string HexCollectToString(List<uint> collect)
        {
            var sb = new StringBuilder(20);
            foreach(var m in collect)
            {
                sb.AppendFormat("{0:X},", m);
            }
            return sb.ToString();

        }

        /// <summary>
        /// 邻居集合        
        /// </summary>
        public string NeighbourCollect
        {
            get
            {
                return HexCollectToString(_currentNode.neighbourCollect);
            }
            set
            {
                RaisePropertyChanged("NeighbourCollect");
            }
        }


        /// <summary>
        /// 操作类型     
        /// </summary>
        public OperateType Operate
        {
            get
            {
                return (OperateType)_currentNode.operateType;;
            }
            set
            {
                _currentNode.operateType = (uint)value;
                RaisePropertyChanged("Operate");
            }
        }


        /// <summary>
        /// 超时类型     
        /// </summary>
        public OverTimeType OverTime
        {
            get
            {
                return (OverTimeType)_currentNode.overTimeType;
            }
            set
            {
                _currentNode.overTimeType = (uint)value;
                RaisePropertyChanged("OverTime");
            }
        }


        /// <summary>
        /// 移除情况     
        /// </summary>
        public ResultType RemovalType
        {
            get
            {
                return (ResultType)_currentNode.removalType;
            }
            set
            {
                _currentNode.removalType = (uint)value;
                RaisePropertyChanged("RemovalType");
            }
        }

        /// <summary>
        /// 隔离情况     
        /// </summary>
        public ResultType InsulateType
        {
            get
            {
                return (ResultType)_currentNode.insulateType;
            }
            set
            {
                _currentNode.insulateType = (uint)value;
                RaisePropertyChanged("InsulateType");
            }
        }

        /// <summary>
        /// 故障状态     
        /// </summary>
        public FaultState FaultState
        {
            get
            {
                return (FaultState)_currentNode.faultState; ;
            }
            set
            {
                _currentNode.faultState = (uint)value;
                RaisePropertyChanged("FaultState");
            }
        }

        /// <summary>
        /// 边缘状态     
        /// </summary>
        public bool IsFaultEdgeConnected
        {
            get
            {
                return _currentNode.isFaultEdgeConnected;
            }
            set
            {
                _currentNode.isFaultEdgeConnected = value;
                RaisePropertyChanged("IsFaultEdgeConnected");
            }
        }

        string IndexAreaToString(List<uint> collect)
        {
            var sb = new StringBuilder(20);
            foreach (var m in collect)
            {
                sb.AppendFormat("{0},", m);
            }
            return sb.ToString();

        }
        /// <summary>
        /// 区域索引     
        /// </summary>
        public string IndexArea
        {
            get
            {
                return IndexAreaToString(_currentNode.indexArea);
            }
            set
            {
                RaisePropertyChanged("IndexArea");
            }

        }

        /// <summary>
        /// 区域是否存在     
        /// </summary>
        public string IsExitArea
        {
            get
            {
                return IsBoolToString(_currentNode.isExitArea); 
            }
            set
            {
                RaisePropertyChanged("IsExitArea");
            }
        }
        string IsBoolToString(List<bool> collect)
        {
            var sb = new StringBuilder(20);
            foreach (var m in collect)
            {
                sb.AppendFormat("{0},", m);
            }
            return sb.ToString();

        }
        /// <summary>
        /// 是否收集完整     
        /// </summary>
        public string IsGather
        {
            get
            {
                return IsBoolToString(_currentNode.isGather);
            }
            set
            {
                RaisePropertyChanged("IsGather");
            }
        }  
        #endregion

        #region connect_switch 联络开关相关
        /// <summary>
        /// 转供电交互
        /// </summary>
        public TransferCode TransferCode
        {
            get
            {
                return (TransferCode)_message.connect.transferCode;
            }
            set
            {
                _message.connect.transferCode = (uint)value;
                RaisePropertyChanged("TransferCode");
            }
        }

        public string Path1
        {
            get
            {
                return HexCollectToString(_message.connect.path1);
                //return _message.connect.path1;
            }
            set
            {
                RaisePropertyChanged("Path1");
            }
        }

        public string Path2
        {
            get
            {
                return HexCollectToString(_message.connect.path2);
                //return _message.connect.path2;
            }
            set
            {
                RaisePropertyChanged("Path2");
            }
        }

        public uint Count
        {
            get
            {
                return _message.connect.count;
            }
            set
            {
                _message.connect.count = (uint)value;
                RaisePropertyChanged("Count");
            }
        }

        public bool IsConnect
        {
            get
            {
                return _message.connect.isConnect;
            }
            set
            {
                _message.connect.isConnect = value;
                RaisePropertyChanged("IsConnect");
            }
        }
        #endregion


        #region distribution_station
        public uint AreaCount
        {
            get
            {
                if (_message.distribution != null)
                {
                    return _message.distribution.areaCount;
                }
                else
                {
                    return 0;
                }
                
            }
            set
            {
                _message.distribution.areaCount = (uint)value;
                RaisePropertyChanged("AreaCount");
            }
        }

        public bool IsComplted
        {
            get
            {
                if (_message.distribution != null)
                {
                    return _message.distribution.isComplted;
                }
                else
                {
                    return false;
                }
                
            }
            set
            {
                _message.distribution.isComplted = value;
                RaisePropertyChanged("IsComplted");
            }
        }

        public bool IsGatherCompleted
        {
            get
            {
                if (_message.distribution != null)
                {
                    return _message.distribution.isGatherCompleted;
                }
                else
                {
                    return false;
                }
                
            }
            set
            {
                _message.distribution.isGatherCompleted = value;
                RaisePropertyChanged("IsGatherCompleted");
            }
        }

        public bool IsGatherCalculateCompleted
        {
            get
            {
                if (_message.distribution != null)
                {
                    return _message.distribution.isGatherCalculateCompleted;
                }
                else
                {
                    return false;
                }
                
            }
            set
            {
                _message.distribution.isGatherCalculateCompleted = value;
                RaisePropertyChanged("IsGatherCalculateCompleted");
            }
        }

        public uint SwitchRef
        {

            get
            {
                if( _message.distribution != null)
                {
                    return _message.distribution.switchRef;
                }
                else
                {
                    return 0;
                }
                    
            }
            set
            {
                _message.distribution.switchRef = (uint)value;
                RaisePropertyChanged("SwitchRef");
            }
        }

        public bool IsAlreayExitedFault
        {
            get
            {
                if (_message.distribution != null)
                {
                    return _message.distribution.isAlreayExitedFault;
                }
                else
                {
                    return false;
                }
                
            }
            set
            {
                _message.distribution.isAlreayExitedFault = value;
                RaisePropertyChanged("IsAlreayExitedFault");
            }
        }

        public bool IsExitedInsulateFailure
        {
            get
            {
                if (_message.distribution != null)
                {
                    return _message.distribution.isExitedInsulateFailure;
                }
                else
                {
                    return false;
                }
                
            }
            set
            {
                _message.distribution.isExitedInsulateFailure = value;
                RaisePropertyChanged("IsExitedInsulateFailure");
            }
        }
        #endregion

        #region distribution_power_area
        string PowerAreaToString(List<distribution_power_area> powerArea)
        {
            var sb = new StringBuilder(30);
            foreach(var  m in powerArea)
            {
                sb.Append("[ Switch:");
                sb.Append(HexCollectToString(m.areaSwitch));
                sb.AppendFormat("--{0:X},{1:X},{2},{3},{4},{5},{6} ", m.upadetedFlag, m.upadetedFull, m.isUpadeted,
                    m.isFaultArea, m.isExitFaultMessage, m.removalType, m.insulateType);
                sb.Append("],");
            }
            return sb.ToString();
        }
        public string PowerAreaMessage
        {
            get
            {
                return PowerAreaToString(_message.power_area);
            }
            set
            {
                RaisePropertyChanged("PowerAreaMessage");
            }
        }


        //public List<uint> AreaSwitch
        //{
        //    get
        //    {
        //        return dis_power_area.areaSwitch;
        //    }
        //}

        //public uint UpdatedFlag
        //{
        //    get
        //    {
        //        return dis_power_area.upadetedFlag;
        //    }
        //    set
        //    {
        //        dis_power_area.upadetedFlag = (uint)value;
        //        RaisePropertyChanged("UpdatedFlag");
        //    }
        //}

        //public bool IsUpadeted
        //{
        //    get
        //    {
        //        return dis_power_area.isUpadeted;
        //    }
        //    set
        //    {
        //        dis_power_area.isUpadeted = value;
        //        RaisePropertyChanged("IsUpadeted");
        //    }
        //}

        //public bool IsFaultArea
        //{
        //    get
        //    {
        //        return dis_power_area.isFaultArea;
        //    }
        //    set
        //    {
        //        dis_power_area.isFaultArea = value;
        //        RaisePropertyChanged("IsFaultArea");
        //    }
        //}

        //public bool IsExitFaultMessage
        //{
        //    get
        //    {
        //        return dis_power_area.isExitFaultMessage;
        //    }
        //    set
        //    {
        //        dis_power_area.isExitFaultMessage = value;
        //        RaisePropertyChanged("IsExitFaultMessage");
        //    }
        //}

        //public uint UpadetedFull
        //{
        //    get
        //    {
        //        return dis_power_area.upadetedFull;
        //    }
        //    set
        //    {
        //        dis_power_area.upadetedFull = (uint)value;
        //        RaisePropertyChanged("UpadetedFull");
        //    }
        //}

        //public uint Dis_RemovalType
        //{
        //    get
        //    {
        //        return dis_power_area.removalType;
        //    }
        //    set
        //    {
        //        dis_power_area.removalType = (uint)value;
        //        RaisePropertyChanged("Dis_RemovalType");
        //    }
        //}

        //public uint Dis_InsulateType
        //{
        //    get
        //    {
        //        return dis_power_area.insulateType;
        //    }
        //    set
        //    {
        //        dis_power_area.insulateType = (uint)value;
        //        RaisePropertyChanged("Dis_InsulateType");
        //    }
        //}

        #endregion

        #region  faultdeal_handle
        public uint SwitchPropertyID
        {
            get
            {
                return _message.fault_handle.switchPropertyID;
            }
            set
            {
                _message.fault_handle.switchPropertyID = (uint)value;
                RaisePropertyChanged("SwitchPropertyID");
            }
        }

        public uint LastState
        {
            get
            {
                return _message.fault_handle.lastState;
            }
            set
            {
                _message.fault_handle.lastState = (uint)value;
                RaisePropertyChanged("LastState");
            }
        }

        public uint NextState
        {
            get
            {
                return _message.fault_handle.nextState;
            }
            set
            {
                _message.fault_handle.nextState = (uint)value;
                RaisePropertyChanged("NextState");
            }
        }

        public bool IsRun
        {
            get
            {
                return _message.fault_handle.isRun;
            }
            set
            {
                _message.fault_handle.isRun = value;
                RaisePropertyChanged("IsRun");
            }
        }

        public uint Step
        {
            get
            {
                return _message.fault_handle.step;
            }
            set
            {
                _message.fault_handle.step = (uint)value;
                RaisePropertyChanged("Step");
            }
        }

        public uint StartTime
        {
            get
            {
                return _message.fault_handle.startTime;
            }
            set
            {
                _message.fault_handle.startTime = (uint)value;
                RaisePropertyChanged("StartTime");
            }
        }

        public uint LimitTime
        {
            get
            {
                return _message.fault_handle.limitTime;
            }
            set
            {
                _message.fault_handle.limitTime = (uint)value;
                RaisePropertyChanged("LimitTime");
            }
        }

        public uint T1
        {
            get
            {
                return _message.fault_handle.t1;
            }
            set
            {
                _message.fault_handle.t1 = (uint)value;
                RaisePropertyChanged("T1");
            }
        }

        public uint T2
        {
            get
            {
                return _message.fault_handle.t2;
            }
            set
            {
                _message.fault_handle.t2 = (uint)value;
                RaisePropertyChanged("T2");
            }
        }

        public uint T3
        {
            get
            {
                return _message.fault_handle.t3;
            }
            set
            {
                _message.fault_handle.t3 = (uint)value;
                RaisePropertyChanged("T3");
            }
        }

        public uint T4
        {
            get
            {
                return _message.fault_handle.t4;
            }
            set
            {
                _message.fault_handle.t4 = (uint)value;
                RaisePropertyChanged("T4");
            }
        }

        public uint T5
        {
            get
            {
                return _message.fault_handle.t5;
            }
            set
            {
                _message.fault_handle.t5 = (uint)value;
                RaisePropertyChanged("T5");
            }
        }

        public uint CheckOpenTime
        {
            get
            {
                return _message.fault_handle.checkOpenTime;
            }
            set
            {
                _message.fault_handle.checkOpenTime = (uint)value;
                RaisePropertyChanged("CheckOpenTime");
            }
        }

        public uint CheckBackupTime
        {
            get
            {
                return _message.fault_handle.checkBackupTime;
            }
            set
            {
                _message.fault_handle.checkBackupTime = (uint)value;
                RaisePropertyChanged("CheckBackupTime");
            }
        }

        public uint RejectTime
        {
            get
            {
                return _message.fault_handle.rejectTime;
            }
            set
            {
                _message.fault_handle.rejectTime = (uint)value;
                RaisePropertyChanged("RejectTime");
            }
        }

        public uint RreciveRemovalSuccessTime
        {
            get
            {
                return _message.fault_handle.reciveRemovalSuccessTime;
            }
            set
            {
                _message.fault_handle.reciveRemovalSuccessTime = (uint)value;
                RaisePropertyChanged("ReciveRemovalSuccessTime");
            }
        }

        public uint ReciveConnectTime
        {
            get
            {
                return _message.fault_handle.reciveConnectTime;
            }
            set
            {
                _message.fault_handle.reciveConnectTime = (uint)value;
                RaisePropertyChanged("ReciveConnectTime");
            }
        }

        public bool IsCheckPass
        {
            get
            {
                return _message.fault_handle.isCheckPass;
            }
            set
            {
                _message.fault_handle.isCheckPass = value;
                RaisePropertyChanged("IsCheckPass");
            }
        }
        #endregion

        #region connect_path


        string ConnectPathToString(List<connect_path> path)
        {
            var sb = new StringBuilder(20);
            foreach (var m in path)
            {
                sb.AppendFormat("[{0:X},{1},{2}],", m.id, m.remainderCapacity, m.isUpdated);
            }
            return sb.ToString();

        }

        public string ConnectPathMessage
        {
            get
            {
                return ConnectPathToString(_message.con_path);
            }
            set
            {
                RaisePropertyChanged("ConnectPathMessage");
            }
        }
        private DateTime _upadeTime;
        public string UpadeTime
        {
            get
            {
                return _upadeTime.ToString("hh:mm:ss");
            }
            set
            {
                RaisePropertyChanged("UpadeTime");
            }
        }

        //public uint Con_ID
        //{
        //    get
        //    {
        //        return con_path.id;
        //    }
        //    set
        //    {
        //        con_path.id = (uint)value;
        //        RaisePropertyChanged("Con_ID");
        //    }
        //}

        //public uint RemainderCapacity
        //{
        //    get
        //    {
        //        return con_path.remainderCapacity;
        //    }
        //    set
        //    {
        //        con_path.remainderCapacity = (uint)value;
        //        RaisePropertyChanged("RemainderCapacity");
        //    }
        //}

        //public bool IsUpdated
        //{
        //    get
        //    {
        //        return con_path.isUpdated;
        //    }
        //    set
        //    {
        //        con_path.isUpdated = value;
        //        RaisePropertyChanged("IsUpdated");
        //    }
        //}
        #endregion


        public void UpdateAll(StationMessage message)
        {
            _message = message;
            _currentNode = FindCurrNode(message, IdOwn);
            _upadeTime = DateTime.Now;
            RaisePropertyChanged("Num");
            RaisePropertyChanged("ID");
            RaisePropertyChanged("Type");
            RaisePropertyChanged("State");
            RaisePropertyChanged("NeighbourCount");
            RaisePropertyChanged("NeighbourCollect");
            RaisePropertyChanged("Operate");
            RaisePropertyChanged("OverTime");
            RaisePropertyChanged("RemovalType");
            RaisePropertyChanged("InsulateType");
            RaisePropertyChanged("FaultState");
            RaisePropertyChanged("IsFaultEdgeConnected");
            RaisePropertyChanged("IndexArea");
            RaisePropertyChanged("IsExitArea");
            RaisePropertyChanged("IsGather");

            RaisePropertyChanged("TransferCode");
            RaisePropertyChanged("Path1");
            RaisePropertyChanged("Path2");
            RaisePropertyChanged("Count");
            RaisePropertyChanged("IsConnect");
            RaisePropertyChanged("AreaCount");
            RaisePropertyChanged("IsComplted");
            RaisePropertyChanged("IsGatherCompleted");
            RaisePropertyChanged("IsGatherCalculateCompleted");
            RaisePropertyChanged("SwitchRef");
            RaisePropertyChanged("IsAlreayExitedFault");
            RaisePropertyChanged("IsExitedInsulateFailure");

            RaisePropertyChanged("PowerAreaMessage");

            RaisePropertyChanged("SwitchPropertyID");
            RaisePropertyChanged("LastState");
            RaisePropertyChanged("NextState");
            RaisePropertyChanged("IsRun");
            RaisePropertyChanged("Step");
            RaisePropertyChanged("StartTime");
            RaisePropertyChanged("LimitTime");
            RaisePropertyChanged("T1");
            RaisePropertyChanged("T2");
            RaisePropertyChanged("T3");
            RaisePropertyChanged("T4");
            RaisePropertyChanged("T5");
            RaisePropertyChanged("CheckOpenTime");
            RaisePropertyChanged("CheckBackupTime");
            RaisePropertyChanged("RejectTime");
            RaisePropertyChanged("ReciveRemovalSuccessTime");
            RaisePropertyChanged("ReciveConnectTime");
            RaisePropertyChanged("IsCheckPass");

            RaisePropertyChanged("ConnectPathMessage");
            RaisePropertyChanged("UpadeTime");
        }

        /// <summary>
        /// Initializes a new instance of the StationInformation class.
        /// </summary>
        public StationInformation(StationMessage message)
        {
            _message = message;
            _currentNode = FindCurrNode(message, IdOwn);
            _upadeTime = DateTime.Now;
        }    
        
        private node_property FindCurrNode(StationMessage message,uint id_own)
        {
            node_property node = null;
            //foreach (var m in message.node)
            //{
            //    if (message.id_own == m.id)
            //    {
            //        node = m;
            //    }
            //}
            for(int i = 0; i < message.node.Count; i++)
            {
                if (message.id_own == message.node[i].id)
                {
                    node = message.node[i];
                }
            }
            return node;
        }
    }
}