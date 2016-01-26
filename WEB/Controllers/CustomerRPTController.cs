using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEnum;

namespace YXERP.Controllers
{

    public class CustomerRPTController : BaseController
    {
        //
        // GET: /CustomerRPT/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SourceReport()
        {
            return View();
        }

        public ActionResult StageReport()
        {
            return View();
        }

        public ActionResult CustomerReport()
        {
            return View();
        }

        public ActionResult UserReport()
        {
            ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }


        #region Ajax 客户来源统计

        public JsonResult GetCustomerSourceScale(string beginTime, string endTime, string UserID, string TeamID)
        {

            var list = CustomerRPTBusiness.BaseBusiness.GetCustomerSourceScale(beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult() 
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomerSourceDate(EnumDateType dateType, string beginTime, string endTime, string UserID, string TeamID)
        {

            var list = CustomerRPTBusiness.BaseBusiness.GetCustomerSourceDate(dateType, beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region Ajax 客户分布统计

        public JsonResult GetCustomerReport(int type, string beginTime, string endTime, string UserID, string TeamID)
        {

            var list = CustomerRPTBusiness.BaseBusiness.GetCustomerReport(type, beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            if (type == 1)
                list.Sort( (g1, g2) => { return Comparer<int>.Default.Compare(g2.value, g1.value); });
            else if (type == 3)
                list.Sort((g1, g2) => { return Comparer<int>.Default.Compare(int.Parse( g2.name),int.Parse( g1.name));});

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 客户转化率

        public JsonResult GetCustomerStageRate(int type, string beginTime, string endTime)
        {
            if (type == 1)
            {
                var list = CustomerRPTBusiness.BaseBusiness.GetCustomerStageRate(beginTime, endTime, CurrentUser.AgentID, CurrentUser.ClientID);
                JsonDictionary.Add("items", list);
            }
            else if (type == 2 || type == 3)
            {
                var list = CustomerRPTBusiness.BaseBusiness.GetUserCustomers("", "", beginTime, endTime, CurrentUser.AgentID, CurrentUser.ClientID);
                JsonDictionary.Add("items", list);
            }
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region 销售客户统计

        public JsonResult GetUserCustomers(string userid, string teamid, string beginTime, string endTime)
        {

            var list = CustomerRPTBusiness.BaseBusiness.GetUserCustomers(userid, teamid, beginTime, endTime, CurrentUser.AgentID, CurrentUser.ClientID);
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
