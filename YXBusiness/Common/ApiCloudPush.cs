using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace IntFactoryBusiness
{
    public class ApiCloudPush
    {
        private string AppId = "A6925589056031";
        private string AppKey = "97A4C2DD-D654-ACE3-CF21-5D8874D2ACF4";
        private static string UrlBase = "https://p.apicloud.com/api/push";
        public static ApiCloudPush BasePush = new ApiCloudPush();

        public string SendPush(string content, string userIds,
            ApiCloudPushModuleType moduleType = ApiCloudPushModuleType.Task,
            ApiCloudPushMessageType messageType = ApiCloudPushMessageType.Message,
            ApiCloudPushPlatform platform = ApiCloudPushPlatform.Android)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("title", GetEnumDesc<ApiCloudPushModuleType>(moduleType));
            paras.Add("content", content);
            paras.Add("type", (int)messageType);
            paras.Add("platform", (int)platform);
            paras.Add("userIds", userIds);
            
            return Push(JsonConvert.SerializeObject(paras));
        }

        public string Push(string jsonStr)
        {
            var Url = UrlBase + "/message";
            byte[] Data = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(jsonStr);
            return Ajax(Url, Data, "POST");
        }

        private string X_APICloud_AppKey
        {
            get
            {
                long amp = (long)(DateTime.Now - new DateTime(1970, 01, 01)).TotalMilliseconds;

                String value = String.Format("{0}UZ{1}UZ{2}", AppId, AppKey, amp);

                byte[] buffer = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(value));

                StringBuilder builder = new StringBuilder();

                foreach (byte num in buffer)
                {
                    builder.AppendFormat("{0:x2}", num);
                }
                return builder.ToString() + "." + amp;
            }
        }

        private string Ajax(string url, byte[] data, string method)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("X-APICloud-AppId", AppId);
                webClient.Headers.Add("X-APICloud-AppKey", X_APICloud_AppKey);
                webClient.Headers.Add("Content-type", "application/json;charset=UTF-8");
                string ResponseData;
                if (data != null)
                {
                    var responseData = webClient.UploadData(url, method, data);
                    ResponseData = System.Text.Encoding.GetEncoding("UTF-8").GetString(responseData);
                }
                else
                {
                    ResponseData = webClient.DownloadString(url);
                }

                return ResponseData;
            }
            catch (WebException e)
            {
                return "{ \"Error\":{ \"msg\": \"" + e.Message + "\"}}";
            }
        }

        private string GetEnumDesc<T>(T Enumtype)
        {
            if (Enumtype == null) throw new ArgumentNullException("Enumtype");
            if (!Enumtype.GetType().IsEnum) throw new Exception("参数类型不正确");
            return ((System.ComponentModel.DescriptionAttribute)Enumtype.GetType().GetField(Enumtype.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description;
        }
    }

    public enum ApiCloudPushModuleType
    {
        [DescriptionAttribute("系统通知")]
        System = 0,
        [DescriptionAttribute("任务通知")]
        Task = 1,
        [DescriptionAttribute("订单通知")]
        Order = 2
    }

    public enum ApiCloudPushMessageType
    {
        [DescriptionAttribute("所有")]
        All = 0,
        [DescriptionAttribute("消息")]
        Message = 1,
        [DescriptionAttribute("通知")]
        Notice = 2
    }

    public enum ApiCloudPushPlatform
    {
        [DescriptionAttribute("所有")]
        All = 0,
        [DescriptionAttribute("ios")]
        Ios = 1,
        [DescriptionAttribute("android")]
        Android = 2
    }

}
