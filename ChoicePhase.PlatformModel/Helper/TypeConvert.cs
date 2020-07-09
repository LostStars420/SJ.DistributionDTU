using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.PlatformModel.Helper
{
    public class TypeConvert
    {
        /// <summary>
        /// 分裂字符串到索引
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="indexArray">ip字节数组</param>
        /// <returns></returns>
        public static bool SplitStrToIndex(string str, out byte[] indexArray)
        {
            char[] separator = new char[] { ',' };
            string[] items = str.Split(separator);
            indexArray = new byte[items.Length];
            if (items.Length == 0)
            {
                return false;
            }
            byte[] index = new byte[items.Length];
            
            int i = 0;
            foreach (var m in items)
            {
                if (byte.TryParse(m, out index[i]) && (index[i] <= 255))
                {
                    i++;
                    continue;
                }
                else
                {
                    return false;
                }
            }
            indexArray = index;
            return true;
        }


        /// <summary>
        /// ip字符串尝试转换成字节数组
        /// </summary>
        /// <param name="ip">ip字符串</param>
        /// <param name="ipArray">ip字节数组</param>
        /// <returns></returns>
        public static bool IpTryToInt(string ip, out byte[] ipArray)
        {
            char[] separator = new char[] { '.' };
            string[]  items = ip.Split(separator);
            ipArray = new byte[4];
            if (items.Length != 4)
            {
                return false;
            }
            int i = 0;
            foreach(var m in items)
            {
                if (byte.TryParse(m, out ipArray[i]) && (ipArray[i] <= 255))
                {
                    i++;
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static long IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            string[] items = ip.Split(separator);
            return long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
        }

        public static string IntToIp(long ipInt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((ipInt >> 24) & 0xFF).Append(".");
            sb.Append((ipInt >> 16) & 0xFF).Append(".");
            sb.Append((ipInt >> 8) & 0xFF).Append(".");
            sb.Append(ipInt & 0xFF);
            return sb.ToString();
        }


        public static UInt32 Combine(byte[] data ,int offset)
        {
            byte byte1 = data[offset];
            byte byte2 = data[offset+1];
            byte byte3 = data[offset+2];
            byte byte4 = data[offset+3];
            UInt32 datacombin = ((UInt32)((((UInt32)(byte4 & 0xFF)) << 24) | (((UInt32)(byte3 & 0xFF)) << 16) | (((UInt32)(byte2 & 0xFF)) << 8) | ((UInt32)(byte1))));
            return datacombin;
        }
    }
}
