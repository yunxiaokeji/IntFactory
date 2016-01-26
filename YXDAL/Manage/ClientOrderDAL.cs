using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL.Manage
{
    public class ClientOrderDAL:BaseDAL
    {
        public static ClientOrderDAL BaseProvider = new ClientOrderDAL();


        #region 添加

        public  bool AddClientOrder(string orderID, int userQuantity, int years, decimal amount, decimal realAmount,int type, string agentID, string clientiD, string createUserID,int payType,int systemType, SqlTransaction tran)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID),
                                     new SqlParameter("@UserQuantity",userQuantity),
                                     new SqlParameter("@Years" , years),
                                     new SqlParameter("@Amount" , amount),
                                     new SqlParameter("@RealAmount" ,realAmount),
                                      new SqlParameter("@Type" , type),
                                     new SqlParameter("@AgentID" , agentID),
                                     new SqlParameter("@ClientiD" , clientiD),
                                     new SqlParameter("@CreateUserID" , createUserID),
                                     new SqlParameter("@PayType" , payType),
                                     new SqlParameter("@SystemType" , systemType)
                                   };
            return ExecuteNonQuery(tran, "M_AddClientOrder", paras, CommandType.StoredProcedure) > 0;
        }

        public bool AddClientOrderDetail(string orderID, string productID, decimal price, int qunatity, string createUserID,SqlTransaction tran)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID),
                                     new SqlParameter("@ProductID",productID),
                                     new SqlParameter("@Price" , price),
                                     new SqlParameter("@Quantity" , qunatity),
                                     new SqlParameter("@CreateUserID" , createUserID)
                                   };


            return ExecuteNonQuery(tran, "M_AddClientOrderDetail", paras, CommandType.StoredProcedure) > 0;

        }

        public bool PayOrderAndAuthorizeClient(string orderID)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@M_OrderID",orderID)
                                   };


            return ExecuteNonQuery("M_PayOrderAndAuthorizeClient", paras, CommandType.StoredProcedure) > 0;
        }
        #endregion

        #region 查
        public DataTable GetClientOrderInfo(string orderID)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID)
                                   };

            string cmdText = "select * from ClientOrder where orderID=@orderID and status<>9";

            return GetDataTable(cmdText, paras, CommandType.Text);
        }

        public DataTable GetClientOrders(int status,int type, string beginDate, string endDate, string agentID, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@totalCount",SqlDbType.Int),
                                    new SqlParameter("@pageCount",SqlDbType.Int),
                                     new SqlParameter("@Status",status),
                                     new SqlParameter("@Type",type),
                                     new SqlParameter("@BeginDate",beginDate),
                                     new SqlParameter("@EndDate",endDate),
                                     new SqlParameter("@AgentID",agentID),
                                     new SqlParameter("@ClientID",clientID),
                                   
                                    new SqlParameter("@pageSize",pageSize),
                                    new SqlParameter("@pageIndex",pageIndex)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;

            DataTable dt= GetDataTable("M_GetClientOrders", paras, CommandType.StoredProcedure);

            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);

            return dt;
        }
        #endregion

        #region 改
        public bool UpdateClientOrderStatus(string orderID, int status)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID),
                                     new SqlParameter("@Status",status),
                                   };

            string cmdText = "update ClientOrder set Status=@Status  where orderID=@orderID";

            return ExecuteNonQuery(cmdText, paras, CommandType.Text)>0;
        }

        public bool UpdateOrderAmount(string orderID, decimal amount)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID),
                                     new SqlParameter("@Amount",amount),
                                   };

            string cmdText = "update ClientOrder set RealAmount=@Amount  where orderID=@orderID";

            return ExecuteNonQuery(cmdText, paras, CommandType.Text) > 0;
        }
        #endregion
    }
}
