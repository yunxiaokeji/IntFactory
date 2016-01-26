
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEnum;
using System.Web.Script.Serialization;
using YXERP.Models;

namespace YXERP.Controllers
{
    public class StorageOutController : BaseController
    {
        //
        // GET: /StorageOut/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StorageOut()
        {
            return View();
        }

        public ActionResult StorageSend()
        {
            return View();
        }

        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/StorageOut/StorageOut");
            }
            var model = AgentOrderBusiness.BaseBusiness.GetAgentOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);

            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/StorageOut/StorageOut");
            }

            ViewBag.Model = model;

            return View();
        }

        public ActionResult OrderOut(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/StorageOut/StorageOut");
            }
            var model = AgentOrderBusiness.BaseBusiness.GetAgentOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/StorageOut/StorageOut");
            }
            if (model.SendStatus > 0)
            {
                return Redirect("/StorageOut/Detail/" + id);
            }
            ViewBag.Model = model;

            return View();
        }

        public ActionResult OrderSend(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/StorageOut/StorageSend");
            }
            var model = AgentOrderBusiness.BaseBusiness.GetAgentOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/StorageOut/StorageSend");
            }
            if (model.SendStatus > 1)
            {
                return Redirect("/StorageOut/Detail/" + id);
            }
            ViewBag.Model = model;

            return View();
        }

        public ActionResult ReturnDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/StorageOut/AuditReturnProduct");
            }
            var model = AgentOrderBusiness.BaseBusiness.GetAgentOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/StorageOut/AuditReturnProduct");
            }
            if (model.ReturnStatus != 1)
            {
                return Redirect("/StorageOut/AuditReturnProduct");
            }
            ViewBag.Model = model;

            return View();
        }

        public ActionResult AuditReturn()
        {
            return View();
        }

        public ActionResult AuditReturnProduct()
        {
            return View();
        }

        public ActionResult OrderTrack()
        {
            return View();
        }

        #region Ajax

        public JsonResult GetAgentOrders(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterAgentsOrders model = serializer.Deserialize<FilterAgentsOrders>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = AgentOrderBusiness.BaseBusiness.GetAgentOrders("", model.status, model.sendstatus, model.returnstatus, model.keywords, model.BeginTime, model.EndTime, model.pagesize, model.pageindex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalcount", totalCount);
            JsonDictionary.Add("pagecount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ConfirmAgentOrderOut(string orderid, string wareid, int issend, string expressid, string expresscode)
        {
            int result = 0;
            string errinfo = "";

            var bl = AgentOrderBusiness.BaseBusiness.ConfirmAgentOrderOut(orderid, wareid, issend, expressid, expresscode, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ConfirmAgentOrderSend(string orderid, string expressid, string expresscode)
        {
            int result = 0;
            string errinfo = "";

            var bl = AgentOrderBusiness.BaseBusiness.ConfirmAgentOrderSend(orderid, expressid, expresscode, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidApplyReturn(string orderid)
        {
            int result = 0;
            string errinfo = "";

            var bl = AgentOrderBusiness.BaseBusiness.InvalidApplyReturn(orderid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditApplyReturn(string orderid)
        {
            int result = 0;
            string errinfo = "";

            var bl = AgentOrderBusiness.BaseBusiness.AuditApplyReturn(orderid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidApplyReturnProduct(string orderid)
        {
            int result = 0;
            string errinfo = "";

            var bl = AgentOrderBusiness.BaseBusiness.InvalidApplyReturnProduct(orderid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditApplyReturnProduct(string orderid, string wareid)
        {
            int result = 0;
            string errinfo = "";

            var bl = AgentOrderBusiness.BaseBusiness.AuditApplyReturnProduct(orderid, wareid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
