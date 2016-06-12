using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEnum;
using IntFactoryDAL;
using IntFactoryEntity;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using IntFactoryBusiness.Manage;

namespace IntFactoryBusiness
{
    public class OrdersBusiness
    {
        public static OrdersBusiness BaseBusiness = new OrdersBusiness();

        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public string FILEPATH = CloudSalesTool.AppSettings.Settings["UploadFilePath"] + "Orders/" + DateTime.Now.ToString("yyyyMM") + "/";
        public string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];

        #region 查询

        public List<OrderEntity> GetOpportunitys(EnumSearchType searchtype, string typeid, string stageid, string searchuserid, string searchteamid, string searchagentid,
                                  string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataSet ds = OrdersDAL.BaseProvider.GetOpportunitys((int)searchtype, typeid, stageid, searchuserid, searchteamid, searchagentid, begintime, endtime, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                list.Add(model);
            }
            return list;
        }


        public List<OrderEntity> GetOrders(EnumSearchType searchtype, string entrustClientID, string typeid, int status, EnumOrderSourceType sourceType, int orderStatus, int mark, int paystatus, int invoicestatus, int returnstatus, string searchuserid, string searchteamid, string searchagentid,
                                                string begintime, string endtime, string keyWords, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataSet ds = OrdersDAL.BaseProvider.GetOrders((int)searchtype, entrustClientID, typeid, status, (int)sourceType, orderStatus, mark, paystatus, invoicestatus, returnstatus, searchuserid, searchteamid, searchagentid, begintime, endtime, keyWords,
                                                         orderBy, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                model.SourceTypeStr = CommonBusiness.GetEnumDesc((EnumOrderSourceType)model.SourceType);

                if (model.OrderStatus == 1)
                {
                    if (model.PlanTime <= DateTime.Now)
                    {
                        model.WarningStatus = 2;
                        model.WarningTime = "超期：" + (DateTime.Now - model.PlanTime).Days.ToString("D2") + "天 " + (DateTime.Now - model.PlanTime).Hours.ToString("D2") + "时 " + (DateTime.Now - model.PlanTime).Minutes.ToString("D2") + "分";
                    }
                    else if ((model.PlanTime - DateTime.Now).TotalHours * 3 < (model.PlanTime - model.OrderTime).TotalHours)
                    {
                        model.WarningStatus = 1;
                        model.WarningTime = "剩余：" + (model.PlanTime - DateTime.Now).Days.ToString("D2") + "天 " + (model.PlanTime - DateTime.Now).Hours.ToString("D2") + "时 " + (model.PlanTime - DateTime.Now).Minutes.ToString("D2") + "分";
                    }
                    else
                    {
                        model.WarningTime = "剩余：" + (model.PlanTime - DateTime.Now).Days.ToString("D2") + "天 " + (model.PlanTime - DateTime.Now).Hours.ToString("D2") + "时 " + (model.PlanTime - DateTime.Now).Minutes.ToString("D2") + "分";
                    }
                }


                list.Add(model);
            }
            return list;
        }

