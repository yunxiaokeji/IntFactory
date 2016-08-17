﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using System.IO;
using System.Web.Script.Serialization;
using System.Text;

using Qiniu.Auth;
using Qiniu.IO;
using Qiniu.IO.Resumable;
using Qiniu.RS;
using Qiniu.RPC;
using Qiniu.Conf;
using IntFactoryEnum;
using IntFactoryEntity;
namespace YXERP.Controllers
{
    public class PlugController : Controller
    {
        /// <summary>
        /// 当前登录用户
        /// </summary>
        protected IntFactoryEntity.Users CurrentUser
        {
            get
            {
                if (Session["ClientManager"] == null)
                {
                    return null;
                }
                else
                {
                    return (IntFactoryEntity.Users)Session["ClientManager"];
                }
            }
            set { Session["ClientManager"] = value; }
        }
        protected Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        /// <summary>
        /// 根据cityCode获取下级地区列表
        /// </summary>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public JsonResult GetCityByPCode(string cityCode)
        {
            var list = CommonBusiness.Citys.Where(c => c.PCode == cityCode);
            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetToken()
        {
            Config.Init();
            //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
            PutPolicy put = new PutPolicy(YXERP.Common.Common.QNBucket, 3600);
            //调用Token()方法生成上传的Token
            string upToken = put.Token();
            JsonDictionary.Add("uptoken", upToken);
            JsonDictionary.Add("qnBucket", YXERP.Common.Common.QNBucket);
            JsonDictionary.Add("qnDomianUrl", YXERP.Common.Common.QNDomianUrl);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public int DeleteAttachment(string key)
        {
            Config.Init();
            //实例化一个RSClient对象，用于操作BucketManager里面的方法
            RSClient client = new RSClient();
            CallRet ret = client.Delete(new EntryPath(YXERP.Common.Common.QNBucket, key));

            return ret.OK ? 1 : 0;
        }

        public bool UploadAttachment(string key)
        {
            Config.Init();
            IOClient target = new IOClient();
            PutExtra extra = new PutExtra();
            //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
            PutPolicy put = new PutPolicy(YXERP.Common.Common.QNBucket, 3600);

            //调用Token()方法生成上传的Token
            string upToken = put.Token();
            //上传文件的路径
            String filePath = string.Empty;

            //调用PutFile()方法上传
            PutRet ret = target.PutFile(upToken, key, filePath, extra);


            return ret.OK;
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadFile()
        {
            string oldPath = "",
                   folder = CloudSalesTool.AppSettings.Settings["UploadTempPath"], 
                   action = "";
            if (Request.Form.AllKeys.Contains("oldPath"))
            {
                oldPath = Request.Form["oldPath"];
            }
            if (Request.Form.AllKeys.Contains("folder") && !string.IsNullOrEmpty(Request.Form["folder"]))
            {
                folder = Request.Form["folder"];
            }
            string uploadPath = HttpContext.Server.MapPath(folder);

            if (Request.Form.AllKeys.Contains("action"))
            {
                action = Request.Form["action"];
            }
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            List<string> list = new List<string>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                if (i == 10) {
                    break;
                }
                HttpPostedFileBase file = Request.Files[i];
                //判断图片类型
                string ContentType = file.ContentType;
                Dictionary<string, string> types = new Dictionary<string, string>();
                types.Add("image/x-png", "1");
                types.Add("image/png", "1");
                types.Add("image/gif", "1");
                types.Add("image/jpeg", "1");
                types.Add("image/tiff", "1");
                types.Add("application/x-MS-bmp", "1");
                types.Add("image/pjpeg", "1");
                if (!types.ContainsKey(ContentType))
                {
                    continue;
                }
                if (file.ContentLength > 1024 * 1024 * 5)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(oldPath) && oldPath != "/modules/images/default.png" && new FileInfo(HttpContext.Server.MapPath(oldPath)).Exists)
                {
                    file.SaveAs(HttpContext.Server.MapPath(oldPath));
                    list.Add(oldPath);

                }
                else
                {
                    string[] arr = file.FileName.Split('.');
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssms") + new Random().Next(1000, 9999).ToString() + "." + arr[arr.Length - 1];
                    string filePath = uploadPath + fileName;
                    file.SaveAs(filePath);
                    list.Add(folder + fileName);
                }
            }

            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadFiles()
        {

            string oldPath = "",
                   folder = CloudSalesTool.AppSettings.Settings["UploadTempPath"],
                   action = "";

            if (Request.Form.AllKeys.Contains("folder") && !string.IsNullOrEmpty(Request.Form["folder"]))
            {
                folder = Request.Form["folder"];
            }
            string uploadPath = HttpContext.Server.MapPath(folder);

            if (Request.Form.AllKeys.Contains("action"))
            {
                action = Request.Form["action"];
            }
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            
            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            bool stage = true;
            if (Request.Files.Count>5)
            {
                stage = false;                
            }
            else
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    int isImage = 2;
                    HttpPostedFileBase file = Request.Files[i];
                    string ContentType = file.ContentType;
                    Dictionary<string, string> types = new Dictionary<string, string>();
                    types.Add("image/x-png", "1");
                    types.Add("image/png", "1");
                    types.Add("image/gif", "1");
                    types.Add("image/jpeg", "1");
                    types.Add("image/tiff", "1");
                    types.Add("application/x-MS-bmp", "1");
                    types.Add("image/pjpeg", "1");

                    Dictionary<string, string> fileTypeItems = new Dictionary<string, string>();
                    fileTypeItems.Add("application/vnd.ms-powerpoint", "1");
                    fileTypeItems.Add("application/vnd.openxmlformats-officedocument.presentationml.presentation", "1");
                    fileTypeItems.Add("application/vnd.ms-excel", "1");
                    fileTypeItems.Add("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "1");
                    fileTypeItems.Add("application/msword", "1");
                    fileTypeItems.Add("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "1");
                    fileTypeItems.Add("text/plain", "1");
                    if (!fileTypeItems.ContainsKey(ContentType) && !types.ContainsKey(ContentType))
                    {
                        continue;
                    }
                    if (types.ContainsKey(ContentType))
                    {
                        isImage = 1;
                    }

                    string[] arr = file.FileName.Split('.');
                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssms") + new Random().Next(1000, 9999).ToString() + i + "." + arr[arr.Length - 1];
                    string saveFilePath = uploadPath + newFileName;
                    string newFilePath = folder;

                    Dictionary<string, object> item = new Dictionary<string, object>();
                    if (string.IsNullOrEmpty(oldPath))
                    {
                        file.SaveAs(saveFilePath);
                        item.Add("filePath", newFilePath);
                    }
                    else
                    {
                        file.SaveAs(HttpContext.Server.MapPath(oldPath));
                        item.Add("filePath", oldPath);
                    }
                    item.Add("fileName", newFileName);
                    item.Add("originalName", Path.GetFileName(file.FileName));
                    item.Add("fileSize", file.ContentLength);
                    item.Add("extensions", arr[arr.Length - 1]);
                    item.Add("isImage", isImage);
                    items.Add(item);
                }
            }

            JsonDictionary.Add("Items", items);
            JsonDictionary.Add("Stage", stage);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult DownLoadFile(string filePath, string fileName, string originalName,string isIE)
        {
            string path = Server.MapPath(filePath + fileName);//服务器文件物理路径
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                if (!string.IsNullOrEmpty(isIE))
                {
                    //加上HttpUtility.UrlEncode()方法，防止文件下载时，文件名乱码，（保存到磁盘上的文件名称应为“中文名.gif”）
                    originalName = HttpUtility.UrlEncode(originalName, Encoding.UTF8);
                }
                HttpContext.Response.Clear();
                HttpContext.Response.ClearContent();
                HttpContext.Response.ClearHeaders();
                HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + originalName);
                HttpContext.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                HttpContext.Response.AddHeader("Content-Transfer-Encoding", "binary");
                HttpContext.Response.ContentType = "application/octet-stream";
                HttpContext.Response.ContentEncoding = Encoding.GetEncoding("gb2312");
                HttpContext.Response.WriteFile(fileInfo.FullName);
                HttpContext.Response.Flush();
                HttpContext.Response.End();
                return null;
            }
            else {
                return Redirect("/Error/NoFindFile");
            }
        }

        /// <summary>
        /// 获取下属列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserBranchs(string userid)
        {

            if (string.IsNullOrEmpty(userid))
            {
                userid = CurrentUser.UserID;
            }
            else if (userid == "-1")
            {
                userid = "6666666666";
            }
            var list = OrganizationBusiness.GetStructureByParentID(userid, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取团队
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTeams()
        {
            var list = SystemBusiness.BaseBusiness.GetTeams(CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        //快递公司
        public JsonResult GetExpress()
        {
            var list = IntFactoryBusiness.Manage.ExpressCompanyBusiness.GetExpressCompanys();
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="type"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult GetObjectLogs(string guid, EnumLogObjectType type, int pageSize, int pageIndex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = LogBusiness.GetLogs(guid, type, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetReplys(string guid, EnumLogObjectType type, int pageSize, int pageIndex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = ReplyBusiness.GetReplys(guid, type, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SavaReply(EnumLogObjectType type, string entity, string attchmentEntity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ReplyEntity model = serializer.Deserialize<ReplyEntity>(entity);
            model.Attachments = serializer.Deserialize<List<Attachment>>(attchmentEntity);
            string replyID = "";

            switch (type)
            {
                case EnumLogObjectType.Customer:
                    replyID = ReplyBusiness.CreateCustomerReply(model.GUID, model.Content, CurrentUser.UserID, CurrentUser.ClientID, model.FromReplyID, model.FromReplyUserID, model.FromReplyAgentID);
                    ReplyBusiness.AddCustomerReplyAttachments(model.GUID, replyID, model.Attachments, CurrentUser.UserID, CurrentUser.ClientID);
                    break;
                case EnumLogObjectType.OrderTask:
                    replyID = ReplyBusiness.CreateTaskReply(model.GUID, model.Content, CurrentUser.UserID, CurrentUser.ClientID, model.FromReplyID, model.FromReplyUserID, model.FromReplyAgentID);
                    ReplyBusiness.AddTaskReplyAttachments(model.GUID, replyID, model.Attachments, CurrentUser.UserID, CurrentUser.ClientID);
                    break;

            }
            

            List<ReplyEntity> list = new List<ReplyEntity>();
            if (!string.IsNullOrEmpty(replyID))
            {
                model.ReplyID = replyID;
                model.CreateTime = DateTime.Now;
                model.CreateUser = OrganizationBusiness.GetUserCacheByUserID(CurrentUser.UserID, CurrentUser.ClientID); ;
                model.CreateUserID = CurrentUser.UserID;
                if (!string.IsNullOrEmpty(model.FromReplyUserID) && !string.IsNullOrEmpty(model.FromReplyAgentID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserCacheByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
