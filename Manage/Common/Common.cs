using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace YXManage.Common
{
    public enum RequestType
    {
        Get = 1,
        Post = 2
    }

    public class Common
    {
        //七牛云存储
        public static string QNBucket = System.Configuration.ConfigurationManager.AppSettings["QN-Bucket"] ?? "zngc-intfactory";
        public static string QNDomianUrl = System.Configuration.ConfigurationManager.AppSettings["QNDomianUrl"] ?? "http://o9h6bx3r4.bkt.clouddn.com/";

        public static string RequestServer(string path,Dictionary<string, object> paras=null, RequestType requestType = RequestType.Get)
        {
            string url = ConfigurationManager.AppSettings["IntFactoryUrl"] ?? "http://localhost:9999";
            url += path;
            string paraStr = string.Empty;

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
    }
}