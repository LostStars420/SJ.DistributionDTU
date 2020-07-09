using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.PlatformModel.DataItemSet
{
    public class User
    {
        private string _name;

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        private string _passwordI;

        /// <summary>
        /// 登陆密钥
        /// </summary>
        public string PasswordI
        {
            get
            {
                return _passwordI;
            }
            set
            {
                _passwordI = value;
            }
        }
        private string _passwordII;

        /// <summary>
        /// 操作密钥II
        /// </summary>
        public string PasswordII
        {
            get
            {
                return _passwordII;
            }
            set
            {
                _passwordII = value;
            }
        }

        public User(string name, string passI, string passII)
        {
            _name = name;
            _passwordI = passI;
            _passwordII = passII;
        }
    }
}
