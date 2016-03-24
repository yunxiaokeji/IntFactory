using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
namespace IntFactoryDAL
{
    public class AliOrderDAL:BaseDAL
    {
        public static AliOrderDAL BaseProvider = new AliOrderDAL();

        #region 阿里订单更新日志
        /// <summary>
        /// 获取订单更新日志列表
        /// </summary>
        /// <param name="orderType">订单类型</param>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public DataTable GetAliOrderUpdateLogs(int orderType,string clientID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderType",orderType),
                                       new SqlParameter("@ClientID",clientID)
                                   };

            DataTable dt = GetDataTable("select * from AliOrderUpdateLog where OrderType=@OrderType and Status<>1 and ClientID=@ClientID ", paras, CommandType.Text);
            return dt;
        }

        /// <summary>
        /// 更新订单更新日志的更新状态
        /// </summary>
        /// <param name="logIDs"></param>
        /// <param name="status">0:未更新；1：已更新；2：更新失败</param>
        /// <returns></returns>
        public bool UpdateAllAliOrderUpdateLogStatus(string aliOrderCodes, int status)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@AliOrderCodes",aliOrderCodes),
                                       new SqlParameter("@Status",status)
                                   };
            return ExecuteNonQuery("P_UpdateAllAliOrderUpdateLogStatus", paras, CommandType.StoredProcedure) > 0;


        }
        #endregion

        #region 阿里订单下载日志
        /// <summary>
        /// 获取阿里订单下载日志列表
        /// </summary>
        /// <param name="orderType"></param>
        /// <param name="isSuccess">下载结果 1：成功；0：失败</param>
        /// <param name="downType">下载类型 1：自动；2手动</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public DataTable GetAliOrderUpdateLogs(int orderType, int isSuccess, int downType, DateTime startTime, DateTime endTime, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@OrderType",orderType),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataTable dt = GetDataTable("P_GetOrders", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);

            return dt;
        }

        /// <summary>
        /// 新增阿里订单下载日志
        /// </summary>
        /// <param name="orderType"></param>
        /// <param name="isSuccess">下载结果 1：成功；0：失败</param>
        /// <param name="downType">下载类型 1：自动；2手动</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="totalCount"></param>
        /// <param name="successCount"></param>
        /// <param name="agentID"></param>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public bool AddAliOrderUpdateLog(int orderType, int isSuccess, int downType, DateTime startTime, DateTime endTime, int successCount, int totalCount,string remark, string agentID, string clientID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@OrderType",orderType),
                                       new SqlParameter("@IsSuccess",isSuccess),
                                       new SqlParameter("@DownType",downType),
                                       new SqlParameter("@StartTime",startTime),
                                       new SqlParameter("@EndTime",endTime),
                                       new SqlParameter("@SuccessCount",successCount),
                                       new SqlParameter("@TotalCount",totalCount),
                                       new SqlParameter("@Remark",remark),
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@ClientID",clientID)
                                   };

            string sqlStr = "insert into AliOrderDownloadLog(OrderType,IsSuccess,DownType,StartTime,EndTime,SuccessCount,TotalCount,Remark,AgentID,ClientID) values(@OrderType,@IsSuccess,@DownType,@StartTime,@EndTime,@SuccessCount,@TotalCount,@Remark,@AgentID,@ClientID)";
            
            return ExecuteNonQuery(sqlStr, paras, CommandType.Text) > 0;
        }
        #endregion

        #region 阿里订单下载计划
        /// <summary>
        /// 获取阿里订单下载计划列表
        /// </summary>
        /// <param name="status">计划状态 1：正常；2：token失效；3；系统异常</param>
        /// <returns></returns>
        public DataTable GetAliOrderDownloadPlans()
        {
            SqlParameter[] paras = { 
                                   };

            DataTable dt = GetDataTable("Select * from AliOrderDownloadPlan where Status<>2 ", paras, CommandType.Text);
            return dt;
        }

        /// <summary>
        /// 获取阿里订单下载计划详情
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public DataTable GetAliOrderDownloadPlanDetail(string clientID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientID)
                                   };

            DataTable dt = GetDataTable("Select * from AliOrderDownloadPlan where ClientID=@ClientID ", paras, CommandType.Text);
            return dt;
        }

        public bool AddAliOrderDownloadPlan(string userID, string memberID, string token, string refreshToken, string agentID, string clientID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userID),
                                       new SqlParameter("@MemberID",memberID),
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@RefreshToken",refreshToken),
                                       new SqlParameter("@Token",token)
                                   };


            return ExecuteNonQuery("P_AddAliOrderDownloadPlan", paras, CommandType.StoredProcedure) > 0;
        }

        /// <summary>
        /// 更新阿里订单下载计划中的token
        /// </summary>
        /// <param name="planID"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public bool UpdateAliOrderDownloadPlanToken(string clientID, string token, string refreshToken)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@RefreshToken",refreshToken),
                                       new SqlParameter("@Token",token)
                                   };


            return ExecuteNonQuery(" update AliOrderDownloadPlan set Token=@Token,RefreshToken=@RefreshToken where ClientID=@ClientID", paras, CommandType.Text) > 0;
        }

        public bool UpdateAliOrderDownloadPlanStatus(string clientID, int status, string remark)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@Remark",remark)
                                   };


            return ExecuteNonQuery(" update AliOrderDownloadPlan set Status=@Status,@Remark=Remark,UpdateTime=getdate() where ClientID=@ClientID", paras, CommandType.Text) > 0;
        }

        public bool UpdateAliOrderDownloadPlanSuccessTime(string clientID, int orderType, DateTime successTime)
        {
            string sqlStr = "";
            if (orderType == 1)
            {
                sqlStr = " update AliOrderDownloadPlan set FentSuccessEndTime='" + successTime + "' where clientID='" + clientID + "'";
            }
            else
            {
                sqlStr = " update AliOrderDownloadPlan set BulkSuccessEndTime='" + successTime + "' where clientID='" + clientID + "'";
            }

            return ExecuteNonQuery(sqlStr) > 0;
        }

        public bool DeleteAliOrderDownloadPlan(string clientID)
        {
            string sqlStr = " delete from AliOrderDownloadPlan ClientID=@ClientID";
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientID)
                                   };


            return ExecuteNonQuery(sqlStr,paras,CommandType.Text) > 0;
        }
        #endregion
    }
}
