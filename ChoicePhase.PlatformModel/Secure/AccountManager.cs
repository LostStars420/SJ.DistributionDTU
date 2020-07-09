using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using ChoicePhase.PlatformModel.Helper;


namespace Monitor.AutoStudio.Secure
{
    /// <summary>
    /// 账户管理器
    /// </summary>
    public class AccountManager
    {

        /// <summary>
        /// 用户账户XML
        /// </summary>
        private XMLAccountOperate xmlAccount;

        /// <summary>
        /// 账户数据集合
        /// </summary>
        private DataSet accountDataSet;
        /// <summary>
        /// 账户数据列表
        /// </summary>
        private List<UserAccount> accountList;

        /// <summary>
        /// 账户数据列表
        /// </summary>
        public List<UserAccount> AccountList
        {
            get
            {
                return accountList;
            }
        }



        /// <summary>
        /// 当前登陆账户
        /// </summary>
        private UserAccount loginAccount;

        /// <summary>
        /// 获取当前登陆账户
        /// </summary>
        public UserAccount LoginAccount
        {
            get
            {
                return loginAccount;
            }
        }
        

        /// <summary>
        /// 载入账户信息
        /// </summary>
        public AccountManager()
        {
            loadAccoutInformation();
        }

        /// <summary>
        /// 载入账户信息
        /// </summary>
        private void loadAccoutInformation()
        {
            xmlAccount = new XMLAccountOperate();
            accountDataSet = xmlAccount.ReadXml(CommonPath.AccountXmlPath, CommonPath.AccountXsdPath);
            accountList = xmlAccount.GetDataCollect(accountDataSet);


        }

        /// <summary>
        /// 存储账户信息
        /// </summary>
        public void SaveAccountInformation()
        {
            //置最后一次登陆标志位
            foreach(var m in accountList)
            {
                m.LastLogin = "no";
            }
            loginAccount.LastLogin = "yes";
            xmlAccount.UpdateTable(accountDataSet, accountList);
            //存储xml账户信息
            accountDataSet.WriteXml(CommonPath.AccountXmlPath);
        }


