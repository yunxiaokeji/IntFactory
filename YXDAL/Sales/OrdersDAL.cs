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

        public DataSet GetOrders(int searchOrderType, int searchtype, string entrustType, string typeid, int status, int sourceType, int orderStatus,int publicStatus,
                                int mark, int paystatus, int warningstatus, int returnstatus, string searchuserid, string searchteamid, string begintime, string endtime,
                                string keyWords, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@SearchOrderType",searchOrderType),
                                       new SqlParameter("@SearchType",searchtype),
                                       new SqlParameter("@EntrustType",entrustType),
                                       new SqlParameter("@TypeID",typeid),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@OrderStatus",orderStatus),
                                       new SqlParameter("@PublicStatus",publicStatus),
                                       new SqlParameter("@Mark",mark),
                                       new SqlParameter("@SourceType",sourceType),
                                       new SqlParameter("@PayStatus",paystatus),
                                       new SqlParameter("@WarningStatus",warningstatus),
                                       new SqlParameter("@ReturnStatus",returnstatus),
                                       new SqlParameter("@SearchUserID",searchuserid),
                                       new SqlParameter("@SearchTeamID",searchteamid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@Keywords",keyWords),
                                       new SqlParameter("@OrderColumn",orderBy),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@UserID",userid),
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


        public DataSet GetOrdersByYXCode(string clientid, string keyWords,int pageSize, int pageIndex, ref int totalCount, ref int pageCount,
            string categoryID, string orderby, string beginPrice, string endPrice) 
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@CategoryID",categoryID),
                                       new SqlParameter("@OrderBy",orderby),
                                       new SqlParameter("@BeginPrice",beginPrice),
                                       new SqlParameter("@EndPrice",endPrice),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;
            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetOrdersByYXCode", paras, CommandType.StoredProcedure, "Orders");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);

            return ds;
        }


        public DataTable GetOrdersByPlanTime(string startPlanTime, string endPlanTime, int orderType, int filterType, int orderStatus,
            string userID, string clientID, string andWhere, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@StartPlanTime",startPlanTime),
                                       new SqlParameter("@EndPlanTime",endPlanTime),
                                       new SqlParameter("@OrderType",orderType),
                                       new SqlParameter("@AndWhere",andWhere),
                                       new SqlParameter("@OrderStatus",orderStatus),
                                       new SqlParameter("@FilterType",filterType),
                                       new SqlParameter("@UserID",userID),
                                       new SqlParameter("@ClientID",clientID)
                                   };

            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataTable dt = GetDataTable("P_GetOrdersByPlanTime", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return dt;
        }

        public int GetExceedOrderCount(string ownerID, string orderType, string clientID)
        {
            SqlParameter[] paras = {
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@OwnerID",ownerID)
                                   };

            string sql = "select count(orderid) from orders where OrderStatus=1 and status<>9 and PlanTime<getdate() and (ClientID=@ClientID or EntrustClientID=@ClientID)";
            if (orderType != "-1")
            {
                sql += orderType;
            }
            if (!string.IsNullOrEmpty(ownerID))
            {
                sql += " and (OwnerID=@OwnerID or CreateUserID=@OwnerID)";
            }

            return (int)ExecuteScalar(sql, paras, CommandType.Text);
        }

        public DataSet GetOrderByID(string orderid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetOrderByID", paras, CommandType.StoredProcedure, "Order|Details|Goods|Tasks|Attrs");
            return ds;
        }

        public DataSet GetGoodsByID(string goodsid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@GoodsID",goodsid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetGoodsByID", paras, CommandType.StoredProcedure, "Goods");
            return ds;
        }

        public DataTable GetOrderDetailsByOrderID(string orderid)
        { 
            SqlParameter[] paras={
                                    new SqlParameter("@OrderID",orderid)
                                 };
            return GetDataTable("select * from OrderDetail where OrderID=@OrderID", paras, CommandType.Text);
        }

        public DataTable GetOrderAttrsByOrderID(string orderid)
        {
            SqlParameter[] paras ={
                                    new SqlParameter("@OrderID",orderid)
                                 };
            return GetDataTable("select * from OrderAttrs where OrderID=@OrderID Order by Sort", paras, CommandType.Text);
        }
        public DataTable GetOrderAttrsByGoodsID(string goodsID)
        {
            SqlParameter[] paras ={
                                    new SqlParameter("@GoodsID",goodsID)
                                 };
            return GetDataTable("select * from OrderAttrs where goodsID=@GoodsID Order by Sort", paras, CommandType.Text);
        }
        public DataSet GetOrderForFentReport(string orderid,  string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetOrderForFentReport", paras, CommandType.StoredProcedure, "Order|Details|Tasks|OrderCoss");
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

            DataTable dt = GetDataTable("Select * from OrderGoods where OrderID=@OrderID Order by Sort ", paras, CommandType.Text);
            return dt;
        }
        #endregion

        #region 添加

        public bool CreateOrder(string orderid, string ordercode, string aliOrderCode, string goodscode, string title, string customerid, string name, string mobile,
                                int sourcetype, int ordertype, string bigcategoryid, string categoryid, decimal price, int quantity, string planTime,
                                string orderimg, string orderimages, string citycode, string address, string expressCode, string remark, string operateid, string clientid)
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
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_CreateOrder", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result > 0;
        }

        public bool CreateDHOrder(string orderid, string originalid, decimal discount, decimal price, string operateid, string clientid, string yxOrderID, SqlTransaction tran, string yxClientID, string personname, string mobiletele, string citycode, string address)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OriginalID",originalid),
                                     new SqlParameter("@Discount",discount),
                                     new SqlParameter("@Price",price),
                                     new SqlParameter("@OrderCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@YXOrderID" , yxOrderID),
                                     new SqlParameter("@YXClientID" , yxClientID),
                                     new SqlParameter("@PersonName" , personname),
                                     new SqlParameter("@MobileTele" , mobiletele),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address)
                                   };
           
            bool bl = ExecuteNonQuery(tran, "P_CreateDHOrder", paras, CommandType.StoredProcedure) > 0;

            return bl;
        }

        public bool AddOrderGoods(string orderid, string saleattr, string attrvalues, string saleattrvalue, decimal quantity, string xRemark, int sort, string yRemark, string xyRemark, string remark, string operateid, string clientid, SqlTransaction tran)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AttrList",saleattr),
                                     new SqlParameter("@ValueList",attrvalues),
                                     new SqlParameter("@AttrValueList",saleattrvalue),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@XRemark",xRemark),
                                     new SqlParameter("@Sort",sort),
                                     new SqlParameter("@YRemark",yRemark),
                                     new SqlParameter("@XYRemark",xyRemark),
                                     new SqlParameter("@Description",remark),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            bool bl = ExecuteNonQuery(tran, "P_AddOrderGoods", paras, CommandType.StoredProcedure) > 0;

            return bl;
        }

        public bool CreateOrderGoodsDoc(string docid, string orderid, string taskid, int doctype, int isover, string expressid, string expresscode, string details, string remark,string ownerid, string operateid, string clientid)
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
                                     new SqlParameter("@OwnerID",ownerid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
    
            bool bl = ExecuteNonQuery("P_CreateOrderGoodsDoc", paras, CommandType.StoredProcedure) > 0;

            return bl;
        }

        public bool CreateGoodsDocReturn(string guid, string orderID, string taskID, int docType, string details, string originalID, string operateid, string clientID, ref int result)
        {
            SqlParameter[] paras ={ 
                                    new SqlParameter("@Result",result),
                                    new SqlParameter("@DocID",guid),
                                    new SqlParameter("@OrderID",orderID),
                                    new SqlParameter("@TaskID",taskID),
                                    new SqlParameter("@DocType",docType),
                                    new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                    new SqlParameter("@GoodDetails",details),
                                    new SqlParameter("@OriginalID",originalID),
                                    new SqlParameter("@OperateID",operateid),
                                    new SqlParameter("@ClientID",clientID)
                                  };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_CreateGoodsDocReturn", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool CreateProductUseQuantity(ref int result,ref string errInfo,string orderID,string details,string userID,string operateIP,string clientID)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@Result",result),
                                    new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                    new SqlParameter("@OrderID",orderID),
                                    new SqlParameter("@ProductsDetails",details),
                                    new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddmmhhssfff")),
                                    new SqlParameter("@UserID",userID),
                                    new SqlParameter("@OperateIP",operateIP),
                                    new SqlParameter("@ClientID",clientID)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            paras[1].Value = errInfo;
            paras[1].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_CreateProductUseQuantity", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            errInfo = paras[1].Value.ToString();
            return result == 1;
        }

        public bool CreateOrderCost(string orderid, decimal price, string remark, string operateid,  string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@Price",price),
                                     new SqlParameter("@Remark",remark),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_CreateOrderCost", paras, CommandType.StoredProcedure) > 0;
        }
        #endregion

        #region 编辑、删除

        public bool UpdateOrderOwner(string orderid, string userid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderOwner", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderPrice(string orderid, string taskid, string autoid, decimal price, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@TaskID",taskid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Price",price),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderPrice", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateProductOrderQuantity(string orderid, string taskid, string autoid, int quantity, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@TaskID",taskid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateProductOrderQuantity", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateProductQuantity(string orderid, string taskid, string autoid, decimal quantity, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@TaskID",taskid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderProductQuantity", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateProductPlanQuantity(string orderid, string taskid, string autoid, decimal quantity, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@TaskID",taskid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateProductPlanQuantity", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateProductLoss(string orderid, string autoid, decimal quantity, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderProductLoss", paras, CommandType.StoredProcedure) > 0;
        }

        public bool DeleteProduct(string orderid, string taskid, string autoid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@TaskID",taskid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_DeleteOrderProduct", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderProcess(string orderid, string processid, string categoryid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@CategoryID",categoryid),
                                     new SqlParameter("@ProcessID",processid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderProcess", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderCategoryID(string orderid, string pid, string categoryid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@PID",pid),
                                     new SqlParameter("@CategoryID",categoryid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderCategoryID", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderBegin(string orderid, string time, string operateid, string clientid, out string errinfo)
        {
            errinfo = "";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ErrorInfo",SqlDbType.NVarChar,100),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@PlanTime",time),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            paras[0].Value = errinfo;
            paras[0].Direction = ParameterDirection.InputOutput;
            bool bl = ExecuteNonQuery("P_UpdateOrderBegin", paras, CommandType.StoredProcedure) > 0;
            errinfo = paras[0].Value.ToString();

            return bl;
        }

        public bool UpdateOrderStatus(string orderid, int status, string time, decimal price, string operateid,string clientid, out string errinfo)
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
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            paras[0].Value = errinfo;
            paras[0].Direction = ParameterDirection.InputOutput;
            bool bl = ExecuteNonQuery("P_UpdateOrderStatus", paras, CommandType.StoredProcedure) > 0;
            errinfo = paras[0].Value.ToString();

            return bl;
        }

        public bool UpdateProfitPrice(string orderid, decimal profit, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@Profit",profit),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateProfitPrice", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderPlanTime(string orderid, string planTime, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@PlanTime",planTime),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderPlanTime", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderDiscount(string orderid, decimal discount, decimal price, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@Discount",discount),
                                     new SqlParameter("@Price",price),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderDiscount", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderTotalMoney(string orderid, decimal totalMoney, string operateid, string clientid)
        {
            return true; //无效，暂留
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@TotalMoney",totalMoney),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderTotalMoney", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderClient(string orderid, string  newclientid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@NewClientID",newclientid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderClient", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderOriginalID(string orderid, string originalorderid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OriginalID",originalorderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderOriginalID", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderCustomer(string orderid, string customerid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderCustomer", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderImages(string orderid, string image, string images, string clientid)
        {
            string sql = "Update Orders set OrderImage=@OrderImage,OrderImages=@OrderImages where OrderID=@OrderID and ClientID=@ClientID";
            sql += " update OrderTask set OrderImg=@OrderImage where OrderID=@OrderID and ClientID=@ClientID ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OrderImage",image),
                                     new SqlParameter("@OrderImages" , images),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateOrderPlateAttr(string orderid, string platehtml)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@Platehtml",platehtml)
                                   };

            return ExecuteNonQuery("P_UpdateOrderPlateAttr", paras, CommandType.StoredProcedure) > 0;
        }

        public bool DeleteOrder(string orderid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_DeleteOrder", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateOrderOver(string orderid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateOrderOver", paras, CommandType.StoredProcedure) > 0;
        }

        public bool EditOrder(string orderid, string goodsCode, string goodsName, string personName, string mobileTele, string cityCode, string address,
                                string postalcode, int expresstype, string remark, string operateid, string clientid, out int result)
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
                                     new SqlParameter("@ExpressType" , expresstype),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@UserID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_EditOrder", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool EffectiveOrderProduct(string orderid, string operateid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@BillingCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_EffectiveOrderProduct", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool ApplyReturnOrder(string orderid, string operateid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_ApplyReturnOrder", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool CreateOrderCustomer(string orderid, string operateid, string clientid, out string customerid, out int result)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",SqlDbType.NVarChar,64),
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            paras[1].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_CreateOrderCustomer", paras, CommandType.StoredProcedure);
            customerid = paras[0].Value.ToString();
            result = Convert.ToInt32(paras[1].Value);
            return !string.IsNullOrEmpty(customerid);
        }

        public bool DeleteOrderCost(string orderid, string autoid, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_DeleteOrderCost", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateGoodsPublicStatus(string goodsid, int publicStatus)
        {
            string sql = " update goods set IsPublic=@IsPublic where GoodsID=@GoodsID ";
            sql += "  update orders set IsPublic=@IsPublic where GoodsID=@GoodsID and ordertype=1";
            SqlParameter[] paras = { 
                                     new SqlParameter("@GoodsID",goodsid),
                                     new SqlParameter("@IsPublic",publicStatus)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }
        #endregion

        #region OrderPriceRange
        public DataTable GetOrderPriceRanges(string orderid)
        {
            string sqltext = "select * from OrderPriceRange where OrderID=@OrderID and status<>9  order by MinQuantity asc";

            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderid)
                                   };

            return GetDataTable(sqltext, paras, CommandType.Text);
        }


        public string AddOrderPriceRange(int minQuantity, decimal price, string orderid, string userid, string clientid)
        {
            var rangeID = Guid.NewGuid().ToString();
            SqlParameter[] paras = { 
                                     new SqlParameter("@RangeID",rangeID),
                                     new SqlParameter("@MinQuantity",minQuantity),
                                     new SqlParameter("@Price",price),
                                     new SqlParameter("@OrderID",orderid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@ClientID",clientid)
                                   };

            string sql = "insert OrderPriceRange(RangeID,MinQuantity,Price,OrderID,CreateUserID,ClientID) values(@RangeID,@MinQuantity,@Price,@OrderID,@UserID,@ClientID)";

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0?rangeID:string.Empty;
        }

        public bool UpdateOrderPriceRange(string rangeid, int minQuantity, decimal price)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@RangeID",rangeid),
                                     new SqlParameter("@MinQuantity",minQuantity),
                                     new SqlParameter("@Price",price)
                                   };
            string sql = "update OrderPriceRange set MinQuantity=@MinQuantity,Price=@Price where RangeID=@RangeID";

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool DeleteOrderPriceRange(string rangeid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@RangeID",rangeid)
                                   };
            string sql = "update OrderPriceRange set status=9 where RangeID=@RangeID";

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }
        #endregion
    }
}
