using CloudSalesTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXManage.Controllers
{
    public class BaseController : Controller
    {
        protected string ClientID = AppSettings.Settings["ClientID"];

        protected int PageSize = 20;

        protected Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        protected IntFactoryEntity.Manage.M_Users CurrentUser
        {
            get
            {
                if (Session["Manager"] == null)
                {
                    return null;
                }
                else
                {
                    return (IntFactoryEntity.Manage.M_Users)Session["Manager"];
                }
            }
            set { Session["Manager"] = value; }
        }
    }
}
