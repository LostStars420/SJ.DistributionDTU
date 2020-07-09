using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.DeviceNet.Element
{
    /// <summary>
    /// 标识符，标识代号与注释
    /// </summary>
    public class Identifier
    {
        /// <summary>
        /// 代号
        /// </summary>
        public UInt32 ID
        {
            get;
            set;
        }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment
        {
            get;
            set;
        }
        /// <summary>
        /// 标识符
        /// </summary>
        /// <param name="str">注释</param>
        /// <param name="id">ID</param>
        public Identifier(string str,UInt32  id)
        {
            Comment = str;
            ID = id;
        }

    }
}