        /// <summary>
        /// 登陆验证，并设置当前用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>成功分返回true，失败返回false</returns>
        public bool LoginCheck(string userName, SecureString password)
        {
            var account = AccountCheck(userName, password);
            if (account != null)
            {
                loginAccount = account;
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 查找输入用户名与密码是否在当前列表之类
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回查找到的账户，否则返回null</returns>
        private  UserAccount AccountCheck(string userName, SecureString password)
        {
            foreach(var m in  accountList)
            {
                if (m.UserName == userName)
                {
                   string word =  getSecureString(password);
                    if (word == m.PassWord)
                    {
                        return m;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取最后登陆账户
        /// </summary>
        /// <returns></returns>
        public UserAccount GetLastAccount()
        {
            foreach(var m in accountList)
            {
                if (m.LastLogin == "yes")
                {
                    return m;
                }
            }
            return null;
        }

        /// <summary>
        /// 更新某个用户的密码
        /// </summary>
        /// <param name="userName">账户</param>
        /// <param name="pass">密码</param>
        /// <returns>成功返回true</returns>
        public bool UpdateAccountPassword(string userName, SecureString pass)
        {
            var account = getUserAccount(userName);
            if (account != null)
            {
                var str =  getSecureString(pass);
                account.passWord = str;
                return true;
            }
            else
            {
                throw new ArgumentNullException(userName + "为空");
            }
        }

        /// <summary>
        /// 修改指定账户的权限
        /// </summary>
        /// <param name="userName">修改的用户</param>
        /// <param name="level">要修改到的权限水平</param>
        /// <returns>成功修改返回true</returns>
        public bool UpdateAccountAuthority(string userName, int level)
        {
            var account = getUserAccount(userName);
            if (account == null)
            {
                throw new ArgumentNullException(userName + "为空");
            }
            else
            {
                if ((level <= 0) || (level > 4))
                {
                    throw new ArgumentOutOfRangeException("选择的等级不合规，需要在1-4之间。");
                }
                //若修改的是当前登录用户，则验证登录等级是否比当前高，若为高则放弃
                if(loginAccount.UserName == userName)
                {
                    if (loginAccount.PowerLevel >= level)
                    {
                        loginAccount.PowerLevel = level;
                        return true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("当前用户权限不足，无法进行权限变更。");
                    }
                }
                else
                {
                    //当前用户等级大于三级才能修改别的用户等级,且被修改用户的当前权限需要小于当前用户等级的权限
                    if ((loginAccount.PowerLevel >= 3) && (loginAccount.PowerLevel > account.PowerLevel))
                    {
                        if (loginAccount.PowerLevel >= level)
                        {
                            account.PowerLevel = level;
                        }
                        return true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("当前用户权限不足，无法进行权限变更。");
                    }
                }
            }
        }


        /// <summary>
        /// 修改指定账户的用户名
        /// </summary>
        /// <param name="oldName">当前用户名称</param>
        /// <param name="newName">新用户名</param>
        /// <returns>修改成功返回true</returns>
        public bool UpdateAccountUserName(string oldName,string newName)
        {
            newName = newName.Trim();
            if (newName == "")
            {
                throw new ArgumentException("用户名不能为空");
            }
            var old = getUserAccount(oldName);
            if (old == null)
            {
                throw new ArgumentNullException("当前用户名不存在");
            }
            var user = getUserAccount(newName);
            if (user == null)
            {
                old.UserName = newName;
                return true;
            }
            else
            {
                throw new ArgumentException("新用户名" + newName + "与已有账户名重复");

            }
        }

        /// <summary>
        /// 获取指定用户名的账户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>若有则返回账户信息，否则返回为空</returns>
        private UserAccount getUserAccount(string userName)
        {
            foreach (var m in accountList)
            {
                if (m.UserName == userName)
                {
                    return m;
                }

            }
            return null;
        }

        /// <summary>
        /// 检测并确认密码
        /// </summary>
        /// <param name="account">比对的账户</param>
        /// <param name="pass">密码</param>
        /// <returns>密码一致返回true，否则返回false</returns>
        public bool CheckConformPassword(UserAccount account, SecureString pass)
        {
            string passStr = getSecureString(pass);
            if (passStr == account.passWord)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测密码是否相同
        /// </summary>
        /// <returns>相同返回true</returns>
        static public bool CheckPasswordSame(SecureString passA, SecureString passB)
        {
            var strA = getSecureString(passA);
            var strB = getSecureString(passB);
            if (strA == strB)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        /// <summary>
        /// 验证密码强度是否符合要求
        /// </summary>
        /// <param name="passWord">密码</param>
        /// <returns>满足返回true</returns>
        public bool CheckPasswordComplexity(SecureString passWord)
        {
            var str = getSecureString(passWord);
            if (checkPasswordComplexity(str))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        

        /// <summary>
        /// 建立新用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="pass">密码</param>
        /// <returns>若成功测返回true，否则返回false</returns>
        public  bool EstablishNewAccount(string userName, SecureString pass, AuthorityLevel level)
        {
            userName = userName.Trim();
            if (userName == "")
            {
                throw new ArgumentException("用户名不能为空");
            }

            if (loginAccount.PowerLevel < 3)
            {
                throw new Exception("当前用户没有建立新用户的权限");
            }


            var account = getUserAccount(userName);
            if (account != null)
            {
                throw new ArgumentException("新建的用户名已经存在");
            }

            if (loginAccount.PowerLevel < (int)level)
            {
                throw new Exception("新建用户的权限超过当前用户的权限，需要不能大于当前用户权限");
            }

            var str = getSecureString(pass);

            var newUser = new UserAccount(userName, str, (int)level, "no");
            accountList.Add(newUser);
            return true;


            
        }

        /// <summary>
        /// 删除指定用户
        /// </summary>
        /// <param name="account">用户</param>
        /// <returns>成功返回true</returns>
        public bool DeleteAccount(UserAccount account)
        {
            if (loginAccount.PowerLevel < 3)
            {
                throw new Exception("当前用户没有删除用户的权限");
            }
            else
            {
                if (loginAccount == account)
                {
                    throw new Exception("不能删除自己的账户");
                }
                if (loginAccount.PowerLevel > account.PowerLevel)
                {
                    accountList.Remove(account);
                    return true;
                }
                else
                {
                    throw new Exception("不能删除比当前用户权限高的或相同的用户");
                }
            }
        }

        /// <summary>
        /// 验证密码强度是否符合要求
        /// </summary>
        /// <param name="str">密码字符串</param>
        /// <returns>满足返回true</returns>
        private bool checkPasswordComplexity(string str)
        {
            var regex = new Regex(@"
                     (?=.*[0-9])                     
                    (?=.*[a-zA-Z])                  
                    (?=([\x21-\x7e]+)[^a-zA-Z0-9])  
                    .{8,30}", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            return regex.IsMatch(str);
        }

        /// <summary>
        /// 获取安全字符串内容
        /// </summary>
        /// <param name="ss">安全字符串</param>
        /// <returns>普通字符串</returns>
       static  private string getSecureString(SecureString ss)
        {
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(ss);

            try
            {

                return System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);

            }

            finally
            {

                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);

            }
        }


    }
}
