using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.ComponentModel;
namespace AlibabaSdk
{
    public class HttpRequest
    {
        public static string RequestServer(ApiOption apiOption, Dictionary<string, object> paras,RequestType requestType = RequestType.Get)
        {
            ;
            string urlPath = "param2/1/cn.alibaba.open/" + GetEnumDesc<ApiOption>(apiOption) + "/" + AppConfig.AppKey;
            //string url = AppConfig.AlibabaApiUrl + "/openapi/" + urlPath;
            string url = AppConfig.AlibabaApiUrl + "/api/" + urlPath;
            string paraStr = string.Empty;

            if (apiOption == ApiOption.accessToken)
            {
                urlPath = "/openapi/http/1/system.oauth2/getToken/" + AppConfig.AppKey;
                url = AppConfig.AlibabaApiUrl + urlPath;
            }
            else
            {
                string signature = sign(urlPath, paras);
                paraStr = "_aop_signature=" + signature;
            }
            
            try
            {
                
                if (paras != null && paras.Count > 0)
                {
                    foreach (string key in paras.Keys)
                    {
                        if (string.IsNullOrEmpty(paraStr))
                            paraStr = key + "=" + paras[key];
                        else
                            paraStr += "&" + key + "=" + paras[key];
                    }

                }



                HttpWebRequest request;
                HttpWebResponse response;
                string strResult = string.Empty;
                if (requestType == RequestType.Get)
                {
                    url += "?" + paraStr;
                    request = (System.Net.HttpWebRequest)WebRequest.Create(url);
                    request.Timeout = 10000;
                    request.Method = "GET";
                    request.ContentType = "application/x-www-form-urlencoded";

                    response = (HttpWebResponse)request.GetResponse();
                    System.Text.Encoding encode = Encoding.ASCII;
                    if (response.CharacterSet.Contains("utf-8"))
                        encode = Encoding.UTF8;
                    StreamReader reader = new StreamReader(response.GetResponseStream(), encode);
                    strResult = reader.ReadToEnd();

                    reader.Close();
                    response.Close();
                }
                else
                {
                    byte[] bData = Encoding.UTF8.GetBytes(paraStr.ToString());

                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.Timeout = 5000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = bData.Length;

                    System.IO.Stream smWrite = request.GetRequestStream();
                    smWrite.Write(bData, 0, bData.Length);
                    smWrite.Close();

                    response = (HttpWebResponse)request.GetResponse();
                    System.IO.Stream dataStream = response.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(dataStream, Encoding.UTF8);
                    strResult = reader.ReadToEnd();

                    reader.Close();
                    dataStream.Close();
                    response.Close(); 
                }

                return strResult;
            }
            catch { }

            return null;
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
