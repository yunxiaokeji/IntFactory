using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
namespace YXERP.Areas.Api.Controllers
{
    [YXERP.Common.ApiAuthorize]
    public class CustomerController : BaseAPIController
    {
        //根据客户ID获取客户信息
        public JsonResult GetCustomerByID(string customerID, string clientID)
        {
            var item = CustomBusiness.BaseBusiness.GetCustomerByID(customerID, clientID, clientID);
            if (item != null)
            {
                JsonDictionary.Add("customerID", item.CustomerID);
                JsonDictionary.Add("name", item.Name);
                JsonDictionary.Add("mobilePhone", item.MobilePhone);
                JsonDictionary.Add("agentID", item.AgentID);
                JsonDictionary.Add("clientID", item.ClientID);
                JsonDictionary.Add("yxAgentID", item.YXAgentID);
                JsonDictionary.Add("yxClientID", item.YXClientID);
                JsonDictionary.Add("yxClientCode", item.YXClientCode);
            }
            else
            {
                JsonDictionary.Add("error_code", 10002);
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //通过手机号获取客户信息
        public JsonResult GetCustomerByMobilePhone(string mobilePhone, string clientID,string name="")
        {
            var item = CustomBusiness.BaseBusiness.GetCustomerByMobilePhone(mobilePhone, clientID,name);
            if (item != null)
            {
                JsonDictionary.Add("customerID", item.CustomerID);
                JsonDictionary.Add("name", item.Name);
                JsonDictionary.Add("mobilePhone", item.MobilePhone);
                JsonDictionary.Add("agentID", item.AgentID);
                JsonDictionary.Add("clientID", item.ClientID);
                JsonDictionary.Add("yxAgentID", item.YXAgentID);
                JsonDictionary.Add("yxClientID", item.YXClientID);
                JsonDictionary.Add("yxClientCode", item.YXClientCode);
            }
            else
            {
                JsonDictionary.Add("error_code", 10002);
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        
        //关联客户与二当家联系
        public JsonResult SetCustomerYXinfo(string customerID, string clientID, string yxAgentID, string yxClientID, string yxClientCode)
        {
            var flag = CustomBusiness.BaseBusiness.SetCustomerYXinfo(customerID, clientID, yxAgentID, yxClientID, yxClientCode);
            JsonDictionary.Add("result", flag?1:0);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
