using IntFactoryBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    [YXERP.Common.UserAuthorize]
    public class BaseController : Controller
    {

        /// <summary>
        /// 默认分页Size
        /// </summary>
        protected int PageSize = 10;

        /// <summary>
        /// 登录IP
        /// </summary>
        protected string OperateIP
        {
            get 
            {
                return string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];
            }
        }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        protected IntFactoryEntity.Users CurrentUser
        {
            get
            {
                if (Session["ClientManager"] == null)
                {
                    return null;
                }
                else
                {
                    return (IntFactoryEntity.Users)Session["ClientManager"];
                }
            }
            set { Session["ClientManager"] = value; }
        }

        /// <summary>
        /// 返回数据集合
        /// </summary>
        protected Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        /// <summary>
        /// 待办列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetClientUpcomings()
        {
            var list = LogBusiness.BaseBusiness.GetClientUpcomings(CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
