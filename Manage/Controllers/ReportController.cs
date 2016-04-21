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
        public JsonResult GetAgentActionReports(string keyword,string startDate,string endDate)
        {
            var list = AgentsBusiness.GetAgentActionReport(keyword, startDate, endDate);
            JsonDictionary.Add("Items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
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
    }
}