        public List<OrderEntity> GetOrders(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            string where = " ClientID='" + clientid + "' and  OrderType=1 and Status= " + (int)EnumOrderStageStatus.FYFJ;
            if (!string.IsNullOrEmpty(keyWords))
            {
                where += "and (OrderCode like '%" + keyWords + "%' or Title like '%" + keyWords + "%' or PersonName like '%" + keyWords + "%' or IntGoodsCode like '%" + keyWords + "%')";
            }
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", where, "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                model.SourceTypeStr = CommonBusiness.GetEnumDesc((EnumOrderSourceType)model.SourceType);

                list.Add(model);
            }
            return list;
        }

        public List<OrderEntity> GetOrdersByMobilePhone(string mobilePhone)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataSet ds = OrdersDAL.BaseProvider.GetOrdersByMobilePhone(mobilePhone);

            DataTable dt = ds.Tables["Orders"];
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                //model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                model.SourceTypeStr = CommonBusiness.GetEnumDesc((EnumOrderSourceType)model.SourceType);

                model.Client = IntFactoryBusiness.Manage.ClientBusiness.GetClientDetail(model.ClientID);

                model.StatusItems = new List<OrderStatusEntity>();
                DataTable orderStatus = ds.Tables["Status"];
                if (model.Status > 0 && orderStatus.Rows.Count > 0)
                {
                    foreach (DataRow statu in orderStatus.Select("OrderID='" + model.OrderID + "'"))
                    {
                        OrderStatusEntity status = new IntFactoryEntity.OrderStatusEntity();
                        status.FillData(statu);
                        model.StatusItems.Add(status);
                    }
                }
                list.Add(model);
            }
            return list;
        }

        public List<OrderEntity> GetOrdersByPlanTime(string startPlanTime, string endPlanTime, int orderType, int filterType, string userID, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataTable dt = OrdersDAL.BaseProvider.GetOrdersByPlanTime(startPlanTime, endPlanTime, orderType, filterType, userID, clientID,pageSize,pageIndex,ref totalCount,ref pageCount);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);
                if (model.OrderStatus == 1)
                {
                    if (model.PlanTime <= DateTime.Now)
                    {
                        model.WarningStatus = 2;
                        model.WarningTime = "超期：" + (DateTime.Now - model.PlanTime).Days.ToString("D2") + "天 " + (DateTime.Now - model.PlanTime).Hours.ToString("D2") + "时 " + (DateTime.Now - model.PlanTime).Minutes.ToString("D2") + "分";
                        model.WarningDays = (DateTime.Now - model.PlanTime).Days;
                        model.UseDays = (model.PlanTime - model.OrderTime).Days;
                    }
                    else if ((model.PlanTime - DateTime.Now).TotalHours * 3 < (model.PlanTime - model.OrderTime).TotalHours)
                    {
                        model.WarningStatus = 1;
                        model.WarningTime = "剩余：" + (model.PlanTime - DateTime.Now).Days.ToString("D2") + "天 " + (model.PlanTime - DateTime.Now).Hours.ToString("D2") + "时 " + (model.PlanTime - DateTime.Now).Minutes.ToString("D2") + "分";
                        model.WarningDays = (model.PlanTime - DateTime.Now).Days;
                        model.UseDays = (DateTime.Now - model.OrderTime).Days;
                    }
                    else
                    {
                        model.WarningTime = "剩余：" + (model.PlanTime - DateTime.Now).Days.ToString("D2") + "天 " + (model.PlanTime - DateTime.Now).Hours.ToString("D2") + "时 " + (model.PlanTime - DateTime.Now).Minutes.ToString("D2") + "分";
                        model.WarningDays = (model.PlanTime - DateTime.Now).Days;
                        model.UseDays = (DateTime.Now - model.OrderTime).Days;
                    }
                }
                else
                {
                    model.UseDays = (model.PlanTime - model.OrderTime).Days;
                }
                list.Add(model);
            }
            return list;
        }

        public int GetNeedOrderCount(string ownerID,int orderType, string clientID)
        {
            return OrdersDAL.BaseProvider.GetNeedOrderCount(ownerID, orderType, clientID);
        }

        public List<OrderEntity> GetOrdersByCustomerID(string keyWords, string customerid, int ordertype, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            string condition="CustomerID='" + customerid + "' and OrderType=" + ordertype + " and Status<>9 and Status<>0";
            if (!string.IsNullOrEmpty(keyWords))
            {
                condition += " and ( OrderCode like '%" + keyWords + "%' or GoodsCode like '%" + keyWords + "%' or MobileTele like '%" + keyWords + "%' or PersonName like '%" + keyWords + "%')";
            }
            
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", condition, "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                list.Add(model);
            }
            return list;
        }

        public List<OrderEntity> GetOrdersByOriginalID(string originalid, int ordertype, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", "OriginalID='" + originalid + "' and OrderType=" + ordertype + " and Status<>9 ", "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                list.Add(model);
            }
            return list;
        }

        public List<OrderEntity> GetNeedsOrderByCustomerID(string keyWords,string customerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            string condition="CustomerID='" + customerid + "' and Status = 0 ";
            if (!string.IsNullOrEmpty(keyWords))
            {
                condition += " and ( OrderCode like '%" + keyWords + "%' or GoodsCode like '%" + keyWords + "%' or MobileTele like '%" + keyWords + "%' or PersonName like '%" + keyWords + "%')";
            }
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*",condition , "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                list.Add(model);
            }
            return list;
        }

        public OrderEntity GetOrderByID(string orderid)
        {
            DataTable dt = OrdersDAL.BaseProvider.GetOrderByID(orderid);
            OrderEntity model = new OrderEntity();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        public OrderEntity GetOrderByID(string orderid, string agentid, string clientid)
        {
            DataSet ds = OrdersDAL.BaseProvider.GetOrderByID(orderid, agentid, clientid);
            OrderEntity model = new OrderEntity();
            if (ds.Tables["Order"].Rows.Count > 0)
            {
                
                model.FillData(ds.Tables["Order"].Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);
                model.ExpressTypeStr = CommonBusiness.GetEnumDesc((EnumExpressType)model.ExpressType);

                if (model.Status == 2)
                {
                    model.SendStatusStr = CommonBusiness.GetEnumDesc((EnumSendStatus)model.SendStatus);
                }
                else if (model.Status < 2)
                {
                    model.SendStatusStr = "--";
                }


                if (!string.IsNullOrEmpty(model.CategoryID))
                {
                    model.CategoryName = ProductsBusiness.BaseBusiness.GetCategoryByID(model.BigCategoryID).CategoryName + ">" + ProductsBusiness.BaseBusiness.GetCategoryByID(model.CategoryID).CategoryName;
                }
                model.OrderProcess = SystemBusiness.BaseBusiness.GetOrderProcessByID(model.ProcessID, model.AgentID, model.ClientID);
                                
                model.OrderProcess.OrderStages = SystemBusiness.BaseBusiness.GetOrderStages(model.ProcessID, model.AgentID, model.ClientID);

                model.Tasts = new List<IntFactoryEntity.Task.TaskEntity>();
                if (model.Status > 0 && ds.Tables["Tasks"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Tasks"].Rows)
                    {
                        IntFactoryEntity.Task.TaskEntity task = new IntFactoryEntity.Task.TaskEntity();
                        task.FillData(dr);
                        task.Owner = OrganizationBusiness.GetUserByUserID(task.OwnerID, model.AgentID);
                        model.Tasts.Add(task);
                    }
                    
                }
                model.StatusItems = new List<OrderStatusEntity>();
                if (model.Status > 0 && ds.Tables["Status"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Status"].Rows)
                    {
                        OrderStatusEntity status = new IntFactoryEntity.OrderStatusEntity();
                        status.FillData(dr);
                        model.StatusItems.Add(status);
                    }
                }

                model.City = CommonBusiness.GetCityByCode(model.CityCode);

                if (ds.Tables["Customer"].Rows.Count > 0)
                {
                    model.Customer = new CustomerEntity();
                    model.Customer.FillData(ds.Tables["Customer"].Rows[0]);
                }
                model.Details = new List<OrderDetail>();
                foreach (DataRow dr in ds.Tables["Details"].Rows)
                {
                    OrderDetail detail = new OrderDetail();
                    detail.FillData(dr);
                    if (!string.IsNullOrEmpty(detail.UnitID))
                    {
                        detail.UnitName = new ProductsBusiness().GetUnitByID(detail.UnitID).UnitName;
                        
                    }
                    model.Details.Add(detail);
                }
                
                model.OrderGoods = new List<OrderGoodsEntity>();
                if (ds.Tables["Goods"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Goods"].Rows)
                    {
                        OrderGoodsEntity detail = new OrderGoodsEntity();
                        detail.FillData(dr);
                        model.OrderGoods.Add(detail);
                    }
                }  
            }
            return model;
        }

        public OrderEntity GetOrderBaseInfoByID(string orderid, string agentid, string clientid)
        {
            DataSet ds = OrdersDAL.BaseProvider.GetOrderBaseInfoByID(orderid, agentid, clientid);
            OrderEntity model = new OrderEntity();
            if (ds.Tables["Order"].Rows.Count > 0)
            {

                model.FillData(ds.Tables["Order"].Rows[0]);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                model.Details = new List<OrderDetail>();
                if (ds.Tables["Details"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Details"].Rows)
                    {
                        OrderDetail detail = new OrderDetail();
                        detail.FillData(dr);
                        if (!string.IsNullOrEmpty(detail.UnitID))
                        {
                            detail.UnitName = new ProductsBusiness().GetUnitByID(detail.UnitID).UnitName;
                        }
                        model.Details.Add(detail);
                    }
                }
            }
            return model;
        }

        public OrderEntity GetOrderForFentReport(string orderid, string agentid, string clientid)
        {
            DataSet ds = OrdersDAL.BaseProvider.GetOrderForFentReport(orderid, agentid, clientid);
            OrderEntity model = new OrderEntity();
            if (ds.Tables["Order"].Rows.Count > 0)
            {

                model.FillData(ds.Tables["Order"].Rows[0]);

                if (model.Status == 2)
                {
                    model.SendStatusStr = CommonBusiness.GetEnumDesc((EnumSendStatus)model.SendStatus);
                }
                else if (model.Status < 2)
                {
                    model.SendStatusStr = "--";
                }


                if (!string.IsNullOrEmpty(model.CategoryID))
                {
                    model.CategoryName = ProductsBusiness.BaseBusiness.GetCategoryByID(model.BigCategoryID).CategoryName + ">" + ProductsBusiness.BaseBusiness.GetCategoryByID(model.CategoryID).CategoryName;
                }

                model.Tasts = new List<IntFactoryEntity.Task.TaskEntity>();
                if (model.Status > 0 && ds.Tables["Tasks"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Tasks"].Rows)
                    {
                        IntFactoryEntity.Task.TaskEntity task = new IntFactoryEntity.Task.TaskEntity();
                        task.FillData(dr);
                        task.Owner = OrganizationBusiness.GetUserByUserID(task.OwnerID, model.AgentID);
                        model.Tasts.Add(task);
                    }

                }

                model.City = CommonBusiness.GetCityByCode(model.CityCode);

                if (ds.Tables["Customer"].Rows.Count > 0)
                {
                    model.Customer = new CustomerEntity();
                    model.Customer.FillData(ds.Tables["Customer"].Rows[0]);
                }

                model.Details = new List<OrderDetail>();
                foreach (DataRow dr in ds.Tables["Details"].Rows)
                {
                    OrderDetail detail = new OrderDetail();
                    detail.FillData(dr);
                    if (!string.IsNullOrEmpty(detail.UnitID))
                    {
                        detail.UnitName = new ProductsBusiness().GetUnitByID(detail.UnitID).UnitName;

                    }
                    var city = CommonBusiness.GetCityByCode(detail.ProviderCityCode);
                    if (city != null)
                    {
                        detail.ProviderAddress = city.Description + detail.ProviderAddress;
                    }
                    model.Details.Add(detail);
                }

                model.OrderCoss = new List<OrderCostEntity>();
                foreach (DataRow dr in ds.Tables["OrderCoss"].Rows)
                {
                    OrderCostEntity cos = new OrderCostEntity();
                    cos.FillData(dr);

                    model.OrderCoss.Add(cos);
                }


            }
            return model;
        }

        public static List<ReplyEntity> GetReplys(string guid, string stageID,int mark, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();
            string whereSql = " Status<>9 and GUID='" + guid + "' ";
            if (mark == 0 && !string.IsNullOrEmpty(stageID))
            {
                whereSql += " and StageID='" + stageID + "' ";
            }
            else
            {
                if (mark != -1)
                {
                    whereSql += " and Mark=" + mark + " ";
                }
            }

            
            DataTable dt = CommonBusiness.GetPagerData("OrderReply", "*", whereSql, "AutoID", "CreateTime desc ", pageSize, pageIndex, out totalCount, out pageCount, false);

            foreach (DataRow dr in dt.Rows)
            {
                ReplyEntity model = new ReplyEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                if (!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }

            return list;

        }

        public static List<ReplyEntity> GetReplys(string guid, int mark, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();
            string whereSql = " Status<>9 and GUID='" + guid + "' ";
            if (mark != -1)
            {
                whereSql += " and Mark=" + mark + " ";
            }

            DataTable dt = CommonBusiness.GetPagerData("OrderReply", "*", whereSql, "AutoID", "CreateTime desc ", pageSize, pageIndex, out totalCount, out pageCount, false);

            foreach (DataRow dr in dt.Rows)
            {
                ReplyEntity model = new ReplyEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                if (!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }

            return list;

        }

        public static List<ReplyEntity> GetReplys(string guid, string stageID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();
            string whereSql = " Status<>9 and GUID='" + guid + "' ";
            if (!string.IsNullOrEmpty(stageID))
            {
                whereSql += " and StageID='" + stageID + "' ";
            }

            DataTable dt = CommonBusiness.GetPagerData("OrderReply", "*", whereSql, "AutoID", "CreateTime desc ", pageSize, pageIndex, out totalCount, out pageCount, false);

            foreach (DataRow dr in dt.Rows)
            {
                ReplyEntity model = new ReplyEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                if (!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }

            return list;

        }

        public static List<OrderEntity> GetOrderPlans(string ownerID, string beginDate, string endDate, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataTable dt = OrdersDAL.BaseProvider.GetOrderPlans(ownerID, beginDate, endDate, clientID, pageSize, pageIndex, ref totalCount, ref pageCount);

            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                //model.Stage = SystemBusiness.BaseBusiness.GetOrderStageByID(model.StageID, model.ProcessID, model.AgentID, model.ClientID);
                //model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                list.Add(model);
            }

            return list;
        }

        public List<OrderCostEntity> GetOrderCosts(string orderid, string clientid)
        {
            List<OrderCostEntity> list = new List<OrderCostEntity>();
            DataTable dt = OrdersDAL.BaseProvider.GetOrderCosts(orderid);
            foreach (DataRow dr in dt.Rows)
            {
                OrderCostEntity model = new OrderCostEntity();
                model.FillData(dr);

                list.Add(model);
            }
            return list;
        }

        public List<OrderGoodsEntity> GetOrderGoods(string orderid) {
            List<OrderGoodsEntity> list = new List<OrderGoodsEntity>();
            DataTable dt = OrdersDAL.BaseProvider.GetOrderGoods(orderid);
            foreach (DataRow dr in dt.Rows)
            {
                OrderGoodsEntity model = new OrderGoodsEntity();
                model.FillData(dr);

                list.Add(model);
            }
            return list;
        }
        #endregion

        #region 添加

        public string CreateOrder(string customerid, string goodscode, string title, string name, string mobile, EnumOrderSourceType sourceType, EnumOrderType ordertype,
                                  string bigcategoryid, string categoryid, string price, int quantity, DateTime planTime, string orderimgs, string citycode, 
                                  string address, string expressCode, string remark, string operateid, string agentid, string clientid, string aliOrderCode = "")
        {
            string id = Guid.NewGuid().ToString();
            string code = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string firstimg = "", allimgs = "";

            if (sourceType == EnumOrderSourceType.AliOrder)
            {
                if (!string.IsNullOrEmpty(orderimgs))
                {
                    firstimg = orderimgs.Trim(',').Split(',')[0];
                    allimgs = orderimgs;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(orderimgs))
                {
                    bool first = true;
                    foreach (var img in orderimgs.Split(','))
                    {
                        string orderimg = img;
                        if (!string.IsNullOrEmpty(orderimg))
                        {
                            if (orderimg.IndexOf("?") > 0)
                            {
                                orderimg = orderimg.Substring(0, orderimg.IndexOf("?"));
                            }

                            DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                            if (!directory.Exists)
                            {
                                directory.Create();
                            }

                            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(orderimg));
                            orderimg = FILEPATH + file.Name;
                            if (first)
                            {
                                firstimg = FILEPATH + "small" + file.Name;
                            }
                            if (file.Exists)
                            {
                                file.MoveTo(HttpContext.Current.Server.MapPath(orderimg));
                            }
                        }
                        if (first)
                        {
                            CommonBusiness.GetThumImage(HttpContext.Current.Server.MapPath(orderimg), 30, 250, HttpContext.Current.Server.MapPath(firstimg));

                            first = false;
                        }
                        allimgs += orderimg + ",";
                    }
                }
                if (allimgs.Length > 0)
                {
                    allimgs = allimgs.Substring(0, allimgs.Length - 1);
                }
            }

            bool bl = OrdersDAL.BaseProvider.CreateOrder(id, code, aliOrderCode, goodscode, title, customerid, name, mobile, (int)sourceType, (int)ordertype, bigcategoryid, categoryid, price, quantity, planTime < DateTime.Now ? DateTime.Now.AddDays(7).ToString() : planTime.ToString(),
                                                        firstimg, allimgs, citycode, address, expressCode, remark, operateid, agentid, clientid);
            if (!bl)
            {
                return "";
            }
            else if (sourceType == EnumOrderSourceType.FactoryOrder)
            {
                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Orders, EnumLogType.Create, "", operateid, agentid, clientid);
            }
            return id;
        }

        public string CreateDHOrder(string orderid, int ordertype, decimal discount, decimal price, List<ProductDetail> details, string operateid, string agentid, string clientid)
        {
            var dal = new OrdersDAL();
            string id = Guid.NewGuid().ToString().ToLower();

            if (ordertype == 2)
            {
                if (!UpdateOrderDiscount(orderid, discount, price, operateid, "", agentid, clientid))
                {
                    return "";
                }
            }

            SqlConnection conn = new SqlConnection(BaseDAL.ConnectionString);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                //打样单
                if (ordertype == (int)EnumOrderType.ProofOrder)
                {
                    bool bl = dal.CreateDHOrder(id, orderid, discount, price, operateid, clientid, tran);
                    //产品添加成功添加子产品
                    if (bl)
                    {
                        foreach (var model in details)
                        {
                            if (!dal.AddOrderGoods(id, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Quantity, model.Description, operateid, clientid, tran))
                            {
                                tran.Rollback();
                                conn.Dispose();
                                return "";
                            }
                        }
                        tran.Commit();
                        conn.Dispose();
                        //日志
                        LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Orders, EnumLogType.Create, "", operateid, agentid, clientid);
                        
                        return id;
                    }
                }
                else
                {
                    foreach (var model in details)
                    {
                        if (!dal.AddOrderGoods(orderid, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Quantity, model.Description, operateid, clientid, tran))
                        {
                            tran.Rollback();
                            conn.Dispose();
                            return "";
                        }
                    }
                    tran.Commit();
                    conn.Dispose();
                    return orderid;
                }
                
                return "";
            }
            catch (Exception ex)
            {
                tran.Rollback();
                conn.Dispose();
                return "";
            }
        }

        public string CreateOrderGoodsDoc(string orderid, string taskid, EnumGoodsDocType type, int isover, string expressid, string expresscode, string details, string remark, string operateid, string agentid, string clientid)
        {
            var dal = new OrdersDAL();
            string id = Guid.NewGuid().ToString().ToLower();

            bool bl = dal.CreateOrderGoodsDoc(id, orderid,taskid, (int)type, isover, expressid, expresscode, details, remark, operateid, clientid);
            if (bl)
            {
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.OrderDoc, EnumLogType.Create, "", operateid, agentid, clientid);
                return id;
            }
            return "";
        }

        public static string CreateReply(string guid,string stageID,int mark, string content, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            return OrdersDAL.BaseProvider.CreateReply(guid, stageID, mark, content, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);
        }

        public bool CreateOrderCost(string orderid, decimal price, string remark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.CreateOrderCost(orderid, price, remark, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "订单添加其他成本：" + price;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        #endregion

        #region 编辑、删除

        public bool UpdateOrderPrice(string orderid, string autoid, string name, decimal price, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderPrice(orderid, autoid, price, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "修改材料" + name + "价格：" + price;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateProductQuantity(string orderid, string autoid, string name, decimal quantity, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateProductQuantity(orderid, autoid, quantity, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "修改材料" + name + "消耗量：" + quantity;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateProductLoss(string orderid, string autoid, string name, decimal quantity, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateProductLoss(orderid, autoid, quantity, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "修改材料" + name + "损耗量：" + quantity;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, agentid, clientid);
            }
            return bl;
        }

        public bool DeleteProduct(string orderid, string autoid, string name, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.DeleteProduct(orderid, autoid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "删除材料" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, agentid, clientid);
            }
            return bl;
        }

        public bool DeleteOrder(string orderid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.DeleteOrder(orderid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "删除需求单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderOver(string orderid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderOver(orderid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "终止订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderProcess(string orderid, string processid, string name, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderProcess(orderid, processid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "订单流程更换为：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, processid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderCategoryID(string orderid, string pid, string categoryid, string name, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderCategoryID(orderid, pid, categoryid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "绑定订单品类：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, categoryid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderOwner(string orderid, string userid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderOwner(orderid, userid, operateid, agentid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
                string msg = "负责人更换为：" + model.Name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, userid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderMark(string orderid, int mark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CommonBusiness.Update("Orders", "Mark", mark, "OrderID='" + orderid + "'");
            if (bl)
            {
                string msg = "标记订单颜色";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, mark.ToString(), agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderStatus(string orderid, EnumOrderStageStatus status, string time, decimal price, string operateid, string ip, string agentid, string clientid, out string errinfo)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderStatus(orderid, (int)status, time, price, operateid, agentid, clientid, out errinfo);
            if (bl)
            {
                string msg = "订单状态更换为：" + CommonBusiness.GetEnumDesc<EnumOrderStageStatus>(status);

                switch (status)
                {
                    case EnumOrderStageStatus.DY:
                        msg = "需求单开始打样，交货日期为：" + time;
                        break;
                    case EnumOrderStageStatus.FYFJ:
                        msg = "打样单完成合价，最终报价为：" + price;
                        break;
                    case EnumOrderStageStatus.DQR:
                        msg = "大货单开始生产，交货日期为：" + time;
                        break;
                }
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Orders, EnumLogType.Update, "", operateid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateProfitPrice(string orderid, decimal profit, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateProfitPrice(orderid, profit, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "订单利润比例设置为：" + profit;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderDiscount(string orderid, decimal discount, decimal price, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderDiscount(orderid, discount, price, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "订单折扣设置为：" + discount + ",单价设置为：" + price;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderClient(string orderid, string newclientid, string name, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderClient(orderid, newclientid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "订单委托给工厂：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, newclientid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderOriginalID(string orderid, string originalorderid, string name, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderOriginalID(orderid, originalorderid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "绑定打样订单：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, originalorderid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderCustomer(string orderid, string customerid, string name, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderCustomer(orderid, customerid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "绑定客户：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, customerid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderImages(string orderid, string images,  string operateid, string ip, string agentid, string clientid)
        {
            string firstimg = "", allimgs = "";

            if (!string.IsNullOrEmpty(images))
            {
                bool first = true;
                foreach (var img in images.Split(','))
                {
                    string orderimg = img;
                    if (!string.IsNullOrEmpty(orderimg) && orderimg.IndexOf(TempPath) >= 0)
                    {
                        if (orderimg.IndexOf("?") > 0)
                        {
                            orderimg = orderimg.Substring(0, orderimg.IndexOf("?"));
                        }

                        DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                        if (!directory.Exists)
                        {
                            directory.Create();
                        }

                        FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(orderimg));
                        orderimg = FILEPATH + file.Name;
                        if (file.Exists)
                        {
                            file.MoveTo(HttpContext.Current.Server.MapPath(orderimg));
                        }
                    }
                    if (first)
                    {
                        if (orderimg.ToLower().IndexOf("http://img.china.alibaba.com") < 0)
                        {
                            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(orderimg));

                            if (file.Exists)
                            {
                                firstimg = orderimg.Substring(0, orderimg.IndexOf(file.Name)) + "small" + file.Name;
                                if (!new FileInfo(HttpContext.Current.Server.MapPath(firstimg)).Exists)
                                {
                                    CommonBusiness.GetThumImage(HttpContext.Current.Server.MapPath(orderimg), 30, 250, HttpContext.Current.Server.MapPath(firstimg));
                                }
                            }
                        }
                        else 
                        {
                            firstimg = orderimg;
                        }
                        first = false;
                    }
                    allimgs += orderimg + ",";
                }
            }
            if (allimgs.Length > 0)
            {
                allimgs = allimgs.Substring(0, allimgs.Length - 1);
            }
            bool bl = OrdersDAL.BaseProvider.UpdateOrderImages(orderid, firstimg, allimgs, agentid, clientid);
            if (bl)
            {
                string msg = "更换订单样图";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderPlateAttr(string orderid, string taskID, string valueIDS, string platehtml, string createUserID, string agentID, string clientID)
        {
            return OrdersDAL.BaseProvider.UpdateOrderPlateAttr(orderid, taskID, valueIDS, platehtml, createUserID, agentID, clientID);
        }

        public bool UpdateOrderPlateRemark(string orderid, string plateRemark)
        {
            return OrdersDAL.BaseProvider.UpdateOrderPlateRemark(orderid, plateRemark);
        }

        public bool EditOrder(string orderid, string goodsCode, string goodsName, string personName, string mobileTele, string cityCode, string address, 
                              string postalcode, string typeid, int expresstype, string remark, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.EditOrder(orderid, goodsCode, goodsName, personName, mobileTele, cityCode, address, postalcode, typeid, expresstype, remark, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "编辑订单信息";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, operateid, agentid, clientid);
            }
            return bl;
        }

        public bool EffectiveOrderProduct(string orderid, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.EffectiveOrderProduct(orderid, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "确认大货单材料采购";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool ApplyReturnOrder(string orderid, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.ApplyReturnOrder(orderid, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "退回委托";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateReturnQuantity(string orderid, string autoid, string name, int quantity, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateReturnQuantity(orderid, autoid, quantity, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "修改产品" + name + "退货数量：" + quantity;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, agentid, clientid);
            }
            return bl;
        }

        public bool ApplyReturnProduct(string orderid, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.ApplyReturnProduct(orderid, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "申请退货";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOpportunityStage(string opportunityid,string processid, string stageid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOpportunityStage(opportunityid, stageid, operateid, agentid, clientid);
            if (bl)
            {
                var model = SystemBusiness.BaseBusiness.GetOrderStageByID(stageid, processid, agentid, clientid);
                string msg = "机会阶段更换为：" + model.StageName;
                LogBusiness.AddLog(opportunityid, EnumLogObjectType.Orders, msg, operateid, ip, stageid, agentid, clientid);
            }
            return bl;
        }

        public bool CreateOrderCustomer(string orderid, string operateid, string ip, string agentid, string clientid)
        {
            string id = "";
            bool bl = OrdersDAL.BaseProvider.CreateOrderCustomer(orderid, operateid, agentid, clientid, out id);
            if (bl)
            {
                string msg = "订单联系人创建新客户";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, id, agentid, clientid);
            }
            return bl;
        }

        public bool DeleteOrderCost(string orderid, string autoid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.DeleteOrderCost(orderid, autoid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "订单删除其他成本";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, agentid, clientid);
            }
            return bl;
        }

        #endregion
    }
}
