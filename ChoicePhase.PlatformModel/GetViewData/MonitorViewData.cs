using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.Helper;

namespace ChoicePhase.PlatformModel.GetViewData
{

    /// <summary>
    /// 监控数据视图：用于提供表格数据，状态栏数据
    /// </summary>
    public partial class MonitorViewData : ObservableObject
    {
        private Dictionary<string, byte> operateCollect;

        public Dictionary<string, byte> OperateCollect
        {
            get
            {
                return operateCollect;
            }
        }

        private ObservableCollection<string> _operateCmd;
        public ObservableCollection<string> OperateCmd
        {
            get
            {
                return _operateCmd;
            }
        }

        /// <summary>
        /// 设定值 属性列表--按顺序分别为永磁控制器A，永磁控制器B，永磁控制器C，DSP,ARM，监控A，监控B，监控C
        /// 永磁控制器A 0 -- 设置属性合集， 1--参数属性合集
        /// 永磁控制器B 2 -- 设置属性合集， 3--参数属性合集
        /// 永磁控制器C 4 -- 设置属性合集， 5--参数属性合集 ---以此类推
        /// </summary>
        private List<ObservableCollection<AttributeItem>> _AttributeCollect;

        /// <summary>
        /// 属性表格名称
        /// </summary>
        private List<string> _tableAttributeName;

        private List<byte> _macList;

        private const byte readonlyStartIndex = 0x41;

        /// <summary>
        /// 站点名称列表
        /// </summary>
        public ObservableCollection<string> StationNameList
        {
            private set;
            get;
        }

        /// <summary>
        /// 数据库操作
        /// </summary>
        private SQLliteDatabase dataBase;

        public MonitorViewData()
        {
            try
            {
                dataBase = new SQLliteDatabase(CommonPath.DataBase);
              
                _AttributeCollect = new List<ObservableCollection<AttributeItem>>();
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);
                _AttributeCollect.Add(null);

                _tableAttributeName = new List<string>();

                _tableAttributeName.Add("AttributeSetYongciA");
                _tableAttributeName.Add("AttributeReadYongciA");
                _tableAttributeName.Add("AttributeSetYongciB");
                _tableAttributeName.Add("AttributeReadYongciB");
                _tableAttributeName.Add("AttributeSetYongciC");
                _tableAttributeName.Add("AttributeReadYongciC");
                _tableAttributeName.Add("AttributeSetDSP");
                _tableAttributeName.Add("AttributeReadDSP");

                //站点名称
                StationNameList = new ObservableCollection<string>();
                StationNameList.Add("A相控制器");
                StationNameList.Add("B相控制器");
                StationNameList.Add("C相控制器");
                StationNameList.Add("同步控制器");


                _macList = new List<byte>();
                _macList.Add(NodeAttribute.MacPhaseA);
                _macList.Add(NodeAttribute.MacPhaseB);
                _macList.Add(NodeAttribute.MacPhaseC);
                _macList.Add(NodeAttribute.MacSynController);

                XMLOperate.ReadYongciErrorCodeRecod();
                XMLOperate.ReadTongbuErrorCodeRecod();


                SwitchPropertyInit();


                operateCollect = new Dictionary<string, byte>();
                operateCollect.Add("合闸", 1);
                operateCollect.Add("分闸", 2);
                operateCollect.Add("置过流", 3);
                operateCollect.Add("故障切除复归", 4);
                operateCollect.Add("设置拒动", 5);
                operateCollect.Add("取消拒动", 6);
                operateCollect.Add("取消故障", 7);
                operateCollect.Add("置电源进线失压", 8);
                operateCollect.Add("复位", 9);
                operateCollect.Add("分布式投入", 10);
                operateCollect.Add("分布式退出", 11);
                operateCollect.Add("分布式投入硬压板", 12);
                operateCollect.Add("分布式退出硬压板", 13);
                operateCollect.Add("更新程序", 14);
                operateCollect.Add("开始打印", 15);
                operateCollect.Add("停止打印", 16);
                operateCollect.Add("K1合闸", 17);
                operateCollect.Add("K1分闸", 18);
                operateCollect.Add("K2合闸", 19);
                operateCollect.Add("K2分闸", 20);
                operateCollect.Add("K3合闸", 21);
                operateCollect.Add("K3分闸", 22);
                operateCollect.Add("K4合闸", 23);
                operateCollect.Add("K4分闸", 24);
                operateCollect.Add("K5合闸", 25);
                operateCollect.Add("K5分闸", 26);
                operateCollect.Add("K6合闸", 27);
                operateCollect.Add("K6分闸", 28);
                _operateCmd = new ObservableCollection<string>();
                foreach (var m in operateCollect)
                {
                    _operateCmd.Add(m.Key);
                }
            }
            catch (Exception ex)
            {
                Common.LogTrace.CLog.LogError(ex.Message + " " + ex.StackTrace);
            }

        }



