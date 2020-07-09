using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.DeviceNet.LogicApplyer
{
    /// <summary>
    /// 命令ID
    /// </summary>
    public enum CommandIdentify : byte
    {
        /// <summary>
        /// 合闸预制
        /// </summary>
        ReadyClose = 1,
        /// <summary>
        /// 合闸执行
        /// </summary>
        CloseAction = 2,
        /// <summary>
        /// 分闸预制
        /// </summary>
        ReadyOpen = 3,
        /// <summary>
        /// 分闸执行
        /// </summary>
        OpenAction = 4,

        /// <summary>
        /// 同步合闸预制
        /// </summary>
        SyncReadyClose = 5,
        /// <summary>
        /// 同步分闸预制
        /// </summary>
        //SyncReadyClose = 6,

        



        /// <summary>
        /// 主站参数设置,顺序
        /// </summary>
      //  MasterParameterSet = 0x10,
        // <summary>
        /// 主站参数设置，非顺序——按点设置
        /// </summary>
        MasterParameterSetOne = 0x11,
        /// <summary>
        /// 主站参数读取，顺序
        /// </summary>
        MasterParameterRead = 0x12,
        /// <summary>
        /// 主站参数读取，非顺序——按点读取
        /// </summary>
       // MasterParameterReadOne = 0x13,

        /// <summary>
        /// 错误
        /// </summary>
        ErrorACK = 0x14,

        /// <summary>
        /// 配置模式
        /// </summary>
        ConfigMode = 0x15,
        /// <summary>
        /// 子站状态改变信息上传
        /// </summary>
        SubstationStatuesChange = 0x1A,

        /// <summary>
        /// 多帧数据
        /// </summary>
        MutltiFrame = 0x1B,

        /// <summary>
        /// 同步控制器 合闸预制
        /// </summary>
        SyncOrchestratorReadyClose = 0x30,
        /// <summary>
        /// 同步控制器 合闸执行
        /// </summary>
        SyncOrchestratorCloseAction = 0x31,
       // SyncOrchestratorReadyOpen = 0x32,
        //SyncOrchestratorOpenAction = 0x32,

        SynTimeSequence = 0x34, //时序同步脉冲信号
        /// <summary>
        /// 同步信号检测命令
        /// </summary>
        //SynSingalSelfTesting = 0x40,
        
    }
}
