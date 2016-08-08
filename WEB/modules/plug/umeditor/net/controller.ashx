<%@ WebHandler Language="C#" Class="UEditorHandler" %>

using System;
using System.Web;
using System.IO;
using System.Collections;
using Newtonsoft.Json;

public class UEditorHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        Handler action = null;
        switch (context.Request["action"])
        {
            case "config":
                action = new ConfigHandler(context);
                break;
            case "uploadimage":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = UeditorConfig.GetStringList("imageAllowFiles"),
                    PathFormat = UeditorConfig.GetString("imagePathFormat"),
                    SizeLimit = UeditorConfig.GetInt("imageMaxSize"),
                    UploadFieldName = UeditorConfig.GetString("imageFieldName")
                });
                break;
            case "uploadscrawl":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = new string[] { ".png" },
                    PathFormat = UeditorConfig.GetString("scrawlPathFormat"),
                    SizeLimit = UeditorConfig.GetInt("scrawlMaxSize"),
                    UploadFieldName = UeditorConfig.GetString("scrawlFieldName"),
                    Base64 = true,
                    Base64Filename = "scrawl.png"
                });
                break;
            case "uploadvideo":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = UeditorConfig.GetStringList("videoAllowFiles"),
                    PathFormat = UeditorConfig.GetString("videoPathFormat"),
                    SizeLimit = UeditorConfig.GetInt("videoMaxSize"),
                    UploadFieldName = UeditorConfig.GetString("videoFieldName")
                });
                break;
            case "uploadfile":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = UeditorConfig.GetStringList("fileAllowFiles"),
                    PathFormat = UeditorConfig.GetString("filePathFormat"),
                    SizeLimit = UeditorConfig.GetInt("fileMaxSize"),
                    UploadFieldName = UeditorConfig.GetString("fileFieldName")
                });
                break;
            case "listimage":
                action = new ListFileManager(context, UeditorConfig.GetString("imageManagerListPath"), UeditorConfig.GetStringList("imageManagerAllowFiles"));
                break;
            case "listfile":
                action = new ListFileManager(context, UeditorConfig.GetString("fileManagerListPath"), UeditorConfig.GetStringList("fileManagerAllowFiles"));
                break;
            case "catchimage":
                action = new CrawlerHandler(context);
                break;
            default:
                action = new NotSupportedHandler(context);
                break;
        }
        action.Process();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}