using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GooseGenerator
{
    /// <summary>
    /// FCDA类
    /// </summary>
    public class FCDA
    {
        private String _ref;
        public String Ref
        {
            get
            {
                return _ref;
            }
            set
            {
                _ref = value;
            }

        }

        private String _type;
        public String Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private String _inVarName;
        public String InVarName
        {
            get
            {
                return _inVarName;
            }
            set
            {
                _inVarName = value;
            }
        }

        public int ACT
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Ref"></param>
        /// <param name="type"></param>
        /// <param name="inVarName"></param>
        /// <param name="act"></param>
        public FCDA(String Ref, String type, String inVarName, int act)
        {
            _ref = Ref;
            _type = type;
            _inVarName = inVarName;
            ACT = act;
        }
    }
}
