using IntFactoryEnum.HelpCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXManage.Models;

namespace YXManage.Controllers
{
    public class HelpCenterController : BaseController
    {
        public ActionResult Types()
        {
            return View();
        }

        public ActionResult AddType()
        {
            return View();
        }

        public ActionResult Contents()
        {
            ViewBag.List = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.GetTypes();
            return View();
        }

        public ActionResult AddContent()
        {
            var list = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.GetTypesByModuleType(ModuleTypeEnum.Function);
            ViewBag.List = list;
            return View();
        }

        public ActionResult EditContent(string id)
        {
            var model = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.GetContentByContentID(id);
            ViewBag.List = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.GetTypesByModuleType((ModuleTypeEnum)model.ModuleType);
            ViewBag.model = model;
            return View();
        }

        public ActionResult ContentDetail(string id)
        {
            var model = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.GetContentByContentID(id);
            ViewBag.model = model;

            return View();
        }

        #region ajax

        public JsonResult GetTypes(string filter)
        {
            JavaScriptSerializer jssl = new JavaScriptSerializer();
            FilterHelpCenterType model = jssl.Deserialize<FilterHelpCenterType>(filter);
            int totalCount = 0;
            int pageCount = 0;
            var list = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.GetTypes(model.Types, model.Keywords, model.BeginTime, model.EndTime, model.OrderBy, model.PageSize, model.PageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetContent(string filter)
        {
            JavaScriptSerializer jssl = new JavaScriptSerializer();
            FilterHelpCenterType model = jssl.Deserialize<FilterHelpCenterType>(filter);
            int totalCount = 0;
            int pageCount = 0;
            var items = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.GetContents(model.Types, model.TypeID, model.Keywords, model.BeginTime, model.EndTime, model.OrderBy, model.PageSize, model.PageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", items);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTypesByModuleType(int type)
        {
            var list = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.GetTypesByModuleType((ModuleTypeEnum)type);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InsertType(string name, string desc, int moduleType, string img)
        {
            var bl = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.InsertType(name, desc, moduleType, img, CurrentUser.UserID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InsertContent(string typeID, string sort, string title, string keyWords, string desc)
        {
            var bl = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.InsertContent(typeID, sort, title, keyWords, desc, CurrentUser.UserID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateType(string typeID, string name, string icon, int moduleType)
        {
            var bl = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.UpdateType(typeID, name, icon, moduleType);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateContent(string id, string title, string sort, string keyWords, string content, string typeID)
        {
            var bl = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.UpdateContent(id, title, sort, keyWords, content, typeID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteType(string typeID)
        {
            var bl = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.DeleteType(typeID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteContent(string contentID)
        {
            var bl = IntFactoryBusiness.HelpCenterBusiness.BaseBusiness.DeleteContent(contentID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