        #region 状态信息
        private string statusMessage = "";


        /// <summary>
        /// 状态信息
        /// </summary>
        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                statusMessage = value;
                RaisePropertyChanged("StatusMessage");
                if (StatusMessage.Length > 5000)
                {
                    StatusMessage = "";
                }
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="der">描述</param>
        public void UpdateStatus(string des)
        {
            StatusMessage += "\n";
            StatusMessage += DateTime.Now.ToLongTimeString() + ":\n";
            StatusMessage += des;
        }

        #endregion

        #region 异常信息
        private string exceptionMessage = "";


        /// <summary>
        /// 异常
        /// </summary>
        public string ExceptionMessage
        {
            get
            {
                return exceptionMessage;
            }
            set
            {
                exceptionMessage = value;
                RaisePropertyChanged("ExceptionMessage");
                if (exceptionMessage.Length > 5000)
                {
                    exceptionMessage = "";
                }
            }
        }
        /// <summary>
        /// 更新显示异常信息
        /// </summary>
        /// <param name="comment"></param>
        public void UpadeExceptionMessage(string comment)
        {
            var str = DateTime.Now.ToLongTimeString() + "  异常处理:\n";
            str += comment + "\n";
            ExceptionMessage += str;
        }


        #endregion

        #region 获取MAC 获取属性索引
        /// <summary>
        /// 根据索引，获取MAC地址
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte GetMacAddr(AttributeIndex index)
        {
            switch (index)
            {
                case AttributeIndex.YongciReadA:
                case AttributeIndex.YongciSetA:
                    {
                        return _macList[0];
                    }
                case AttributeIndex.YongciReadB:
                case AttributeIndex.YongciSetB:
                    {
                        return _macList[1];
                    }
                case AttributeIndex.YongciReadC:
                case AttributeIndex.YongciSetC:
                    {
                        return _macList[2];
                    }
                case AttributeIndex.DspRead:
                case AttributeIndex.DspSet:
                    {
                        return _macList[3];
                    }
                case AttributeIndex.ArmRead:
                case AttributeIndex.ArmSet:
                    {
                        return _macList[4];
                    }
                case AttributeIndex.MonitorReadA:
                case AttributeIndex.MonitorSetA:
                    {
                        return _macList[5];
                    }
                case AttributeIndex.MonitorReadB:
                case AttributeIndex.MonitorSetB:
                    {
                        return _macList[6];
                    }
                case AttributeIndex.MonitorReadC:
                case AttributeIndex.MonitorSetC:
                    {
                        return _macList[7];
                    }
                default:
                    {
                        return 0xFF;
                    }
            }
        }


