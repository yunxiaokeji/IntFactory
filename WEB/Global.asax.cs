﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace YXERP
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
                new string[] { "YXERP.Controllers" }
            );

        }

        private YXERP.Common.SyncTaskRecord syncreocRecord = new YXERP.Common.SyncTaskRecord();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            //syncreocRecord.OnStart();
        }
        
        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    string urlReferrer = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : Request.Url.AbsoluteUri;
        //    Exception exception = Server.GetLastError();
        //    HttpException httpException = exception as HttpException;
        //    RouteData routeData = new RouteData();

        //    routeData.Values.Add("controller", "Error");
        //    if (exception == null)
        //    {
        //        routeData.Values.Add("action", "NotAccess");
        //    }
        //    else if (httpException == null)
        //    {
        //        routeData.Values.Add("action", "NotAccess");
        //    }
        //    else
        //    {
        //        switch (httpException.GetHttpCode())
        //        {
        //            case 404:
        //                {
        //                    routeData.Values.Add("action", "NotAccess");
        //                    break;
        //                }
        //            case 500:
        //                routeData.Values.Add("action", "NotAccess");
        //                break;
        //            case 403:
        //                {
        //                    urlReferrer = exception.Message;
        //                    routeData.Values.Add("action", "NoRoot");
        //                    break;
        //                }
        //            default:
        //                routeData.Values.Add("action", "NotAccess");
        //                break;
        //        }
        //    }
        //    // Pass exception details to the target error View.  
        //    routeData.Values.Add("urlReferrer", urlReferrer);
        //    // Clear the error on server.  
        //    Server.ClearError();
        //    // Call target Controller and pass the routeData.  
        //    IController errorController = new YXERP.Controllers.ErrorController();
        //    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        //}

        protected void Application_End(object sender, EventArgs e)
        {
            //结束时记得释放   
            syncreocRecord.OnStop();
        }  
    }
}