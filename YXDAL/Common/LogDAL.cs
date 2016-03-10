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
        public DataTable GetClientActions(string datetime, string clientid, ref int customercount, ref int ordercount, ref decimal totalmoney)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@CustomerCount",SqlDbType.Int),
                                       new SqlParameter("@OrderCount",SqlDbType.Int),
                                       new SqlParameter("@TotalMoney",SqlDbType.Decimal),
                                       new SqlParameter("@DateTime",datetime),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            paras[0].Value = customercount;
            paras[1].Value = ordercount;
            paras[2].Value = totalmoney;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            paras[2].Direction = ParameterDirection.InputOutput;

            DataTable dt = GetDataTable("R_GetClientActions", paras, CommandType.StoredProcedure);

            customercount = Convert.ToInt32(paras[0].Value);
            ordercount = Convert.ToInt32(paras[1].Value);
            totalmoney = Convert.ToDecimal(paras[2].Value);

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
