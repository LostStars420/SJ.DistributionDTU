using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportProtocol.Comself
{
    public class RtuFrameArgs : EventArgs
    {
        /// <summary>
        /// RTUFrame
        /// </summary>
        public RTUFrame Frame;

        public RtuFrameArgs(RTUFrame frame)
        {
            Frame = frame;
        }
    }

    /// <summary>
    /// FrameRtu参数
    /// </summary>
    public class FrameRtuArgs : EventArgs
    {
        /// <summary>
        /// Frame帧
        /// </summary>
        public FrameRtu Rtu
        {
            get;
            set;
        }

        public FrameRtuArgs(FrameRtu rtu)
        {
            Rtu = rtu;
        }
    }
}
