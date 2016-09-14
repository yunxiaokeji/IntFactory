using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntFactoryBusiness;
using IntFactoryBusiness.Manage;
using IntFactoryEntity;
using IntFactoryEnum;

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
            
            if (item!=null && !string.IsNullOrEmpty(item.ClientID)) {
                obj.Add("clientID", item.ClientID);
                obj.Add("clientCode", item.ClientCode);
                obj.Add("companyName", item.CompanyName);
                obj.Add("logo", item.Logo);
                obj.Add("cityCode", item.CityCode);
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
        public ActionResult GetAllCategorys(int layerid = -1, EnumCategoryType type = EnumCategoryType.All)
        {
            List<Category> obj = ProductsBusiness.BaseBusiness.GetCategorys();
            if (layerid > -1)
            {
                obj = obj.Where(x => x.Layers == layerid).ToList();
            }
            if (type != EnumCategoryType.All)
            {
                int t=(int) type;
                obj = obj.Where(x => x.CategoryType == t).ToList();
            }
            JsonDictionary.Add("result", obj);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        //获取分类
        public ActionResult GetClientCategorys(string categoryID, EnumCategoryType type)
        {
            List<Category> obj = ProductsBusiness.BaseBusiness.GetChildCategorysByID(categoryID, EnumCategoryType.Order);
            JsonDictionary.Add("result",obj);
                return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        //根据categoryID获取分类
        public ActionResult GetCategoryID(string categoryID)
        {
            Category obj = ProductsBusiness.BaseBusiness.GetCategoryByID(categoryID);
            JsonDictionary.Add("result", obj);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        //获取加工品类
        public ActionResult GetProcessCategorys(string clientID)
        {
            List<ProcessCategory> obj = ProductsBusiness.BaseBusiness.GetClientProcessCategorys(clientID);
            JsonDictionary.Add("result", obj);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
