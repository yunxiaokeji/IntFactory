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
using IntFactoryBusiness.Manage;

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

        public ActionResult MyOrder(string id)
        {
            ViewBag.Title = "我的订单";
            ViewBag.Type = (int)EnumSearchType.Myself;
            int State = 1;
            if (!string.IsNullOrEmpty(id))
            {
                if (id.Equals("need", StringComparison.OrdinalIgnoreCase))
                {
                    State = 0;
                }
            }
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Orders).ToList();
            ViewBag.State = State;
            return View("Orders");
        }

        public ActionResult BranchOrder()
        {
            ViewBag.Title = "下属订单";
            ViewBag.Type = (int)EnumSearchType.Branch;
            return View("Orders");
        }

        public ActionResult Orders(string id)
        {
            ViewBag.Title = "所有订单";
            ViewBag.Type = (int)EnumSearchType.All;
            int State = 1;
            if (!string.IsNullOrEmpty(id))
            {
                if (id.Equals("need", StringComparison.OrdinalIgnoreCase))
                {
                    State = 0;
                }
            }
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Orders).ToList();
            ViewBag.State = State;
            return View();
        }

        public ActionResult EntrustOrders(string id)
        {
            ViewBag.Title = "委托订单列表";
            ViewBag.Type = (int)EnumSearchType.Entrust;

            int State = 1;
            if (!string.IsNullOrEmpty(id))
            {
                if (id.Equals("need", StringComparison.OrdinalIgnoreCase))
                {
                    State = 0;
                }
            }
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Orders).ToList();
            ViewBag.State = State;

            return View();
        }

        public ActionResult Create(string cid)
        {
            var list = new ProductsBusiness().GetClientCategorysByPID("", EnumCategoryType.Order, CurrentUser.ClientID);
            ViewBag.CID = cid;
            ViewBag.Items = list;
            return View();
        }

        public ActionResult OrderSuccess(string id)
        {
            var order = OrdersBusiness.BaseBusiness.GetOrderByID(id);
            if (order == null || string.IsNullOrEmpty(order.OrderID))
            {
                return Redirect("/Home/Login");
            }
            ViewBag.Order = order;
            return View();
        }

        public ActionResult OrderDetail(string id)
        {
            var model = OrdersBusiness.BaseBusiness.GetOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);

            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/Orders/Orders");
            }
            if (model.Status == 0 || model.Status == 4)
            {
                ViewBag.Stages = SystemBusiness.BaseBusiness.GetOrderStages(model.ProcessID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            model.IsSelf = model.ClientID == CurrentUser.ClientID;
            ViewBag.Model = model;
            
            if (model.OrderType == 1)
            {
                ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Orders).ToList();
                return View();
            }
            else
            {
                return View("OrderDetailDH");
            }
        }

        public ActionResult OrderLayer(string id)
        {
            var model = OrdersBusiness.BaseBusiness.GetOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);

            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/Orders/Orders");
            }
            if (model.Status == 0)
            {
                ViewBag.Stages = SystemBusiness.BaseBusiness.GetOrderStages(model.ProcessID, CurrentUser.AgentID, CurrentUser.ClientID);
            }

            return PartialView(model);
        }

        public ActionResult ChooseProducts(string id, string tid)
        {
            ViewBag.Type = (int)EnumDocType.Order;
            ViewBag.GUID = id;
            ViewBag.TID = tid;
            ViewBag.Title = "选择材料";
            return View("FilterProducts");
        }

        public ActionResult ChooseMaterial(string id, string tid)
        {
            ViewBag.Type = (int)EnumDocType.Order;
            ViewBag.GUID = id;
            ViewBag.TID = tid;
            ViewBag.Title = "选择材料";
            return View("FilterProducts");
        }

        public ActionResult DocDetail(string id)
        {
            var model = StockBusiness.GetGoodsDocDetail(id, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Orders/Orders");
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult OrderPlans()
        {
            return View();
        }

        public ActionResult FentOrderReport(string id)
        {
            var model = OrdersBusiness.BaseBusiness.GetOrderForFentReport(id, CurrentUser.AgentID, CurrentUser.ClientID);
            var client = ClientBusiness.GetClientDetail(model.ClientID);  
            ViewBag.Model = model;
            ViewBag.Client = client;
            return View();
        }

        public ActionResult PlateMakingProcess(string id)
        {   
            var taskid="";
            var type = 1;
            var model = OrdersBusiness.BaseBusiness.GetOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            var list = StockBusiness.GetGoodsDocByOrderID(id, taskid, (EnumDocType)type, CurrentUser.ClientID);
            ViewBag.Model = model;
            ViewBag.List = list;
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

            var list = OrdersBusiness.BaseBusiness.GetOrders(model.SearchType, model.EntrustClientID, model.TypeID, model.Status, (EnumOrderSourceType)model.SourceType, model.OrderStatus, model.Mark, model.PayStatus, model.InvoiceStatus, model.ReturnStatus, model.UserID, model.TeamID, model.AgentID, model.BeginTime, model.EndTime, model.Keywords, model.OrderBy, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
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

            var list = OrdersBusiness.BaseBusiness.GetOrders(keywords, 10, 1, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrdersByCustomerID(string keyWords, string customerid, int ordertype, int pagesize, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOrdersByCustomerID(keyWords, customerid, ordertype, pagesize, pageindex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrdersByOriginalID(string originalid, int ordertype, int pagesize, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOrdersByOriginalID(originalid, ordertype, pagesize, pageindex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetNeedsOrderByCustomerID(string keyWords, string customerid, int pagesize, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetNeedsOrderByCustomerID(keyWords, customerid, pagesize, pageindex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
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

            string orderid = OrdersBusiness.BaseBusiness.CreateOrder(model.CustomerID, model.GoodsCode, model.Title, model.PersonName, model.MobileTele, EnumOrderSourceType.FactoryOrder,
                                                                    (EnumOrderType)model.OrderType, model.BigCategoryID, model.CategoryID, model.PlanPrice, model.PlanQuantity, model.PlanTime,
                                                                     model.OrderImage, model.CityCode, model.Address, model.ExpressCode, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", orderid);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateDHOrder(string entity, int ordertype, decimal discount, decimal price)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderGoodsModel model = serializer.Deserialize<OrderGoodsModel>(entity);

            string orderid = OrdersBusiness.BaseBusiness.CreateDHOrder(model.OrderID, ordertype, discount, price, model.OrderGoods, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", orderid);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderCutOutDoc(string orderid, int doctype, int isover, string expressid, string expresscode, string details, string remark, string taskid="")
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid,taskid, (EnumGoodsDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderSewnDoc(string orderid, string taskid, int doctype, int isover, string expressid, string expresscode, string details, string remark)
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid,taskid, (EnumGoodsDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderSendDoc(string orderid, string taskid, int doctype, int isover, string expressid, string expresscode, string details, string remark)
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid,taskid, (EnumGoodsDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderSend(string orderid,string taskid, int doctype, int isover, string expressid, string expresscode, string details, string remark)
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid,taskid, (EnumGoodsDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
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

            int result = 0;

            var bl = OrdersBusiness.BaseBusiness.EditOrder(model.OrderID, model.IntGoodsCode, model.GoodsName, model.PersonName, model.MobileTele, model.CityCode, model.Address, model.PostalCode, model.TypeID, model.ExpressType, model.Remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
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

        public JsonResult UpdateOrderStatus(string orderid, int status, string time, decimal price)
        {
            string errinfo = "";
            if (!string.IsNullOrEmpty(time))
            {
                time = Convert.ToDateTime(time).ToString("yyyy-MM-dd") + " 23:59:59";
            }
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderStatus(orderid, (EnumOrderStageStatus)status, time, price, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderMark(string ids, int mark)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && OrdersBusiness.BaseBusiness.UpdateOrderMark(id, mark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
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

        public JsonResult UpdateOrderDiscount(string orderid, decimal discount, decimal price)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderDiscount(orderid, discount, price, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderTotalMoney(string orderid, decimal totalMoney)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderTotalMoney(orderid, totalMoney,
                CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
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

        public JsonResult UpdateOrderOver(string orderid)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderOver(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
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

        public JsonResult GetGoodsDocByOrderID(string orderid, int type, string taskid="")
        {
            var list = StockBusiness.GetGoodsDocByOrderID(orderid,taskid, (EnumDocType)type, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetGoodsDocDetail(string docid)
        {
            var model = StockBusiness.GetGoodsDocDetail(docid, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderCustomer(string orderid)
        {
            int result = 0;
            var bl = OrdersBusiness.BaseBusiness.CreateOrderCustomer(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
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
            bool canDown=true;

            if (canDown)
            {
                DateTime downStartTime = DateTime.Parse(startTime);
                DateTime downEndTime = DateTime.Parse(endTime);
                downEndTime = downEndTime.AddDays(1);

                if ((downEndTime - downStartTime).TotalDays < 61)
                {
                    var plan = AliOrderBusiness.BaseBusiness.GetAliOrderDownloadPlanDetail(CurrentUser.ClientID);

                    if (plan != null)
                    {
                        int successCount = 0;
                        int total = 0;

                        string token = plan.Token;
                        string refreshToken = plan.RefreshToken;
                        string error;
                        if (downOrderType == 1)
                        {
                            flag = AliOrderBusiness.DownFentOrders(downStartTime, downEndTime, token, refreshToken, CurrentUser.UserID,
                                CurrentUser.AgentID, CurrentUser.ClientID, ref successCount, ref total, out error, AlibabaSdk.AliOrderDownType.Hand);

                            //新增阿里打样订单下载日志
                            AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog(EnumOrderType.ProofOrder, flag, AlibabaSdk.AliOrderDownType.Hand, downStartTime, downEndTime,
                                successCount, total, error, plan.AgentID, plan.ClientID);
                        }
                        else
                        {

                            flag = AliOrderBusiness.DownBulkOrders(downStartTime, downEndTime, token, refreshToken, CurrentUser.UserID,
                                CurrentUser.AgentID, CurrentUser.ClientID, ref successCount, ref total, out error, AlibabaSdk.AliOrderDownType.Hand);

                            //新增阿里大货订单下载日志
                            AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog(EnumOrderType.LargeOrder, flag, AlibabaSdk.AliOrderDownType.Hand, downStartTime, downEndTime,
                                successCount, total, plan.AgentID, error, plan.ClientID);
                        }

                        result = flag ? 1 : 0;

                        //拉取订单记录日期
                        if (flag)
                        {
                            LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.PullOrder, EnumLogType.Create, "", OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                        }
                        JsonDictionary.Add("totalOrderCount", total);
                        JsonDictionary.Add("successOrderCount", successCount);

                        //if (flag)
                        //{
                        //    if (AliOrderBusiness.DownAliOrderLogs.ContainsKey(CurrentUser.ClientID))
                        //        AliOrderBusiness.DownAliOrderLogs[CurrentUser.ClientID] = DateTime.Now.AddHours(12);
                        //    else
                        //        AliOrderBusiness.DownAliOrderLogs.Add(CurrentUser.ClientID, DateTime.Now.AddHours(12));
                        //}
                    }
                    else
                        result = 2;
                }
                else
                    result = 3;
            }
            else
                result = 4;

            JsonDictionary.Add("result", result);
            
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetAliInfo()
        {
            var plan= AliOrderBusiness.BaseBusiness.GetAliOrderDownloadPlanDetail(CurrentUser.ClientID);

            if (plan != null)
            {
                JsonDictionary.Add("result", 1);
                JsonDictionary.Add("plan", plan);
                JsonDictionary.Add("downBeginTime", plan.CreateTime.Date<DateTime.Now.AddDays(-15).Date?DateTime.Now.AddDays(-15).Date:plan.CreateTime.Date);
            }
            else
            {
                JsonDictionary.Add("result", 0);
            }

            return new JsonResult()
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

        public JsonResult UpdateTaskLockStatus(string taskid, int status)
        {
            var bl = false;
            int result=0;
            if (status == 2)
            {
                bl = TaskBusiness.UnLockTask(taskid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            }
            else 
            {
                bl = TaskBusiness.LockTask(taskid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /*价格区间设置*/

        public JsonResult GetOrderPriceRanges(string orderid)
        {
            var obj = IntFactoryBusiness.OrdersBusiness.GetOrderPriceRanges(orderid);
            JsonDictionary.Add("items",obj);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SavePriceRange(string model)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderPriceRange models = serializer.Deserialize<OrderPriceRange>(model);
            
            if (string.IsNullOrEmpty(models.RangeID))
            {
                string id = IntFactoryBusiness.OrdersBusiness.AddOrderPriceRange(models, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                JsonDictionary.Add("id", id);                
            }else
            { 
               bool bl = IntFactoryBusiness.OrdersBusiness.UpdateOrderPriceRange(models, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                JsonDictionary.Add("id", bl?"1":"");                
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };            
        }
        
        public JsonResult DeleteOrderPriceRange(string rangeid)
        {
            var status = IntFactoryBusiness.OrdersBusiness.DeleteOrderPriceRange(rangeid);
            JsonDictionary.Add("status",status);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
