using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Element
{
    /// <summary>
    /// 网络异常消息
    /// </summary>
    public  class NetExcptionEventArgs : EventArgs
    {
        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception OriginException;

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment;

        /// <summary>
        /// 网络异常信息
        /// </summary>
        /// <param name="except">异常</param>
        /// <param name="comment">注释</param>
        public NetExcptionEventArgs(Exception except, string comment)
        {
            OriginException = except;
            Comment = comment;
        }
    }
}
