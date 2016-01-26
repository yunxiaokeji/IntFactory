using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class AgentOrderDAL : BaseDAL
    {
        public static AgentOrderDAL BaseProvider = new AgentOrderDAL();

        public DataSet GetAgentOrders(string searchagentid, int status, int sendstatus, int returnstatus, string keywords, string begintime, string endtime, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@TotalCount",SqlDbType.Int),
                                       new SqlParameter("@PageCount",SqlDbType.Int),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@SendStatus",sendstatus),
                                       new SqlParameter("@ReturnStatus",returnstatus),
                                       new SqlParameter("@KeyWords",keywords),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@PageSize",pageSize),
                                       new SqlParameter("@PageIndex",pageIndex),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetAgentOrders", paras, CommandType.StoredProcedure, "Orders");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public DataSet GetAgentOrderByID(string orderid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetAgentOrderByID", paras, CommandType.StoredProcedure, "Order|Details");
            return ds;
        }

        public bool ConfirmAgentOrderOut(string orderid, string wareid, int issend, string expressid, string expresscode, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@WareID",wareid),
                                       new SqlParameter("@IsSend",issend),
                                       new SqlParameter("@ExpressID",expressid),
                                       new SqlParameter("@ExpressCode",expresscode),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_ConfirmAgentOrderOut", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public bool ConfirmAgentOrderSend(string orderid, string expressid, string expresscode, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@ExpressID",expressid),
                                       new SqlParameter("@ExpressCode",expresscode),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_ConfirmAgentOrderSend", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public bool InvalidApplyReturn(string orderid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_InvalidApplyReturn", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public bool AuditApplyReturn(string orderid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_AuditApplyReturn", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public bool InvalidApplyReturnProduct(string orderid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_InvalidApplyReturnProduct", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public bool AuditApplyReturnProduct(string orderid, string wareid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                       new SqlParameter("@WareID",wareid),
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_AuditApplyReturnProduct", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }
    }
}
