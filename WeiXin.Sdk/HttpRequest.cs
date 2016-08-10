using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeiXin.Sdk
{
    public class HttpRequest
    {
        public static string RequestServer(ApiOption apiOption, Dictionary<string, object> paras, RequestType requestType = RequestType.Get)
        {
            string urlPath = GetEnumDesc<ApiOption>(apiOption);
            string url = AppConfig.WeiXinApiUrl + urlPath;

            string paraStr = string.Empty;
            if (paras != null && paras.Count > 0)
            {
                foreach (var key in paras.Keys) {
                    paraStr += key+"="+paras[key] +"&";
                }
                
            }

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


            return strResult;

        }

        public static string SendPush(string url,string paras)
        {
            string strResult = string.Empty;
            try
            {
                byte[] postData = Encoding.UTF8.GetBytes(paras);
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
