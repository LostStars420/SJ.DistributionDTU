using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.DeviceNet.Element
{
    /// <summary>
    /// 网络建立步骤
    /// </summary>
    public enum NetStep
    {
        /// <summary>
        /// 启动
        /// </summary>
        Start = 0xA1,
        /// <summary>
        /// 正在建立连接
        /// </summary>
        Linking = 0xA2,
        /// <summary>
        /// 状态改变连接
        /// </summary>
        StatusChange = 0xA4,
        /// <summary>
        /// 处于循环模式，轮询状态
        /// </summary>
        Cycle = 0xA8
    }


    /// <summary>
    /// 站点信息定义
    /// </summary>
    public class DefStationInformation
    {
        /// <summary>
        /// 站点MAC
        /// </summary>
        public byte MacID
        {
            get;
            set;
        }
        /// <summary>
        /// 站点配置状态--已经建立的连接
        /// </summary>
        public byte State
        {
            get;
            set;
        }
        /// <summary>
        /// 用于进行状态改变对比
        /// </summary>
        public byte OldState
        {
            get;
            set;
        }
        /// <summary>
        /// 在线标志 TRUE--在线， OFF--离线
        /// </summary>
        public bool Online
        {
            get;
            set;
        }
        /// <summary>
        /// 处理步骤
        /// </summary>
        public NetStep Step
        {
            get;
            set;
        }
        /// <summary>
        /// TRUE-完成 FALSE-失败完成标志
        /// </summary>
        public bool Complete
        {
            get;
            set;
        }

        /// <summary>
        /// 延时时间
        /// </summary>
        public uint DelayTime
        {
            get;
            set;
        }
        public uint StartTime; //启动时间
        public uint EndTime; //结束时间
        /// <summary>
        /// 超时次数
        /// </summary>
        public byte OverTimeCount
        {
            get;
            set;
        }

        public bool InitEnable
        {
            get;
            set;
        }
        
        /// <summary>
        /// //使能
        /// </summary>
        public bool Enable
        {
            get;
            set;
        }
        
        /// <summary>
        /// 注释
        /// </summary>
        public string Comment
        {
            get;
            set;
        }
        /// <summary>
        /// 等待标识
        /// </summary>
        public bool WaitFlag
        {
            get;
            set;
        }

        /// <summary>
        /// CAN发送报文
        /// </summary>
        public CanMessage SendMessage
        {
            get;
            set;
        }
        /// <summary>
        /// CAN接收报文
        /// </summary>
        public CanMessage ReciveMessage
        {
            get;
            set;
        }
        /// <summary>
        /// 站点信息定义
        /// </summary>
        /// <param name="mac">MAC地址</param>
        /// <param name="enable">使能</param>
        public DefStationInformation(byte mac, bool enable, string comment)
        {
            MacID = mac;
            State = 0;
            OldState = 0;
            Online = false;
            Complete = false;
            Step = NetStep.Start;
            Comment = comment;
            OverTimeCount = 0;
            DelayTime = 500;
            WaitFlag = false;
            Enable = enable;
            InitEnable = Enable;
        }
    }
}
