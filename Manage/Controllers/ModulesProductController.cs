using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness.Manage;
using CloudSalesTool;
using IntFactoryEntity.Manage;
using System.Web.Script.Serialization;
namespace YXManage.Controllers
{
    [YXManage.Common.UserAuthorize]
    public class ModulesProductController :BaseController
    {
        //
        // GET: /ModulesProduct/

        #region view
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(string id)
        {
            //ViewBag.Modules = IntFactoryBusiness.ModulesBusiness.GetModules();
            if(string.IsNullOrEmpty(id))
                ViewBag.ID = 0;
            else
                ViewBag.ID = int.Parse(id);

            return View();
        }
        #endregion

        #region ajax
        public JsonResult GetModulesProducts(int pageIndex, string keyWords)
        {
            int totalCount = 0, pageCount = 0;
            var list = ModulesProductBusiness.GetModulesProducts(keyWords, PageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetModulesProductDetail(int id)
        {
            var item = ModulesProductBusiness.GetModulesProductDetail(id);
            JsonDictionary.Add("Item", item);
            JsonDictionary.Add("Result", 1);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveModulesProduct(string modulesProduct)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ModulesProduct model = serializer.Deserialize<ModulesProduct>(modulesProduct);

            bool flag = false;
            if (model.AutoID==0)
            {
                model.CreateUserID =string.Empty;
                 flag =ModulesProductBusiness.InsertModulesProduct(model);
            }
            else
            {
                model.CreateUserID = string.Empty;
                flag = ModulesProductBusiness.UpdateModulesProduct(model);
            }
            JsonDictionary.Add("Result", flag ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteModulesProduct(int id)
        {
            bool flag = ModulesProductBusiness.DeleteModulesProduct(id);
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
