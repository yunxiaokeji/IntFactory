using CloudSales.Sdk.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CloudSales.Sdk.Http
{

    public class HttpRequest
    {
        public static T RequestServer<T>(ApiOption apiOption, Dictionary<string, object> paras, RequestType requestType = RequestType.Post)
        {
            string urlPath = GetEnumDesc<ApiOption>(apiOption);
            string url = AppConfig.ApiUrl + urlPath;
            string paraStr = string.Empty;
            string userID = string.Empty;
            if (paras.ContainsKey("userID"))
            {
                userID = paras["userID"].ToString();
            }
            else
            {
                userID = "BC6802E9-285C-471C-8172-3867C87803E2";
                paras.Add("userID", userID);
            }

            if (paras != null && paras.Count > 0)
            {
                paraStr += CreateParameterStr(paras);
            }
            //签名认证
            string signature = Signature.GetSignature(AppConfig.AppKey, AppConfig.AppSecret, userID);
            paraStr += "signature=" + signature;

            string strResult = string.Empty;
            try
            {
                if (requestType == RequestType.Get)
                {
                    url += "?" + paraStr;
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
                else
                {
                    byte[] postData = Encoding.UTF8.GetBytes(paraStr);
                    Uri uri = new Uri(url);
                    HttpWebRequest httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;

                    httpWebRequest.Method = "POST";
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.AllowAutoRedirect = true;
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
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



            return JsonConvert.DeserializeObject<T>(strResult);

        }

        private static String CreateParameterStr(Dictionary<String, Object> parameters)
        {
            StringBuilder paramBuilder = new StringBuilder();
            foreach (KeyValuePair<string, object> kvp in parameters)
            {
                String encodedValue = null;
                if (kvp.Value != null)
                {
                    String tempValue = kvp.Value.ToString();
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(tempValue);
                    encodedValue = System.Web.HttpUtility.UrlEncode(byteArray, 0, byteArray.Length);
                }
                paramBuilder.Append(kvp.Key).Append("=").Append(encodedValue);
                paramBuilder.Append("&");
            }
            return paramBuilder.ToString();
        }

        public static string GetEnumDesc<T>(T Enumtype)
        {
            if (Enumtype == null) throw new ArgumentNullException("Enumtype");
            if (!Enumtype.GetType().IsEnum) throw new Exception("参数类型不正确");
            return ((DescriptionAttribute)Enumtype.GetType().GetField(Enumtype.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description;
        }
    }


    public enum RequestType
    {
        Get = 1,
        Post = 2
    }

}
