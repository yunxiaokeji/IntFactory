using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class LogDAL :BaseDAL
    {
        public DataTable GetAgentActions(string datetime,string agentid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@DateTime",datetime),
                                       new SqlParameter("@AgentID",agentid)
                                   };

            DataTable dt = GetDataTable("R_GetAgentActions_Log", paras, CommandType.StoredProcedure);

            return dt;
        }

        public DataTable GetClientUpcomings(string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataTable dt = GetDataTable("R_GetClientUpcomings", paras, CommandType.StoredProcedure);

            return dt;
        }

        public static Task<bool> AddLoginLog(string loginname, int status, int systemtype, string operateip, string userid, string agentid, string clientid)
        {
            string sqlText = "insert into Log_Login(LoginName,SystemType,Status,CreateTime,OperateIP,UserID,AgentID,ClientID) "+
                            " values(@LoginName,@SystemType,@Status,GETDATE(),@OperateIP,@UserID,@AgentID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@LoginName" , loginname),
                                     new SqlParameter("@SystemType" , systemtype),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@OperateIP" , operateip),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return Task.Run(() => { return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0; });
        }

        public static Task<bool> AddOperateLog(string userid, string funcname, int type, int modules, int entity, string guid, string message, string operateip)
        {
            string sqlText = "insert into Log_Operate(UserID,FuncName,Type,Modules,Entity,GUID,Message,CreateTime,OperateIP) " +
                            " values(@UserID,@FuncName,@Type,@Modules,@Entity,@GUID,@Message,GETDATE(),@OperateIP)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@FuncName" , funcname),
                                     new SqlParameter("@Type" , type),
                                     new SqlParameter("@Modules" , modules),
                                     new SqlParameter("@Entity" , entity),
                                     new SqlParameter("@GUID" , guid),
                                     new SqlParameter("@Message" , message),
                                     new SqlParameter("@OperateIP" , operateip)
                                   };
            return Task.Run(() => { return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0; });
        }

        public static Task<bool> AddErrorLog(string userid, string message, int systemtype, string operateip)
        {
            string sqlText = "insert into Log_Error(UserID,Message,SystemType,CreateTime,OperateIP) values(@UserID,@Message,@SystemType,GETDATE(),@OperateIP)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@Message" , message),
                                     new SqlParameter("@SystemType" , systemtype),
                                     new SqlParameter("@OperateIP" , operateip)
                                   };
            return Task.Run(() => { return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0; });
        }

        public static Task<bool> AddLog(string tablename, string Logguid, string remark, string userid, string operateip, string guid, string agentid, string clientid)
        {
            string sqlText = "insert into " + tablename + "(LogGUID,Remark,CreateUserID,OperateIP,GUID,AgentID,ClientID) values(@LogGUID,@Remark,@CreateUserID,@OperateIP,@GUID,@AgentID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@LogGUID" , Logguid),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip),
                                     new SqlParameter("@GUID" , guid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return Task.Run(() => { return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0; });
        }

        public static Task<bool> AddActionLog(int systemtype, int objecttype, int actiontype, string operateip, string userid, string agentid, string clientid)
        {
            string sqlText = "insert into Log_Action(SystemType,ObjectType,ActionType,CreateTime,OperateIP,UserID,AgentID,ClientID) " +
                            " values(@SystemType,@ObjectType,@ActionType,GETDATE(),@OperateIP,@UserID,@AgentID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@SystemType" , systemtype),
                                     new SqlParameter("@ObjectType" , objecttype),
                                     new SqlParameter("@ActionType" , actiontype),
                                     new SqlParameter("@OperateIP" , operateip),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return Task.Run(() => { return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0; });
        }

    }
}
