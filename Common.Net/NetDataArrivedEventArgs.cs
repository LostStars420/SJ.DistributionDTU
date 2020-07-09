using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net
{
    public class NetDataArrivedEventArgs : EventArgs
    {
        public NetDataArrivedEventArgs(string data)
        {
            DataMsg = data;
        }

        public string DataMsg;
    }
}
