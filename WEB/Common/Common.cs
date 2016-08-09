using Qiniu.Conf;
using Qiniu.IO;
using Qiniu.RS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace YXERP.Common
{
    public class Common
    {
        public static string Version = "2016-7-19";
        //云销客户端的ClientID、AgentID
        public static string YXClientID = System.Configuration.ConfigurationManager.AppSettings["YXClientID"] ?? string.Empty;
        public static string YXAgentID = System.Configuration.ConfigurationManager.AppSettings["YXAgentID"] ?? string.Empty;

        //支付宝对接页面
        public static string AlipaySuccessPage = System.Configuration.ConfigurationManager.AppSettings["AlipaySuccessPage"] ?? string.Empty;
        public static string AlipayNotifyPage = System.Configuration.ConfigurationManager.AppSettings["AlipayNotifyPage"] ?? string.Empty;

       /// <summary>
       /// 获取请求方ip
       /// </summary>
       /// <param name="request"></param>
       /// <returns></returns>
        public static string GetRequestIP()
        {
            return string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers.Get("X-Real-IP")) ? System.Web.HttpContext.Current.Request.UserHostAddress : System.Web.HttpContext.Current.Request.Headers["X-Real-IP"];
        }

        /// <summary>
        /// 写支付宝文本日志
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool WriteAlipayLog(string content) {
            try
            {
                string path = HttpContext.Current.Server.MapPath(@"C:\WebLog\Alipay");
                //string path = HttpContext.Current.Server.MapPath("~/Log/Alipay");
                if (!Directory.Exists(path))//判断是否有该文件  
                    Directory.CreateDirectory(path);
                string logFileName = path + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";//生成日志文件  
                if (!File.Exists(logFileName))//判断日志文件是否为当天  
                    File.Create(logFileName);//创建文件  
                StreamWriter writer = File.AppendText(logFileName);//文件中添加文件流  
                writer.WriteLine(DateTime.Now.ToString() + " " + content);
                writer.Flush();
                writer.Dispose();
                writer.Close();
                
                return true;
            }
            catch
            {
                return false;
            };
        }

        /// <summary>
        /// 存入手机验证码会话
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="code"></param>
        public static void SetCodeSession(string mobilePhone, string code) 
        {
            HttpContext.Current.Session[mobilePhone] = code;
        }

        /// <summary>
        /// 验证手机验证码
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool ValidateMobilePhoneCode(string mobilePhone, string code)
        {
            if (HttpContext.Current.Session[mobilePhone] != null)
            {
              return  HttpContext.Current.Session[mobilePhone].ToString() == code;
            }

            return false;
        }

        /// <summary>
        /// 清除手机验证码会话
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static void ClearMobilePhoneCode(string mobilePhone)
        {
            if (HttpContext.Current.Session[mobilePhone] != null)
            {
                 HttpContext.Current.Session.Remove(mobilePhone);
            }
        }

        /// <summary>
        /// 获取今天是星期几
        /// </summary>
        /// <param name="msg">显示周日还是星期日</param>
        /// <returns></returns>
        public static string Week(string msg, int day)
        {
            if (string.IsNullOrEmpty(msg)) {
                msg = "星期";
            }
            string[] weekdays = { "" + msg + "日", "" + msg + "一", "" + msg + "二", "" + msg + "三", "" + msg + "四", "" + msg + "五", "" + msg + "六" };
            string week = weekdays[day];
            return week;
        }



        public static string UploadAttachment(string filepath, string files = "orders")
        {
            string allFilePath = "";
            Config.Init();
            IOClient target = new IOClient();
            PutExtra extra = new PutExtra();
            //设置上传的空间
            string bucket = System.Configuration.ConfigurationManager.AppSettings["QN-Bucket"] ?? "zngc-intfactory";
            string url = bucket == "zngc-intfactory" ? "o9h6bx3r4.bkt.clouddn.com" : "o9vwxv40j.bkt.clouddn.com";
            //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
            PutPolicy put = new PutPolicy(bucket, 3600);

            //调用Token()方法生成上传的Token
            string upToken = put.Token();
            //上传文件的路径
            if (!string.IsNullOrEmpty(filepath))
            {
                string[] filepaths=filepath.Split(',');
                foreach (string file in filepaths)
                {
                    if (!string.IsNullOrEmpty(file))
                    { 
                        var fileExtension = file.Substring(file.LastIndexOf(".") + 1).ToLower();
                        var key = files + (DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "/") + GetTimeStamp() + "." + fileExtension;
                  
                        //调用PutFile()方法上传
                        PutRet ret = target.PutFile(upToken, key, file, extra);
                        if (ret.OK)
                        {
                            allFilePath += url+ret.key+",";
                        }
                    }  
                }
            }
            return allFilePath.TrimEnd(',');
        }
        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        } 
        #region 缓存

        #region 用户登录密码错误缓存
        private static Dictionary<string, PwdErrorUserEntity> _cachePwdErrorUsers;
        public static Dictionary<string, PwdErrorUserEntity> CachePwdErrorUsers
        {
            set { _cachePwdErrorUsers = value; }

            get { 

                if(_cachePwdErrorUsers==null)
                {
                    _cachePwdErrorUsers= new Dictionary<string, PwdErrorUserEntity>();
                }

                return _cachePwdErrorUsers;
            }
        }

        #endregion


        #endregion
    }

    public class PwdErrorUserEntity
    {
        public int ErrorCount;
        public DateTime ForbidTime;
    }
}