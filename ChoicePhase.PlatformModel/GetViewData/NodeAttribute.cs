using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChoicePhase.DeviceNet.LogicApplyer;
using ChoicePhase.PlatformModel.DataItemSet;

namespace ChoicePhase.PlatformModel.GetViewData
{
    /// <summary>
    /// 节点属性集合
    /// </summary>
    public class NodeAttribute
    {

        /// <summary>
        /// MAC ID列表
        /// </summary>
        static private byte[]  _macID = new byte[] { 0x0D, 0x10, 0x12, 0x14,0x02,};
 

        /// <summary>
        /// 同步控制器
        /// </summary>
        public const byte IndexSynController = 0;

        /// <summary>
        /// A相
        /// </summary>
        public const byte IndexPhaseA = 1;

        /// <summary>
        /// A相
        /// </summary>
        public const byte IndexPhaseB = 2;

        /// <summary>
        /// C相
        /// </summary>
        public const byte IndexPhaseC = 3;


        /// <summary>
        /// 回路I
        /// </summary>
        public const byte LoopI = 0x01;

        /// <summary>
        /// 回路II
        /// </summary>
        public const byte LoopII = 0x02;

        /// <summary>
        /// 回路III
        /// </summary>
        public const byte LoopIII = 0x04; 

        /// <summary>
        /// 主站索引
        /// </summary>
        public const byte IndexMainStation = 4;

        /// <summary>
        /// MAC ID列表
        /// </summary>
        static public byte[] MacID
        {
            get
            {
                return _macID;
            }
        }

        /// <summary>
        /// 主站 MAC
        /// </summary>
        static public byte MacMainStation
        {
            get
            {
                return _macID[IndexMainStation];
            }
        }

        /// <summary>
        /// 同步控制器MAC
        /// </summary>
        static public byte MacSynController
        {
            get
            {
                return _macID[IndexSynController];
            }
        }

        /// <summary>
        /// A相 MAC
        /// </summary>
        static public byte MacPhaseA
        {
            get
            {
                return _macID[IndexPhaseA];
            }
        }

        /// <summary>
        /// B相 MAC
        /// </summary>
        static public byte MacPhaseB
        {
            get
            {
                return _macID[IndexPhaseB];
            }
        }

        /// <summary>
        /// C相 MAC
        /// </summary>
        static public byte MacPhaseC
        {
            get
            {
                return _macID[IndexPhaseC];
            }
        }

        /// <summary>
        /// 合闸时间
        /// </summary>
        private static int[] _closeTime = new int[] {0, 0, 0, 0};

        /// <summary>
        /// 合闸时间,索引1开始为A,B,C
        /// </summary>
        public static int[] CloseTime
        {
            get
            {
                return _closeTime;
            }
        }


        /// <summary>
        /// 补偿时间
        /// </summary>
        private static int[] _compensationTime = new int[] { 0, 0, 0, 0 };

        /// <summary>
        /// 补偿时间,索引1开始为A,B,C
        /// </summary>
        public static int[] CompensationTime
        {
            get
            {
                return _compensationTime;
            }
        }


        private static int _period = 20000;
        /// <summary>
        /// 周期
        /// </summary>
        public static int Period
        {
            get
            {
                return _period;
            }
            set
            {
                _period = value;
            }
        }

        private static byte _closePowerOnTime = 50;
        /// <summary>
        /// 合闸通电时间
        /// </summary>
        public static byte ClosePowerOnTime
        {
            get
            {
                return _closePowerOnTime;
            }
            set
            {
                _closePowerOnTime = value;
            }
        }

        private static byte _openPowerOnTime = 30;
        /// <summary>
        /// 分闸通电时间
        /// </summary>
        public static byte OpenPowerOnTime
        {
            get
            {
                return _openPowerOnTime;
            }
            set
            {
                _openPowerOnTime = value;
            }
        }

        /// <summary>
        /// 分闸时间
        /// </summary>
        private static byte[] _openTime = new byte[] {0, 40, 40, 40 };

        /// <summary>
        /// 合闸时间,索引1开始为A,B,C
        /// </summary>
        public static byte[] OpenTime
        {
            get
            {
                return _openTime;
            }
        }

        private static int _closeActionOverTime = 10000;

        /// <summary>
        /// 合闸操作超时时间ms
        /// </summary>
        public static int CloseActionOverTime
        {
            get
            {
                return _closeActionOverTime;
            }
            set
            {
                _closeActionOverTime = value;
            }
        }

        private static int _openActionOverTime = 10000;
        /// <summary>
        ///分闸操作超时时间ms
        /// </summary>
        public static int OpenActionOverTime
        {
            get
            {
                return _openActionOverTime;
            }
            set
            {
                _openActionOverTime = value;
            }
        }

        private static int _synCloseActionOverTime = 10000;

        /// <summary>
        /// 同步合闸操作超时时间
        /// </summary>
        public static int SynCloseActionOverTime
        {
            get
            {
                return _synCloseActionOverTime;
            }
            set
            {
                _synCloseActionOverTime = value;
            }
        }

        private static User _user = new User("Admin", "12345", "12345");
        /// <summary>
        /// 一级登陆密码
        /// </summary>
        public static User CurrentUser
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }


      
        /// <summary>
        /// 一级登陆密码
        /// </summary>
        public static string PasswordI
        {
            get
            {
                return _user.PasswordI;
            }
           
        }

                
        /// <summary>
        /// 二级执行密码
        /// </summary>
        public static  string  PasswordII
        {
            get
            {
                return _user.PasswordII;
            }
        }



        /// <summary>
        /// 获取超时时间
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static int GetOverTime(CommandIdentify cmd)
        {
            switch (cmd)
            {
                case CommandIdentify.ReadyClose:
                    {
                        return CloseActionOverTime;
                    }
                case CommandIdentify.ReadyOpen:
                    {
                        return OpenActionOverTime;
                    }
                case CommandIdentify.SyncReadyClose:
                    {
                        return SynCloseActionOverTime;
                    }
                default:
                    {
                        throw new Exception("为实现的超时选相");
                    }
            }

        }

        //0--普通三相模式， 11--单相模式，22--单开关三相模式
        private static int _workMode = 0;
        /// <summary>
        /// 工作模式
        /// </summary>
        public static int WorkMode
        {
            get
            {
                return _workMode;
            }
            set
            {
                _workMode = value;
            }
        }

        /// <summary>
        /// 普通三相模式
        /// </summary>
        public static bool NormalMode
        {
            get
            {
                return (WorkMode == 0);
            }
        }
        /// <summary>
        /// 单相模式
        /// </summary>
        public static bool SingleMode
        {
            get
            {
                return (WorkMode == 11);
            }
        }
        /// <summary>
        /// 单开关，三相模式
        /// </summary>
        public static bool SingleThreeMode
        {
            get
            {
                return (WorkMode == 22);
            }
        }

        private static byte _enabitSelect = 0x0F;
        /// <summary>
        /// 使能bit bit0-同步控制器 bit1-A相 bit2-B相 bit3-C相
        /// </summary>
        static public byte EnabitSelect
        {
            get
            {
                return _enabitSelect;
            }
            set
            {
                _enabitSelect = value;
            }
        }
    }
}
