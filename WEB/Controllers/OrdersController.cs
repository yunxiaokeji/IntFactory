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

            var list = OrdersBusiness.BaseBusiness.GetOrders(model.SearchType, model.TypeID, model.Status, model.PayStatus, model.InvoiceStatus, model.ReturnStatus, model.UserID, model.TeamID, model.AgentID, model.BeginTime, model.EndTime, model.Keywords, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrdersByCustomerID(string customerid, int pagesize, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOrdersByCustomerID(customerid, pagesize, pageindex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOpportunityaByCustomerID(string customerid, int pagesize, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOpportunityaByCustomerID(customerid, pagesize, pageindex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
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
                                                                     model.OrderImage, model.CityCode, model.Address, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
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

        public JsonResult CreateOrderGoodsDoc(string orderid, int doctype, int isover, string expressid, string expresscode, string details, string remark)
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

        string token = "d3e257aa-4955-404e-a094-96f7d59a89a5";
        public JsonResult DownAliOrders()
        {
            //bool flag= DownFentOrders(DateTime.Now.AddMonths(-3), DateTime.Now, token,CurrentUser.UserID,CurrentUser.AgentID,CurrentUser.ClientID);

            bool flag = DownBulkOrders(DateTime.Now.AddMonths(-3), DateTime.Now, token, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("result", flag?1:0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public static bool DownFentOrders(DateTime gmtFentStart, DateTime gmtFentEnd, string token, string userID, string agentID, string clientID)
        {
            AlibabaSdk.GoodsCodesResult goodsCodesResult =AlibabaSdk.OrderBusiness.pullFentGoodsCodes(gmtFentStart, gmtFentEnd, token);

            //获取打样订单编码失败
            if (goodsCodesResult.error_code > 0)
            {
                return false;
            }
            else
            {
                var List = goodsCodesResult.goodsCodeList;
                int numb = 10;
                int size = (int)Math.Ceiling((decimal)List.Count / numb);

                Dictionary<string, int> totalCount = new Dictionary<string, int>();
                totalCount.Add("total", List.Count);
                totalCount.Add("successCount", 0);
                if (AlibabaSdk.CacheBusiness.SuccessOrderCountCache.ContainsKey(agentID))
                {
                    AlibabaSdk.CacheBusiness.SuccessOrderCountCache[agentID] = totalCount;
                }
                else
                {
                    AlibabaSdk.CacheBusiness.SuccessOrderCountCache.Add(agentID, totalCount);
                }

                for (int i = 1; i <= size; i++)
                {
                    var qList = List.Skip((i - 1) * numb).Take(numb).ToList();

                    AlibabaSdk.OrderListResult orderListResult = AlibabaSdk.OrderBusiness.pullFentDataList(qList, token);
                    //根据订单编码获取打样订单列表失败
                    if (orderListResult.error_code > 0)
                    {
                        return false;
                    }
                    else
                    {
                        int len = orderListResult.fentOrderList.Count;
                        for (var j = 0; j < len; j++)
                        {
                            var order = orderListResult.fentOrderList[j];

                            string orderID= OrdersBusiness.BaseBusiness.CreateOrder(string.Empty, order.productCode, order.title,
                                order.buyerName, order.buyerMobile, EnumOrderSourceType.AliOrder, EnumOrderType.ProofOrder, string.Empty, string.Empty,
                                order.fentPrice.ToString(), order.bulkCount, order.samplePicList == null ? string.Empty : string.Join(",", order.samplePicList.ToArray()),
                                string.Empty, "dizhi", string.Empty,
                                userID, agentID, clientID);

                            if (string.IsNullOrEmpty(orderID))
                            {
                                return false;
                            }
                            else
                            {
                                Dictionary<string, int> totalCount2 =AlibabaSdk.CacheBusiness.SuccessOrderCountCache[agentID];
                                totalCount2["successCount"] += 1;
                                AlibabaSdk.CacheBusiness.SuccessOrderCountCache[agentID] = totalCount2;
                            }

                        }

                        return true;
                    }
                }


            }
            return true;
        }

        public static bool DownBulkOrders(DateTime gmtBulkStart, DateTime gmtBulkEnd, string token, string userID, string agentID, string clientID)
        {
            AlibabaSdk.GoodsCodesResult goodsCodesResult = AlibabaSdk.OrderBusiness.pullBulkGoodsCodes(gmtBulkStart, gmtBulkEnd, token);

            //获取打样订单编码失败
            if (goodsCodesResult.error_code > 0) return false;
            else
            {
                var List = goodsCodesResult.goodsCodeList;
                if (List.Count == 0) return false;
                int numb = 10;
                int size = (int)Math.Ceiling((decimal)List.Count / numb);

                Dictionary<string, int> totalCount = new Dictionary<string, int>();
                totalCount.Add("total", List.Count);
                totalCount.Add("successCount", 0);
                if (AlibabaSdk.CacheBusiness.SuccessOrderCountCache.ContainsKey(agentID))
                {
                    AlibabaSdk.CacheBusiness.SuccessOrderCountCache[agentID] = totalCount;
                }
                else
                {
                    AlibabaSdk.CacheBusiness.SuccessOrderCountCache.Add(agentID, totalCount);
                }

                for (int i = 1; i <= size; i++)
                {
                    var qList = List.Skip((i - 1) * numb).Take(numb).ToList();

                    AlibabaSdk.OrderListResult orderListResult = AlibabaSdk.OrderBusiness.pullBulkDataList(qList, token);
                    //根据订单编码获取打样订单列表失败
                    if (orderListResult.error_code > 0) return false;
                    else
                    {
                        int len = orderListResult.bulkOrderList.Count;
                        for (var j = 0; j < len; j++)
                        {
                            var order = orderListResult.bulkOrderList[j];

                            string orderID = OrdersBusiness.BaseBusiness.CreateOrder(string.Empty, order.productCode, order.title,
                                order.buyerName, order.buyerMobile, EnumOrderSourceType.AliOrder, EnumOrderType.ProofOrder, string.Empty, string.Empty,
                                order.fentPrice.ToString(), order.bulkCount, order.samplePicList == null ? string.Empty : string.Join(",", order.samplePicList.ToArray()),
                                string.Empty, "dizhi", string.Empty,
                                userID, agentID, clientID);

                            //新增订单失败
                            if (string.IsNullOrEmpty(orderID)) return false;
                            else
                            {
                                Dictionary<string, int> totalCount2 = AlibabaSdk.CacheBusiness.SuccessOrderCountCache[agentID];
                                totalCount2["successCount"] += 1;
                                AlibabaSdk.CacheBusiness.SuccessOrderCountCache[agentID] = totalCount2;
                            }

                        }
                    }

                }

            }

            return true;
        }

        public JsonResult GetSuccessOrderCount()
        {
            int result = 0;
            int totalOrderCount = 0;
            int successOrderCount = 0;
            if (AlibabaSdk.CacheBusiness.SuccessOrderCountCache.ContainsKey(CurrentUser.AgentID))
            {
                Dictionary<string, int> totalCount = AlibabaSdk.CacheBusiness.SuccessOrderCountCache[CurrentUser.AgentID];
                totalOrderCount = totalCount["total"];
                successOrderCount = totalCount["successCount"];

                if (totalOrderCount==successOrderCount)
                    AlibabaSdk.CacheBusiness.SuccessOrderCountCache.Remove(CurrentUser.AgentID);

                result = 1;
            }
            else
            {
                result = 2;
 
            }

            JsonDictionary.Add("result", result);
            JsonDictionary.Add("totalOrderCount", totalOrderCount);
            JsonDictionary.Add("successOrderCount", successOrderCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
