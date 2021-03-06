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

        public bool CreateTask(string orderID)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID)
                                   };

            return ExecuteNonQuery("P_CreateTask", paras, CommandType.StoredProcedure) > 0;
        }

        public DataTable GetTasks(string keyWords, string ownerID, int isParticipate, int status, int finishStatus, int invoiceStatus, int preFinishStatus,
            int colorMark, int taskType, string beginDate, string endDate, string beginEndDate, string endEndDate,
            int orderType, string orderProcessID, string orderStageID, int taskOrderColumn, int isAsc, string clientID,
            int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@OwnerID",ownerID),
                                       new SqlParameter("@IsParticipate",isParticipate),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@FinishStatus",finishStatus),
                                       new SqlParameter("@InvoiceStatus",invoiceStatus),
                                       new SqlParameter("@PreFinishStatus",preFinishStatus),
                                       new SqlParameter("@OrderType",orderType),
                                       new SqlParameter("@OrderProcessID",orderProcessID),
                                       new SqlParameter("@OrderStageID",orderStageID),
                                       new SqlParameter("@ColorMark",colorMark),
                                       new SqlParameter("@TaskType",taskType),
                                       new SqlParameter("@KeyWords",keyWords),
                                       new SqlParameter("@BeginDate",beginDate),
                                       new SqlParameter("@EndDate",endDate),
                                       new SqlParameter("@BeginEndDate",beginEndDate),
                                       new SqlParameter("@EndEndDate",endEndDate),
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@TaskOrderColumn",taskOrderColumn),
                                       new SqlParameter("@IsAsc",isAsc),
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

        public DataSet GetTasksByEndTime(string startEndTime, string endEndTime,
            int orderType, int filterType, int finishStatus, int preFinishStatus, int taskType,
            string userID, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                      new SqlParameter("@StartEndTime",startEndTime),
                                       new SqlParameter("@EndEndTime",endEndTime),
                                        new SqlParameter("@OrderType",orderType),
                                       new SqlParameter("@FilterType",filterType),
                                        new SqlParameter("@FinishStatus",finishStatus),
                                         new SqlParameter("@PreFinishStatus",preFinishStatus),
                                        new SqlParameter("@TaskType",taskType),
                                       new SqlParameter("@UserID",userID),
                                       new SqlParameter("@ClientID",clientID)
   
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds= GetDataSet("P_GetTasksByEndTime", paras, CommandType.StoredProcedure,"Tasks|Orders");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public DataSet GetTaskReplys(string guid, string stageid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                      new SqlParameter("@OrderID",guid),
                                       new SqlParameter("@StageID",stageid)
   
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetTaskReplys", paras, CommandType.StoredProcedure, "Replys|Attachments");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public int GetNoAcceptTaskCount(string ownerID,int orderType, string clientID)
        {
            SqlParameter[] paras = {
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@OwnerID",ownerID),
                                       new SqlParameter("@OrderType",orderType)
                                   };

            string sql = "select count(taskid) from Ordertask where finishstatus=0 and status<>9 and ClientID=@ClientID";
            if (orderType != -1)
            {
                sql += " and OrderType=@OrderType";
            }
            if (!string.IsNullOrEmpty(ownerID))
            {
                sql += " and OwnerID=@OwnerID";
            }
            return (int)ExecuteScalar(sql, paras, CommandType.Text);
        }

        public int GetexceedTaskCount(string ownerID, int orderType, string clientID)
        {
            SqlParameter[] paras = {
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@OwnerID",ownerID),
                                       new SqlParameter("@OrderType",orderType)
                                   };

            string sql = "select count(taskid) from Ordertask where finishstatus=1 and status<>8 and endtime<getdate() and ClientID=@ClientID";
            if (orderType != -1)
            {
                sql += " and OrderType=@OrderType";
            }
            if (!string.IsNullOrEmpty(ownerID))
            {
                sql += " and OwnerID=@OwnerID";
            }
            return (int)ExecuteScalar(sql, paras, CommandType.Text);
        }

        public  DataTable GetTasksByOrderID(string orderID)
        {
            string sqltext = "select * from  OrderTask where OrderID=@OrderID and status<>9";
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderID",orderID)
                                   };

            return GetDataTable(sqltext, paras, CommandType.Text);

        }

        public DataSet GetTaskDetail(string taskID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@TaskID",taskID)
                                   };

            return GetDataSet("P_GetTaskDetail", paras, CommandType.StoredProcedure, "OrderTask|TaskMember");

        }
        public bool UpdateTaskOwner(string taskID, string ownerID, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@OwnerID",ownerID)
                                   };

            paras[0].Value = result;
            paras[0].Direction = ParameterDirection.InputOutput;

            ExecuteNonQuery("P_UpdateTaskOwner", paras, CommandType.StoredProcedure);

            result = Convert.ToInt32(paras[0].Value);

            return result == 1;
        }

        public bool AddTaskReplyAttachment(string taskid, string replyid, int attachmentType,
            string serverUrl, string filePath, string fileName, string originalName, string thumbnailName,long size,
            string userid,string clientid,SqlTransaction tran)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@TaskID",taskid),
                                     new SqlParameter("@ReplyID",replyid),
                                     new SqlParameter("@Type",attachmentType),
                                     new SqlParameter("@ServerUrl",serverUrl),
                                     new SqlParameter("@FilePath",filePath),
                                     new SqlParameter("@FileName",fileName),
                                     new SqlParameter("@OriginalName",originalName),
                                     new SqlParameter("@ThumbnailName",thumbnailName),
                                     new SqlParameter("@Size",size),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery(tran, "P_AddTaskReplyAttachment", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateTaskRemark(string taskID, string remark)
        {
            string sqltext = "update OrderTask set Remark=@Remark where TaskID=@TaskID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@Remark",remark)
                                   };

            return ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
        }

        public bool UpdateTaskEndTime(string taskID, DateTime? endTime,string userID, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@UserID",userID),
                                     new SqlParameter("@EndTime",endTime)
                                   };

            paras[0].Value = result;
            paras[0].Direction = ParameterDirection.InputOutput;
            ExecuteNonQuery("P_UpdateTaskEndTime", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);

            return result == 1;
        }

        public bool FinishTask(string taskID, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@UserID",operateid)
                                   };

            paras[0].Value = result;
            paras[0].Direction = ParameterDirection.InputOutput;

            ExecuteNonQuery("P_FinishTask", paras, CommandType.StoredProcedure);

            result = Convert.ToInt32(paras[0].Value);

            return result == 1;
        }

        public bool LockTask(string taskID, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@UserID",operateid)
                                   };

            paras[0].Value = result;
            paras[0].Direction = ParameterDirection.InputOutput;

            ExecuteNonQuery("P_LockTask", paras, CommandType.StoredProcedure);

            result = Convert.ToInt32(paras[0].Value);

            return result == 1;
        }

        public bool UnLockTask(string taskID, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@UserID",operateid)
                                   };

            paras[0].Value = result;
            paras[0].Direction = ParameterDirection.InputOutput;

            ExecuteNonQuery("P_UnLockTask", paras, CommandType.StoredProcedure);

            result = Convert.ToInt32(paras[0].Value);

            return result == 1;
        }

        public bool DeleteTaskReplyAttachment(string attachmentid, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@AttachmentID",attachmentid),
                                     new SqlParameter("@UserID",operateid)
                                   };

            paras[0].Value = result;
            paras[0].Direction = ParameterDirection.InputOutput;

            ExecuteNonQuery("P_DeleteTaskReplyAttachment", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);

            return result == 1;
        }

        public bool AddTaskMembers(string taskID, string memberIDs,string operateID, string agentID,out int result)
        {
            result=0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@TaskID",taskID),
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@UserID",operateID),
                                       new SqlParameter("@MemberIDs",memberIDs)
                                   };

            paras[0].Value = result;
            paras[0].Direction = ParameterDirection.InputOutput;
            ExecuteNonQuery("P_AddTaskMembers", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);

            return  result==1;
        }

        public bool RemoveTaskMember(string taskID,string memberID)
        {
            string sqltext = "update TaskMember set status=9  where TaskID=@TaskID and MemberID=@MemberID";
            SqlParameter[] paras = { 
                                       new SqlParameter("@TaskID",taskID),
                                       new SqlParameter("@MemberID",memberID)
                                   };

            return ExecuteNonQuery(sqltext, paras, CommandType.Text)>0;
        }

        public bool UpdateMemberPermission(string taskID, string memberID, int taskMemberPermissionType)
        {
            string sqltext = "update TaskMember set PermissionType=@PermissionType  where TaskID=@TaskID and MemberID=@MemberID";
            SqlParameter[] paras = { 
                                       new SqlParameter("@TaskID",taskID),
                                       new SqlParameter("@MemberID",memberID),
                                       new SqlParameter("@PermissionType",taskMemberPermissionType)
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


        #region PlateMaking
        public DataTable GetPlateMakings(string orderID)
        {
            string sqltext = "select * from PlateMaking where OrderID=@OrderID and status<>9  order by Type asc, createtime asc";

            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID)
                                   };

            return GetDataTable(sqltext, paras, CommandType.Text);
        }

        public DataTable GetPlateMakings(string orderID, string taskID)
        {
            string sqltext = "select * from PlateMaking where OrderID=@OrderID and TaskID=@TaskID and status<>9  order by Type asc, createtime asc";
            if (string.IsNullOrEmpty(taskID))
            {
                sqltext = "select * from PlateMaking where OrderID=@OrderID and status<>9  order by createtime desc";
            }
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderID",orderID),
                                     new SqlParameter("@TaskID",taskID)
                                   };

            return GetDataTable(sqltext, paras, CommandType.Text);
        }

        public DataTable GetPlateMakingDetail(string plateID)
        {
            string sqltext = "select * from PlateMaking where  PlateID=@PlateID and status<>9 ";

            SqlParameter[] paras = { 
                                     new SqlParameter("@PlateID",plateID)
                                   };

            return GetDataTable(sqltext, paras, CommandType.Text);
        }

        public bool AddPlateMaking(string title,string remark,string icon,string taskID,int type,string orderID,string userID,string agentID)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@Title",title),
                                     new SqlParameter("@Remark",remark),
                                     new SqlParameter("@Icon",icon),
                                     new SqlParameter("@TaskID",taskID),
                                     new SqlParameter("@Type",type),
                                     new SqlParameter("@OrderID",orderID),
                                     new SqlParameter("@UserID",userID),
                                     new SqlParameter("@AgentID",agentID)
                                   };
            return ExecuteNonQuery("P_AddPlateMaking", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdatePlateMaking(string plateID, string title, string remark, string icon,int type)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@PlateID",plateID),
                                     new SqlParameter("@Title",title),
                                     new SqlParameter("@Remark",remark),
                                     new SqlParameter("@Icon",icon),
                                     new SqlParameter("@Type",type)
                                   };

            return ExecuteNonQuery("P_UpdatePlateMaking", paras, CommandType.StoredProcedure) > 0;
        }

        public bool DeletePlateMaking(string plateID)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@PlateID",plateID)
                                   };
            return ExecuteNonQuery("P_DeletePlateMaking", paras, CommandType.StoredProcedure) > 0;
        }
        #endregion

    }
}
