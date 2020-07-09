using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Monitor.AutoStudio.Secure
{
    public class UserAccount 
    {
        /// <summary>
        /// 用户名
        /// </summary>
        private string userName;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
               
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string passWord;

        public string PassWord
        {
            get
            {
                return passWord;
            }
            set
            {
                passWord = value;
            }
        }

        /// <summary>
        /// 权限等级
        /// </summary>
        private int powerlevel;

        /// <summary>
        /// 权限等级
        /// </summary>
        public int PowerLevel
        {
            get
            {
                return powerlevel;
            }
            set
            {
                powerlevel = value;
            }
        }

        /// <summary>
        /// 是否是最后一次登陆 yes、no
        /// </summary>
        private string lastLogin;

        /// <summary>
        /// 最后一次登陆 yes、no， null
        /// </summary>
        public string LastLogin
        {
            get
            {
                return lastLogin;
            }
            set
            {
                lastLogin = value;
            }
        }



        /// <summary>
        /// 用户账户初始化
        /// </summary>
        /// <param name="inUserName">用户名</param>
        /// <param name="inPass">用户密码</param>
        /// <param name="inPowerlevel">用户等级</param>
        /// <param name="inlastDescrible">最后一次登陆标志</param>
        public UserAccount(string inUserName, string inPass, int inPowerlevel, string inlastDescrible)
        {
            userName = inUserName;
            passWord = inPass;
            powerlevel = inPowerlevel;
            lastLogin = inlastDescrible;
        }

        
        
    }
}
