using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using ChoicePhase.PlatformModel.DataItemSet;

namespace ChoicePhase.PlatformModel.GetViewData
{
    /// <summary>
    /// SQLliteDatabase数据库
    /// </summary>
    public class SQLliteDatabase
    {
        string datasource = @"config\Database\Das.db";//默认位置

      /// <summary>
      /// 数据库实例
      /// </summary>
      /// <param name="dataBaseSource">数据库源</param>
        public SQLliteDatabase(string dataBaseSource)
        {
            //  System.Data.SQLite.SQLiteConnection.CreateFile(datasource);
            datasource = dataBaseSource;
        }

        /// <summary>
        /// 创建
        /// </summary>
        private void connectDatabase()
        {
            //连接
            System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection();
            System.Data.SQLite.SQLiteConnectionStringBuilder connstr = new System.Data.SQLite.SQLiteConnectionStringBuilder();
            connstr.DataSource = datasource;
            //connstr.Password = "admin";//设置密码，SQLite ADO.NET实现了数据库密码保护
            conn.ConnectionString = connstr.ToString();
            conn.Open();

            //创建表格
            //System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
            //string sql = "CREATE TABLE control_authority(ElementName varchar(32), MinLevel int,ToStr varchar(32))";
            //cmd.CommandText = sql;
            //cmd.Connection = conn;
            //cmd.ExecuteNonQuery();
        }
       
        /// <summary>
        /// 创建表格
        /// </summary>
        /// <param name="sql">sql创建表格语句</param>       
        public void CreateTale(string sql)
        {
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection())
                {
                    var connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = datasource;
                    //connstr.Password = "admin";//设置密码，SQLite ADO.NET实现了数据库密码保护
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {                    
                        
                        cmd.CommandText = sql;
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    

        /// <summary>
        /// 插入表格数据
        /// </summary>
        /// <param name="sqlString">插入语句合集</param>
        public void InsertTable(List<string> sqlString)
        {
            InsertTable(sqlString, null);
        }

        /// <summary>
        /// 插入表格
        /// </summary>
        /// <param name="sqlString">插入语句</param>
        /// <param name="sqlClear">清空语句</param>
        public void InsertTable(List<string> sqlString, string sqlClear)
        {
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection())
                {
                    var connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = datasource;
                    //connstr.Password = "admin";//设置密码，SQLite ADO.NET实现了数据库密码保护
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {                                  
                        cmd.Connection = conn;
                        //删除表格中索引的数据
                        if ( sqlClear != null)
                        {
                            cmd.CommandText = sqlClear;
                            cmd.ExecuteNonQuery();
                        }               
                        foreach (var m in sqlString)
                        {
                            //sql = "INSERT INTO test VALUES('a','b')";
                            //sql = string.Format("INSERT INTO  control_authority VALUES(\'{0}\',{1},\'{2}\')", m.ElementName, m.MinLevel, m.ToStr);
                            cmd.CommandText = m;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
      

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <param name="sqlSelect">查询语句</param>
        /// <param name="ReadDataDelegate">读取委托</param>
        public  void  ReadTable(string sqlSelect, Func<System.Data.SQLite.SQLiteDataReader, bool> ReadDataDelegate)
        {
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection())
                {
                    var connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = datasource;
                    //connstr.Password = "admin";//设置密码，SQLite ADO.NET实现了数据库密码保护
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        //var collect = new List<Tuple<string, int>>();
                        cmd.Connection = conn;
                        //string sql = "SELECT * from control_authority";
                        cmd.CommandText = sqlSelect;
                        System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            //collect.Add(new Tuple<string, int>(reader.GetString(0), reader.GetInt32(1)));
                           if( !ReadDataDelegate(reader))
                           {
                               break;
                           }
                        }
                        conn.Close();                  

                    }                  
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }


        /// <summary>
        /// 删除表格中的一些项目
        /// </summary>
        /// <param name="list">删除列表</param>
        //public void DeleteAuthorityTableItem(List<string> list)
        //{
        //    try
        //    {
        //        if (list.Count > 0)
        //        {

        //            using (var conn = new System.Data.SQLite.SQLiteConnection())
        //            {
        //                var connstr = new SQLiteConnectionStringBuilder();
        //                connstr.DataSource = datasource;
        //                //connstr.Password = "admin";//设置密码，SQLite ADO.NET实现了数据库密码保护
        //                conn.ConnectionString = connstr.ToString();
        //                conn.Open();
        //                using (SQLiteCommand cmd = new SQLiteCommand())
        //                {


        //                    var collect = new List<Tuple<string, int>>();

        //                    cmd.Connection = conn;

        //                    StringBuilder sqlBuilder = new StringBuilder();
        //                    sqlBuilder.Append(@"SELECT * from control_authority where ");
        //                    int cn = 0;
        //                    foreach (var m in list)
        //                    {
        //                        sqlBuilder.Append("ElementName=");
        //                        sqlBuilder.Append(m);
        //                        if(++cn != list.Count)
        //                        {
        //                            sqlBuilder.Append(" or ");
        //                        }
                                
        //                    }

        //                    cmd.CommandText = sqlBuilder.ToString();
        //                    cmd.ExecuteNonQuery();
        //                    conn.Close();
        //                }


        //            }
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
