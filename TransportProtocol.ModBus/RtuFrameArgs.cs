using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.Modbus
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
}
