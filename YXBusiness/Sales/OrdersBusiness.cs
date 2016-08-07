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

        public List<OrderEntity> GetOrders(EnumOrderSearchType searchOrderType, EnumSearchType searchtype, string entrustType, string typeid, int status, EnumOrderSourceType sourceType, int orderStatus, int mark,
                                                int paystatus, int warningstatus, int returnstatus, string searchuserid, string searchteamid,
                                                string begintime, string endtime, string keyWords, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataSet ds = OrdersDAL.BaseProvider.GetOrders((int)searchOrderType, (int)searchtype, entrustType, typeid, status, (int)sourceType, orderStatus, mark, paystatus, warningstatus, 
                                                         returnstatus, searchuserid, searchteamid, begintime, endtime, keyWords,
                                                         orderBy, pageSize, pageIndex, ref totalCount, ref pageCount, userid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                model.SourceTypeStr = CommonBusiness.GetEnumDesc((EnumOrderSourceType)model.SourceType);

                GetWarningData(model);

                list.Add(model);
            }
            return list;
        }

        public List<OrderEntity> GetOrders(string keyWords,string categoryid,int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            string where = " ClientID='" + clientid + "' and  OrderType=1 and Status= " + (int)EnumOrderStageStatus.FYFJ;
            if (!string.IsNullOrEmpty(keyWords))
            {
                where += "and (OrderCode like '%" + keyWords + "%' or Title like '%" + keyWords + "%' or PersonName like '%" + keyWords + "%' or IntGoodsCode like '%" + keyWords + "%' or GoodsCode like '%" + keyWords + "%')";
            }
            if (!string.IsNullOrEmpty(categoryid))
            {
                where += " and (CategoryID='" + categoryid + "')";
            }
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", where, "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);

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

        /// <summary>
        /// 获取订单列表根据二当家客户端编码
        /// </summary>
        /// <param name="yxCode"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public List<OrderEntity> GetOrdersByYXCode(string yxCode, string clientid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataSet ds = OrdersDAL.BaseProvider.GetOrdersByYXCode(yxCode, clientid, pageSize,pageIndex,ref totalCount,ref pageCount);
            DataTable dt = ds.Tables["Orders"];
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                list.Add(model);
            }

            return list;
        }

        public List<OrderEntity> GetOrdersByPlanTime(string startPlanTime, string endPlanTime, int orderType, int filterType, int orderStatus,
            string userID, string clientID, string andWhere, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataTable dt = OrdersDAL.BaseProvider.GetOrdersByPlanTime(startPlanTime, endPlanTime, orderType, filterType, orderStatus,
                userID, clientID, andWhere, pageSize, pageIndex, ref totalCount, ref pageCount);

            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);
                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                GetWarningData(model);

                list.Add(model);
            }
            return list;
        }

        public void GetWarningData(OrderEntity model)
        {
            if (model.OrderStatus == 1 && model.Status != (int)EnumOrderStageStatus.DDH)
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
            else if (model.OrderStatus == 2)
            {
                model.WarningStatus = 3;
                model.UseDays = (model.PlanTime - model.OrderTime).Days;
                model.WarningDays = (DateTime.Now - model.EndTime).Days;
            }
        }

        public int GetExceedOrderCount(string ownerID, string orderType, string clientID)
        {
            return OrdersDAL.BaseProvider.GetExceedOrderCount(ownerID, orderType, clientID);
        }

        public List<OrderEntity> GetOrdersByCustomerID(string keyWords, string customerid, int ordertype, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            string condition="CustomerID='" + customerid + "' and OrderType=" + ordertype + " and Status<>9 and Status<>0";
            if (!string.IsNullOrEmpty(keyWords))
            {
                condition += " and ( OrderCode like '%" + keyWords + "%' or GoodsCode like '%" + keyWords + "%' or MobileTele like '%" + keyWords + "%' or PersonName like '%" + keyWords + "%' or IntGoodsCode like '%" + keyWords + "%')";
            }
            
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", condition, "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);
                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);
                GetWarningData(model);
                list.Add(model);
            }
            return list;
        }

        public List<OrderEntity> GetNeedsOrderByCustomerID(string keyWords, string customerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            string condition = "CustomerID='" + customerid + "' and Status = 0 ";
            if (!string.IsNullOrEmpty(keyWords))
            {
                condition += " and ( OrderCode like '%" + keyWords + "%' or GoodsCode like '%" + keyWords + "%' or MobileTele like '%" + keyWords + "%' or PersonName like '%" + keyWords + "%')";
            }
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", condition, "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);

                list.Add(model);
            }
            return list;
        }

        public List<OrderEntity> GetOrdersByOriginalID(string originalid, int ordertype, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", "OriginalID='" + originalid + "' and OrderType=" + ordertype + " and Status<>9 ", "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

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

        public OrderEntity GetOrderByID(string orderid, string clientid)
        {
            DataSet ds = OrdersDAL.BaseProvider.GetOrderByID(orderid, clientid);
            OrderEntity model = new OrderEntity();
            if (ds.Tables["Order"].Rows.Count > 0)
            {
                
                model.FillData(ds.Tables["Order"].Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.ClientID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                if (model.Status == 2)
                {
                    model.SendStatusStr = CommonBusiness.GetEnumDesc((EnumSendStatus)model.SendStatus);
                }
                else if (model.Status < 2)
                {
                    model.SendStatusStr = "--";
                }

                if (!string.IsNullOrEmpty(model.BigCategoryID))
                {
                    var category = SystemBusiness.BaseBusiness.GetProcessCategoryByID(model.BigCategoryID);
                    model.ProcessCategoryName = category == null ? "" : category.Name;
                }
                if (!string.IsNullOrEmpty(model.CategoryID))
                {
                    var category = ProductsBusiness.BaseBusiness.GetCategoryByID(model.CategoryID);
                    var pcategory = ProductsBusiness.BaseBusiness.GetCategoryByID(category.PID);
                    model.CategoryName = pcategory.CategoryName + " > " + category.CategoryName;
                }

                model.OrderProcess = SystemBusiness.BaseBusiness.GetOrderProcessByID(model.ProcessID, model.ClientID);
                                
                model.OrderProcess.OrderStages = SystemBusiness.BaseBusiness.GetOrderStages(model.ProcessID,  model.ClientID);

                model.Tasts = new List<IntFactoryEntity.Task.TaskEntity>();
                if (model.Status > 0 && ds.Tables["Tasks"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Tasks"].Rows)
                    {
                        IntFactoryEntity.Task.TaskEntity task = new IntFactoryEntity.Task.TaskEntity();
                        task.FillData(dr);
                        task.Owner = OrganizationBusiness.GetUserByUserID(task.OwnerID, model.ClientID);
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

        public OrderEntity GetOrderBaseInfoByID(string orderid,  string clientid)
        {
            DataSet ds = OrdersDAL.BaseProvider.GetOrderBaseInfoByID(orderid, clientid);
            OrderEntity model = new OrderEntity();
            if (ds.Tables["Order"].Rows.Count > 0)
            {

                model.FillData(ds.Tables["Order"].Rows[0]);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);
                if (!string.IsNullOrEmpty(model.BigCategoryID))
                {
                    var category = SystemBusiness.BaseBusiness.GetProcessCategoryByID(model.BigCategoryID);
                    model.ProcessCategoryName = category == null ? "" : category.Name;
                }
                if (!string.IsNullOrEmpty(model.CategoryID))
                {
                    var category = ProductsBusiness.BaseBusiness.GetCategoryByID(model.CategoryID);
                    var pcategory = ProductsBusiness.BaseBusiness.GetCategoryByID(category.PID);
                    model.CategoryName = pcategory.CategoryName + " > " + category.CategoryName;
                }

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

        public OrderEntity GetOrderForFentReport(string orderid, string clientid)
        {
            DataSet ds = OrdersDAL.BaseProvider.GetOrderForFentReport(orderid, clientid);
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
                        task.Owner = OrganizationBusiness.GetUserByUserID(task.OwnerID, model.ClientID);
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

        public static List<OrderEntity> GetOrderPlans(string ownerID, string beginDate, string endDate, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataTable dt = OrdersDAL.BaseProvider.GetOrderPlans(ownerID, beginDate, endDate, clientID, pageSize, pageIndex, ref totalCount, ref pageCount);

            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);

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

        public List<OrderDetail> GetOrderDetailsByOrderID(string orderid)
        {
            List<OrderDetail> list = new List<OrderDetail>();
            DataTable dt = OrdersDAL.BaseProvider.GetOrderDetailsByOrderID(orderid);
            foreach (DataRow dr in dt.Rows)
            {
                OrderDetail model = new OrderDetail();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }



        #endregion

        #region 添加

        public string CreateOrder(string customerid, string goodscode, string title, string name, string mobile, EnumOrderSourceType sourceType, EnumOrderType ordertype,
                                  List<OrderGoodsEntity> details, string bigcategoryid, string categoryid, decimal price, int quantity, DateTime planTime, string orderimgs, string citycode,
                                  string address, string expressCode, string remark, string operateid, string clientid, string aliOrderCode = "")
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
                orderimgs = orderimgs.Trim(',');
                if (orderimgs.Length > 0)
                {
                    allimgs = orderimgs;
                    firstimg = allimgs.Split(',')[0];
                }
            }

            bool bl = OrdersDAL.BaseProvider.CreateOrder(id, code, aliOrderCode, goodscode, title, customerid, name, mobile, (int)sourceType, (int)ordertype, bigcategoryid, categoryid, price, quantity, planTime < DateTime.Now ? "" : planTime.ToString(),
                                                        firstimg, allimgs, citycode, address, expressCode, remark, operateid, clientid);
            if (bl)
            {
                if (ordertype == EnumOrderType.LargeOrder && details.Count > 0)
                {
                    SqlConnection conn = new SqlConnection(BaseDAL.ConnectionString);
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    SqlTransaction tran = conn.BeginTransaction();
                    foreach (var model in details)
                    {
                        if (!OrdersDAL.BaseProvider.AddOrderGoods(id, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Quantity, model.XRemark, model.YRemark, model.XYRemark, model.Remark, operateid, clientid, tran))
                        {
                            tran.Rollback();
                            conn.Dispose();
                            return "";
                        }
                    }
                    tran.Commit();
                    conn.Dispose();
                }
                if (sourceType == EnumOrderSourceType.FactoryOrder)
                {
                    //日志
                    LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Orders, EnumLogType.Create, "", operateid, clientid);
                }
                return id;
            }

            return "";
        }

        public string CreateDHOrder(string orderid, int ordertype, decimal discount, decimal price, List<OrderGoodsEntity> details, string operateid, string clientid, string yxOrderID = "")
        {
            var dal = new OrdersDAL();
            string id = Guid.NewGuid().ToString().ToLower();

            if (ordertype == 2)
            {
                if (!UpdateOrderDiscount(orderid, discount, price, operateid, "", clientid))
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
                    bool bl = dal.CreateDHOrder(id, orderid, discount, price, operateid, clientid,yxOrderID, tran);
                    //产品添加成功添加子产品
                    if (bl)
                    {
                        foreach (var model in details)
                        {
                            if (!dal.AddOrderGoods(id, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Quantity, model.XRemark, model.YRemark, model.XYRemark, model.Remark, operateid, clientid, tran))
                            {
                                tran.Rollback();
                                conn.Dispose();
                                return "";
                            }
                        }
                        tran.Commit();
                        conn.Dispose();
                        //日志
                        LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Orders, EnumLogType.Create, "", operateid, clientid);
                        
                        return id;
                    }
                }
                else
                {
                    foreach (var model in details)
                    {
                        if (!dal.AddOrderGoods(orderid, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Quantity, model.XRemark, model.YRemark, model.XYRemark, model.Remark, operateid, clientid, tran))
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

        public string CreateOrderGoodsDoc(string orderid, string taskid, EnumGoodsDocType type, int isover, string expressid, string expresscode, string details, string remark, string operateid, string clientid)
        {
            var dal = new OrdersDAL();
            string id = Guid.NewGuid().ToString().ToLower();

            bool bl = dal.CreateOrderGoodsDoc(id, orderid,taskid, (int)type, isover, expressid, expresscode, details, remark, operateid, clientid);
            if (bl)
            {
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.OrderDoc, EnumLogType.Create, "", operateid, clientid);
                return id;
            }
            return "";
        }

        public bool CreateGoodsDocReturn(string orderID, string taskID, EnumDocType docType, string details, string originalID, string clientID,ref int result)
        {
            string id = Guid.NewGuid().ToString();
            bool b1 = OrdersDAL.BaseProvider.CreateGoodsDocReturn(id, orderID, taskID, (int)docType, details, originalID, clientID,ref result);
            return b1;
        }

        public static string CreateReply(string guid,string stageID,int mark, string content, string userID, string clientid, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            return OrdersDAL.BaseProvider.CreateReply(guid, stageID, mark, content, userID, clientid, fromReplyID, fromReplyUserID, fromReplyAgentID);
        }

        public bool CreateOrderCost(string orderid, decimal price, string remark, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.CreateOrderCost(orderid, price, remark, operateid, clientid);
            if (bl)
            {
                string msg = "订单添加其他成本：" + price;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "",  clientid);
            }
            return bl;
        }

        public bool CreateProductUseQuantity(ref int result, ref string errInfo, string orderID, string details, string userID, string operateIP, string clientID)
        {
            bool b1 = OrdersDAL.BaseProvider.CreateProductUseQuantity(ref result, ref errInfo, orderID, details, userID, operateIP, clientID);
            return b1;
        }

        #endregion

        #region 编辑、删除

        public bool UpdateOrderPrice(string orderid, string autoid, string name, decimal price, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderPrice(orderid, autoid, price, operateid, clientid);
            if (bl)
            {
                string msg = "修改材料" + name + "价格：" + price;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, clientid);
            }
            return bl;
        }

        public bool UpdateProductQuantity(string orderid, string autoid, string name, decimal quantity, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateProductQuantity(orderid, autoid, quantity, operateid, clientid);
            if (bl)
            {
                string msg = "修改材料" + name + "消耗量：" + quantity;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, clientid);
            }
            return bl;
        }

        public bool UpdateProductLoss(string orderid, string autoid, string name, decimal quantity, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateProductLoss(orderid, autoid, quantity, operateid, clientid);
            if (bl)
            {
                string msg = "修改材料" + name + "损耗量：" + quantity;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, clientid);
            }
            return bl;
        }

        public bool DeleteProduct(string orderid, string autoid, string name, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.DeleteProduct(orderid, autoid, operateid, clientid);
            if (bl)
            {
                string msg = "删除材料" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, clientid);
            }
            return bl;
        }

        public bool DeleteOrder(string orderid, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.DeleteOrder(orderid, operateid, clientid);
            if (bl)
            {
                string msg = "删除需求单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
            }
            return bl;
        }

        public bool UpdateOrderOver(string orderid, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderOver(orderid, operateid, clientid);
            if (bl)
            {
                string msg = "终止订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
            }
            return bl;
        }

        public bool UpdateOrderProcess(string orderid, string processid, string categoryid, string name, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderProcess(orderid, processid, categoryid, operateid, clientid);
            if (bl)
            {
                string msg = "订单流程更换为：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, processid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderCategoryID(string orderid, string pid, string categoryid, string name, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderCategoryID(orderid, pid, categoryid, operateid, clientid);
            if (bl)
            {
                string msg = "更换订单类别：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, categoryid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderOwner(string orderid, string userid, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderOwner(orderid, userid, operateid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, clientid);
                string msg = "负责人更换为：" + model.Name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, userid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderMark(string orderid, int mark, string operateid, string ip, string clientid)
        {
            bool bl = CommonBusiness.Update("Orders", "Mark", mark, "OrderID='" + orderid + "'");
            if (bl)
            {
                string msg = "标记订单颜色";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, mark.ToString(), clientid);
            }
            return bl;
        }

        public bool UpdateOrderBegin(string orderid, string time, string operateid, string ip, string clientid, out string errinfo)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderBegin(orderid, time, operateid, clientid, out errinfo);
            if (bl)
            {
                string msg = "需求单转为订单，交货日期为：" + time;


                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Orders, EnumLogType.Update, "", operateid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderStatus(string orderid, EnumOrderStageStatus status, string time, decimal price, string operateid, string ip, string clientid, out string errinfo)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderStatus(orderid, (int)status, time, price, operateid, clientid, out errinfo);
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
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Orders, EnumLogType.Update, "", operateid, clientid);
            }
            return bl;
        }

        public bool UpdateProfitPrice(string orderid, decimal profit, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateProfitPrice(orderid, profit, operateid, clientid);
            if (bl)
            {
                string msg = "订单利润比例设置为：" + profit;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
            }
            return bl;
        }

        public bool UpdateOrderDiscount(string orderid, decimal discount, decimal price, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderDiscount(orderid, discount, price, operateid, clientid);
            if (bl)
            {
                string msg = "订单折扣设置为：" + discount + ",单价设置为：" + price;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
            }
            return bl;
        }

        public bool UpdateOrderTotalMoney(string orderid, decimal totalMoney, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderTotalMoney(orderid, totalMoney, operateid, clientid);
            if (bl)
            {
                string msg = "订单总金额设置为：" + totalMoney;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
            }
            return bl;
        }

        public bool UpdateOrderClient(string orderid, string newclientid, string name, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderClient(orderid, newclientid, operateid, clientid);
            if (bl)
            {
                string msg = "订单委托给工厂：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, newclientid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderOriginalID(string orderid, string originalorderid, string name, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderOriginalID(orderid, originalorderid, operateid, clientid);
            if (bl)
            {
                string msg = "绑定打样订单：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, originalorderid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderCustomer(string orderid, string customerid, string name, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderCustomer(orderid, customerid, operateid, clientid);
            if (bl)
            {
                string msg = "绑定客户：" + name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, customerid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderImages(string orderid, string images, string operateid, string ip, string clientid)
        {
            string firstimg = "", allimgs = "";
            images = images.Trim(',');
            if (!string.IsNullOrEmpty(images))
            {
                allimgs = images;
                firstimg = allimgs.Split(',')[0];
            }
            bool bl = OrdersDAL.BaseProvider.UpdateOrderImages(orderid, firstimg, allimgs, clientid);
            if (bl)
            {
                string msg = "更换订单样图";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
            }
            return bl;
        }

        public bool UpdateOrderPlateAttr(string orderid, string taskid, string platehtml, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderPlateAttr(orderid, platehtml);
            if (bl)
            {
                string msg = "编辑制版信息";
                LogBusiness.AddLog(taskid, EnumLogObjectType.OrderTask, msg, operateid, ip, operateid, clientid);
            }
            return bl;
        }

        public bool EditOrder(string orderid, string goodsCode, string goodsName, string personName, string mobileTele, string cityCode, string address,
                              string postalcode, int expresstype, string remark, string operateid, string ip, string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.EditOrder(orderid, goodsCode, goodsName, personName, mobileTele, cityCode, address, postalcode, expresstype, remark, operateid, clientid, out result);
            if (bl)
            {
                string msg = "编辑订单信息";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, operateid, clientid);
            }
            return bl;
        }

        public bool EffectiveOrderProduct(string orderid, string operateid, string ip, string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.EffectiveOrderProduct(orderid, operateid, clientid, out result);
            if (bl)
            {
                string msg = "确认大货单材料采购";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
            }
            return bl;
        }

        public bool ApplyReturnOrder(string orderid, string operateid, string ip,  string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.ApplyReturnOrder(orderid, operateid, clientid, out result);
            if (bl)
            {
                string msg = "退回委托";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", clientid);
            }
            return bl;
        }

        public bool CreateOrderCustomer(string orderid, string operateid, string ip, string clientid, out int result)
        {
            string id = "";
            bool bl = OrdersDAL.BaseProvider.CreateOrderCustomer(orderid, operateid, clientid, out id, out result);
            if (bl)
            {
                string msg = "订单联系人创建新客户";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, id, clientid);
            }
            return bl;
        }

        public bool DeleteOrderCost(string orderid, string autoid, string operateid, string ip, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.DeleteOrderCost(orderid, autoid, operateid,  clientid);
            if (bl)
            {
                string msg = "订单删除其他成本";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid,  clientid);
            }
            return bl;
        }

        #endregion

        #region 订单区间价位
        /// <summary>
        /// 获取订单区间价位
        /// </summary>
        public static List<OrderPriceRange> GetOrderPriceRanges(string orderID)
        {
            List<OrderPriceRange> list = new List<OrderPriceRange>();
            DataTable dt = OrdersDAL.BaseProvider.GetOrderPriceRanges(orderID);

            foreach (DataRow dr in dt.Rows)
            {
                OrderPriceRange item = new OrderPriceRange();
                item.FillData(dr);
                list.Add(item);
            }
            return list;
        }


        /// <summary>
        /// 添加订单区间价位
        /// </summary>
        public static string AddOrderPriceRange(OrderPriceRange range, string operateid, string ip, string clientid)
        {
            return OrdersDAL.BaseProvider.AddOrderPriceRange(range.MinQuantity, range.Price, range.OrderID, operateid, clientid);
        }

        /// <summary>
        /// 修改订单区间价位
        /// </summary>
        public static bool UpdateOrderPriceRange(OrderPriceRange range, string operateid, string ip,  string clientid)
        {
            bool flag = OrdersDAL.BaseProvider.UpdateOrderPriceRange(range.RangeID, range.MinQuantity, range.Price);

            return flag;
        }

        /// <summary>
        /// 删除订单区间价位
        /// </summary>
        public static bool DeleteOrderPriceRange(string rangeid)
        {
            bool flag = OrdersDAL.BaseProvider.DeleteOrderPriceRange(rangeid);

            return flag;
        }
        #endregion
    }
}
