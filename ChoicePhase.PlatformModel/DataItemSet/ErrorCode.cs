using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    /// <summary>
    /// 错误代码，永磁控制器，同步控制器
    /// </summary>
    public class ErrorCode
    {
        private int _id;

        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private string _sign;

        public string Sign
        {
            get
            {
                return _sign;
            }
            set
            {
                _sign = value;
            }
        }

        private string _comment;

        private string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        /// <summary>
        /// 初始化错误代码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sign"></param>
        /// <param name="comment"></param>
        public  ErrorCode(int id, string sign, string comment)
        {
            _id = id;
            _sign = sign;
            _comment = comment;

        }

        static private List<ErrorCode> _yongciErrorCode = new List<ErrorCode>();


        /// <summary>
        /// 永磁控制器错误代码
        /// </summary>
        static public List<ErrorCode> YongciErrorCode
        {
            get
            {
                return _yongciErrorCode;
            }            
        }


        static private List<ErrorCode> _tongbuErrorCode = new List<ErrorCode>();

        /// <summary>
        /// 同步控制器错误代码
        /// </summary>
        static public List<ErrorCode> TongbuErrorCode
        {
            get
            {
                return _tongbuErrorCode;
            }
        }

        /// <summary>
        /// 获取永磁错误代码注释
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>代码注释</returns>
        static public string GetYongciErrorComment(int id)
        {
            foreach(var m in _yongciErrorCode)
            {
                if ( m.ID == id)
                {
                    return string.Format("{0}({1}),代号0x{2:X2}", m.Comment, m.Sign, m.ID);
                }
            }
            return "未识别的ID";
        }


        /// <summary>
        /// 获取同步控制器错误代码注释
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>代码注释</returns>
        static public  string GetTongbuErrorComment(int id)
        {
            foreach (var m in _tongbuErrorCode)
            {
                if (m.ID == id)
                {
                    return string.Format("{0}({1}),代号{2:X2}", m.Comment, m.Sign, m.ID);
                }
            }
            return "未识别的ID";
        }
       
    }
}
