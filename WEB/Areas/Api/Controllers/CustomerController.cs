﻿using System;
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
            var item = CustomBusiness.BaseBusiness.GetCustomerByID(customerID, clientID);
            if (item != null)
            {
                Dictionary<string, object> customer = new Dictionary<string, object>();
                customer.Add("customerID", item.CustomerID);
                customer.Add("name", item.Name);
                customer.Add("mobilePhone", item.MobilePhone);
                customer.Add("clientID", item.ClientID);
                customer.Add("yxAgentID", item.YXAgentID);
                customer.Add("yxClientID", item.YXClientID);
                customer.Add("yxClientCode", item.YXClientCode);

                JsonDictionary.Add("customer", customer);
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
        public JsonResult SetCustomerYXinfo(string customerID,string name, string mobilePhone, string clientID, string yxAgentID, string yxClientID, string yxClientCode)
        {
            var flag = CustomBusiness.BaseBusiness.SetCustomerYXinfo(customerID,name, mobilePhone, clientID, yxAgentID, yxClientID, yxClientCode);
            JsonDictionary.Add("result", flag?1:0);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
