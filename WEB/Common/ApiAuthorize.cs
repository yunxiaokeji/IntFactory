using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryEntity;
using IntFactoryBusiness;
using IntFactoryEnum;
using Newtonsoft.Json;
namespace YXERP.Common
{
    public class ApiAuthorize : AuthorizeAttribute
    {
        public static string AppKey = "BC6802E9-285C-471C-8172-3867C87803E2";
        public static string AppSecret = "9F8AF979-8A3B-4E23-B19C-AB8702988466";

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string signature = HttpContext.Current.Request["signature"];
            string userID = HttpContext.Current.Request["userID"];

            if (!string.IsNullOrEmpty(signature) && !string.IsNullOrEmpty(userID) )
            {
                if ( signature.Equals(Signature.GetSignature(AppKey,AppSecret,userID),StringComparison.OrdinalIgnoreCase) )
                {
                    return true;
                }
                
            }

            httpContext.Response.StatusCode = 401;
            return false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 401)
            {
                Dictionary<String, object> jsonDictionary = new Dictionary<string, object>();
                jsonDictionary.Add("error_code",401);

                filterContext.RequestContext.HttpContext.Response.Write( JsonConvert.SerializeObject(jsonDictionary).ToString() );
                filterContext.RequestContext.HttpContext.Response.End();

            }
        }


    }
}