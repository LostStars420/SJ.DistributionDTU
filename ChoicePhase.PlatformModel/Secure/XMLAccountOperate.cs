using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ChoicePhase.PlatformModel.Helper;



namespace Monitor.AutoStudio.Secure
{
    public class XMLAccountOperate
    {

        /// <summary>
        /// 读取数据集合
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="xsdPath"></param>
        /// <returns></returns>
        public DataSet ReadXml(string xmlPath, string xsdPath)
        {
            try
            {
                if (!File.Exists(xmlPath))
                {
                    throw new Exception("路径:" + xmlPath + ", 所指向的文件不存在，请重新选择");
                }
                if (!File.Exists(xsdPath))
                {
                    throw new Exception("路径:" + xsdPath + ", 所指向的文件不存在，请重新选择");
                }


                string xmlData = "";
                

                using (StreamReader reader = new StreamReader(xmlPath))
                {
                    xmlData = reader.ReadToEnd();
                }


                xmlData = xmlData.Trim();
                var ds = XmlDatasetConvert.ConvertXMLToDataSet(xmlData, xsdPath);

               // XmlDatasetConvert.ConvertDataSetToXMLFile(ds, "t2.xml");



                return ds;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="ds">数据/param>
        /// <returns>账户数据集合</returns>
        public List<UserAccount> GetDataCollect(DataSet ds)
        {
            try
            {

                var dataCollect = new List<UserAccount>(); ;
                foreach (DataRow productRow in ds.Tables["UserAccount"].Rows)
                {
                    dataCollect.Add(new UserAccount(
                        (string)productRow["UserName"],
                        (string)productRow["PassWord"],
                        (int)productRow["PowerLevel"],
                        (string)productRow["LastLogin"]));

                }
                return dataCollect;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新表格
        /// </summary>
        /// <param name="ds">将要更新的数据集合</param>
        /// <param name="datas">数据集合</param>
        public void UpdateTable(DataSet ds, List<UserAccount> datas)
        {
            try
            {
                ds.Tables["UserAccount"].Rows.Clear();
               
                foreach (var m in datas)
                {
                    var productRow = ds.Tables["UserAccount"].NewRow();
                    productRow["UserName"] = m.UserName;
                    productRow["PassWord"] = m.PassWord;
                    productRow["PowerLevel"] = m.PowerLevel;
                    productRow["LastLogin"] = m.LastLogin;
                    ds.Tables["UserAccount"].Rows.Add(productRow);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
