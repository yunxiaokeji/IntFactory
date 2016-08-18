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
using IntFactoryEntity.Task;
using IntFactoryEnum;

namespace IntFactoryBusiness
{
    public class WeiXinMPPush
    {
        public static WeiXinMPPush BasePush = new WeiXinMPPush();
        public string WeiXinMPPushUrl = "https://api.weixin.qq.com/cgi-bin/message/template/send";
        public static string WeiXinMPApiUrl = ConfigurationManager.AppSettings["WeiXinMPApiUrl"] ?? "https://api.weixin.qq.com";
        public static string WeiXinMPAppKey = ConfigurationManager.AppSettings["WeiXinMPAppKey"] ?? "wxa69a4127f24be469";
        public static string WeiXinMPAppSecret = ConfigurationManager.AppSettings["WeiXinMPAppSecret"] ?? "28e5342f879a312ff666f0e3b23f6a78";
        public static string WeiXinMPToken = string.Empty;
        public static DateTime WeiXinMPTokenExpiresTime =DateTime.Now;

        //任务完成给它下级任务推送通知
        public async void SendTaskFinishPush(string taskid)
        {
            await SendWeiXinMPPush(WeiXinMPPushType.SendTaskFinishPush, taskid);
        }

        //订单分解新任务推送通知
        public async void SendNewTasksPush(string orderid)
        {
            await SendWeiXinMPPush(WeiXinMPPushType.SendNewTaskPush, orderid);
        }

        //订单更换负责人推送通知
        public async void SendChangeOrderOwnerPush(string orderid)
        {
            await SendWeiXinMPPush(WeiXinMPPushType.SendChangeOrderOwnerPush, orderid);
        }

        //任务更换负责人推送通知
        public async void SendChangeTaskOwnerPush(string taskid)
        {
            await SendWeiXinMPPush(WeiXinMPPushType.SendChangeTaskOwnerPush, taskid);
        }

        public Task<bool> SendWeiXinMPPush(WeiXinMPPushType pushType, string guid)
        {
            string result = string.Empty;
            if (pushType == WeiXinMPPushType.SendTaskFinishPush)
            {
                TaskEntity task = TaskBusiness.GetPushTaskForFinishTask(guid);
                if (!string.IsNullOrEmpty( task.OpenID))
                {
                    string content = GetPushContent(pushType, task.OpenID, task.Title, task.PreTitle, task.Owner.Name);
                    result=SendPush(content);   
                }
                if (task.Order != null)
                {
                    var order = task.Order;
                    pushType = WeiXinMPPushType.SendTasksFinishPush;
                    string content = GetPushContent(pushType, order.OpenID, order.OrderCode, string.Empty, CommonBusiness.GetEnumDesc((EnumOrderStageStatus)order.Status));
                    result = SendPush(content);
                }
            }
            else if (pushType == WeiXinMPPushType.SendNewTaskPush)
            {
                List<TaskEntity> tasks = TaskBusiness.GetPushTasksForNewOrder(guid);
                if (tasks.Count > 0)
                {
                    foreach (var task in tasks)
                    {
                        string content = GetPushContent(pushType, task.OpenID, task.Title, "", "", task.EndTime);
                        result=SendPush(content);
                    }
                }
            }
            else if (pushType == WeiXinMPPushType.SendChangeOrderOwnerPush)
            {
                TaskEntity task = TaskBusiness.GetPushTaskForChangeOrderOwner(guid);
                if (task != null)
                {
                    string content = GetPushContent(pushType, task.OpenID, task.Title, "", "", task.EndTime);
                    result=SendPush(content);
                }
            }
            else if (pushType == WeiXinMPPushType.SendChangeTaskOwnerPush)
            {
                TaskEntity task = TaskBusiness.GetPushTaskForChangeTaskOwner(guid);
                if (task != null)
                {
                    string content = GetPushContent(pushType, task.OpenID, task.Title, "", "", task.EndTime);
                    result = SendPush(content);
                }
            }

            return Task.Run(() => { return !string.IsNullOrEmpty(result); });
        }