        /// <summary>
        /// 根据mac和ID，获取属性索引
        /// </summary>
        /// <param name="mac">MAC地址</param>
        /// <param name="id">ID号</param>
        /// <returns>属性索引</returns>
        public AttributeIndex GetAttributeIndex(int mac, int id)
        {
            if (_macList[0] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.YongciSetA;
                }
                else
                {
                    return AttributeIndex.YongciReadA;
                }
            }
            if (_macList[1] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.YongciSetB;
                }
                else
                {
                    return AttributeIndex.YongciReadB;
                }
            }
            if (_macList[2] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.YongciSetC;
                }
                else
                {
                    return AttributeIndex.YongciReadC;
                }
            }

            if (_macList[3] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.DspSet;
                }
                else
                {
                    return AttributeIndex.DspRead;
                }
            }
            if (_macList[4] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.ArmSet;
                }
                else
                {
                    return AttributeIndex.ArmRead;
                }
            }
            if (_macList[5] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.MonitorSetA;
                }
                else
                {
                    return AttributeIndex.MonitorReadA;
                }
            }

            if (_macList[6] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.MonitorSetB;
                }
                else
                {
                    return AttributeIndex.MonitorReadB;
                }
            }

            if (_macList[7] == mac)
            {
                if (id < readonlyStartIndex)
                {
                    return AttributeIndex.MonitorSetC;
                }
                else
                {
                    return AttributeIndex.MonitorReadC;
                }
            }
            return AttributeIndex.Null;
        }
        #endregion


        #region 属性列表操作--通用形式

        /// <summary>
        /// 创建Attribute
        /// </summary>
        public void CrateAttributeTable(AttributeIndex index)
        {
            string sql =
                "CREATE TABLE " + _tableAttributeName[(int)index] + "( ConfigID int, Name text,RawValue int,  dataType int, value double, comment text)";
            dataBase.CreateTale(sql);
        }

        //public void Create()
        //{
        //    string sql =
        //       "CREATE TABLE " + _tableAttributeName[8] + "( id int, faultType int,validRow int,  K1 int, K2 int, K3 int, K4 int, K5 int, K6 int,K7 int, K8 int, K9 int, K10 int, K11 int, K12 int,K13 int, K14 int, K15 int,K16 int, K17 int, K18 int, K19 int, K20 int, K21 int, K22 int, K23 int, K24 int, K25 int, K26 int, K27 int, K28 int, K29 int, K30 int, K31 int, K32 int)";
        //    dataBase.CreateTale(sql);
        //}



        /// <summary>
        /// 清空历史数据，将数据插入表格
        /// </summary>       
        public void InsertAttribute(AttributeIndex index)
        {

            var tableName = _tableAttributeName[(int)index];
            var collect = _AttributeCollect[(int)index];


            var listStr = new List<String>();
            foreach (var m in collect)
            {
                string sql = string.Format("INSERT INTO " + tableName + " VALUES({0},\'{1}\',{2},{3},{4},\'{5}\')",
                   m.ConfigID, m.Name, m.RawValue, m.DataType, m.Value, m.Comment);
                listStr.Add(sql);
            }
            if (listStr.Count > 0)
            {
                string sqlClear = "delete from " + tableName;
                dataBase.InsertTable(listStr, sqlClear);
            }
        }

        private AttributeIndex _currentAttributeIndex;
        /// <summary>
        /// 读取属性数据表格
        /// </summary>
        /// <param name="flag">true--重新更新, false--若当前已存在则直接使用</param>
        /// <param name="tableName">表格名称</param>
        /// <returns>永磁属性合集</returns>       
        public ObservableCollection<AttributeItem> ReadAttribute(AttributeIndex index, bool flag)
        {
            var tableName = _tableAttributeName[(int)index];
            var collect = _AttributeCollect[(int)index];


            if (collect == null || flag)
            {
                collect = new ObservableCollection<AttributeItem>();
                string sql = "SELECT * from " + tableName;
                _currentAttributeIndex = index; //同步更新currentAttributeIndex
                _AttributeCollect[(int)index] = collect;
                dataBase.ReadTable(sql, GetAttribute);
                return collect;
            }
            else
            {
                return collect;
            }
        }


        private bool GetAttribute(System.Data.SQLite.SQLiteDataReader reader)
        {

            _AttributeCollect[(int)_currentAttributeIndex].Add(new AttributeItem(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2),
                reader.GetInt32(3), reader.GetDouble(4), reader.GetString(5)));

            return true;
        }


        #endregion



      


        /// <summary>
        /// 更新属性字节
        /// </summary>
        /// <param name="id">ID号</param>
        /// <param name="data">数据字节</param>
        public void UpdateAttributeData(int mac, int id, byte[] data)
        {
            var index = GetAttributeIndex(mac, id);
            if (index != AttributeIndex.Null)
            {
                ReadAttribute(index, false);
                var collect = _AttributeCollect[(int)index];
                for (int i = 0; i < collect.Count; i++)
                {
                    if (collect[i].ConfigID == id)
                    {
                        collect[i].UpdateAttribute(data);
                    }
                }
            }
        }
    }
  


  
    /// <summary>
    /// 参数表格索引
    /// </summary>
    public enum AttributeIndex
    {
        /// <summary>
        /// 永磁设置参数A
        /// </summary>
        YongciSetA  = 0,
        /// <summary>
        /// 永磁只读参数A
        /// </summary>
        YongciReadA = 1,
        /// <summary>
        /// 永磁设置参数B
        /// </summary>
        YongciSetB = 2,
        /// <summary>
        /// 永磁只读参数B
        /// </summary>
        YongciReadB = 3,
        /// <summary>
        /// 永磁设置参数C
        /// </summary>
        YongciSetC = 4,
        /// <summary>
        /// 永磁只读参数A
        /// </summary>
        YongciReadC = 5,
        /// <summary>
        /// DSP设置参数
        /// </summary>
        DspSet = 6,
        /// <summary>
        /// DSP只读参数
        /// </summary>
        DspRead = 7,

      
        /// <summary>
        /// DSP设置参数
        /// </summary>
        ArmSet = 8,
        /// <summary>
        /// DSP只读参数
        /// </summary>
        ArmRead = 9,

  
        /// <summary>
        /// 监控设置参数A
        /// </summary>
        MonitorSetA = 10,
        /// <summary>
        /// 监控只读参数A
        /// </summary>
        MonitorReadA = 11,
        /// <summary>
        /// 监控设置参数B
        /// </summary>
        MonitorSetB = 12,
        /// <summary>
        /// 监控只读参数B
        /// </summary>
        MonitorReadB = 13,
        /// <summary>
        /// 监控设置参数C
        /// </summary>
        MonitorSetC = 14,
        /// <summary>
        /// 监控只读参数C
        /// </summary>
        MonitorReadC = 15,
        Null = 0xFF,
    }
}
