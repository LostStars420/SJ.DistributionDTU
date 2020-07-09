using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ChoicePhase.PlatformModel.Communication;
using ChoicePhase.PlatformModel.DataItemSet;
using ChoicePhase.PlatformModel.GetViewData;



namespace ChoicePhase.PlatformModel.Helper
{
    public class XMLOperate
    {
        static public DataSet ReadXml(string xmlPath, string xsdPath)
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

                var ds = new DataSet();
                ds.ReadXmlSchema(xsdPath);
                ds.ReadXml(xmlPath);
                
                return ds;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取串口数据集合
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="typeEnum"></param>
        /// <returns></returns>
        public  static SerialPortAttribute ReadLastPortRecod()
        {
            try
            {
                var ds = ReadXml(CommonPath.CommonPortXmlPath, CommonPath.CommonPortXsdPath);

                DataRow productRow = ds.Tables["SerialPort"].Rows[0];

                return new ChoicePhase.PlatformModel.Communication.SerialPortAttribute(
                    (int)productRow["CommonPort"],
                    (int)productRow["Baud"],
                    (int)productRow["DataBit"],
                    (int)productRow["ParityBit"],
                    (int)productRow["StopBit"]);                           

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取配置参数集合
        /// </summary>
        public static void ReadLastConfigRecod()
        {
            try
            {
                var ds = ReadXml(CommonPath.CommonPortXmlPath, CommonPath.CommonPortXsdPath);

                DataRow productRow = ds.Tables["ConfigParameter"].Rows[0];

                NodeAttribute.EnabitSelect =(byte) (int)productRow["EnabitSelect"];
                NodeAttribute.SynCloseActionOverTime = (int)productRow["SynCloseActionOverTime"];
                NodeAttribute.CloseActionOverTime = (int)productRow["CloseActionOverTime"];
                NodeAttribute.OpenActionOverTime = (int)productRow["OpenActionOverTime"];

                NodeAttribute.ClosePowerOnTime = (byte)(int)productRow["ClosePowerOnTime"];
                NodeAttribute.OpenPowerOnTime = (byte)(int)productRow["OpenPowerOnTime"];
                NodeAttribute.WorkMode = (int)productRow["WorkMode"];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 存储配置参数集合
        /// </summary>
        public static void WriteLastConfigRecod()
        {
            try
            {
                var ds = ReadXml(CommonPath.CommonPortXmlPath, CommonPath.CommonPortXsdPath);

                DataRow productRow = ds.Tables["ConfigParameter"].Rows[0];

                productRow["EnabitSelect"] = (int)NodeAttribute.EnabitSelect;
                productRow["SynCloseActionOverTime"] = (int)NodeAttribute.SynCloseActionOverTime;
                productRow["CloseActionOverTime"]  = (int)NodeAttribute.CloseActionOverTime;
                productRow["OpenActionOverTime"] = (int) NodeAttribute.OpenActionOverTime;

                productRow["ClosePowerOnTime"]  = (int)NodeAttribute.ClosePowerOnTime;
                productRow["OpenPowerOnTime"] = (int) NodeAttribute.OpenPowerOnTime;
                productRow["WorkMode"]  = (int)NodeAttribute.WorkMode;

                ds.WriteXml(CommonPath.CommonPortXmlPath); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 读取用户信息
        /// </summary>
        /// <returns></returns>
        public static User ReadUserRecod()
        {
            try
            {
                var ds = ReadXml(CommonPath.UserXmlPath, CommonPath.UserXsdPath);
                 DataRow productRow = ds.Tables["User"].Rows[0];
                 var user = new User(
                    (string)productRow["Name"],
                    (string)productRow["PasswordI"],
                    (string)productRow["PasswordII"]);                 
                 return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>        
        /// 保存用户信息
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="typeEnum"></param>
        /// <returns></returns>
        public static void WriteUserRecod(User user)
        {
            try
            {
                var ds = ReadXml(CommonPath.UserXmlPath, CommonPath.UserXsdPath);

                ds.Tables["User"].Rows.Clear();

                var productRow = ds.Tables["User"].NewRow();

                productRow["Name"] = user.Name;
                productRow["PasswordI"] = user.PasswordI;
                productRow["PasswordII"] = user.PasswordII;
                ds.Tables["User"].Rows.Add(productRow);
                ds.WriteXml(CommonPath.UserXmlPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>        
        /// 保存命令
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="typeEnum"></param>
        /// <returns></returns>
        public static void WriteLastPortRecod(SerialPortAttribute attribute)
        {
            try
            {
                var ds = ReadXml(CommonPath.CommonPortXmlPath, CommonPath.CommonPortXsdPath);

                ds.Tables["SerialPort"].Rows.Clear();

                var productRow = ds.Tables["SerialPort"].NewRow();
               
                productRow["CommonPort"] = attribute.CommonPortNum;
                productRow["Baud"]  = attribute.Baud;
                productRow["DataBit"] = attribute.DataBit;
                productRow["ParityBit"] = attribute.ParityBitMark;
                productRow["StopBit"] = attribute.StopBitMark;
                ds.Tables["SerialPort"].Rows.Add(productRow);
                ds.WriteXml(CommonPath.CommonPortXmlPath);               

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 读取永磁控制器错误代码集合
        /// </summary>
        public static void ReadYongciErrorCodeRecod()
        {
            try
            {
                var ds = ReadXml(CommonPath.YongciErrorCodeXmlPath, CommonPath.ErrorCodeXsdPath);
               
                var rows =  ds.Tables["ErrorCode"].Rows;
                for (int i = 0; i < rows.Count; i++)
                {

                    var ele = new ErrorCode((int)rows[i]["ID"], (string)rows[i]["Sign"], (string)rows[i]["Comment"]);
                     ErrorCode.YongciErrorCode.Add(ele);
                }
         
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 读取同步控制器错误代码集合
        /// </summary>
        public static void ReadTongbuErrorCodeRecod()
        {
            try
            {
                var ds = ReadXml(CommonPath.TongbuErrorCodeXmlPath, CommonPath.ErrorCodeXsdPath);

                var rows = ds.Tables["ErrorCode"].Rows;
                for (int i = 0; i < rows.Count; i++)
                {

                    var ele = new ErrorCode((int)rows[i]["ID"], (string)rows[i]["Sign"], (string)rows[i]["Comment"]);
                    ErrorCode.TongbuErrorCode.Add(ele);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
