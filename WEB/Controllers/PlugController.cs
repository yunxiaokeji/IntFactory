using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using System.IO;
using System.Web.Script.Serialization;

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
                if (string.IsNullOrEmpty(oldPath) || oldPath == "/modules/images/default.png")
                {
                    string[] arr = file.FileName.Split('.');
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssms") + new Random().Next(1000, 9999).ToString() + i + "." + arr[arr.Length - 1];
                    string filePath = uploadPath + fileName;
                    file.SaveAs(filePath);
                    list.Add(folder + fileName);
                }
                else
                {
                    file.SaveAs(HttpContext.Server.MapPath(oldPath));
                    list.Add(oldPath);
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
            for (int i = 0; i < Request.Files.Count; i++)
            {
                int isImage = 2;
                if (i == 10)
                {
                    break;
                }
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
                if (types.ContainsKey(ContentType))
                {
                    isImage = 1;
                }

                string[] arr = file.FileName.Split('.');
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssms") + new Random().Next(1000, 9999).ToString() + i + "." + arr[arr.Length - 1];
                string saveFilePath = uploadPath + newFileName;
                string newFilePath = folder + newFileName;

                Dictionary<string, object> item = new Dictionary<string, object>();
                if (string.IsNullOrEmpty(oldPath))
                {
                    file.SaveAs(saveFilePath);
                    item.Add("path", newFilePath);
                }
                else
                {
                    file.SaveAs(HttpContext.Server.MapPath(oldPath));
                    item.Add("path", oldPath);
                }
                item.Add("fileName", file.FileName);
                item.Add("fileSize", file.ContentLength / 1024);
                item.Add("extensions", arr[arr.Length - 1]);
                item.Add("isImage", isImage);
                items.Add(item);
            }

            JsonDictionary.Add("Items", items);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取下属列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserBranchs(string userid, string agentid)
        {
            if (string.IsNullOrEmpty(agentid))
            {
                agentid = CurrentUser.AgentID;
            }
            if (string.IsNullOrEmpty(userid))
            {
                userid = CurrentUser.UserID;
            }
            else if (userid == "-1")
            {
                userid = "6666666666";
            }
            var list = OrganizationBusiness.GetStructureByParentID(userid, agentid);
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
        public JsonResult GetTeams(string agentid)
        {
            if (string.IsNullOrEmpty(agentid))
            {
                agentid = CurrentUser.AgentID;
            }
            var list = SystemBusiness.BaseBusiness.GetTeams(agentid);
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

    }
}
