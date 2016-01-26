using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class SalesRPTDAL : BaseDAL
    {
        public static SalesRPTDAL BaseProvider = new SalesRPTDAL();

        public DataSet GetUserOrders(string userid, string teamid, string begintime, string endtime, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@TeamID",teamid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetUserOrders", paras, CommandType.StoredProcedure, "Users");
            return ds;
        }

        public DataTable GetOrderMapReport(int type, string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetOrderMapReport", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataSet GetOpportunityStageRate(string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetOpportunityStageRate", paras, CommandType.StoredProcedure, "Data");
            return ds;
        }

        public DataSet GetUserOpportunitys(string userid, string teamid, string begintime, string endtime, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@TeamID",teamid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetUserOpportunitys", paras, CommandType.StoredProcedure, "Users");
            return ds;
        }
    }
}
