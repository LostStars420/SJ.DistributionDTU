using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GooseGenerator
{
    /// <summary>
    /// 目的地址类
    /// </summary>
    public class DstAddr
    {
        public string Addr  //原来是byte[],现在和MAC地址一样，所以变成string 方便
        {
            get;
            set;
        }

        public int Priority
        {
            get;
            set;
        }

        public int VID
        {
            get;
            set;
        }

        public int Appid
        {
            get;
            set;
        }

        public int MinTime
        {
            get;
            set;
        }

        public int MaxTime
        {
            get;
            set;
        }

        public DstAddr(string addr, int priority, int vid, int appId, int minTime, int maxTime, int appid)
        {
            this.Addr = addr;
            this.Priority = priority;
            this.VID = vid;
            this.Appid = appid; //1000 + addr[5]
            this.MinTime = minTime;
            this.MaxTime = maxTime;
        }
    }
}
