using Distribution.station;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.Helper;

namespace ChoicePhase.PlatformModel.GetViewData
{

    /// <summary>
    /// 监控数据视图：开关属性数据
    /// </summary>
    public partial class MonitorViewData : ObservableObject
    {
        private List<ObservableCollection<SwitchProperty>> _SwitchPropertyCollect;
        private List<string> _tableSwitchPropertyName;
        //新加的
        private List<ObservableCollection<FaultStatus>> _faultStatusCollect;

        public void SwitchPropertyInit()
        {
            try
            {
                _tableSwitchPropertyName = new List<string>();
                _tableSwitchPropertyName.Add("switchList");
               
                _SwitchPropertyCollect = new List<ObservableCollection<SwitchProperty>>();
                _SwitchPropertyCollect.Add(null);

                // 12/4新加的
                _tableSwitchPropertyName.Add("FaultStatus");
                _faultStatusCollect = new List<ObservableCollection<FaultStatus>>();
                _faultStatusCollect.Add(null);
                _stationObservable = new ObservableCollection<StationInformation>();
            }
            catch (Exception ex)
            {
                Common.LogTrace.CLog.LogError(ex.Message + " " + ex.StackTrace);
            }
        }

        #region 开关站信息
        private ObservableCollection<StationInformation> _stationObservable;

        /// <summary>
        /// 站点信息列表
        /// </summary>
        public ObservableCollection<StationInformation> StationObservable
        {
            get
            {
                return _stationObservable;
            }
        }

        public void UpadteStationObservable(StationMessage message)
        {
            var findid = message.id_own;
            foreach(var m in _stationObservable)
            {
                if(m.IdOwn == findid)
                {
                    m.UpdateAll(message);
                    return;
                }
            }
            _stationObservable.Add(new StationInformation(message));
        }
        #endregion

        #region 12/4新加的faultStatus表格操作

        public void InsertFaultStatus(SwitchPropertyIndex index)
        {
            var tableName = _tableSwitchPropertyName[(int)index];
            var collect = _faultStatusCollect[0];

            var listStr = new List<String>();
            foreach (var m in collect)
            {
                string sql = string.Format("INSERT INTO " + tableName +
                    " VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34})",
                   m.ID, m.FaultType, m.ValidRow, m.K1, m.K2, m.K3, m.K4, m.K5, m.K6, m.K7, m.K8,
                   m.K9, m.K10, m.K11, m.K12, m.K13, m.K14, m.K15, m.K16, m.K17, m.K18, m.K19, m.K20,
                   m.K21, m.K22, m.K23, m.K24, m.K25, m.K26, m.K27, m.K28, m.K29, m.K30, m.K31, m.K32);
                listStr.Add(sql);
            }

            if (listStr.Count > 0)
            {
                string sqlClear = "delete from " + tableName;
                dataBase.InsertTable(listStr, sqlClear);
            }
        }


        private SwitchPropertyIndex _currentFaultStatusIndex;
        /// <summary>
        /// 读取属性数据表格
        /// </summary>
        /// <param name="flag">true--重新更新, false--若当前已存在则直接使用</param>
        /// <param name="tableName">表格名称</param>
        /// <returns>永磁属性合集</returns>       
        public ObservableCollection<FaultStatus> ReadFaultStatus(SwitchPropertyIndex index, bool flag)
        {
            var tableName = _tableSwitchPropertyName[(int)index];
            var collect = _faultStatusCollect[0];

            if (collect == null || flag)
            {
                collect = new ObservableCollection<FaultStatus>();
                string sql = "SELECT * from " + tableName;
                _currentFaultStatusIndex = index-1; //同步更新currentAttributeIndex
                _faultStatusCollect[0] = collect;
                dataBase.ReadTable(sql, GetFaultStatus);
                return collect;
            }
            else
            {
                return collect;
            }
        }


