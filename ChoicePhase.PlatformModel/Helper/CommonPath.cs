using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChoicePhase.PlatformModel.Helper
{
    /// <summary>
    /// 通用的公共路径,  
    /// TODO：需要检查每个路径
    /// </summary>
    public class CommonPath
    {
        /// <summary>
        /// 账户xml路径
        /// </summary>
        public static string AccountXmlPath = @"Config\Account\UserAccount.xml";
        // string pathxmlAccount = @"t2.xml";

        /// <summary>
        /// 账户xsd路径
        /// </summary>
        public static string AccountXsdPath = @"Config\Account\UserAccount.xsd";

        /// <summary>
        /// 日志文件夹路径
        /// </summary>
        public static string LogDirectoryPath = @"log\log";


        /// <summary>
        /// 串口XML表格路径
        /// </summary>
        public static string CommonPortXmlPath = @"Config\XML\Config.xml";

        /// <summary>
        /// 串口XSD架构路径 User
        /// </summary>
        public static string UserXsdPath = @"Config\XSD\User.xsd";

        /// <summary>
        /// 串口XML表格路径 user
        /// </summary>
        public static string UserXmlPath = @"Config\XML\User.xml";

        /// <summary>
        /// 串口XSD架构路径
        /// </summary>
        public static string CommonPortXsdPath = @"Config\XSD\Config.xsd";

        /// <summary>
        /// 永磁错误代码XML表格路径
        /// </summary>
        public static string YongciErrorCodeXmlPath = @"Config\XML\YongciErrorCode.xml";

        /// <summary>
        /// 同步控制器错误代码XML表格路径
        /// </summary>
        public static string TongbuErrorCodeXmlPath = @"Config\XML\TongbuErrorCode.xml";

        /// <summary>
        /// 错误代码XSD架构路径
        /// </summary>
        public static string ErrorCodeXsdPath = @"Config\XSD\ErrorCode.xsd";
       
        
        public static string DataBase =  @"config\Database\Das.db";
    }
}
