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

        /// <summary>
        /// 根据agentID更新代理商信息缓存
        /// </summary>
        public JsonResult UpdatetAgentCache(string agentID)
        {
            if (!string.IsNullOrEmpty(agentID))
            {
                AgentsBusiness.UpdatetAgentCache(agentID);

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
