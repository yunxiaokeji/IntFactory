
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
            return View();
        }

        public ActionResult OrderOut(string id)
        {
            return View();
        }

        public ActionResult OrderSend(string id)
        {
            return View();
        }

        public ActionResult ReturnDetail(string id)
        {
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

        public JsonResult InvalidApplyReturnProduct(string orderid)
        {
            int result = 0;
            string errinfo = "";

            var bl = AgentOrderBusiness.BaseBusiness.InvalidApplyReturnProduct(orderid, CurrentUser.UserID,  CurrentUser.ClientID, ref result, ref errinfo);
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

            var bl = AgentOrderBusiness.BaseBusiness.AuditApplyReturnProduct(orderid, wareid, CurrentUser.UserID, CurrentUser.ClientID, ref result, ref errinfo);
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
