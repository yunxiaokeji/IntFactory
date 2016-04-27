using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult NotAccess(string aspxerrorpath)
        {
            ViewBag.aspxerrorpath = aspxerrorpath;
            return View();
        }

        public ActionResult NoRoot(string aspxerrorpath)
        {
            ViewBag.aspxerrorpath = aspxerrorpath;
            return View();
        }


    }
}
