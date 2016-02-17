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
                model.Stage = SystemBusiness.BaseBusiness.GetOrderStageByID(model.StageID, model.ProcessID, model.AgentID, model.ClientID);
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
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", "CustomerID='" + customerid + "' and Status between 1 and 3", "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
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
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", "CustomerID='" + customerid + "' and Status =0 ", "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Stage = SystemBusiness.BaseBusiness.GetOrderStageByID(model.StageID, model.ProcessID, model.AgentID, model.ClientID);

                list.Add(model);
            }
            return list;
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
            }
            return model;
        }

        public static List<ReplyEntity> GetReplys(string guid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();
            string whereSql = " Status<>9 and GUID='" + guid + "' ";
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

        public string CreateOrder(string name, string mobile, int type, string categoryid, string price, string orderimg, string citycode, string address, string remark, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            string code = DateTime.Now.ToString("yyyyMMddHHmmssfff");

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

            bool bl = OrdersDAL.BaseProvider.CreateOrder(id, code, name, mobile, type, categoryid, price, orderimg, citycode, address, remark, operateid, agentid, clientid);
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

        public static string CreateReply(string guid, string content, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            return OrdersDAL.BaseProvider.CreateReply(guid, content, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);
        }

        #endregion

        #region 编辑、删除

        public bool UpdateOrderPrice(string orderid, string autoid, string name, decimal price, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderPrice(orderid, autoid, price, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "修改产品" + name + "价格：" + price;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, agentid, clientid);
            }
            return bl;
        }

        public bool DeleteOrder(string orderid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.DeleteOrder(orderid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "删除订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderOwner(string orderid, string userid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderOwner(orderid, userid, operateid, agentid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
                string msg = "拥有者更换为：" + model.Name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, userid, agentid, clientid);
            }
            return bl;
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

        public bool EffectiveOrder(string orderid, string operateid, string ip, string agentid, string clientid,out int result)
        {
            bool bl = OrdersDAL.BaseProvider.EffectiveOrder(orderid, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "审核订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool ApplyReturnOrder(string orderid, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.ApplyReturnOrder(orderid, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "申请退单";
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
