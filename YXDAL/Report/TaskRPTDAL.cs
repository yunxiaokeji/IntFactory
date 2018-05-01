using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class RPTDAL : BaseDAL
    {
        public static RPTDAL BaseProvider = new RPTDAL();

        public DataTable GetUserTaskQuantity(string begintime, string endtime, string UserID, string TeamID, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetUserTaskQuantity", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataTable GetUserLoadReport(string begintime, string endtime, int docType, string keyWords, string UserID, string TeamID, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@DocType",docType),
                                       new SqlParameter("@KeyWords",keyWords),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetUserWorkloadDate", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataTable GetUserSewnProcessReport(string begintime, string endtime, string UserID, string TeamID, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetUserSewnProcessReport", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataTable GetOrderProductionRPT(int timeType, int entrustType, string begintime, string endtime, string keyWords, string UserID, string TeamID, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@TimeType",timeType),
                                       new SqlParameter("@EntrustType",entrustType),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@KeyWords",keyWords),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetOrderProductionRPT", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataTable GetCustomerRateRPT(string begintime, string endtime, string keyWords, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@KeyWords",keyWords),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetCustomerRateRPT", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataTable GetKanbanRPT(string begintime, string endtime, string todaybegintime, string todayendtime, string lastbegintime, string lastendtime, string clientid)
        {
            SqlParameter[] paras = {
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@TodayBeginTime",todaybegintime),
                                       new SqlParameter("@TodayEndTime",todayendtime),
                                       new SqlParameter("@LastBeginTime",lastbegintime),
                                       new SqlParameter("@LastEndTime",lastendtime),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetKanbanRPT", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataTable GetKanbanItemRPT(string itemType,int dateType, string begintime, string endtime,string clientid)
        {
            SqlParameter[] paras = {
                                    new SqlParameter("@DateType",dateType),
                                    new SqlParameter("@ItemType",itemType),
                                    new SqlParameter("@BeginTime",begintime),
                                    new SqlParameter("@EndTime",endtime),
                                    new SqlParameter("@ClientID",clientid)
                                   };
            return GetDataTable("R_GetKanbanItemRPT", paras, CommandType.StoredProcedure);
        }

        public DataTable GetUserLoadDetailByOrderID(string begintime, string endtime, int docType, string userid, string orderid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@DocType",docType),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@OrderID",orderid)
                                   };
            DataTable dt = GetDataTable("R_GetUserLoadDetailByOrderID", paras, CommandType.StoredProcedure);
            return dt;
        }
    }
}
