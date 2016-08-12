using IntFactoryDAL;
using IntFactoryEntity;
using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryBusiness
{
    public class ReplyBusiness
    {
        public static ReplyBusiness BaseBusiness = new ReplyBusiness();

        #region 查询

        public static List<ReplyEntity> GetReplys(string guid, EnumLogObjectType type, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid)
        {
            switch (type)
            {
                case EnumLogObjectType.Customer:
                    return GetCustomerReplys(guid, pageSize, pageIndex, ref totalCount, ref pageCount);
                case EnumLogObjectType.OrderTask:
                    return GetTaskReplys(guid, pageSize, pageIndex, ref totalCount, ref pageCount);
                case EnumLogObjectType.Orders:
                    break;
            }

            return null;
        }

        public static List<ReplyEntity> GetCustomerReplys(string guid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();

            DataSet ds = ReplyDAL.BaseProvider.GetCustomerReplys(guid, pageSize, pageIndex, ref totalCount, ref pageCount);
            DataTable replys = ds.Tables["Replys"];
            DataTable attachments = ds.Tables["Attachments"];
            foreach (DataRow dr in replys.Rows)
            {
                ReplyEntity model = new ReplyEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserCacheByUserID(model.CreateUserID, model.ClientID);
                if (!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserCacheByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }

                if (attachments.Rows.Count > 0)
                {
                    model.Attachments = new List<Attachment>();
                    foreach (DataRow dr2 in attachments.Select(" Guid='" + model.ReplyID + "'"))
                    {
                        Attachment attachment = new Attachment();
                        attachment.FillData(dr2);

                        model.Attachments.Add(attachment);
                    }
                }
                list.Add(model);
            }

            return list;

        }

        public static List<ReplyEntity> GetTaskReplys(string guid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();

            DataSet ds = ReplyDAL.BaseProvider.GetTaskReplys(guid, pageSize, pageIndex, ref totalCount, ref pageCount);
            DataTable replys = ds.Tables["Replys"];
            DataTable attachments = ds.Tables["Attachments"];
            foreach (DataRow dr in replys.Rows)
            {
                ReplyEntity model = new ReplyEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserCacheByUserID(model.CreateUserID, model.ClientID);
                if (!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserCacheByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }

                model.Attachments = new List<Attachment>();
                if (attachments.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in attachments.Select(" Guid='" + model.ReplyID + "'"))
                    {
                        Attachment attachment = new Attachment();
                        attachment.FillData(dr2);

                        model.Attachments.Add(attachment);
                    }
                }
                list.Add(model);
            }

            return list;

        }

        #endregion

        #region 添加.删除


        public static string CreateCustomerReply(string guid, string content, string userID, string clientid, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            return ReplyDAL.BaseProvider.CreateCustomerReply(guid, content, userID, clientid, fromReplyID, fromReplyUserID, fromReplyAgentID);
        }

        public static bool AddCustomerReplyAttachments(string customerid, string replyid, List<Attachment> attachments, string userid, string clientid)
        {
            SqlConnection conn = new SqlConnection(CustomDAL.ConnectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();

            foreach (var attachment in attachments)
            {
                if (!ReplyDAL.BaseProvider.AddCustomerReplyAttachments(customerid, replyid, attachment.Type,
                    attachment.ServerUrl, attachment.FilePath, attachment.FileName, attachment.OriginalName, attachment.ThumbnailName, attachment.Size,
                    userid, clientid, tran))
                {
                    tran.Rollback();
                    conn.Dispose();

                    return false;
                }
            }

            tran.Commit();
            conn.Dispose();

            return true;
        }

        public static string CreateTaskReply(string guid, string content, string userID, string clientid, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            return ReplyDAL.BaseProvider.CreateTaskReply(guid, content, userID, clientid, fromReplyID, fromReplyUserID, fromReplyAgentID);
        }

        public static bool AddTaskReplyAttachments(string taskid, string replyid, List<Attachment> attachments, string userid, string clientid)
        {
            SqlConnection conn = new SqlConnection(TaskDAL.ConnectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();


            foreach (var attachment in attachments)
            {
                if (!ReplyDAL.BaseProvider.AddTaskReplyAttachment(taskid, replyid, attachment.Type,
                    attachment.ServerUrl, attachment.FilePath, attachment.FileName, attachment.OriginalName, attachment.ThumbnailName, attachment.Size,
                    userid, clientid, tran))
                {
                    tran.Rollback();
                    conn.Dispose();

                    return false;
                }
            }

            tran.Commit();
            conn.Dispose();

            return true;
        }

        public static bool DeleteReply(EnumLogObjectType type, string replyid)
        {
            string tablename = "";
            switch (type)
            {
                case EnumLogObjectType.Customer:
                    tablename = "CustomerReply";
                    break;
                case EnumLogObjectType.OrderTask:
                    tablename = "TaskReply";
                    break;
                case EnumLogObjectType.Orders:
                    tablename = "OrderReply";
                    break;
            }

            bool bl = CommonBusiness.Update(tablename, "Status", 9, "ReplyID='" + replyid + "'");
            return bl;
        }

        #endregion
    }
}
