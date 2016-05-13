using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryEntity;
using IntFactoryBusiness;
using IntFactoryEnum;

namespace YXERP.Common
{
    public class UserAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["ClientManager"] == null)
            {
                httpContext.Response.StatusCode = 401;
                return false;
            }
            else
            {
                //if (user.LogGUID != OrganizationBusiness.GetUserByUserID(user.UserID, user.AgentID).LogGUID)
                //{
                //    httpContext.Response.StatusCode = 402;
                //    return false;
                //}
            }
            return true;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 401)
            {
                string source = HttpContext.Current.Request.QueryString["source"];
                if (!string.IsNullOrEmpty(source) && source == "md")
                {
                    filterContext.Result = new RedirectResult("/Home/MDLogin?ReturnUrl=" + HttpContext.Current.Request.Url);
                }
                else 
                {
                    filterContext.Result = new RedirectResult("/Home/Login?ReturnUrl=" + HttpContext.Current.Request.Url);
                }
                return;
            }

            var currentUser = (IntFactoryEntity.Users)filterContext.HttpContext.Session["ClientManager"];
            var agent = AgentsBusiness.GetAgentDetail(currentUser.AgentID);
            if (agent.EndTime < DateTime.Now) {
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("result", "10001");
                    filterContext.Result = new JsonResult()
                    {
                        Data = result,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Error/NoUse");
                }

                return;
            }
 
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            var action = filterContext.ActionDescriptor.ActionName.ToLower();
            var menu = CommonBusiness.ClientMenus.Where(m => m.Controller.ToLower() == controller && m.View.ToLower() == action).FirstOrDefault();

            //需要判断权限
            if (menu != null && menu.IsLimit == 1)
            {
                IntFactoryEntity.Users user = (IntFactoryEntity.Users)filterContext.HttpContext.Session["ClientManager"];
                if (user.Menus.Where(m => m.MenuCode == menu.MenuCode).Count() <= 0)
                {
                    
                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        Dictionary<string, string> result = new Dictionary<string, string>();
                        result.Add("result", "10001");
                        filterContext.Result = new JsonResult()
                        {
                            Data = result,
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        throw new HttpException(403, filterContext.RequestContext.HttpContext.Request.UrlReferrer.AbsoluteUri);
                        //filterContext.RequestContext.HttpContext.Response.Write("<script>alert('您没有权限访问此页面');history.back();</script>");
                        //filterContext.RequestContext.HttpContext.Response.End();
                    }
                }
            }

        }
    }
}