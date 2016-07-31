﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryEntity;
using IntFactoryBusiness;
using IntFactoryEnum;
using IntFactoryBusiness.Manage;

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

            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            var action = filterContext.ActionDescriptor.ActionName.ToLower();

            var currentUser = (IntFactoryEntity.Users)filterContext.HttpContext.Session["ClientManager"];

            if (currentUser.Client.GuideStep != 0)
            {
                if (controller != "default")
                {
                    filterContext.Result = new RedirectResult("/Default/Index");
                }
                return;
            }

            var client = ClientBusiness.GetClientDetail(currentUser.ClientID);
            if (client.EndTime < DateTime.Now)
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
                    filterContext.Result = new RedirectResult("/Home/UseExpired");
                }

                return;
            }

            
            var menu = CommonBusiness.ClientMenus.Where(m => m.Controller.ToLower() == controller && m.View.ToLower() == action).FirstOrDefault();

            //需要判断权限
            if (menu != null && menu.IsLimit == 1)
            {
                if (currentUser.Menus.Where(m => m.MenuCode == menu.MenuCode).Count() <= 0)
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
                        var urlRequest = filterContext.RequestContext.HttpContext.Request;
                        throw new HttpException(403, urlRequest.UrlReferrer != null ? urlRequest.UrlReferrer.AbsoluteUri : urlRequest.Url.AbsoluteUri);
                    }
                }
            }

        }
    }
}