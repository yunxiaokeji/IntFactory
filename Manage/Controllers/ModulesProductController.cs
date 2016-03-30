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
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetModulesProductDetail(int id)
        {
            var item = ModulesProductBusiness.GetModulesProductDetail(id);
            JsonDictionary.Add("item", item);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveModulesProduct(string modulesProduct)
        {
            bool flag = false;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ModulesProduct model = serializer.Deserialize<ModulesProduct>(modulesProduct);
            model.CreateUserID =CurrentUser.UserID;

            if (model.AutoID==0){
                 flag =ModulesProductBusiness.InsertModulesProduct(model);
            }
            else{
                flag = ModulesProductBusiness.UpdateModulesProduct(model);
            }

            JsonDictionary.Add("result", flag ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteModulesProduct(int id)
        {
            bool flag = ModulesProductBusiness.DeleteModulesProduct(id);
            JsonDictionary.Add("result", flag ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
