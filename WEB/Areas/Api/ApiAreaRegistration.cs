using System.Web.Mvc;

namespace YXERP.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Api_default",
                "Api/{controller}/{action}/{id}",
                
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "YXERP.Areas.Api.Controllers" }
            );
        }
    }
}
