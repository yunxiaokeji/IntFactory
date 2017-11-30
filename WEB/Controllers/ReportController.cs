using IntFactoryBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    public class ReportController : BaseController
    {
        //
        // GET: /Report/

        public ActionResult MyWorkReport()
        {
            return View();
        }

        public ActionResult UserTaskReport()
        {
            return View();
        }

        public ActionResult UserLoadReport()
        {
            return View();
        }

        #region 任务统计

        /// <summary>
        /// 员工任务数
        /// </summary>
        public JsonResult GetUserTaskQuantity(string beginTime, string endTime, string UserID, string TeamID)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetUserTaskQuantity(beginTime, endTime, UserID, TeamID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserWorkLoad(string beginTime, string endTime, string UserID, string TeamID)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetUserLoadReport(beginTime, endTime, UserID, TeamID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion
    }
}
