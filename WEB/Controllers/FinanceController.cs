using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXERP.Models;

using IntFactoryBusiness;
using IntFactoryEntity;

namespace YXERP.Controllers
{
    public class FinanceController : BaseController
    {
        //
        // GET: /Finance/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PayableBills()
        {
            return View();
        }

        public ActionResult PayableBillsDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("PayableBills");
            }
            ViewBag.ID = id;
            return View();
        }

        public ActionResult ReceivableBills()
        {
            return View();
        }

        public ActionResult OrderBillsDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("ReceivableBills");
            }
            ViewBag.ID = id;
            return View();
        }

        public ActionResult AccountBills()
        {
            return View();
        }


        public JsonResult GetAccountBills(int Mark, string Keywords, string BeginTime, string EndTime, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = FinanceBusiness.BaseBusiness.GetAccountBills(Mark, BeginTime, EndTime, Keywords, PageSize, PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        #region Ajax应付账款

        public JsonResult GetPayableBills(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterBills model = serializer.Deserialize<FilterBills>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = FinanceBusiness.BaseBusiness.GetPayableBills(model.PayStatus, model.InvoiceStatus, model.BeginTime, model.EndTime, model.Keywords, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetPayableBillByID(string id)
        {
            var model = FinanceBusiness.BaseBusiness.GetPayableBillByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveStorageBillingPay(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StorageBillingPay model = serializer.Deserialize<StorageBillingPay>(entity);

            bool bl = FinanceBusiness.BaseBusiness.CreateStorageBillingPay(model.BillingID, model.Type, model.PayType, model.PayMoney, model.PayTime, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            if (bl)
            {
                model.CreateUser = CurrentUser;
                JsonDictionary.Add("item", model);
            }
            
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveStorageBillingInvoice(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StorageBillingInvoice model = serializer.Deserialize<StorageBillingInvoice>(entity);

            var id = FinanceBusiness.BaseBusiness.CreateStorageBillingInvoice(model.BillingID, model.Type, model.InvoiceMoney, model.InvoiceCode, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            model.InvoiceID = id;
            model.CreateUser = CurrentUser;
            JsonDictionary.Add("item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteStorageBillingInvoice(string id, string billingid)
        {
            var bl = FinanceBusiness.BaseBusiness.DeleteStorageBillingInvoice(id, billingid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region Ajax应收账款

        public JsonResult GetOrderBills(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterBills model = serializer.Deserialize<FilterBills>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = FinanceBusiness.BaseBusiness.GetOrderBills(model.PayStatus, model.InvoiceStatus, model.ReturnStatus, model.BeginTime, model.EndTime, model.Keywords, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderBillByID(string id)
        {
            var model = FinanceBusiness.BaseBusiness.GetOrderBillByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveOrderBillingPay(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BillingPay model = serializer.Deserialize<BillingPay>(entity);

            bool bl = FinanceBusiness.BaseBusiness.CreateBillingPay(model.BillingID, model.Type, model.PayType, model.PayMoney, model.PayTime, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderBillingPays(string orderid)
        {

            var list = FinanceBusiness.BaseBusiness.GetOrderPays(orderid);

            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveBillingInvoice(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BillingInvoice model = serializer.Deserialize<BillingInvoice>(entity);

            var id = FinanceBusiness.BaseBusiness.CreateBillingInvoice(model.BillingID, model.Type, model.CustomerType, model.InvoiceMoney, model.InvoiceTitle, model.CityCode, model.Address, model.PostalCode, model.ContactName, model.ContactPhone, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            model.InvoiceID = id;
            model.CreateUser = CurrentUser;
            JsonDictionary.Add("item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteBillingInvoice(string id, string billingid)
        {
            var bl = FinanceBusiness.BaseBusiness.DeleteBillingInvoice(id, billingid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditBillingInvoice(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BillingInvoice model = serializer.Deserialize<BillingInvoice>(entity);

            bool bl = FinanceBusiness.BaseBusiness.AuditBillingInvoice(model.InvoiceID, model.BillingID, model.InvoiceMoney, model.InvoiceCode, model.ExpressID, model.ExpressCode, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);

            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion
    }
}
