using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class CustomerRPTDAL : BaseDAL
    {
        public static CustomerRPTDAL BaseProvider = new CustomerRPTDAL();

        public DataTable GetCustomerSourceScale(string begintime, string endtime, string UserID, string TeamID,string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetCustomerSourceScale", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataSet GetCustomerSourceDate(int type, string begintime, string endtime, string UserID, string TeamID, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@DateType",type),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetCustomerSourceDate", paras, CommandType.StoredProcedure, "SourceData|DateName");
            return ds;
        }

        public DataSet GetCustomerStageRate(string begintime, string endtime, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetCustomerStageRate", paras, CommandType.StoredProcedure, "Data");
            return ds;
        }

        public DataTable GetCustomerReport(int type, string begintime, string endtime, string UserID, string TeamID, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetCustomerReport", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataSet GetUserCustomers(string userid, string teamid, string begintime, string endtime, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@TeamID",teamid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetUserCustomers", paras, CommandType.StoredProcedure, "Users");
            return ds;
        }
    }
}
