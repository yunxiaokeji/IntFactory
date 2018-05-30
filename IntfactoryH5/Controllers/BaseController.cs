using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntfactoryH5.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/
        private IntFactoryEntity.Users _currentUser;
        public IntFactoryEntity.Users CurrentUser
        {
            set
            {
                _currentUser = value;
            }
            get
            {
                if (Session["ClientManager"] != null)
                {
                    _currentUser = (IntFactoryEntity.Users)Session["ClientManager"];
                }

                return _currentUser;
            }
        }

        public Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        public static string GetRequestIP()
        {
            return string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers.Get("X-Real-IP")) ? System.Web.HttpContext.Current.Request.UserHostAddress : System.Web.HttpContext.Current.Request.Headers["X-Real-IP"];
        }

    }
}