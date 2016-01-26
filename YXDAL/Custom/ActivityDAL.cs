using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class ActivityDAL : BaseDAL
    {
        public static ActivityDAL BaseProvider = new ActivityDAL();

        #region 查询

        public DataTable GetActivitys(string userid, int stage,int filterType, string keyWords, string beginTime, string endTime, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@Stage",stage),
                                       new SqlParameter("@FilterType",filterType),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@BeginTime",beginTime),
                                       new SqlParameter("@EndTime",endTime),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataTable dt = GetDataTable("P_GetActivitys", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return dt;

        }

        public DataTable GetActivityByID(string activityid, string agentid, string clientid)
        {
            string sqlText = "select * from Activity where ActivityID=@ActivityID and ClientID=@ClientID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityID",activityid),
                                     new SqlParameter("@ClientID",clientid)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        public DataTable GetActivityByCode(string activitycode, string agentid, string clientid)
        {
            string sqlText = "select * from Activity where ActivityCode=@ActivityCode  and ClientID=@ClientID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityCode",activitycode),
                                     new SqlParameter("@ClientID",clientid)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        #endregion

        #region 添加

        public string GetActivityCode()
        {
            string code = GetCode(8);
            if (CommonDAL.Select("Activity", "Count(0)", "ActivityCode='" + code + "'").ToString() != "0")
            {
                return GetActivityCode();
            }
            return code;
        }

        public bool CreateActivity(string activityid, string name, string poster, string begintime, string endtime, string address, string ownerid,string memberid ,string remark, string userid, string agentid, string clientid)
        {
            string sqlText = @"insert into Activity(ActivityID,Name,ActivityCode,Poster,BeginTime,EndTime,Address,Status,OwnerID,MemberID,Remark,CreateUserID,AgentID,ClientID)
                                values(@ActivityID,@Name,@ActivityCode,@Poster,@BeginTime,@EndTime,@Address,1,@OwnerID,@MemberID,@Remark,@CreateUserID,@AgentID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityID",activityid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@ActivityCode",GetActivityCode()),
                                     new SqlParameter("@Poster" , poster),
                                     new SqlParameter("@BeginTime" , begintime),
                                     new SqlParameter("@EndTime" , endtime),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@OwnerID" , ownerid),
                                     new SqlParameter("@MemberID" , memberid),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }


        public string CreateActivityReply(string activityID, string msg, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            string replyID = Guid.NewGuid().ToString();

            string sqlText = @"insert into ActivityReply(ReplyID,ActivityID,Msg,CreateUserID,AgentID,FromReplyID,FromReplyUserID,FromReplyAgentID)
                                values(@ReplyID,@ActivityID,@Msg,@CreateUserID,@AgentID,@FromReplyID,@FromReplyUserID,@FromReplyAgentID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityID",activityID),
                                     new SqlParameter("@ReplyID",replyID),
                                     new SqlParameter("@Msg",msg),
                                     new SqlParameter("@FromReplyID",fromReplyID),
                                     new SqlParameter("@CreateUserID" , userID),
                                     new SqlParameter("@AgentID" , agentID),
                                     new SqlParameter("@FromReplyUserID" , fromReplyUserID),
                                     new SqlParameter("@FromReplyAgentID" , fromReplyAgentID),
                                   };

            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0?replyID:string.Empty;
        }
        #endregion

        #region 编辑/删除

        public bool UpdateActivity(string activityid, string name, string poster, string begintime, string endtime, string address, string remark, string ownerid, string memberid)
        {
            string sqlText = @"update Activity set Name=@Name,Poster=@Poster,BeginTime=@BeginTime,EndTime=@EndTime,Address=@Address,Remark=@Remark,OwnerID=@OwnerID,MemberID=@MemberID
                               where ActivityID=@ActivityID ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityID",activityid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@Poster" , poster),
                                     new SqlParameter("@BeginTime" , begintime),
                                     new SqlParameter("@EndTime" , endtime),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@OwnerID" , ownerid),
                                     new SqlParameter("@MemberID" , memberid)
                                   };

            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteActivity(string activityid)
        {
            string sqlText = @"update Activity set status=9
                               where ActivityID=@ActivityID ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ActivityID",activityid),
                                   };

            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteActivityReply(string replyID)
        {
            string sqlText = @"update ActivityReply set status=9
                               where ActivityID=@ActivityID ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ReplyID",replyID),
                                   };

            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }
        #endregion
    }
}
