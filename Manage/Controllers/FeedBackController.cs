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
    public class FeedBackController :BaseController
    {
        //
        // GET: /FeedBack/

        public ActionResult FeedBacks()
        {
            return View();
        }

        public ActionResult FeedBackDetail(string id)
        {
            ViewBag.id = id;
            return View();
        }

        #region ajax
        public JsonResult GetFeedBacks(int pageIndex, int type, int status, string keyWords, string beginDate, string endDate)
        {

            int totalCount = 0, pageCount = 0;
            var list = FeedBackBusiness.GetFeedBacks(keyWords,string.Empty, beginDate, endDate, type, status, PageSize, pageIndex, out totalCount, out pageCount);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetFeedBackDetail(string id) {
            var item = FeedBackBusiness.GetFeedBackDetail(id);
            JsonDictionary.Add("item", item);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateFeedBackStatus(string id,int status,string content)
        {
            bool flag = FeedBackBusiness.UpdateFeedBackStatus(id, status, content);
            JsonDictionary.Add("result", flag?1:0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