        //获取推送内容
        public string GetPushContent(WeiXinMPPushType pushType, string openid, string title, string preTitle="", string onwerName="",DateTime? endTime=null)
        {
            string[] template_ids = new string[] { "IJx0o_kXBfNhTmCkQrsZ42MSON0MX9wfyGUTLoVYZfQ", 
                "9wgmCkkS2fxIKsPIB2hEYEJcmYkfEiPZVZKokNrWtzs",
                "sb_DkErldHIMGSLH0ta-ca0o43j5GluUL1mOmYbWPAc",
                 "9wgmCkkS2fxIKsPIB2hEYEJcmYkfEiPZVZKokNrWtzs",
                "_QG1QkmjxIS80sbrMBhrW9gnPoM52LgJ2BmmEkrqYuo"};
            Dictionary<string, object> parasObj = new Dictionary<string, object>();
            Dictionary<string, object> dataObj = new Dictionary<string, object>();
            Dictionary<string, object> firstObj = new Dictionary<string, object>() { };
            Dictionary<string, object> keyword1Obj = new Dictionary<string, object>() { };
            Dictionary<string, object> keyword2Obj = new Dictionary<string, object>() { };
            Dictionary<string, object> keyword3Obj = new Dictionary<string, object>() { };
            Dictionary<string, object> remarkObj = new Dictionary<string, object>() { };

            string color = "#173177";
            string first = "您好，您有一个上级任务" + preTitle + "已提交完成！";
            string keyword1 = title;
            string keyword2 = onwerName;
            string keyword3 = DateTime.Now.ToString("yyyy-MM-dd hh:mm");
            string remark = "请按时完成任务！";

            if (pushType == WeiXinMPPushType.SendNewTaskPush || pushType == WeiXinMPPushType.SendChangeTaskOwnerPush) {
                first = "您收到了一个新任务！";
                if (pushType == WeiXinMPPushType.SendChangeTaskOwnerPush)
                {
                    first = "有一个任务将您设为负责人！";
                }
                keyword2 = endTime.Value.ToString("yyyy-MM-dd hh:mm");
                remark = "请按时完成任务！";
            }
            else if (pushType == WeiXinMPPushType.SendChangeOrderOwnerPush)
            {
                first = "有一个订单将您设为负责人！";
                keyword2 = endTime.Value.ToString("yyyy-MM-dd hh:mm");
                remark = "请尽快处理！";
            }
            else if (pushType == WeiXinMPPushType.SendTasksFinishPush)
            {
                first = "您有一个订单的所有任务已完成！";
                remark = "请确认！";
            }
            firstObj.Add("value", first);
            firstObj.Add("color", color);
            keyword1Obj.Add("value", title);
            keyword1Obj.Add("color", color);
            keyword2Obj.Add("value", keyword2);
            keyword2Obj.Add("color", color);
            keyword3Obj.Add("value", keyword3);
            keyword3Obj.Add("color", color);
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
        public string  GetWeiXinMPToken() 
        {
            if (string.IsNullOrEmpty(WeiXinMPToken) || WeiXinMPTokenExpiresTime > DateTime.Now)
            {
                string url = string.Format("{0}/cgi-bin/token?grant_type={1}&appid={2}&secret={3}", WeiXinMPApiUrl,
                    "client_credential", WeiXinMPAppKey, WeiXinMPAppSecret);
                string resultStr = HttGet(url);
                WeiXinMPToken token = JsonConvert.DeserializeObject<WeiXinMPToken>(resultStr);
                if (!string.IsNullOrEmpty(token.access_token))
                {
                    WeiXinMPTokenExpiresTime = DateTime.Now.AddMinutes(110);
                    WeiXinMPToken = token.access_token;
                }
            }

            return WeiXinMPToken;
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

        SendChangeOrderOwnerPush = 2,

        SendChangeTaskOwnerPush = 3,

        SendTasksFinishPush = 4
    }

    //微信公众号token实体
    public class WeiXinMPToken {
        public string access_token { get; set; }

        public long expires_in { get; set; }

        public string errcode { get; set; }
    }
}
