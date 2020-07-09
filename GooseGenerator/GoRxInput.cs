using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GooseGenerator
{
    public class GoRxInput
    {
        public int GoCbIndex
        {
            get;
            set;
        }
        public int GoCbEntryIndex
        {
            get;
            set;
        }
        public string Ref
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public string OutVarName
        {
            get;
            set;
        }

        public GoRxInput(int goCbIndex, int goCbEntryIndex,
            string refinput, string type, string outVarName)
        {
            GoCbIndex = goCbIndex;
            GoCbEntryIndex = goCbEntryIndex;
            Ref = refinput;
            Type = type;
            OutVarName = outVarName;
        }
    }
}
