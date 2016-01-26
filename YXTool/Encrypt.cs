using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesTool
{
    public class Encrypt
    {
        /// <summary>
        ///  通过用户名加密密码
        /// </summary>
        /// <param name="pwd">密码</param>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static string GetEncryptPwd(string pwd, string loginname)
        {
            var code = "Sj2yF98jUhg8874G";
            for (int i = 0; i < code.Length; i++)
            {
                if (i % 3 == 1)
                {
                    pwd = code[i] + pwd;
                }
            }
            for (int i = 0; i < code.Length; i++)
            {
                if (i % 2 == 0)
                {
                    pwd += code[i];
                }
            }
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "MD5");//System.Web.Configuration.FormsAuthPasswordFormat;
        }
    }
}
