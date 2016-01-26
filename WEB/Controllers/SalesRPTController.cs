using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;

namespace YXERP.Controllers
{
    public class SalesRPTController : BaseController
    {
        //
        // GET: /SalesRPT/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserOrderReport()
        {
            ViewBag.Types = SystemBusiness.BaseBusiness.GetOrderTypes(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }

        public ActionResult OrderFunnelReport()
        {
            return View();
        }

        public ActionResult OrderMapReport()
        {
            //ViewBag.Types = SystemBusiness.BaseBusiness.GetOrderTypes(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }


        #region 销售订单统计

        public JsonResult GetUserOrders(string userid, string teamid, string beginTime, string endTime)
        {

            var list = SalesRPTBusiness.BaseBusiness.GetUserOrders(userid, teamid, beginTime, endTime, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region Ajax 订单分布统计

        public JsonResult GetOrderMapReport(int type, string beginTime, string endTime, string UserID, string TeamID)
        {

            var list = SalesRPTBusiness.BaseBusiness.GetOrderMapReport(type, beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            if (type == 1)
                list.Sort((g1, g2) => { return Comparer<int>.Default.Compare(g2.value, g1.value); });
            else if (type == 3)
                list.Sort((g1, g2) => { return Comparer<int>.Default.Compare(int.Parse(g2.name), int.Parse(g1.name)); });

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 销售转化率

        public JsonResult GetOpportunityStageRate(string beginTime, string endTime, string UserID, string TeamID)
        {
            decimal forecast = 0;
            var list = SalesRPTBusiness.BaseBusiness.GetOpportunityStageRate(beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID, out forecast);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("forecast", forecast);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserOpportunitys(string userid, string teamid, string beginTime, string endTime)
        {

            var list = SalesRPTBusiness.BaseBusiness.GetUserOpportunitys(userid, teamid, beginTime, endTime, CurrentUser.AgentID, CurrentUser.ClientID);
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
