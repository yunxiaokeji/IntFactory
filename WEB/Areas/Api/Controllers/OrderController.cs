using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
namespace YXERP.Areas.Api.Controllers
{
    [YXERP.Common.ApiAuthorize]
    public class OrderController : BaseAPIController
    {
        //获取订单列表根据二当家客户端编码
        public JsonResult GetOrdersByYXClientCode(string yxClientCode, string zngcClientID = "")
        {
            var list= OrdersBusiness.BaseBusiness.GetOrdersByYXCode(yxClientCode, zngcClientID);
            var objs=new List<Dictionary<string, object>>();
            foreach (var item in list) {
                Dictionary<string, object> obj = new Dictionary<string, object>();
                obj.Add("orderID", item.OrderID);
                obj.Add("goodsName", item.GoodsName);
                obj.Add("goodsCode", item.GoodsCode);
                obj.Add("finalPrice", item.FinalPrice);
                obj.Add("orderImage", item.OrderImage);
                obj.Add("orderImages", item.OrderImages);
                obj.Add("createTime", item.CreateTime);
                obj.Add("endTime", item.EndTime);

                objs.Add(obj);
            }
            JsonDictionary.Add("orders",objs);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
