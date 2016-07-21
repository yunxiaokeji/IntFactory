using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WeiXin.Sdk
{
    public class AppConfig
    {
        public static string WeiXinApiUrl = ConfigurationManager.AppSettings["WeiXinApiUrl"] ?? "https://api.weixin.qq.com";
        public static string AppKey = ConfigurationManager.AppSettings["WeiXinAppKey"] ?? "wxa416ba063af66180";
        public static string AppSecret = ConfigurationManager.AppSettings["WeiXinAppSecret"] ?? "50db82ecf9bc0ad52872a0d39c3f9349";
        public static string CallBackUrl = ConfigurationManager.AppSettings["WeiXinCallBackUrl"] ?? "localhost:9999/User/WeiXinCallBack";
    }
}