        private bool GetFaultStatus(System.Data.SQLite.SQLiteDataReader reader)
        {
            _faultStatusCollect[(int)_currentFaultStatusIndex].Add(new FaultStatus(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3),
                           reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7), reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10),
                           reader.GetInt32(11), reader.GetInt32(12), reader.GetInt32(13), reader.GetInt32(14), reader.GetInt32(15), reader.GetInt32(16), reader.GetInt32(17),
                           reader.GetInt32(18), reader.GetInt32(19), reader.GetInt32(20), reader.GetInt32(21), reader.GetInt32(22), reader.GetInt32(23), reader.GetInt32(24),
                           reader.GetInt32(25), reader.GetInt32(26), reader.GetInt32(27), reader.GetInt32(28), reader.GetInt32(29), reader.GetInt32(30), reader.GetInt32(31),
                           reader.GetInt32(32), reader.GetInt32(33), reader.GetInt32(34)));

            return true;
        }


        #endregion


        #region 开关属性

        /// <summary>
        /// 清空历史数据，将数据插入表格
        /// </summary>       
        public void InsertAttribute(SwitchPropertyIndex index)
        {

            var tableName = _tableSwitchPropertyName[(int)index];
            var collect = _SwitchPropertyCollect[(int)index];

            var listStr = new List<String>();
            foreach (var m in collect)
            {
                string sql = string.Format("INSERT INTO " + tableName + " VALUES({0},\'{1}\',{2},{3},{4},\'{5}\',{6},\'{7}\',\'{8}\',{9},\'{10}\',\'{11}\',{12},\'{13}\', \'{14}\',\'{15}\', \'{16}\' , {17}, {18}, {19})",
                   m.ConfigID, m.IP, m.Type, m.State, m.NeighboorNum, m.NeighboorCollect, m.Capacity, m.Comment,m.M,m.MCount,m.MType,m.N,m.NCount,m.NType,m.Addr,m.Gocb,m.MACAddress, 
                   m.APPID, m.SwitchID, m.MaxTime);
                listStr.Add(sql);
            }
            if (listStr.Count > 0)
            {
                string sqlClear = "delete from " + tableName;
                dataBase.InsertTable(listStr, sqlClear);
            }
        }

        private SwitchPropertyIndex _currentSwitchPropertyIndex;
        /// <summary>
        /// 读取属性数据表格
        /// </summary>
        /// <param name="flag">true--重新更新, false--若当前已存在则直接使用</param>
        /// <param name="tableName">表格名称</param>
        /// <returns>永磁属性合集</returns>       
        public ObservableCollection<SwitchProperty> ReadAttribute(SwitchPropertyIndex index, bool flag)
        {
            var tableName = _tableSwitchPropertyName[(int)index];
            var collect = _SwitchPropertyCollect[(int)index];

            if (collect == null || flag)
            {
                collect = new ObservableCollection<SwitchProperty>();
                string sql = "SELECT * from " + tableName;
                _currentSwitchPropertyIndex = index; //同步更新currentAttributeIndex
                _SwitchPropertyCollect[(int)index] = collect;
                dataBase.ReadTable(sql, GetSwitchProperty);
                return collect;
            }
            else
            {
                return collect;
            }
        }


        private bool GetSwitchProperty(System.Data.SQLite.SQLiteDataReader reader)
        {
            _SwitchPropertyCollect[(int)_currentSwitchPropertyIndex].Add(new SwitchProperty(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3),
                           reader.GetInt32(4), reader.GetString(5), (float)reader.GetDouble(6),reader.GetString(8),reader.GetInt32(9),reader.GetString(10),
                           reader.GetString(11),reader.GetInt32(12),reader.GetString(13),reader.GetString(14), 
                           reader.GetString(15), reader.GetString(16),reader.GetInt32(17), reader.GetInt32(18), reader.GetInt32(19)));
            //_SwitchPropertyCollect[(int)_currentSwitchPropertyIndex].Add(new SwitchProperty("s", 23, 23,
            //    23, "23", 0));
            return true;
        }


        public void UpdateSwitchProperty(byte[] data)
        {
            UInt32 id = TypeConvert.Combine(data, 1);
            var collect = _SwitchPropertyCollect[(int)SwitchPropertyIndex.SwitchList];
            foreach(var m in collect)
            {
                if (TypeConvert.IpToInt(m.IP) == id)
                {
                    m.Update(data, 5);
                    break;
                }
            }
        }

        #endregion
    
    }


    public enum SwitchPropertyIndex
    {
        /// <summary>
        /// 开关列表
        /// </summary>
        SwitchList = 0,
        FaultStatus = 1,

    }

}
