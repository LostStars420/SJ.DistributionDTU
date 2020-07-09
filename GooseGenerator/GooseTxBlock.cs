using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GooseGenerator
{
    public class GooseTxBlock
    {
        public GoCBTx GoCB
        {
            get;
            set;
        }

        public DstAddr DestAdress
        {
            get;
            set;
        }

        public List<FCDA> FCDA
        {
            get;
            set;
        }

        /// <summary>
        /// 将GOCB的内容写入字符串
        /// </summary>
        /// <returns>返回该字符串</returns>
        public string GoCBToString()
        {
            StringBuilder sb = new StringBuilder(200);
            if (GoCB.GoCBRef.Contains("PIGO"))
            {
                sb.AppendFormat("[GOOSE Tx]\n");
                sb.AppendFormat("numGoCb           = {0}\n", 1);
                sb.AppendFormat("[GoCB{0}]\n", 1);
            }
            else
            {
                sb.AppendFormat("[GoCB{0}]\n", 2);
            }

            sb.AppendFormat("GoCBRef           = {0}\n", GoCB.GoCBRef);
            sb.AppendFormat("AppID             = {0}\n", GoCB.AppID);
            sb.AppendFormat("DatSet            = {0}\n", GoCB.DatSet);
            sb.AppendFormat("ConfRev           = {0}\n", GoCB.ConfRev);
            sb.AppendFormat("numDatSetEntries  = {0}\n", FCDA.Count);
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// 将目的地址写进字符串
        /// </summary>
        /// <returns></returns>
        public string DestAdressToString()
        {
            StringBuilder sb = new StringBuilder(200);
            sb.AppendFormat("[DstAddr]\n");
            sb.AppendFormat("Addr              = {0}\n", DestAdress.Addr);
            //sb.AppendFormat("Addr              = {0:X}-{1:X}-{2:X}-{3:X}-{4:X}-{5}\n",
            //    DestAdress.Addr[0].ToString("X2"), DestAdress.Addr[1].ToString("X2"), DestAdress.Addr[2].ToString("X2"),
            //    DestAdress.Addr[3].ToString("X2"), DestAdress.Addr[4].ToString("X2"), DestAdress.Addr[5].ToString());
            sb.AppendFormat("Priority          = {0}\n", DestAdress.Priority);
            sb.AppendFormat("VID               = {0}\n", DestAdress.VID);
            sb.AppendFormat("Appid             = {0}\n", DestAdress.Appid);
            //sb.AppendFormat("Appid             = {0:X}\n", DestAdress.Appid);
            sb.AppendFormat("MinTime           = {0}\n", DestAdress.MinTime);
            sb.AppendFormat("MaxTime           = {0}\n", DestAdress.MaxTime);
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// 将FCDA写进字符串
        /// </summary>
        /// <returns></returns>
        public string FCDAToString()
        {
            StringBuilder sbAll = new StringBuilder(10000);
            for (int i = 0; i < FCDA.Count; i++)
            {
                StringBuilder sb = new StringBuilder(200);
                if (FCDA[i].Ref == null)
                {
                    continue;
                }
                sb.AppendFormat("[FCDA{0}]\n", i + 1);
                sb.AppendFormat("Ref               = {0}\n", FCDA[i].Ref);
                sb.AppendFormat("Type              = {0}\n", FCDA[i].Type);
                sb.AppendFormat("InVarName         = {0}\n", FCDA[i].InVarName);
                sb.AppendFormat("ACT               = {0}\n", FCDA[i].ACT);
                sb.AppendLine();
                sbAll.Append(sb);
            }
            return sbAll.ToString();
        }

        /// <summary>
        /// 将上述三个写进stringBuilder，并转化为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(20000);
            sb.Append(GoCBToString());
            sb.Append(DestAdressToString());
            sb.Append(FCDAToString());
            return sb.ToString();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="goCB"></param>
        /// <param name="id"></param>
        /// <param name="num"></param>
        /// <param name="reffcda"></param>
        /// <param name="inName"></param>
        public GooseTxBlock(GoCBTx goCB, string addr, int num, string[] reffcda, string[] inName,
            int appid, int maxTime)
        {
            GoCB = goCB;
            DestAdress = new DstAddr(addr, 4, 1, 4, 2, maxTime, appid);

            FCDA = new List<FCDA>();
            for (int i = 0; i < num; i++)
            {
                var fcda = new FCDA(reffcda[i], "Bool", inName[i], 0);
                FCDA.Add(fcda);
            }
        }
    }
}
