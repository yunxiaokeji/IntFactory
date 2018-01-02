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

        public ActionResult UserCutReport()
        {
            return View();
        }

        public ActionResult UserSewnReport()
        {
            return View();
        }

        public ActionResult OrderProductionRPT()
        {
            return View();
        }

        public ActionResult CustomerRateRPT()
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

        public JsonResult GetUserWorkLoad(string beginTime, string endTime, int docType, string UserID, string TeamID)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetUserLoadReport(beginTime, endTime, docType, UserID, TeamID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 订单统计

        /// <summary>
        /// 订单生产数据统计
        /// </summary>
        public JsonResult GetOrderProductionRPT(string beginTime, string endTime, string keyWords, string UserID, string TeamID, int TimeType)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetOrderProductionRPT(TimeType, beginTime, endTime, keyWords, UserID, TeamID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion 

        #region 客户统计

        /// <summary>
        /// 客户转化率统计
        /// </summary>
        public JsonResult GetCustomerRateRPT(string beginTime, string endTime, string keyWords)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetCustomerRateRPT(beginTime, endTime, keyWords, CurrentUser.ClientID);
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
