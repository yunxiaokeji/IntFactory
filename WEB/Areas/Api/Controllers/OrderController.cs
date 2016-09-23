using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using IntFactoryBusiness;
using IntFactoryBusiness.Manage;
using IntFactoryEntity;
using IntFactoryEnum;
using Newtonsoft.Json;
namespace YXERP.Areas.Api.Controllers
{
    [YXERP.Common.ApiAuthorize]
    public class OrderController : BaseAPIController
    {
        //获取订单列表根据二当家客户端编码
        public JsonResult GetOrdersByYXClientCode(int pageSize, int pageIndex, string clientID = "", string keywords = "",
            string categoryID = "", string orderby = "", string beginPrice = "", string endPrice = "")
        {
            int totalCount=0, pageCount = 0;
            var list = OrdersBusiness.BaseBusiness.GetOrdersByYXCode(clientID, keywords, pageSize, pageIndex, ref totalCount, ref pageCount,
                categoryID, orderby, beginPrice, endPrice);
            var objs=new List<Dictionary<string, object>>();
            foreach (var item in list) 
            {
                Dictionary<string, object> obj = new Dictionary<string, object>();
                obj.Add("orderID", item.OrderID);
                obj.Add("goodsName", item.GoodsName);
                obj.Add("categoryID", item.CategoryID);
                obj.Add("categoryName", item.CategoryName);
                obj.Add("processCategoryName", item.ProcessCategoryName);
                obj.Add("intGoodsCode", item.IntGoodsCode);
                obj.Add("finalPrice", item.FinalPrice);
                obj.Add("orderImage", item.OrderImage);
                obj.Add("orderImages", item.OrderImages);
                obj.Add("createTime", item.CreateTime);
                obj.Add("endTime", item.EndTime);
                obj.Add("clientID", item.ClientID);
                obj.Add("goodsID", item.GoodsID);
                //obj.Add("logo", item.Client.Logo);
                //obj.Add("clientName", item.Client.CompanyName);
                //obj.Add("clientContactName", item.Client.ContactName);
                //obj.Add("clientCode", item.Client.ClientCode); 
                //obj.Add("clientMobile", item.Client.MobilePhone);
                //obj.Add("clientAddress", item.Client.Address);
                //obj.Add("clientCityCode", item.Client.CityCode);
                //obj.Add("clientUserNum", "0-50人");
                //obj.Add("clientUserLables", "金牌工厂，深度验厂,交期保障");
                //obj.Add("clientCity", item.Client.City != null ? item.Client.City.City + item.Client.City.Counties : ""); 
                //obj.Add("orderAttrs", OrdersBusiness.BaseBusiness.GetOrderArrrsByOrderID(item.OrderID));
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
            var item = OrdersBusiness.BaseBusiness.GetOrderByIDForApi(orderID, clientID);
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("orderID", item.OrderID);
            obj.Add("goodsName", item.GoodsName);
            obj.Add("intGoodsCode", item.IntGoodsCode);
            obj.Add("finalPrice", item.FinalPrice);
            obj.Add("orderImage", item.OrderImage);
            obj.Add("orderImages", item.OrderImages);
            obj.Add("categoryID", item.CategoryID);
            obj.Add("categoryName", item.CategoryName);
            obj.Add("processCategoryName", item.ProcessCategoryName);
            obj.Add("platemaking", item.Platemaking);
            obj.Add("createTime", item.CreateTime);
            obj.Add("endTime", item.EndTime);
            obj.Add("clientID", item.ClientID);
            //var client = ClientBusiness.GetClientDetail(item.ClientID);
            //obj.Add("clientName", client.CompanyName);
            //obj.Add("clientCode", client.ClientCode);
            //obj.Add("clientContactName", client.ContactName);
            //obj.Add("clientMobile", client.MobilePhone);
            //obj.Add("clientAddress", client.City!=null?client.City.Description:"--");
            //obj.Add("clientCityCode", client.CityCode);
            obj.Add("goodsID", item.GoodsID);
            obj.Add("orderAttrs", item.OrderAttrs);

            //订单品类
            var category = new ProductsBusiness().GetCategoryByID(item.CategoryID);
            var attrLists = new List<Dictionary<string, object>>();
            var saleAttrs = new List<Dictionary<string, object>>(); 
            Dictionary<string, object> cmAttr = new Dictionary<string, object>();
            cmAttr.Add("AttrID", category.AttrLists[0].AttrID);
            cmAttr.Add("AttrName", "尺码");
            var cmlist=new List<Dictionary<string, object>>();
            item.OrderAttrs.Where(x => x.AttrType == 1).ToList().ForEach(x =>
            {
                Dictionary<string, object> valueObj = new Dictionary<string, object>();
                valueObj.Add("ValueID", x.OrderAttrID);
                valueObj.Add("ValueName", x.AttrName.Replace("【", "").Replace("】",""));
                cmlist.Add(valueObj);
            });
            if (cmlist.Any())
            {
                cmAttr.Add("AttrValues",cmlist);
                attrLists.Add(cmAttr);
            }
            Dictionary<string, object> ysAttr = new Dictionary<string, object>();
            ysAttr.Add("AttrID", category.SaleAttrs[0].AttrID);
            ysAttr.Add("AttrName", "颜色");
            var yslist = new List<Dictionary<string, object>>();
            item.OrderAttrs.Where(x => x.AttrType == 2).ToList().ForEach(x =>
            {
                Dictionary<string, object> valueObj = new Dictionary<string, object>();
                valueObj.Add("ValueID", x.OrderAttrID);
                valueObj.Add("ValueName", x.AttrName.Replace("【", "").Replace("】", ""));
                yslist.Add(valueObj);
            });
            if (yslist.Any())
            {
                ysAttr.Add("AttrValues", yslist);
                saleAttrs.Add(ysAttr);
            }
            obj.Add("AttrLists", attrLists);
            obj.Add("SaleAttrs", saleAttrs);

            var rangs= OrdersBusiness.GetOrderPriceRanges(orderID);
            var rangArr = new List<Dictionary<string, object>>();
            foreach (var rang in rangs) {
                var rangObj = new Dictionary<string, object>();
                rangObj.Add("rangeID", rang.RangeID);
                rangObj.Add("minQuantity", rang.MinQuantity);
                rangObj.Add("price", rang.Price);
                rangArr.Add(rangObj);
            }
            obj.Add("orderPriceRanges", rangArr);
            JsonDictionary.Add("order",obj);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderAttrsListByOrderID(string orderID)
        {
            var list = OrdersBusiness.BaseBusiness.GetOrderArrrsByOrderID(orderID);
            JsonDictionary.Add("attrList", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderAttrsListByGoodsID(string goodsID)
        {
            var list = OrdersBusiness.BaseBusiness.GetOrderArrrsByGoodsID(goodsID);
            JsonDictionary.Add("attrList", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //新建大货订单
        public JsonResult CreateDHOrder(string orderID, decimal price, string details, string clientID, string yxOrderID,string yxClientID="",string personname="",string mobiletele="",string citycode="",string address="")
        {
            var productDetails = JsonConvert.DeserializeObject< List<IntFactoryEntity.OrderGoodsEntity > >(details);
            string id = OrdersBusiness.BaseBusiness.CreateDHOrder(orderID, 2, true, 1, price, productDetails, string.Empty, clientID, yxOrderID, yxClientID, personname, mobiletele, citycode, address);
            JsonDictionary.Add("id", id);

          return new JsonResult
          {
              Data = JsonDictionary,
              JsonRequestBehavior = JsonRequestBehavior.AllowGet
          };
        }

        //获取大货下单明细
        public JsonResult GetOrderGoods(string orderID)
        {
            List<OrderGoodsEntity> items = IntFactoryBusiness.OrdersBusiness.BaseBusiness.GetOrderGoods(orderID);
            JsonDictionary.Add("items", items);
            return new JsonResult
            {
                Data = items,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //
        public JsonResult CreateOrder(string entity, string clientid, string opearid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderEntity model = serializer.Deserialize<OrderEntity>(entity);
            //string qiniuImgUrl =Common.Common.UploadAttachment(model.OrderImage);
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

        //获取订单详情
        public JsonResult GetOrderDetailsByOrderID(string orderID)
        {
            var list = OrdersBusiness.BaseBusiness.GetOrderDetailsByOrderID(orderID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
