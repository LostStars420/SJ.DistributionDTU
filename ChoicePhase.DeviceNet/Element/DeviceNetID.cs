using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.DeviceNet.Element
{
    /// <summary>
    /// 11bit CAN ID标识符
    /// </summary>
    public class DeviceNetID
    {
        /// <summary>
        /// ID 标识
        /// </summary>
        UInt32 ID
        {
            get;
            set;
        }

        /// <summary>
        /// 获取组号
        /// </summary>
        /// <returns>数字代表组号, 0-为实现</returns>
        public int GetGroupNumber()
        {
            if (((ID) & 0x07C0) <= 0x3C0)
            {
                return 1;
            }
             else if (((ID) & 0x0600) == 0x0400)
            {
                return 2;
            }
            else
            {
                return 0;
            }

        }
        /// <summary>
        /// 获取MAC
        /// </summary>
        /// <returns>MAC地址，小于0为错误</returns>
        public int GetMAC()
        {
           var num =  GetGroupNumber();
           switch (num)
           {
               case 1:
                   {
                       return (int)((ID) & 0x3F);                       
                   }
               case 2:
                   {
                       return (int)((((ID) >> 3)) & 0x003F);
                   }
               default :
                   {
                       return -1;
                   }
           }

        }
        /// <summary>
        /// 获取功能码
        /// </summary>
        /// <returns></returns>
        public UInt16 GetFunctionCode()
        {
            var num = GetGroupNumber();
            switch (num)
            {
                case 1:
                    {
                        return (UInt16)((((ID) >> 6)) & 0x003F);
                    }
                case 2:
                    {
                        return (UInt16)((ID) & 0x0007);
                    }
                default:
                    {
                        throw new Exception("没有功能码");
                    }
            }
        }

        /// <summary>
        /// 生产Group1 ID识别码
        /// </summary>
        /// <param name="function">功能</param>
        /// <param name="mac">MAC地址</param>
        /// <returns>ID号</returns>
       static  public UInt16 MakeGroupIDOne(byte function, byte mac)
        {
            return (UInt16)(((function & 0x1F) >> 6) | (mac & 0x3F));
        }
        /// <summary>
        /// 生产Group2 ID识别码
        /// </summary>
        /// <param name="function">功能</param>
        /// <param name="mac">MAC地址</param>
        /// <returns>ID号</returns>
        static public UInt16 MakeGroupIDTwo(byte function, byte mac)
        {
            return (UInt16)((0x0400) | ((UInt16)(mac & 0x3F) << 3) | (function & 0x07));

        }

        /// <summary>
        /// 使用ID初始化DeviceNet标识符
        /// </summary>
        /// <param name="id">CAN ID</param>
        public DeviceNetID(ushort id)
        {
            ID = id;
        }

       
 
    }
}
