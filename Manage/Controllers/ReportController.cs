using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using System.Web.Script.Serialization;
using IntFactoryEntity;
using IntFactoryBusiness.Manage;
namespace YXManage.Controllers
{
    public class ReportController :BaseController
    {
        //
        // GET: /Report/

        public ActionResult AgentActionReport()
        {
            return View();
        }
        public ActionResult ClientsGrowReport()
        {
            return View();
        }
        public ActionResult ClientsLoginReport()
        {
            return View();
        }
        public ActionResult ClientsAgentActionReport()
        {
            return View();
        }
        #region Ajax
        /// <summary>
        /// 客户行为统计
        /// </summary>
        public JsonResult GetAgentActionReports(string keyword, string startDate, string endDate, int pageIndex, string orderBy = "SUM(a.CustomerCount) desc")
        {
            int totalCount = 0, pageCount = 0;
            var list = AgentsBusiness.GetAgentActionReport(keyword, startDate, endDate,orderBy, PageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetAgentActionReportsByClientID(string keyword, string startDate, string endDate, int pageIndex, string clientID = "")
        {
            var list = AgentsBusiness.GetAgentActionReport(keyword, startDate, endDate,clientID);
            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 工厂增量统计
        /// </summary> 
        public JsonResult GetClientsGrow(int dateType, string beginTime, string endTime)
        {
            var list = ClientBusiness.GetClientsGrow(dateType, beginTime, endTime);
            JsonDictionary.Add("items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 工厂登录统计
        /// </summary>
        public JsonResult GetClientsLoginReport(int dateType, string beginTime, string endTime)
        {
            var list = ClientBusiness.GetClientsLoginReport(dateType, beginTime, endTime);
            JsonDictionary.Add("items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetClientsAgentActionReport(int dateType, string beginTime, string endTime,string clientId)
        {
            var list = ClientBusiness.GetClientsAgentActionReport(dateType, beginTime, endTime, clientId);
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
