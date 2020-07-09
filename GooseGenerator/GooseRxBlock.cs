using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GooseGenerator
{
    public class GooseRxBlock
    {
        /// <summary>
        /// 列表长度，即FCDA的长度
        /// </summary>
        const int elementNum = 24;

        GoCBRx[] gocbrx;

        public string GoCBToString()
        {
            StringBuilder sb = new StringBuilder(200);
            sb.AppendFormat("[GOOSE Rx]\n");
            sb.AppendFormat("numGoCb               = {0}\n", gocbrx.Length);
            sb.AppendFormat("numInput              = {0}\n", GoRxInput.Count);
            for (int i = 0; i < gocbrx.Length; i++)
            {
                sb.AppendFormat("[GoCB{0}]\n", i + 1);//i + 1
                sb.AppendFormat("Addr                = {0}\n", gocbrx[i].Addr);
                //sb.AppendFormat("Addr                = {0:X}-{1:X}-{2:X}-{3:X}-{4:X}-{5}\n",
                //    gocbrx[i].Addr[0].ToString("X2"), gocbrx[i].Addr[1].ToString("X2"), gocbrx[i].Addr[2].ToString("X2"),
                //    gocbrx[i].Addr[3].ToString("X2"), gocbrx[i].Addr[4].ToString("X2"), gocbrx[i].Addr[5].ToString());
                sb.AppendFormat("Appid               = {0}\n", gocbrx[i].Appid);
                //sb.AppendFormat("Appid               = {0:X}\n", gocbrx[i].Appid);
                sb.AppendFormat("GoCBRef             = {0}\n", gocbrx[i].GoCBRef);
                sb.AppendFormat("AppID               = {0}\n", gocbrx[i].AppID);
                sb.AppendFormat("DatSet              = {0}\n", gocbrx[i].DatSet);
                sb.AppendFormat("ConfRev             = {0}\n", gocbrx[i].ConfRev);
                sb.AppendFormat("numDatSetEntries    = {0}\n", GoRxInput.Count / gocbrx.Length);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public List<GoRxInput> GoRxInput
        {
            get;
            set;
        }

        public string INPUTToString()
        {
            StringBuilder sbAll = new StringBuilder(10000);
            for (int i = 0; i < GoRxInput.Count; i++)
            {
                StringBuilder sb = new StringBuilder(200);
                sb.AppendFormat("[INPUT{0}]\n", i + 1);
                sb.AppendFormat("GoCbIndex           = {0}\n", GoRxInput[i].GoCbIndex);
                sb.AppendFormat("GoCbEntryIndex      = {0}\n", GoRxInput[i].GoCbEntryIndex);
                sb.AppendFormat("Ref                 = {0}\n", GoRxInput[i].Ref);
                sb.AppendFormat("Type                = {0}\n", GoRxInput[i].Type);
                sb.AppendFormat("OutVarName          = {0}\n", GoRxInput[i].OutVarName);

                sb.AppendLine();
                sbAll.Append(sb);
            }


            return sbAll.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(20000);
            sb.Append(GoCBToString());
            sb.Append(INPUTToString());
            return sb.ToString();
        }

        public GooseRxBlock(GoCBRx[] goCB, List<GoRxInput> goRxInput)
        {
            gocbrx = goCB;
            GoRxInput = goRxInput;
        }
    }
}
