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


        public List<OrderEntity> GetOrders(EnumSearchType searchtype, string typeid, int status, int paystatus, int invoicestatus, int returnstatus, string searchuserid, string searchteamid, string searchagentid,
                                                string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataSet ds = OrdersDAL.BaseProvider.GetOrders((int)searchtype, typeid, status, paystatus, invoicestatus, returnstatus, searchuserid, searchteamid, searchagentid, begintime, endtime, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStatus)model.Status);
                if (model.Status == 2)
                {
                    model.SendStatusStr = CommonBusiness.GetEnumDesc((EnumSendStatus)model.SendStatus);
                }
                else if (model.Status < 2)
                {
                    model.SendStatusStr = "--";
                }
                list.Add(model);
            }
            return list;
        }


        public List<OrderEntity> GetOrdersByCustomerID(string customerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", "CustomerID='" + customerid + "' and Status<>9 and Status<>0", "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStatus)model.Status);

                list.Add(model);
            }
            return list;
        }

        public List<OrderEntity> GetOpportunityaByCustomerID(string customerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", "CustomerID='" + customerid + "' and Status = 0 ", "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
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

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStatus)model.Status);
                model.ExpressTypeStr = CommonBusiness.GetEnumDesc((EnumExpressType)model.ExpressType);

                if (model.Status == 2)
                {
                    model.SendStatusStr = CommonBusiness.GetEnumDesc((EnumSendStatus)model.SendStatus);
                }
                else if (model.Status < 2)
                {
                    model.SendStatusStr = "--";
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
                model.OrderStatus = new List<OrderStatusEntity>();
                if (model.Status > 0 && ds.Tables["Status"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Status"].Rows)
                    {
                        OrderStatusEntity status = new IntFactoryEntity.OrderStatusEntity();
                        status.FillData(dr);
                        model.OrderStatus.Add(status);
                    }
                }

                model.City = CommonBusiness.GetCityByCode(model.CityCode);

                if (ds.Tables["Customer"].Rows.Count > 0)
                {
                    model.Customer = new CustomerEntity();
                    model.Customer.FillData(ds.Tables["Customer"].Rows[0]);
                }
                model.Details = new List<OrderDetail>();
                if (ds.Tables["Details"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Details"].Rows)
                    {
                        OrderDetail detail = new OrderDetail();
                        detail.FillData(dr);
                        model.Details.Add(detail);
                    }
                }
                model.OrderGoods = new List<GoodsDetailEntity>();
                if (ds.Tables["Goods"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Goods"].Rows)
                    {
                        GoodsDetailEntity detail = new GoodsDetailEntity();
                        detail.FillData(dr);
                        model.OrderGoods.Add(detail);
                    }
                }  
            }
            return model;
        }

        public static List<ReplyEntity> GetReplys(string guid, string stageID,int mark, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();
            string whereSql = " Status<>9 and GUID='" + guid + "' ";
            //if (!string.IsNullOrEmpty(stageID))
            //{
            //    whereSql += " and StageID='" + stageID+"' ";
            //}

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
                //model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStatus)model.Status);

                list.Add(model);
            }

            return list;
        }
        #endregion

        #region 添加

        public string CreateOrder(string customerid, string goodscode, string title, string name, string mobile, int type, string bigcategoryid, string categoryid, string price, int quantity, string orderimgs, string citycode, string address, string remark, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            string code = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string firstimg = "", allimgs = "";

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
                        if (file.Exists)
                        {
                            file.MoveTo(HttpContext.Current.Server.MapPath(orderimg));
                        }
                    }
                    if (first)
                    {
                        firstimg = orderimg;
                        first = false;
                    }
                    allimgs += orderimg + ",";
                }
            }
            if (allimgs.Length > 0)
            {
                allimgs = allimgs.Substring(0, allimgs.Length - 1);
            }

            bool bl = OrdersDAL.BaseProvider.CreateOrder(id, code, goodscode, title, customerid, name, mobile, type, bigcategoryid, categoryid, price, quantity, firstimg, allimgs, citycode, address, remark, operateid, agentid, clientid);
            if (!bl)
            {
                return "";
            }
            else
            {
                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Orders, EnumLogType.Create, "", operateid, agentid, clientid);
            }
            return id;
        }


        public string CreateDHOrder(string orderid, string originalid, List<ProductDetail> details, string operateid, string agentid, string clientid)
        {
            var dal = new OrdersDAL();
            string id = Guid.NewGuid().ToString().ToLower();

            SqlConnection conn = new SqlConnection(BaseDAL.ConnectionString);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(originalid))
                {
                    bool bl = dal.CreateDHOrder(id, orderid, operateid, clientid, tran);
                    //产品添加成功添加子产品
                    if (bl)
                    {
                        foreach (var model in details)
                        {
                            if (!dal.AddOrderGoods(id, orderid, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Quantity, model.Description, operateid, clientid, tran))
                            {
                                tran.Rollback();
                                conn.Dispose();
                                return "";
                            }
                        }
                        tran.Commit();
                        conn.Dispose();
                        return id;
                    }
                }
                else
                {
                    foreach (var model in details)
                    {
                        if (!dal.AddOrderGoods(orderid, originalid, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Quantity, model.Description, operateid, clientid, tran))
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


        public static string CreateReply(string guid,string stageID,int mark, string content, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            return OrdersDAL.BaseProvider.CreateReply(guid, stageID, mark, content, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);
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

        public bool UpdateOrderStatus(string orderid, EnumOrderStatus status, int quantity, decimal price, string operateid, string ip, string agentid, string clientid, out string errinfo)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderStatus(orderid, (int)status, quantity, price, operateid, agentid, clientid, out errinfo);
            if (bl)
            {
                string msg = "订单状态更换为：" + CommonBusiness.GetEnumDesc<EnumOrderStatus>(status);

                switch (status)
                {
                    case EnumOrderStatus.FYFJ:
                        msg = "打样单完成合价，最终报价为：" + price;
                        break;
                    case EnumOrderStatus.DDH:
                        msg = "打样单大货下单，大货数量为：" + quantity;
                        break;
                }
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
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

        public bool UpdateOrderPlateAttr(string orderid, string taskID, string valueIDS, string platehtml, string createUserID, string agentID, string clientID)
        {
            return OrdersDAL.BaseProvider.UpdateOrderPlateAttr(orderid, taskID, valueIDS, platehtml, createUserID, agentID, clientID);
        }

        public bool UpdateOrderPlateRemark(string orderid, string plateRemark)
        {
            return OrdersDAL.BaseProvider.UpdateOrderPlateRemark(orderid, plateRemark);
        }

        public bool SubmitOrder(string orderid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.SubmitOrder(orderid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "提交订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, operateid, agentid, clientid);

                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Orders, EnumLogType.Create, ip, operateid, agentid, clientid);
            }
            return bl;
        }

        public bool EditOrder(string orderid, string personName, string mobileTele, string cityCode, string address, string postalcode, string typeid, int expresstype, string remark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.EditOrder(orderid, personName, mobileTele, cityCode, address, postalcode, typeid, expresstype, remark, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "编辑收货信息";
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

        #endregion
    }
}
