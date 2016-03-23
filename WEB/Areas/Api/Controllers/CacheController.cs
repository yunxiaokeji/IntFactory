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

    }
}
