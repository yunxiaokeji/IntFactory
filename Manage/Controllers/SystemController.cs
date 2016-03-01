using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness.Manage;
using System.Web.Script.Serialization;
using IntFactoryEntity;
using IntFactoryEntity.Manage;
namespace YXManage.Controllers
{
     [YXManage.Common.UserAuthorize]
    public class SystemController : BaseController
    {
        #region view
         public ActionResult Admin()
        {
            return View();
        }

         public ActionResult Menu()
         {
             return View();
         }

         public ActionResult MenuDetail(string id)
         {
             var model = new SystemMenu();
             var menuCode = string.Empty;
             var pCodeName=string.Empty;

             if (!string.IsNullOrEmpty(id))
             {
                 model = ManageSystemBusiness.GetSystemMenu(id);
                 menuCode = model.MenuCode;
                 pCodeName = model.PCodeName;
             }
             ViewBag.Model = model;
             ViewBag.MenuCode = menuCode;
             ViewBag.PCodeName = pCodeName;

             return View();
         }

         public ActionResult AddSubMenu(string id)
         {
             var model = new SystemMenu();
             var menuCode = string.Empty;
             var pCode = string.Empty;
             var pCodeName = string.Empty;
             var layer = 0;

             model = ManageSystemBusiness.GetSystemMenu(id);
             pCode = model.MenuCode;
             pCodeName = model.Name;
             layer = model.Layer+1;

             ViewBag.PCode = pCode;
             ViewBag.PCodeName = pCodeName;
             ViewBag.Layer = layer;
             ViewBag.Model = new SystemMenu();
             ViewBag.MenuCode = menuCode;
             return View("MenuDetail");
         }
        #endregion

        #region ajax
        public JsonResult GetAdminDetail()
        {
            JsonDictionary.Add("LoginName", CurrentUser.LoginName);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ConfirmAdminPwd(string pwd)
        {
            IntFactoryEntity.Manage.M_Users model = IntFactoryBusiness.M_UsersBusiness.GetM_UserByUserName(CurrentUser.LoginName, pwd, string.Empty);
            JsonDictionary.Add("Result", model!=null?1:0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SetAdminAccount(string loginName, string newPwd)
        {
            bool flag = IntFactoryBusiness.M_UsersBusiness.SetAdminAccount(CurrentUser.UserID, loginName, newPwd);
            
            JsonDictionary.Add("Result", flag ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public JsonResult GetSystemMenus()
        {
            var list = ManageSystemBusiness.GetSystemMenus();

            JsonDictionary.Add("Items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetSystemMenu(string menuCode)
        {
            var item = ManageSystemBusiness.GetSystemMenu(menuCode);
            JsonDictionary.Add("Item", item);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet 
            };
        }

        public JsonResult SaveSystemMenu(string menu)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            SystemMenu model = serializer.Deserialize<SystemMenu>(menu);
            bool flag = false;

            if (!string.IsNullOrEmpty(model.MenuCode))
            {
                flag = ManageSystemBusiness.UpdateSystemMenu(model);
            }
            else
            {
                model.MenuCode = DateTime.Now.Ticks.ToString();
                model.Type = 2;
                flag = ManageSystemBusiness.AddSystemMenu(model);
            }

            JsonDictionary.Add("Result",flag?1:0);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteSystemMenu(string menuCode)
        {
            bool flag = false;

            flag = ManageSystemBusiness.DeleteSystemMenu(menuCode);


            JsonDictionary.Add("Result", flag ? 1 : 0);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
