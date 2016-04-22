using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Web;
namespace AlibabaSdk
{
    public class HttpRequest
    {
        public static string RequestServer(ApiOption apiOption, Dictionary<string, object> paras,RequestType requestType = RequestType.Get)
        {
            string urlPath = "param2/1/cn.alibaba.open/" + GetEnumDesc<ApiOption>(apiOption) + "/" + AppConfig.AppKey;
            string url = AppConfig.AlibabaApiUrl + "/openapi/" + urlPath;
            //string url = AppConfig.AlibabaApiUrl + "/api/" + urlPath;
            string paraStr = string.Empty;

            if (apiOption == ApiOption.getToken)
            {
                urlPath = "/openapi/http/1/system.oauth2/getToken/" + AppConfig.AppKey;
                url = AppConfig.AlibabaApiUrl + urlPath;
            }
            else
            {
                string signature = sign(urlPath, paras);
                paraStr = "_aop_signature=" + signature;
            }
            
            if (paras != null && paras.Count > 0)
                paraStr += "&" + CreateParameterStr(paras);

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

        /// <summary>
        /// 获取参数签名算法
        /// </summary>
        /// <param name="paramDic">请求参数，即queryString + request body 中的所有参数</param>
        public static string sign(Dictionary<string, string> paramDic)
        {
            byte[] signatureKey = Encoding.UTF8.GetBytes(AppConfig.AppSecret);
            //第一步：拼装key+value
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> kv in paramDic)
            {
                list.Add(kv.Key + kv.Value);
            }
            //第二步：排序
            list.Sort();
            //第三步：拼装排序后的各个字符串
            string tmp = "";
            foreach (string kvstr in list)
            {
                tmp = tmp + kvstr;
            }
            //第四步：将拼装后的字符串和app密钥一起计算签名
            //HMAC-SHA1
            HMACSHA1 hmacsha1 = new HMACSHA1(signatureKey);
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(tmp));
            byte[] hash = hmacsha1.Hash;
            //TO HEX
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
        }

        /// <summary>
        /// 获取API签名算法
        /// </summary>
        /// <param name="urlPath">基础url部分，格式为protocol/apiVersion/namespace/apiName/appKey，如 json/1/system/currentTime/1；如果为客户端授权时此参数置为空串""</param>
        /// <param name="paramDic">请求参数，即queryString + request body 中的所有参数</param>
        public static string sign(string urlPath, Dictionary<string, object> paramDic)
        {
            byte[] signatureKey = Encoding.UTF8.GetBytes(AppConfig.AppSecret);//此处用自己的签名密钥
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, object> kv in paramDic)
            {
                list.Add(kv.Key + kv.Value.ToString());
            }
            list.Sort();
            string tmp = urlPath;
            foreach (string kvstr in list)
            {
                tmp = tmp + kvstr;
            }

            //HMAC-SHA1
            HMACSHA1 hmacsha1 = new HMACSHA1(signatureKey);
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(tmp));
            /*
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(urlPath));
            foreach (string kvstr in list)
            {
                hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(kvstr));
            }
             */
            byte[] hash = hmacsha1.Hash;
            //TO HEX
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
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
