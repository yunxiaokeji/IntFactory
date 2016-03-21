using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using IntFactoryEntity;
using YXERP.Models;
using IntFactoryBusiness;
using IntFactoryEnum;

namespace YXERP.Controllers
{
    public class OrdersController : BaseController
    {
        //
        // GET: /Orders/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyOrder()
        {
            ViewBag.Title = "我的订单";
            ViewBag.Type = (int)EnumSearchType.Myself;
            return View("Orders");
        }

        public ActionResult BranchOrder()
        {
            ViewBag.Title = "下属订单";
            ViewBag.Type = (int)EnumSearchType.Branch;
            return View("Orders");
        }

        public ActionResult Orders()
        {
            ViewBag.Title = "所有订单";
            ViewBag.Type = (int)EnumSearchType.All;
            
            return View("Orders");
        }

        public ActionResult ChooseProducts(string id)
        {
            ViewBag.Type = (int)EnumDocType.Order;
            ViewBag.GUID = id;
            ViewBag.Title = "加工订单-选择材料";
            return View("FilterProducts");
        }

        public ActionResult OrderPlans()
        {
            return View();
        }

        public ActionResult Detail(string id)
        {
            var model = OrdersBusiness.BaseBusiness.GetOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);

            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/Orders/Orders");
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult ApplyReturn(string id)
        {
            var model = OrdersBusiness.BaseBusiness.GetOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);

            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/Orders/Orders");
            }

            if (model.Status != 2 || model.SendStatus == 0 || model.ReturnStatus == 1)
            {
                return Redirect("/Orders/Detail/" + id);
            }

