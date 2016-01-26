using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace AlibabaSdk
{
    public class AppConfig
    {
        public static string AlibabaApiUrl = ConfigurationManager.AppSettings["AlibabaApiUrl"] ?? "http://gw.open.1688.com/";
        public static string AppKey = ConfigurationManager.AppSettings["AppKey"] ?? "1023561";
        public static string AppSecret = ConfigurationManager.AppSettings["AppSecret"] ?? "0DGxbxYXFM";
        public static string CallBackUrl = ConfigurationManager.AppSettings["CallBackUrl"] ?? "http://gyl.movedbuy.com";
    }
}
