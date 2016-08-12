using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class ReplyDAL : BaseDAL
    {
        public static ReplyDAL BaseProvider = new ReplyDAL();

        public DataSet GetCustomerReplys(string guid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                      new SqlParameter("@CustomerID",guid)
   
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetCustomerReplys", paras, CommandType.StoredProcedure, "Replys|Attachments");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public DataSet GetTaskReplys(string guid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                      new SqlParameter("@TaskID",guid)
   
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

        public string CreateOrderReply(string guid, string content, string userID, string clientid, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            string replyID = Guid.NewGuid().ToString();

            SqlParameter[] paras = { 
                                     new SqlParameter("@ReplyID",replyID),
                                     new SqlParameter("@GUID",guid),
                                     new SqlParameter("@Content",content),
                                     new SqlParameter("@FromReplyID",fromReplyID),
                                     new SqlParameter("@CreateUserID" , userID),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@FromReplyUserID" , fromReplyUserID),
                                     new SqlParameter("@FromReplyAgentID" , fromReplyAgentID),
                                   };

            return ExecuteNonQuery("P_CreateOrderReply", paras, CommandType.StoredProcedure) > 0 ? replyID : string.Empty;
        }

        public string CreateTaskReply(string guid, string content, string userID, string clientid, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            string replyID = Guid.NewGuid().ToString();

            SqlParameter[] paras = { 
                                     new SqlParameter("@ReplyID",replyID),
                                     new SqlParameter("@GUID",guid),
                                     new SqlParameter("@Content",content),
                                     new SqlParameter("@FromReplyID",fromReplyID),
                                     new SqlParameter("@CreateUserID" , userID),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@FromReplyUserID" , fromReplyUserID),
                                     new SqlParameter("@FromReplyAgentID" , fromReplyAgentID),
                                   };

            return ExecuteNonQuery("P_CreateTaskReply", paras, CommandType.StoredProcedure) > 0 ? replyID : string.Empty;
        }

        public string CreateCustomerReply(string guid, string content, string userID, string clientid, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            string replyID = Guid.NewGuid().ToString();

            SqlParameter[] paras = { 
                                     new SqlParameter("@ReplyID",replyID),
                                     new SqlParameter("@GUID",guid),
                                     new SqlParameter("@Content",content),
                                     new SqlParameter("@FromReplyID",fromReplyID),
                                     new SqlParameter("@CreateUserID" , userID),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@FromReplyUserID" , fromReplyUserID),
                                     new SqlParameter("@FromReplyAgentID" , fromReplyAgentID),
                                   };

            return ExecuteNonQuery("P_CreateCustomerReply", paras, CommandType.StoredProcedure) > 0 ? replyID : string.Empty;
        }

        public bool AddCustomerReplyAttachments(string customerid, string replyid, int attachmentType,
           string serverUrl, string filePath, string fileName, string originalName, string thumbnailName, long size,
           string userid, string clientid, SqlTransaction tran)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
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

            return ExecuteNonQuery(tran, "P_AddCustomerReplyAttachment", paras, CommandType.StoredProcedure) > 0;
        }

        public bool AddTaskReplyAttachment(string taskid, string replyid, int attachmentType,
           string serverUrl, string filePath, string fileName, string originalName, string thumbnailName, long size,
           string userid, string clientid, SqlTransaction tran)
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

    }
}
