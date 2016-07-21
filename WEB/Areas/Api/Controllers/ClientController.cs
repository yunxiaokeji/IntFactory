using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness.Manage;
namespace YXERP.Areas.Api.Controllers
{
    [YXERP.Common.ApiAuthorize]
    public class ClientController :BaseAPIController
    {
       //获取客户端信息
        public ActionResult GetClientInfo(string clientID)
        {
            var item = ClientBusiness.GetClientDetail(clientID);
            Dictionary<string, object> obj = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(item.ClientID)) {
                obj.Add("clientID", item.ClientID);
                obj.Add("clientCode", item.ClientCode);
                obj.Add("companyName", item.CompanyName);
                obj.Add("logo", item.Logo);
                obj.Add("contactName", item.ContactName);
                obj.Add("mobilePhone", item.MobilePhone);
                obj.Add("address", item.Address);
            }
            JsonDictionary.Add("client",obj);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
