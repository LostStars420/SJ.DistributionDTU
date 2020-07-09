using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GooseGenerator
{
    /// <summary>
    /// 输入的GoCB类
    /// </summary>
    public class GoCBRx
    {
        public String GoCBRef
        {
            get;
            set;
        }

        public int Appid
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

        public string Addr   //原来是byte[],现在和MAC-ADDRESS一致，就写成string
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goCBRef"></param>
        /// <param name="appID"></param>
        /// <param name="appid"></param>
        /// <param name="datSet"></param>
        /// <param name="confRev"></param>
        /// <param name="addr"></param>
        public GoCBRx(String goCBRef, String appID, int appid, String datSet, int confRev, string addr)
        {
            this.GoCBRef = goCBRef;
            this.AppID = appID;
            this.Appid = appid;
            this.DatSet = datSet;
            this.ConfRev = confRev;
            this.Addr = addr;
        }
    }
}
