using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Element
{
    public class NetDataArrayEventArgs : EventArgs
    {
        public NetDataArrayEventArgs(byte[] dataArray)
        {
            DataArray = new byte[dataArray.Length];
            dataArray.CopyTo(DataArray, 0);
        }
        public NetDataArrayEventArgs(byte[] dataArray, int len)
        {
            DataArray = new byte[len];
            Array.Copy(dataArray, 0, DataArray, 0, len);
        }
        public byte[] DataArray;
    }
}
