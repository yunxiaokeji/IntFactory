using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class OrdersDAL : BaseDAL
    {
        public static OrdersDAL BaseProvider = new OrdersDAL();
        #region 查询

        public DataSet GetOpportunitys(int searchtype, string typeid, string stageid, string searchuserid, string searchteamid, string searchagentid, string begintime, string endtime,
                        string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@SearchType",searchtype),
                                       new SqlParameter("@TypeID",typeid),
                                       new SqlParameter("@StageID",stageid),
                                       new SqlParameter("@SearchUserID",searchuserid),
                                       new SqlParameter("@SearchTeamID",searchteamid),
                                       new SqlParameter("@SearchAgentID",searchagentid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@Keywords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetOpportunitys", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }


        public DataSet GetOrders(int searchtype, string entrustClientID, string typeid, int status, int sourceType, int orderStatus, int mark, int paystatus, int invoicestatus, int returnstatus, string searchuserid, string searchteamid, string searchagentid, string begintime, string endtime,
                                string keyWords, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@SearchType",searchtype),
                                       new SqlParameter("@EntrustClientID",entrustClientID),
                                       new SqlParameter("@TypeID",typeid),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@OrderStatus",orderStatus),
                                       new SqlParameter("@Mark",mark),
                                       new SqlParameter("@SourceType",sourceType),
                                       new SqlParameter("@PayStatus",paystatus),
                                       new SqlParameter("@InvoiceStatus",invoicestatus),
                                       new SqlParameter("@ReturnStatus",returnstatus),
                                       new SqlParameter("@SearchUserID",searchuserid),
                                       new SqlParameter("@SearchTeamID",searchteamid),
                                       new SqlParameter("@SearchAgentID",searchagentid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@Keywords",keyWords),
                                       new SqlParameter("@OrderColumn",orderBy),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetOrders", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public DataTable GetOrderByID(string orderid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderid)
                                   };

            DataTable dt = GetDataTable("Select * from Orders where OrderID=@OrderID", paras, CommandType.Text);
            return dt;
        }

        public DataSet GetOrdersByMobilePhone(string mobilePhone)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@MobilePhone",mobilePhone)
                                   };

            DataSet ds = GetDataSet("P_GetOrdersByMobilePhone", paras, CommandType.StoredProcedure, "Orders|Status");
            return ds;
        }

        public DataSet GetOrderByID(string orderid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetOrderByID", paras, CommandType.StoredProcedure, "Order|Customer|Details|Goods|Tasks|Status");
            return ds;
        }

        public DataSet GetOrderBaseInfoByID(string orderid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("p_GetOrderBaseInfoByID", paras, CommandType.StoredProcedure, "Order|Details");
            return ds;
        }

        public DataTable GetOrderPlans(string ownerID, string beginDate, string endDate, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@OwnerID",ownerID),
                                       new SqlParameter("@BeginDate",beginDate),
                                       new SqlParameter("@EndDate",endDate),
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@PageSize",pageSize),
                                       new SqlParameter("@PageIndex",pageIndex),
   
                                   };

            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;

            DataSet ds = GetDataSet("P_GetOrderPlans", paras, CommandType.StoredProcedure);

            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);

            return ds.Tables[0];
        }

        public DataTable GetOrderCosts(string orderid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderid)
                                   };

            DataTable dt = GetDataTable("Select * from OrderCosts where OrderID=@OrderID and Status=1 ", paras, CommandType.Text);
            return dt;
        }

        public DataTable GetOrderGoods(string orderid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderid)
                                   };

            DataTable dt = GetDataTable("Select * from OrderGoods where OrderID=@OrderID ", paras, CommandType.Text);
            return dt;
        }
        #endregion

        #region 添加

        public bool CreateOrder(string orderid, string ordercode, string aliOrderCode, string goodscode, string title, string customerid, string name, string mobile,
                                int sourcetype, int ordertype, string bigcategoryid, string categoryid, string price, int quantity, string planTime,
                                string orderimg, string orderimages, string citycode, string address, string expressCode, string remark, string operateid, string agentid, string clientid)
        {
            int result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OrderCode",ordercode),
                                     new SqlParameter("@AliOrderCode",aliOrderCode),
                                     new SqlParameter("@GoodsCode",goodscode),
                                     new SqlParameter("@Title",title),
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@Mobile" , mobile),
                                     new SqlParameter("@SourceType" , sourcetype),
                                     new SqlParameter("@OrderType" , ordertype),
                                     new SqlParameter("@BigCategoryID" , bigcategoryid),
                                     new SqlParameter("@CategoryID" , categoryid),
                                     new SqlParameter("@PlanPrice" , price),
                                     new SqlParameter("@PlanQuantity" , quantity),
                                     new SqlParameter("@PlanTime" , planTime),
                                     new SqlParameter("@OrderImg" , orderimg),
                                     new SqlParameter("@OrderImages" , orderimages),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@ExpressCode" , expressCode),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@UserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_CreateOrder", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result > 0;
        }

        public bool CreateDHOrder(string orderid, string originalid, decimal discount, string operateid, string clientid, SqlTransaction tran)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OriginalID",originalid),
                                     new SqlParameter("@Discount",discount),
                                     new SqlParameter("@OrderCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            bool bl = ExecuteNonQuery(tran, "P_CreateDHOrder", paras, CommandType.StoredProcedure) > 0;

            return bl;
        }

        public bool AddOrderGoods(string orderid, string saleattr, string attrvalues, string saleattrvalue, decimal quantity, string remark, string operateid, string clientid, SqlTransaction tran)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AttrList",saleattr),
                                     new SqlParameter("@ValueList",attrvalues),
                                     new SqlParameter("@AttrValueList",saleattrvalue),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@Description",remark),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            bool bl = ExecuteNonQuery(tran, "P_AddOrderGoods", paras, CommandType.StoredProcedure) > 0;

            return bl;
        }

        public bool CreateOrderGoodsDoc(string docid, string orderid, string taskid, int doctype, int isover, string expressid, string expresscode, string details, string remark, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@TaskID",taskid),
                                     new SqlParameter("@DocType",doctype),
                                     new SqlParameter("@IsOver",isover),
                                     new SqlParameter("@ExpressID",expressid),
                                     new SqlParameter("@ExpressCode",expresscode),
                                     new SqlParameter("@GoodDetails",details),
                                     new SqlParameter("@Remark",remark),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
    
            bool bl = ExecuteNonQuery("P_CreateOrderGoodsDoc", paras, CommandType.StoredProcedure) > 0;

            return bl;
        }

        

        public string CreateReply(string guid,string stageID,int mark, string content, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            string replyID = Guid.NewGuid().ToString();

            SqlParameter[] paras = { 
                                     new SqlParameter("@ReplyID",replyID),
                                     new SqlParameter("@GUID",guid),
                                     new SqlParameter("@StageID",stageID),
                                     new SqlParameter("@Mark",mark),
                                     new SqlParameter("@Content",content),
                                     new SqlParameter("@FromReplyID",fromReplyID),
                                     new SqlParameter("@CreateUserID" , userID),
                                     new SqlParameter("@AgentID" , agentID),
                                     new SqlParameter("@FromReplyUserID" , fromReplyUserID),
                                     new SqlParameter("@FromReplyAgentID" , fromReplyAgentID),
                                   };

            return ExecuteNonQuery("P_CreateOrderReply", paras, CommandType.StoredProcedure) > 0 ? replyID : string.Empty;
        }

        public bool CreateOrderCost(string orderid, decimal price, string remark, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@Price",price),
                                     new SqlParameter("@Remark",remark),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_CreateOrderCost", paras, CommandType.StoredProcedure) > 0;
        }
        #endregion

        #region 编辑、删除

        public bool UpdateOrderOwner(string orderid, string userid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderOwner", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderPrice(string orderid, string autoid, decimal price, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Price",price),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderPrice", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateProductQuantity(string orderid, string autoid, decimal quantity, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderProductQuantity", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateProductLoss(string orderid, string autoid, decimal quantity, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderProductLoss", paras, CommandType.StoredProcedure) > 0;
        }

        public bool DeleteProduct(string orderid, string autoid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_DeleteOrderProduct", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderProcess(string orderid, string processid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@ProcessID",processid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderProcess", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderCategoryID(string orderid, string pid, string categoryid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@PID",pid),
                                     new SqlParameter("@CategoryID",categoryid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderCategoryID", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderStatus(string orderid, int status, string time, decimal price, string operateid, string agentid, string clientid, out string errinfo)
        {
            errinfo = "";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ErrorInfo",SqlDbType.NVarChar,100),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@Status",status),
                                     new SqlParameter("@PlanTime",time),
                                     new SqlParameter("@FinalPrice",price),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            paras[0].Value = errinfo;
            paras[0].Direction = ParameterDirection.InputOutput;
            bool bl = ExecuteNonQuery("P_UpdateOrderStatus", paras, CommandType.StoredProcedure) > 0;
            errinfo = paras[0].Value.ToString();

            return bl;
        }

        public bool UpdateProfitPrice(string orderid, decimal profit, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@Profit",profit),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateProfitPrice", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderDiscount(string orderid, decimal discount, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@Discount",discount),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderDiscount", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderClient(string orderid, string  newclientid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@NewClientID",newclientid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderClient", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderOriginalID(string orderid, string originalorderid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OriginalID",originalorderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderOriginalID", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderCustomer(string orderid, string customerid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderCustomer", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderImages(string orderid, string image, string images, string agentid, string clientid)
        {
            string sql = "Update Orders set OrderImage=@OrderImage,OrderImages=@OrderImages where OrderID=@OrderID and ClientID=@ClientID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OrderImage",image),
                                     new SqlParameter("@OrderImages" , images),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateOrderPlateAttr(string orderid, string taskID, string valueIDS, string platehtml, string createUserID, string agentID, string clientID)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@Platehtml",platehtml),
                                     new SqlParameter("@ValueIDS",valueIDS),
                                     new SqlParameter("@CreateUserID",createUserID),
                                     new SqlParameter("@AgentID",agentID),
                                     new SqlParameter("@ClientID",clientID)
                                   };

            return ExecuteNonQuery("P_UpdateOrderPlateAttr", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderPlateRemark(string orderid, string plateRemark)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@PlateRemark",plateRemark)
                                   };
            string sqlText = " update orders set PlateRemark=@PlateRemark where OrderID=@OrderID";

            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteOrder(string orderid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_DeleteOrder", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderOver(string orderid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderOver", paras, CommandType.StoredProcedure) > 0;
        }

        public bool EditOrder(string orderid, string goodsCode, string goodsName, string personName, string mobileTele, string cityCode, string address,
                                string postalcode, string typeid, int expresstype, string remark, string operateid, string agentid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@IntGoodsCode",goodsCode),
                                     new SqlParameter("@GoodsName" , goodsName),
                                     new SqlParameter("@PersonName",personName),
                                     new SqlParameter("@MobileTele" , mobileTele),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@PostalCode" , postalcode),
                                     new SqlParameter("@TypeID" , typeid),
                                     new SqlParameter("@ExpressType" , expresstype),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@UserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_EditOrder", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool EffectiveOrderProduct(string orderid, string operateid, string agentid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@BillingCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_EffectiveOrderProduct", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool ApplyReturnOrder(string orderid, string operateid, string agentid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_ApplyReturnOrder", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool UpdateReturnQuantity(string orderid, string autoid, int quantity, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateReturnQuantity", paras, CommandType.StoredProcedure) > 0;
        }

        public bool ApplyReturnProduct(string orderid, string operateid, string agentid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_ApplyReturnProduct", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool UpdateOpportunityStage(string opportunityid, string stageid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OpportunityID",opportunityid),
                                     new SqlParameter("@StageID",stageid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOpportunityStage", paras, CommandType.StoredProcedure) > 0;
        }

        public bool CreateOrderCustomer(string orderid, string operateid, string agentid, string clientid, out string customerid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",SqlDbType.NVarChar,64),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_CreateOrderCustomer", paras, CommandType.StoredProcedure);
            customerid = paras[0].Value.ToString();
            return !string.IsNullOrEmpty(customerid);
        }

        public bool DeleteOrderCost(string orderid, string autoid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_DeleteOrderCost", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion

    }
}
