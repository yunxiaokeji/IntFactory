﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
namespace IntFactoryDAL
{
    public class TaskDAL:BaseDAL
    {
        public static TaskDAL BaseProvider = new TaskDAL();

        public bool CreateTask(string orderID, string stageID)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID),
                                     new SqlParameter("@StageID",stageID)
                                   };

            return ExecuteNonQuery("P_CreateTask", paras, CommandType.StoredProcedure) > 0;
        }

        public DataTable GetTasks(string ownerID, string beginDate, string endDate, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
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

            DataSet ds = GetDataSet("P_GetTasks", paras, CommandType.StoredProcedure);

            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);

            return ds.Tables[0];
        }

        public bool UpdateTaskOwner(string taskID, string ownerID)
        {
            string sqltext = "update OrderTask set OwnerID=@OwnerID where TaskID=@TaskID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@OwnerID",ownerID)
                                   };

            return ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
        }

        public bool UpdateTaskTitle(string taskID, string title)
        {
            string sqltext = "update OrderTask set Title=@Title where TaskID=@TaskID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@Title",title)
                                   };

            return ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
        }

        public bool UpdateTaskEndTime(string taskID, DateTime endTime)
        {
            string sqltext = "update OrderTask set endTime=@EndTime where TaskID=@TaskID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@EndTime",endTime)
                                   };

            return ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
        }

        public bool FinishTask(string taskID)
        {
            string sqltext = "update OrderTask set FinishStatus=1 where TaskID=@TaskID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@TaskID",taskID)
                                   };

            return ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
        }

        public bool UnFinishTask(string taskID)
        {
            string sqltext = "update OrderTask set FinishStatus=0 where TaskID=@TaskID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@TaskID",taskID)
                                   };

            return ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
        }

    }
}
