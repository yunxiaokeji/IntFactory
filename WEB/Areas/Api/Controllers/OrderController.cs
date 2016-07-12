using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEntity;
using Newtonsoft.Json;
namespace YXERP.Areas.Api.Controllers
{
    [YXERP.Common.ApiAuthorize]
    public class OrderController : BaseAPIController
    {
        //获取订单列表根据二当家客户端编码
        public JsonResult GetOrdersByYXClientCode(string yxClientCode,int pageSize, int pageIndex, string zngcClientID = "")
        {
            int totalCount=0, pageCount = 0;
            var list = OrdersBusiness.BaseBusiness.GetOrdersByYXCode(yxClientCode, zngcClientID,pageSize, pageIndex, ref totalCount, ref pageCount);
            var objs=new List<Dictionary<string, object>>();
            foreach (var item in list) {
                Dictionary<string, object> obj = new Dictionary<string, object>();
                obj.Add("orderID", item.OrderID);
                obj.Add("goodsName", item.GoodsName);
                obj.Add("intGoodsCode", item.IntGoodsCode);
                obj.Add("finalPrice", item.FinalPrice);
                obj.Add("orderImage", item.OrderImage);
                obj.Add("orderImages", item.OrderImages);
                obj.Add("createTime", item.CreateTime);
                obj.Add("endTime", item.EndTime);
                obj.Add("clientID", item.ClientID);

                objs.Add(obj);
            }
            JsonDictionary.Add("orders",objs);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderDetailByID(string orderID,string clientID)
        {
            var item = OrdersBusiness.BaseBusiness.GetOrderBaseInfoByID(orderID, clientID, clientID);
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("orderID", item.OrderID);
            obj.Add("goodsName", item.GoodsName);
            obj.Add("intGoodsCode", item.IntGoodsCode);
            obj.Add("finalPrice", item.FinalPrice);
            obj.Add("orderImage", item.OrderImage);
            obj.Add("orderImages", item.OrderImages);
            obj.Add("categoryID", item.CategoryID);
            obj.Add("categoryName", item.CategoryName);
            obj.Add("platemaking", item.Platemaking);
            obj.Add("createTime", item.CreateTime);
            obj.Add("endTime", item.EndTime);
            obj.Add("clientID", item.ClientID);

            var details = new List<Dictionary<string, object>>();
            foreach (var d in item.Details) {
                Dictionary<string, object> detail = new Dictionary<string, object>();
                detail.Add("detailsCode", d.DetailsCode);
                detail.Add("imgS", d.ImgS);
                detail.Add("price", d.Price);
                detail.Add("unitName", d.UnitName);
                detail.Add("productCode", d.ProductCode);
                detail.Add("productName", d.ProductName);
                detail.Add("productImage", d.ProductImage);

                details.Add(detail);
            }
            obj.Add("details", details);

            var plateMakings = TaskBusiness.GetPlateMakings(orderID);
            var plates = new List<Dictionary<string, object>>();
            foreach (var p in plateMakings) {
                Dictionary<string, object> plate = new Dictionary<string, object>();
                plate.Add("plateID", p.PlateID);
                plate.Add("icon", p.Icon);
                plate.Add("title", p.Title);
                plate.Add("remark", p.Remark);
                plate.Add("type", p.Type);

                plates.Add(plate);
            }
            obj.Add("plateMakings", plates);

            var category = new ProductsBusiness().GetOrderCategoryDetailsByID(item.CategoryID, item.OrderID);
            var attrLists = new List<Dictionary<string, object>>();
            var saleAttrs = new List<Dictionary<string, object>>();
            foreach (var attr in category.AttrLists) {
                Dictionary<string, object> attrObj= new Dictionary<string, object>();
                attrObj.Add("attrID", attr.AttrID);
                attrObj.Add("attrName", attr.AttrName);
                var attrValues = new List<Dictionary<string, object>>();
                foreach (var value in attr.AttrValues) {
                    Dictionary<string, object> valueObj = new Dictionary<string, object>();
                    attrObj.Add("valueID", value.ValueID);
                    attrObj.Add("valueName", value.ValueName);

                    attrValues.Add(attrObj);
                }
                attrObj.Add("attrValues", attrValues);
                attrLists.Add(attrObj);
            }
            foreach (var attr in category.SaleAttrs)
            {
                Dictionary<string, object> attrObj = new Dictionary<string, object>();
                attrObj.Add("attrID", attr.AttrID);
                attrObj.Add("attrName", attr.AttrName);
                var attrValues = new List<Dictionary<string, object>>();
                foreach (var value in attr.AttrValues)
                {
                    Dictionary<string, object> valueObj = new Dictionary<string, object>();
                    attrObj.Add("valueID", value.ValueID);
                    attrObj.Add("valueName", value.ValueName);

                    attrValues.Add(attrObj);
                }
                attrObj.Add("attrValues", attrValues);
                saleAttrs.Add(attrObj);
            }
            obj.Add("attrLists", attrLists);
            obj.Add("saleAttrs", saleAttrs);
            JsonDictionary.Add("order",obj);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateDHOrder(string orderID, decimal price, string details, string clientID, string yxOrderID)
        {
            var productDetails = JsonConvert.DeserializeObject< List<IntFactoryEntity.ProductDetail > >(details);
            string id = OrdersBusiness.BaseBusiness.CreateDHOrder(orderID, 2, 1, price, productDetails,
                string.Empty, clientID, clientID);
          JsonDictionary.Add("id",id);

          return new JsonResult
          {
              Data = JsonDictionary,
              JsonRequestBehavior = JsonRequestBehavior.AllowGet
          };
        }



    }
}
