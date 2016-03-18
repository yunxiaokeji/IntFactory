using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using IntFactoryEntity;
using IntFactoryEnum;
using IntFactoryDAL;
namespace IntFactoryBusiness
{
    public class AliOrderBusiness
    {
        public static AliOrderBusiness BaseBusiness = new AliOrderBusiness();


        #region 阿里订单更新日志
        /// <summary>
        /// 获取订单更新日志列表
        /// </summary>
        /// <param name="orderType">订单类型</param>
        /// <param name="status">0:未更新；1：已更新；2：更新失败</param>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public List<AliOrderUpdateLog> GetAliOrderUpdateLogs(EnumOrderType orderType, int status, string agentID)
        {
            List<AliOrderUpdateLog> list = new List<AliOrderUpdateLog>();
            DataTable dt = AliOrderDAL.BaseProvider.GetAliOrderUpdateLogs((int)orderType, status, agentID);

            return list;
        }

        /// <summary>
        /// 更新订单更新日志的更新状态
        /// </summary>
        /// <param name="logIDs"></param>
        /// <param name="status">0:未更新；1：已更新；2：更新失败</param>
        /// <returns></returns>
        public bool UpdateAllAliOrderUpdateLogStatus(int logIDs, int status)
        {
            bool flag = AliOrderDAL.BaseProvider.UpdateAllAliOrderUpdateLogStatus(logIDs,status);

            return flag;

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
        public List<AliOrderUpdateLog> GetAliOrderUpdateLogs(EnumOrderType orderType, int isSuccess, int downType, DateTime startTime, DateTime endTime, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentID, string clientID)
        {
            List<AliOrderUpdateLog> list = new List<AliOrderUpdateLog>();
            DataTable dt = AliOrderDAL.BaseProvider.GetAliOrderUpdateLogs((int)orderType, isSuccess, downType,
                startTime, endTime, pageSize,
                pageIndex, ref totalCount, ref pageCount, agentID, clientID);

            return list;
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
        public bool AddAliOrderUpdateLog(EnumOrderType orderType, int isSuccess, int downType, DateTime startTime, DateTime endTime, int totalCount, int successCount, int agentID, int clientID)
        {
            bool flag = AliOrderDAL.BaseProvider.AddAliOrderUpdateLog((int)orderType, isSuccess, downType,
                startTime, endTime, totalCount, successCount,
                agentID, clientID);

            return flag;
        }
        #endregion

        #region 阿里订单下载计划
        /// <summary>
        /// 获取阿里订单下载计划列表
        /// </summary>
        /// <param name="status">计划状态 1：正常；2：token失效；3；系统异常</param>
        /// <returns></returns>
        public List<AliOrderDownloadPlan> GetAliOrderDownloadPlans(int status) {
            List<AliOrderDownloadPlan> list = new List<AliOrderDownloadPlan>();
            DataTable dt = AliOrderDAL.BaseProvider.GetAliOrderDownloadPlans(status);
            return list;
        }

        /// <summary>
        /// 获取阿里订单下载计划详情
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public AliOrderDownloadPlan GetAliOrderDownloadPlanDetail(string agentID)
        {
            AliOrderDownloadPlan item = new AliOrderDownloadPlan();

            DataTable dt = AliOrderDAL.BaseProvider.GetAliOrderDownloadPlanDetail(agentID);

            return item;
        }

        /// <summary>
        /// 更新阿里订单下载计划中的token
        /// </summary>
        /// <param name="planID"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public bool UpdateAliOrderDownloadPlanToken(string planID, string token, string refreshToken)
        {
            bool flag =AliOrderDAL.BaseProvider.UpdateAliOrderDownloadPlanToken(planID,token,refreshToken) ;

            return flag;
        }
        #endregion

    }
}
