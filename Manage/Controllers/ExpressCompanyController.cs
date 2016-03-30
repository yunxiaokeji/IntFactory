using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness.Manage;
using System.Web.Script.Serialization;
using IntFactoryEntity.Manage;
namespace YXManage.Controllers
{
    [YXManage.Common.UserAuthorize]
    public class ExpressCompanyController:BaseController
    {
        #region view
        public ActionResult ExpressCompanys()
        {
            return View();
        }

        public ActionResult ExpressCompanyDetail(string id)
        {
            ViewBag.ExpressID =id;

            return View();
        }
        #endregion

        #region ajax
        /// <summary>
        /// 快递公司列表
        /// </summary>
        public JsonResult GetExpressCompanys(int pageIndex, string keyWords)
        {
            int totalCount = 0, pageCount = 0;
            var list = ExpressCompanyBusiness.GetExpressCompanys(keyWords, PageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 快递公司详情
        /// </summary>
        public JsonResult GetExpressCompanyDetail(string id)
        {
            var item = ExpressCompanyBusiness.GetExpressCompanyDetail(id);
            JsonDictionary.Add("item", item);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存快递公司
        /// </summary>
        public JsonResult SaveExpressCompany(string expressCompany)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ExpressCompany model = serializer.Deserialize<ExpressCompany>(expressCompany);

            bool flag = false;
            if (string.IsNullOrEmpty(model.ExpressID))
            {
                model.CreateUserID = string.Empty;
                flag = ExpressCompanyBusiness.InsertExpressCompany(model);
            }
            else
            {
                model.CreateUserID = string.Empty;
                flag = ExpressCompanyBusiness.UpdateExpressCompany(model);
            }
            JsonDictionary.Add("result", flag ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除快递公司
        /// </summary>
        public JsonResult DeleteExpressCompany(string id)
        {
            bool flag = ExpressCompanyBusiness.DeleteExpressCompany(id);
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
