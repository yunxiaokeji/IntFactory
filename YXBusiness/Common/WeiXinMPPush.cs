using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;
using System.IO;
using System.Configuration;

namespace IntFactoryBusiness
{
    public class WeiXinMPPush
    {
        public static WeiXinMPPush BasePush = new WeiXinMPPush();
        public string WeiXinMPPushUrl = "https://api.weixin.qq.com/cgi-bin/message/template/send";
        public static string WeiXinMPApiUrl = ConfigurationManager.AppSettings["WeiXinMPApiUrl"] ?? "https://api.weixin.qq.com";
        public static string WeiXinMPAppKey = ConfigurationManager.AppSettings["WeiXinMPAppKey"] ?? "wxa69a4127f24be469";
        public static string WeiXinMPAppSecret = ConfigurationManager.AppSettings["WeiXinMPAppSecret"] ?? "28e5342f879a312ff666f0e3b23f6a78";
        //任务完成给它下级任务推送通知
        public  void SendTaskFinishPush(string openid,string preTitle, string title, string onwerName)
        {
            string content = GetPushContent(WeiXinMPPushType.SendTaskFinishPush, openid, preTitle,
                title, onwerName);

            SendPush(content);
        }

        //获取推送内容
        public string GetPushContent(WeiXinMPPushType pushType, string openid, string preTitle, string title, string onwerName)
        {
            string[] template_ids = new string[] { "IJx0o_kXBfNhTmCkQrsZ42MSON0MX9wfyGUTLoVYZfQ", 
                "9wgmCkkS2fxIKsPIB2hEYEJcmYkfEiPZVZKokNrWtzs","sb_DkErldHIMGSLH0ta-ca0o43j5GluUL1mOmYbWPAc" };
            string color = "#173177";
            string first = string.Empty;
            string keyword1 = string.Empty;
            string keyword2 = string.Empty;
            string keyword3 = string.Empty;
            string remark = string.Empty;
            Dictionary<string, object> parasObj = new Dictionary<string, object>();
            Dictionary<string, object> dataObj = new Dictionary<string, object>();
            Dictionary<string, object> firstObj = new Dictionary<string, object>() { };
            Dictionary<string, object> keyword1Obj = new Dictionary<string, object>() { };
            Dictionary<string, object> keyword2Obj = new Dictionary<string, object>() { };
            Dictionary<string, object> keyword3Obj = new Dictionary<string, object>() { };
            Dictionary<string, object> remarkObj = new Dictionary<string, object>() { };

            first = "您好，您有一个上级任务" + preTitle + "已提交完成！";
            firstObj.Add("value", first);
            firstObj.Add("color", color);

            keyword1 = title;
            keyword1Obj.Add("value", title);
            keyword1Obj.Add("color", color);

            keyword2 = onwerName;
            keyword2Obj.Add("value", keyword2);
            keyword2Obj.Add("color", color);

            keyword3=DateTime.Now.ToString("yyyy-MM-dd hh:mm");
            keyword3Obj.Add("value", keyword3);
            keyword3Obj.Add("color", color);

            remark = "请按时完成任务！";
            remarkObj.Add("value", remark);
            remarkObj.Add("color", color);

            parasObj.Add("touser", openid);
            parasObj.Add("template_id", template_ids[(int)pushType]);
            dataObj.Add("first", firstObj);
            dataObj.Add("keyword1", keyword1Obj);
            dataObj.Add("keyword2", keyword2Obj);
            dataObj.Add("keyword3", keyword3Obj);
            dataObj.Add("remark", remarkObj);
            parasObj.Add("data", dataObj);

            return JsonConvert.SerializeObject(parasObj);
        }

        //推送消息
        public  string SendPush(string content)
        {
            string token = GetWeiXinMPToken();
            string url = WeiXinMPPushUrl + "?access_token=" + token;

            return HttPost(url, content);
        }

        //获取微信公众号token
        public string  GetWeiXinMPToken() {
            string url = string.Format("{0}/cgi-bin/token?grant_type={1}&appid={2}&secret={3}", WeiXinMPApiUrl,
                "client_credential", WeiXinMPAppKey, WeiXinMPAppSecret);
            string resultStr = HttGet(url);
            WeiXinMPToken token =JsonConvert.DeserializeObject<WeiXinMPToken>(resultStr);

            return token.access_token;
        }

        //发起后台请求
        public string HttPost(string url, string content) {
            string strResult = string.Empty;
            try
            {
                byte[] postData = Encoding.UTF8.GetBytes(content);
                Uri uri = new Uri(url);
                HttpWebRequest httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;

                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = false;
                httpWebRequest.AllowAutoRedirect = true;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.UserAgent = "Ocean/NET-SDKClient";
                httpWebRequest.ContentLength = postData.Length;

                System.IO.Stream outputStream = httpWebRequest.GetRequestStream();
                outputStream.Write(postData, 0, postData.Length);
                outputStream.Close();
                HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
                Stream responseStream = response.GetResponseStream();

                System.Text.Encoding encode = Encoding.UTF8;
                StreamReader reader = new StreamReader(response.GetResponseStream(), encode);
                strResult = reader.ReadToEnd();

                reader.Close();
                response.Close();
            }
            catch (System.Net.WebException webException)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                strResult = reader.ReadToEnd();

                reader.Close();
                response.Close();
            }

            return strResult;
        }

        public string HttGet(string url)
        {
            string strResult = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;

                httpWebRequest.Method = "GET";
                httpWebRequest.KeepAlive = false;
                httpWebRequest.AllowAutoRedirect = true;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.UserAgent = "Ocean/NET-SDKClient";

                HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
                Stream responseStream = response.GetResponseStream();
                System.Text.Encoding encode = Encoding.UTF8;
                StreamReader reader = new StreamReader(response.GetResponseStream(), encode);
                strResult = reader.ReadToEnd();

                reader.Close();
                response.Close();
            }
            catch (System.Net.WebException webException)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                strResult = reader.ReadToEnd();

                reader.Close();
                response.Close();
            }

            return strResult;
        }
    }

    //推送消息类型
    public enum WeiXinMPPushType {
        SendTaskFinishPush=0,

        SendNewTaskPush=1,

        SendNewOrderPush=2
    }

    //微信公众号token实体
    public class WeiXinMPToken {
        public string access_token { get; set; }

        public long expires_in { get; set; }

        public string errcode { get; set; }
    }
}
