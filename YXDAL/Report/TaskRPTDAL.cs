﻿using System;
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

        public DataTable GetUserLoadReport(string begintime, string endtime, int docType, string UserID, string TeamID, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@DocType",docType),
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

        public DataTable GetOrderProductionRPT(int timeType, string begintime, string endtime, string keyWords, string UserID, string TeamID, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@TimeType",timeType),
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
    }
}
