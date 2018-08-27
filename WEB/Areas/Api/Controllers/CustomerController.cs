using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using System.Web.Script.Serialization;
using YXERP.Models;
using IntFactoryEntity;
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

        public JsonResult GetCustomers(string filter, string userID, string clientID)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterCustomer model = serializer.Deserialize<FilterCustomer>(filter);
            int totalCount = 0;
            int pageCount = 0;

            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomers(model.SearchType, model.Type, model.SourceType,
                model.SourceID, model.StageID, model.Status, model.Mark, model.UserID,
                model.TeamID, model.BeginTime, model.EndTime,
                model.FirstName, model.Keywords, model.OrderBy, model.PageSize, model.PageIndex,
                ref totalCount, ref pageCount, userID, clientID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomersByKeywords(string keywords, string userID, string clientID)
        {

            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomersByKeywords(keywords, string.Empty, clientID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