            ViewBag.Model = model;
            return View();
        }

        #region Ajax

        public JsonResult GetOrders(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterOrders model = serializer.Deserialize<FilterOrders>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOrders(model.SearchType, model.TypeID, model.Status, (EnumOrderSourceType)model.SourceType, model.PayStatus, model.InvoiceStatus, model.ReturnStatus, model.UserID, model.TeamID, model.AgentID, model.BeginTime, model.EndTime, model.Keywords, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetDYOrders(string keywords)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOrders(EnumSearchType.All, "1", 3, EnumOrderSourceType.All, -1, -1, -1, "", "", "", "", "", keywords, 10, 1, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrdersByCustomerID(string customerid, int ordertype, int pagesize, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOrdersByCustomerID(customerid, ordertype, pagesize, pageindex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetNeedsOrderByCustomerID(string customerid, int pagesize, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetNeedsOrderByCustomerID(customerid, pagesize, pageindex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderPlans(bool isMy, string beginDate, string endDate, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;
            string ownerID = string.Empty;
            if (isMy)
            {
                ownerID = CurrentUser.UserID;
            }

            List<OrderEntity> list =OrdersBusiness.GetOrderPlans(ownerID, beginDate, endDate, CurrentUser.ClientID, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrder(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderEntity model = serializer.Deserialize<OrderEntity>(entity);

            string orderid = OrdersBusiness.BaseBusiness.CreateOrder(model.CustomerID, model.GoodsCode, model.Title, model.PersonName, model.MobileTele, EnumOrderSourceType.FactoryOrder, (EnumOrderType)model.OrderType, model.BigCategoryID, model.CategoryID, model.PlanPrice, model.PlanQuantity,
                                                                     model.OrderImage, model.CityCode, model.Address, model.ExpressCode, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", orderid);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateDHOrder(string entity, string originalid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderGoodsModel model = serializer.Deserialize<OrderGoodsModel>(entity);

            string orderid = OrdersBusiness.BaseBusiness.CreateDHOrder(model.OrderID, originalid, model.OrderGoods, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", orderid);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderCutOutDoc(string orderid, int doctype, int isover, string expressid, string expresscode, string details, string remark)
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid, (EnumDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderSewnDoc(string orderid, int doctype, int isover, string expressid, string expresscode, string details, string remark)
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid, (EnumDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderSendDoc(string orderid, int doctype, int isover, string expressid, string expresscode, string details, string remark)
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid, (EnumDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderOwner(string ids, string userid)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && OrdersBusiness.BaseBusiness.UpdateOrderOwner(id, userid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }


            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult EditOrder(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderEntity model = serializer.Deserialize<OrderEntity>(entity);


            var bl = OrdersBusiness.BaseBusiness.EditOrder(model.OrderID, model.PersonName, model.MobileTele, model.CityCode, model.Address, model.PostalCode, model.TypeID, model.ExpressType, model.Remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderPrice(string orderid, string autoid, string name, decimal price)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderPrice(orderid, autoid, name, price, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateProductQuantity(string orderid, string autoid, string name, decimal quantity)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateProductQuantity(orderid, autoid, name, quantity, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateProductLoss(string orderid, string autoid, string name, decimal quantity)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateProductLoss(orderid, autoid, name, quantity, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteProduct(string orderid, string autoid, string name)
        {
            var bl = OrdersBusiness.BaseBusiness.DeleteProduct(orderid, autoid, name, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderProcess(string orderid, string processid, string name)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderProcess(orderid, processid, name, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderCategoryID(string orderid, string pid, string categoryid,string name)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderCategoryID(orderid, pid, categoryid, name, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderStatus(string orderid, int status, int quantity, decimal price)
        {
            string errinfo = "";

            var bl = OrdersBusiness.BaseBusiness.UpdateOrderStatus(orderid, (EnumOrderStatus)status, quantity, price, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateProfitPrice(string orderid, decimal profit)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateProfitPrice(orderid, profit, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderClient(string orderid, string clientid, string name)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderClient(orderid, clientid, name, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderOriginalID(string orderid, string originalorderid, string name)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderOriginalID(orderid, originalorderid, name, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderCustomer(string orderid, string customerid, string name)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderCustomer(orderid, customerid, name, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderImages(string orderid, string images)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderImages(orderid, images, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOrder(string orderid)
        {
            var bl = OrdersBusiness.BaseBusiness.DeleteOrder(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderLogs(string orderid, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = LogBusiness.GetLogs(orderid, EnumLogObjectType.Orders, 10, pageindex, ref totalCount, ref pageCount, CurrentUser.AgentID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult EffectiveOrderProduct(string orderid)
        {
            int result = 0;
            var bl = OrdersBusiness.BaseBusiness.EffectiveOrderProduct(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ApplyReturnOrder(string orderid)
        {
            int result = 0;
            var bl = OrdersBusiness.BaseBusiness.ApplyReturnOrder(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateReturnQuantity(string orderid, string autoid, string name, int quantity)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateReturnQuantity(orderid, autoid, name, quantity, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ApplyReturnProduct(string orderid)
        {
            int result = 0;
            var bl = OrdersBusiness.BaseBusiness.ApplyReturnProduct(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetGoodsDocByOrderID(string orderid, int type)
        {
            
            var list = StockBusiness.GetGoodsDocByOrderID(orderid, (EnumDocType)type, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderCustomer(string orderid)
        {
            var bl = OrdersBusiness.BaseBusiness.CreateOrderCustomer(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DownAliOrders(int downOrderType, string startTime, string endTime)
        {
            int result = 0;
            bool flag = false;
            DateTime downStartTime = DateTime.Parse(startTime);
            DateTime downEndTime = DateTime.Parse(endTime);

            if ((downEndTime - downStartTime).Days < 16)
            {
                var plan = AliOrderBusiness.BaseBusiness.GetAliOrderDownloadPlanDetail(CurrentUser.ClientID);

                if (plan != null)
                {
                    int successCount;
                    int total;

                    string token = plan.Token;
                    string refreshToken = plan.RefreshToken;
                    string error;
                    if (downOrderType == 1)
                    {
                        flag = AliOrderBusiness.DownFentOrders(downStartTime, downEndTime, token, refreshToken, CurrentUser.UserID,
                            CurrentUser.AgentID, CurrentUser.ClientID, out successCount, out total,out error, AlibabaSdk.AliOrderDownType.Hand);

                        //新增阿里打样订单下载日志
                        AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog(EnumOrderType.ProofOrder, flag, AlibabaSdk.AliOrderDownType.Hand, downStartTime, downEndTime,
                            successCount, total,error, plan.AgentID, plan.ClientID);
                    }
                    else
                    {

                        flag = AliOrderBusiness.DownBulkOrders(downStartTime, downEndTime, token, refreshToken, CurrentUser.UserID,
                            CurrentUser.AgentID, CurrentUser.ClientID, out successCount, out total,out error, AlibabaSdk.AliOrderDownType.Hand);

                        //新增阿里大货订单下载日志
                        AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog(EnumOrderType.LargeOrder, flag, AlibabaSdk.AliOrderDownType.Hand, downStartTime, downEndTime,
                            successCount, total, plan.AgentID,error, plan.ClientID);
                    }

                    result = flag ? 1 : 0;
                    JsonDictionary.Add("totalOrderCount", total);
                    JsonDictionary.Add("successOrderCount", successCount);
                }
                else
                    result = 2;
            }
            else
                result = 3;

            JsonDictionary.Add("result", result);
            
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderCost(string orderid, decimal price, string remark)
        {
            var bl = OrdersBusiness.BaseBusiness.CreateOrderCost(orderid, price, remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderCosts(string orderid)
        {
            var list = OrdersBusiness.BaseBusiness.GetOrderCosts(orderid, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOrderCost(string orderid, string autoid)
        {
            var bl = OrdersBusiness.BaseBusiness.DeleteOrderCost(orderid, autoid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
