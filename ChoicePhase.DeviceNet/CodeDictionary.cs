using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChoicePhase.DeviceNet.Element;

namespace ChoicePhase.DeviceNet
{
    /// <summary>
    /// 数据仓库--标识符，代号索引等
    /// </summary>
    public class CodeDictionary
    {
       // public Identifier GroupFunctionCode;

        /// <summary>
        /// 分组代号
        /// </summary>
        static  public Dictionary<string, UInt32> GroupFunctionCode
        {
            get;
            set;
        }
        /// <summary>
        /// 服务代码定义
        /// </summary>
        static   public Dictionary<string, UInt32> ServerCode
        {
            get;
            set;
        }
        /// <summary>
        /// 错误代码定义
        /// </summary>
        static  public Dictionary<string, UInt32> ErrorCode
        {
            get;
            set;
        }
        
        /// <summary>
        /// 初始化代码字典，存储各种代号与标识
        /// </summary>
        public CodeDictionary(List<Identifier> groupList, List<Identifier> serverList, List<Identifier> errorList)
        {
            try
            {
                GroupFunctionCode = new Dictionary<string, UInt32>();
                foreach (var m in groupList)
                {
                    GroupFunctionCode.Add(m.Comment, m.ID);
                }
                ServerCode = new Dictionary<string, uint>();
                foreach (var m in serverList)
                {
                    ServerCode.Add(m.Comment, m.ID);
                }
                ErrorCode = new Dictionary<string, uint>();
                foreach (var m in errorList)
                {
                    ErrorCode.Add(m.Comment, m.ID);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
