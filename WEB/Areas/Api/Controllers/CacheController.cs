using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
namespace YXERP.Areas.Api.Controllers
{
    public class CacheController : BaseAPIController
    {
        //
        // GET: /Api/Cache/

        /// <summary>
        /// 清除分类缓存
        /// </summary>
        public JsonResult ClearCategoryCache()
        {
            ProductsBusiness.ClearCategoryCache();

            JsonDictionary.Add("result",1);

            return new JsonResult() {
                Data = JsonDictionary,
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 清除分类属性缓存
        /// </summary>
        public JsonResult ClearAttrsCache()
        {
            ProductsBusiness.ClearAttrsCache();

            JsonDictionary.Add("result", 1);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 清除单位缓存
        /// </summary>
        public JsonResult ClearUnitCache()
        {
            ProductsBusiness.ClearUnitCache();

            JsonDictionary.Add("result", 1);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public JsonResult UpdatetClientCache(string clientid)
        {
            if (!string.IsNullOrEmpty(clientid))
            {
                IntFactoryBusiness.Manage.ClientBusiness.UpdateClientCache(clientid);

                JsonDictionary.Add("result", 1);
            }
            else
            {
                JsonDictionary.Add("result", 0);
            }

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
