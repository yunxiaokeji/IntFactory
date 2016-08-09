using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using IntFactoryBusiness;
using IntFactoryEntity;
using IntFactoryEnum;
using Newtonsoft.Json;
namespace YXERP.Areas.Api.Controllers
{
    [YXERP.Common.ApiAuthorize]
    public class OrderController : BaseAPIController
    {
        //获取订单列表根据二当家客户端编码
        public JsonResult GetOrdersByYXClientCode(string yxClientCode,int pageSize, int pageIndex, string clientID = "")
        {
            int totalCount=0, pageCount = 0;
            var list = OrdersBusiness.BaseBusiness.GetOrdersByYXCode(yxClientCode, clientID,pageSize, pageIndex, ref totalCount, ref pageCount);
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

        //获取订单详情
        public JsonResult GetOrderDetailByID(string orderID,string clientID)
        {
            var item = OrdersBusiness.BaseBusiness.GetOrderBaseInfoByID(orderID, clientID);
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

            //材料列表
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
            //制版工艺
            var plateMakings = TaskBusiness.GetPlateMakings(orderID);
            var plates = new List<Dictionary<string, object>>();
            foreach (var p in plateMakings) {
                Dictionary<string, object> plate = new Dictionary<string, object>();
                plate.Add("plateID", p.PlateID);
                plate.Add("icon", p.Icon);
                plate.Add("title", p.Title);
                plate.Add("remark", p.Remark);
                plate.Add("type", p.TypeName);

                plates.Add(plate);
            }
            obj.Add("plateMakings", plates);
            //订单品类
            var category = new ProductsBusiness().GetCategoryByID(item.CategoryID);
            var attrLists = new List<Dictionary<string, object>>();
            var saleAttrs = new List<Dictionary<string, object>>();
            foreach (var attr in category.AttrLists) {
                Dictionary<string, object> attrObj= new Dictionary<string, object>();
                attrObj.Add("attrID", attr.AttrID);
                attrObj.Add("attrName", attr.AttrName);
                var attrValues = new List<Dictionary<string, object>>();
                foreach (var value in attr.AttrValues) {
                    Dictionary<string, object> valueObj = new Dictionary<string, object>();
                    valueObj.Add("valueID", value.ValueID);
                    valueObj.Add("valueName", value.ValueName);

                    attrValues.Add(valueObj);
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
                    valueObj.Add("valueID", value.ValueID);
                    valueObj.Add("valueName", value.ValueName);

                    attrValues.Add(valueObj);
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

        //新建大货订单
        public JsonResult CreateDHOrder(string orderID, decimal price, string details, string clientID, string yxOrderID)
        {
            var productDetails = JsonConvert.DeserializeObject< List<IntFactoryEntity.OrderGoodsEntity > >(details);
            string id = OrdersBusiness.BaseBusiness.CreateDHOrder(orderID, 1, 1, price, productDetails, string.Empty, clientID, yxOrderID);
          JsonDictionary.Add("id",id);

          return new JsonResult
          {
              Data = JsonDictionary,
              JsonRequestBehavior = JsonRequestBehavior.AllowGet
          };
        }

        //
        public JsonResult CreateOrder(string entity, string clientid, string opearid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderEntity model = serializer.Deserialize<OrderEntity>(entity);

            string orderid = OrdersBusiness.BaseBusiness.CreateOrder(model.CustomerID, model.GoodsCode, model.Title, model.PersonName, model.MobileTele, EnumOrderSourceType.FactoryOrder,
                                                                    (EnumOrderType)model.OrderType, model.OrderGoods, model.BigCategoryID, model.CategoryID, model.PlanPrice, model.PlanQuantity, model.PlanTime,
                                                                     model.OrderImage, model.CityCode, model.Address, model.ExpressCode, model.Remark, opearid, clientid);
            JsonDictionary.Add("id", orderid);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取订单流程
        public JsonResult GetOrderProcess(string userID, string clientID)
        {
            var list = SystemBusiness.BaseBusiness.GetOrderProcess(clientID);
            List<Dictionary<string, object>> processss = new List<Dictionary<string, object>>();

            foreach (var item in list)
            {
                Dictionary<string, object> processs = new Dictionary<string, object>();
                processs.Add("processID", item.ProcessID);
                processs.Add("type", item.ProcessType);
                processs.Add("processName", item.ProcessName);
                processss.Add(processs);
            }

            JsonDictionary.Add("processs", processss);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取流程阶段
        public JsonResult GetOrderStages(string processID, string userID, string clientID)
        {
            if (!string.IsNullOrEmpty(processID))
            {
                var list = SystemBusiness.BaseBusiness.GetOrderStages(processID, clientID);
                List<Dictionary<string, object>> stages = new List<Dictionary<string, object>>();

                foreach (var item in list)
                {
                    Dictionary<string, object> stage = new Dictionary<string, object>();
                    stage.Add("stageID", item.StageID);
                    stage.Add("processID", item.ProcessID);
                    stage.Add("stageName", item.StageName);
                    stage.Add("mark", item.Mark);
                    stages.Add(stage);
                }

                JsonDictionary.Add("processStages", stages);
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取大货明细
        public JsonResult GetGoodsDocByOrderID(string orderID, int type, string taskID,string clientID)
        {
            var list = StockBusiness.GetGoodsDocByOrderID(orderID, taskID, (EnumDocType)type, clientID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取订单加工成本
        public JsonResult GetOrderCosts(string orderID, string userID, string clientID)
        {
            var list = OrdersBusiness.BaseBusiness.GetOrderCosts(orderID, clientID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取工艺说明
        public JsonResult GetPlateMakings(string orderID)
        {
            var list = TaskBusiness.GetPlateMakings(orderID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取订单流程
        public JsonResult GetOrderProcess(string userID, string clientID)
        {
            var list = SystemBusiness.BaseBusiness.GetOrderProcess(clientID);
            List<Dictionary<string, object>> processss = new List<Dictionary<string, object>>();

            foreach (var item in list)
            {
                Dictionary<string, object> processs = new Dictionary<string, object>();
                processs.Add("processID", item.ProcessID);
                processs.Add("type", item.ProcessType);
                processs.Add("processName", item.ProcessName);
                processss.Add(processs);
            }

            JsonDictionary.Add("processs", processss);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
