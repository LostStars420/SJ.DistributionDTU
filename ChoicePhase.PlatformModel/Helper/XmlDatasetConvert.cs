using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ChoicePhase.PlatformModel.Helper
{
    /// <summary>
    /// XMl与DataSet之间进行转换
    /// </summary>
    public class XmlDatasetConvert
    {
        /// <summary>
        /// 将xmlData读取字符串数据转化为DataSet
        /// </summary>
        /// <param name="xmlData">XML数据</param>
        /// <param name="xsdPath">XSD路径</param>
        /// <returns>DataSet</returns>
        public static DataSet ConvertXMLToDataSet(string xmlData, string xsdPath)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                xmlDS.ReadXmlSchema(xsdPath);
                stream = new StringReader(xmlData);
                //从stream装载到XmlTextReader
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// 将xml文件转换为DataSet
        /// </summary>
        /// <param name="xmlFile">XML路径</param>
        /// <param name="xsdPath">XSD路径</param>
        /// <returns>DataSet</returns>
        public static DataSet ConvertXMLFileToDataSet(string xmlFile, string xsdPath)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                XmlDocument xmld = new XmlDocument();
                xmld.Load(xmlFile);

                DataSet xmlDS = new DataSet();
                xmlDS.ReadXmlSchema(xsdPath);
                stream = new StringReader(xmld.InnerXml);
                //从stream装载到XmlTextReader
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                //xmlDS.ReadXml(xmlFile);
                return xmlDS;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }



        /// <summary>
        /// 将DataSet转换为xml对象字符串
        /// </summary>
        /// <param name="xmlDS">DataSet</param>
        /// <returns>xml对象字符串</returns>
        public static string ConvertDataSetToXML(DataSet xmlDS)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;

            try
            {
                stream = new MemoryStream();
                //从stream装载到XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);

                //用WriteXml方法写入文件.
                xmlDS.WriteXml(writer);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);

                UnicodeEncoding utf = new UnicodeEncoding();
                return utf.GetString(arr).Trim();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

       /// <summary>
        ///  //将DataSet转换为xml文件
       /// </summary>
       /// <param name="xmlDS">DataSet</param>
       /// <param name="xmlFile">指定的XML路径</param>
        public static void ConvertDataSetToXMLFile(DataSet xmlDS, string xmlFile)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;

            try
            {
                stream = new MemoryStream();
                //从stream装载到XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);

                //用WriteXml方法写入文件.
                xmlDS.WriteXml(writer);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);

                //返回Unicode编码的文本
             
                UnicodeEncoding utf = new UnicodeEncoding();
               
                StreamWriter sw = new StreamWriter(xmlFile);
                string str = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                
                sw.WriteLine(str.Trim());
                
                sw.WriteLine(utf.GetString(arr).Trim());
              
                sw.Close();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

    }
}
