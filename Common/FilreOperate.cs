using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public class FileOperate
    {
        /// <summary>
        /// 打开文本文件，并写入内容。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="writeConent"></param>
        static public void WriteTextFile(string path, string writeConent)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(writeConent);  //写入文件中
                sw.Flush();//清理缓冲区
                sw.Close();//关闭文件
            }
        }

       

    }
}
