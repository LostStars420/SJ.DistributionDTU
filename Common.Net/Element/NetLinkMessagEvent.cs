using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Element
{
    /// <summary>
    /// 网络连接信息事件
    /// </summary>
    public  class NetLinkMessagEvent : EventArgs
    {
        /// <summary>
        /// 信息
        /// </summary>
        public string Message;

        /// <summary>
        /// 网络状态
        /// </summary>
        public NetState State;
        /// <summary>
        /// 网络异常信息
        /// </summary>
        public NetLinkMessagEvent(string messag, NetState state)
        {
            Message = messag;
            State = state;
        }
    }

    /// <summary>
    /// 网络状态
    /// </summary>
    public enum NetState
    {
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 1,

        /// <summary>
        /// 启动连接
        /// </summary>
        StartLink = 2,

        /// <summary>
        /// 建立连接
        /// </summary>
        EstablishLink = 3,
    }
}
