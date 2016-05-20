using IntFactoryBusiness.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    public class DefaultController : BaseController
    {
        //
        // GET: /Default/

        public ActionResult Index()
        {
            if (CurrentUser.Client.GuideStep == 1)
            {
                return Redirect("/Default/SetProcess");
            }
            ViewBag.ID = CurrentUser.Client.GuideStep;
            return View();//"/Home/Index"
        }

        public ActionResult SetProcess()
        {
            if (CurrentUser.Client.GuideStep != 1)
            {
                return Redirect("/Default/Index");
            }
            return View();
        }


        #region Ajax

        public JsonResult SetClientProcess(int type)
        {
            int guideStep = ClientBusiness.SetClientProcess(type, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("value", guideStep);
            CurrentUser.Client.GuideStep = guideStep;
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        #endregion

    }
}
