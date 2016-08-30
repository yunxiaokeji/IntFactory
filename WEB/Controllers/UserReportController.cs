using IntFactoryBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    public class UserReportController : BaseController
    {
        //
        // GET: /UserReport/

        public ActionResult Index()
        {
            return View();
        }

        #region UserPeport Ajax

        public JsonResult GetUserWorkLoad(string beginTime, string endTime, string UserID, string TeamID)
        {
            var list = UserRptBusiness.BaseBusiness.GetUserLoadReport(beginTime, endTime, UserID, TeamID, CurrentUser.ClientID);
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
