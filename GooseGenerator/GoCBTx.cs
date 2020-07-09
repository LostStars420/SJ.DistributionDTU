using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GooseGenerator
{
    public class GoCBTx
    {
        public String GoCBRef
        {
            get;
            set;
        }

        public String AppID
        {
            get;
            set;
        }

        public String DatSet
        {
            get;
            set;
        }

        public int ConfRev
        {
            get;
            set;
        }

        public GoCBTx(String GoCBRef, String AppID, String DatSet, int ConfRev)
        {
            this.GoCBRef = GoCBRef;
            this.AppID = AppID;
            this.DatSet = DatSet;
            this.ConfRev = ConfRev;
        }
    }
}
