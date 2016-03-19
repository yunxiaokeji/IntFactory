﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using IntFactoryEntity;
using IntFactoryEnum;
using IntFactoryDAL;
using System.Web;
namespace IntFactoryBusiness
{
    public class AliOrderBusiness
    {
        public static AliOrderBusiness BaseBusiness = new AliOrderBusiness();

        /// <summary>
        /// 执行下载阿里订单计划
        /// </summary>
        /// <returns></returns>
        public static bool ExecuteDownAliOrdersPlan() {
            int successCount, total;
            //获取阿里订单下载计划列表
            var list = AliOrderBusiness.BaseBusiness.GetAliOrderDownloadPlans();
            
            foreach (var item in list) {
                //下载阿里打样订单
                var gmtFentEnd = DateTime.Now;
                bool flag= DownFentOrders(item.FentSuccessEndTime, gmtFentEnd, item.Token, item.RefreshToken,
                    item.UserID, item.AgentID, item.ClientID, out successCount, out total);

                //新增阿里打样订单下载日志
                AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog(EnumOrderType.ProofOrder, flag, AlibabaSdk.AliOrderDownType.Auto, item.FentSuccessEndTime, gmtFentEnd,
                    successCount, total, item.AgentID, item.ClientID);

                
                //下载阿里大货订单列表
                var gmtBulkEnd = DateTime.Now;
                flag = DownBulkOrders(item.BulkSuccessEndTime, gmtBulkEnd, item.Token, item.RefreshToken,
                    item.UserID, item.AgentID, item.ClientID, out successCount, out total);

                //新增阿里大货订单下载日志
                AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog(EnumOrderType.LargeOrder, flag, AlibabaSdk.AliOrderDownType.Auto, item.BulkSuccessEndTime, gmtBulkEnd,
                    successCount, total, item.AgentID, item.ClientID);

            }

            return true;
        }

        /// <summary>
        /// 下载阿里打样订单
        /// </summary>
        public static bool DownFentOrders(DateTime gmtFentStart, DateTime gmtFentEnd, string token, string refreshToken, string userID, string agentID, string clientID, out int successCount, out int total)
        {
            successCount = 0;
            total = 0;
            //获取打样订单编码
            AlibabaSdk.GoodsCodesResult goodsCodesResult = AlibabaSdk.OrderBusiness.PullFentGoodsCodes(gmtFentStart, gmtFentEnd, token);

            #region 获取订单编码失败
            if (goodsCodesResult.error_code > 0)
            {
                //token失效
                if (goodsCodesResult.error_code == 401)
                {
                    //通过refreshToken获取用户token
                    var tokenResult = AlibabaSdk.OauthBusiness.GetTokenByRefreshToken(refreshToken);

                    if (!string.IsNullOrEmpty(tokenResult.access_token))
                    {
                        token = tokenResult.access_token;
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanToken(clientID, tokenResult.access_token, refreshToken);

                        goodsCodesResult = AlibabaSdk.OrderBusiness.PullFentGoodsCodes(gmtFentStart, gmtFentEnd, token);
                    }
                    else
                    {
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanStatus(clientID, AlibabaSdk.AliOrderPlanStatus.RefreshTokenError);
                        return false;
                    }

                }
                else
                {
                    AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanStatus(clientID, AlibabaSdk.AliOrderPlanStatus.Exception);
                    return false;
                }
            }
            #endregion

            if (goodsCodesResult.goodsCodeList.Count == 0) return true;

            //订单编码分页
            var goodsCodeList = goodsCodesResult.goodsCodeList;
            var totalCount = goodsCodeList.Count;
            total = totalCount;
            int numb = 10;
            int size = (int)Math.Ceiling((decimal)totalCount / numb);

            //分页获取订单列表
            for (int i = 1; i <= size; i++)
            {
                var qList = goodsCodeList.Skip((i - 1) * numb).Take(numb).ToList();
                AlibabaSdk.OrderListResult orderListResult = AlibabaSdk.OrderBusiness.PullFentDataList(qList, token);
                int len = orderListResult.fentOrderList.Count;

                for (var j = 0; j < len; j++)
                {
                    var order = orderListResult.fentOrderList[j];

                    string orderID = OrdersBusiness.BaseBusiness.CreateOrder(string.Empty, order.productCode, order.title,
                       HttpUtility.UrlDecode(order.buyerName), order.buyerMobile, EnumOrderSourceType.AliOrder, EnumOrderType.ProofOrder, string.Empty, string.Empty,
                        order.fentPrice.ToString(), order.bulkCount, order.samplePicList == null ? string.Empty : string.Join(",", order.samplePicList.ToArray()),
                        string.Empty, string.Empty, string.Empty, string.Empty,
                        userID, agentID, clientID, order.fentGoodsCode);

                    //新增订单失败
                    if (string.IsNullOrEmpty(orderID)) return false;
                    else
                    {
                        successCount++;
                        //更新订单下载成功时间
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanSuccessTime(clientID, EnumOrderType.ProofOrder, order.gmtCreate);
                    }
                }

            }

            return true;
        }

        /// <summary>
        /// 下载阿里大货订单
        /// </summary>
        public static bool DownBulkOrders(DateTime gmtBulkStart, DateTime gmtBulkEnd, string token, string refreshToken, string userID, string agentID, string clientID, out int successCount, out int total)
        {
            successCount = 0;
            total = 0;
            //获取大货订单编码
            AlibabaSdk.GoodsCodesResult goodsCodesResult = AlibabaSdk.OrderBusiness.PullBulkGoodsCodes(gmtBulkStart, gmtBulkEnd, token);

            #region 获取订单编码失败
            if (goodsCodesResult.error_code > 0) {
                //token失效
                if (goodsCodesResult.error_code == 401)
                {
                    //通过refreshToken获取用户token
                    var tokenResult = AlibabaSdk.OauthBusiness.GetTokenByRefreshToken(refreshToken);

                    if (!string.IsNullOrEmpty(tokenResult.access_token))
                    {
                        token = tokenResult.access_token;
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanToken(clientID, tokenResult.access_token, refreshToken);

                        goodsCodesResult = AlibabaSdk.OrderBusiness.PullBulkGoodsCodes(gmtBulkStart, gmtBulkEnd, token);
                    }
                    else
                    {
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanStatus(clientID, AlibabaSdk.AliOrderPlanStatus.RefreshTokenError);
                        return false;
                    }

                }
                else
                {
                    AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanStatus(clientID, AlibabaSdk.AliOrderPlanStatus.Exception);
                    return false; 
                }
            }
            #endregion

            if (goodsCodesResult.goodsCodeList.Count == 0) return true;

            //订单编码分页
            var goodsCodeList = goodsCodesResult.goodsCodeList;
            var totalCount = goodsCodeList.Count;
            total = totalCount;
            int numb = 10;
            int size = (int)Math.Ceiling((decimal)totalCount / numb);

            //分页获取订单列表
            for (int i = 1; i <= size; i++)
            {
                var qList = goodsCodeList.Skip((i - 1) * numb).Take(numb).ToList();
                AlibabaSdk.OrderListResult orderListResult = AlibabaSdk.OrderBusiness.PullBulkDataList(qList, token);
                int len = orderListResult.bulkOrderList.Count;

                for (var j = 0; j < len; j++)
                {
                    var order = orderListResult.bulkOrderList[j];

                    string orderID = OrdersBusiness.BaseBusiness.CreateOrder(string.Empty, order.productCode, order.title,
                        HttpUtility.UrlDecode(order.buyerName), order.buyerMobile, EnumOrderSourceType.AliOrder, EnumOrderType.LargeOrder, string.Empty, string.Empty,
                        order.bulkPrice.ToString(), order.bulkCount, order.samplePicList == null ? string.Empty : string.Join(",", order.samplePicList.ToArray()),
                        string.Empty, string.Empty, string.Empty, string.Empty,
                        userID, agentID, clientID, order.bulkGoodsCode);

                    //新增订单失败
                    if (string.IsNullOrEmpty(orderID)) return false;
                    else
                    {
                        successCount++;
                        //更新订单下载成功时间
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanSuccessTime(clientID, EnumOrderType.LargeOrder, order.gmtCreate);
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// 批量更新打样订单
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static bool UpdateAliFentOrders(string clientID, string token, string refreshToken)
        {
            #region 获取需要更新的阿里打样订单列表
            List<AliOrderUpdateLog> logs = AliOrderBusiness.BaseBusiness.GetAliOrderUpdateLogs(EnumOrderType.ProofOrder, clientID);
            List<AlibabaSdk.MutableOrder> list = new List<AlibabaSdk.MutableOrder>();
            foreach (var log in logs) 
            {
                AlibabaSdk.MutableOrder item = new AlibabaSdk.MutableOrder();
                item.fentGoodsCode = log.AliOrderCode;
                item.status = AlibabaSdk.HttpRequest.GetEnumDesc<AlibabaSdk.EnumOrderStatus>( (AlibabaSdk.EnumOrderStatus)log.OrderStatus );
                item.statusDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                item.statusDesc = string.Empty;

                list.Add(item);
            }
            #endregion

            //分页更新
            var totalCount = list.Count;
            int numb = 10;
            int size = (int)Math.Ceiling((decimal)totalCount / numb);

            for (int i = 1; i <= size; i++)
            {
                var qList = list.Skip((i - 1) * numb).Take(numb).ToList();

                var batchUpdateResult = AlibabaSdk.OrderBusiness.BatchUpdateFentList(qList, token);
                #region 批量更新打样订单失败
                if (batchUpdateResult.error_code > 0)
                {
                    //token失效
                    if (batchUpdateResult.error_code == 401)
                    {
                        //通过refreshToken获取用户token
                        var tokenResult = AlibabaSdk.OauthBusiness.GetTokenByRefreshToken(refreshToken);
                        if (!string.IsNullOrEmpty(tokenResult.access_token))
                        {
                            token = tokenResult.access_token;
                            AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanToken(clientID, tokenResult.access_token, refreshToken);

                            batchUpdateResult = AlibabaSdk.OrderBusiness.BatchUpdateFentList(qList, token);
                        }
                        else
                        {
                            AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanStatus(clientID, AlibabaSdk.AliOrderPlanStatus.RefreshTokenError);
                            return false;
                        }
                    }
                    else
                    {
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanStatus(clientID, AlibabaSdk.AliOrderPlanStatus.Exception);
                        return false; 
                    }
                }
                #endregion

                //更新订单更新日志的更新状态成功
                List<string> succeseGodesCodeList = batchUpdateResult.succeseGodesCodeList;
                AliOrderBusiness.BaseBusiness.UpdateAllAliOrderUpdateLogStatus(string.Join(",", succeseGodesCodeList), 1);

                //更新订单更新日志的更新状态失败
                if (succeseGodesCodeList.Count < list.Count)
                {
                    List<string> failGodesCodeList = new List<string>();
                    foreach (var item in list)
                    {
                        if (!succeseGodesCodeList.Contains(item.bulkGoodsCode))
                        {
                            failGodesCodeList.Add(item.bulkGoodsCode);
                        }

                    }
                    AliOrderBusiness.BaseBusiness.UpdateAllAliOrderUpdateLogStatus(string.Join(",", failGodesCodeList), 0);
                }

            }

            
            return true;
        }

        /// <summary>
        /// 批量更新大货订单
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static bool UpdateAliBulkOrders(string clientID, string token, string refreshToken)
        {
            #region 获取需要更新的阿里大货订单列表
            List<AliOrderUpdateLog> logs = AliOrderBusiness.BaseBusiness.GetAliOrderUpdateLogs(EnumOrderType.LargeOrder, clientID);
            List<AlibabaSdk.MutableOrder> list = new List<AlibabaSdk.MutableOrder>();
            foreach (var log in logs)
            {
                AlibabaSdk.MutableOrder item = new AlibabaSdk.MutableOrder();
                item.bulkGoodsCode = log.AliOrderCode;
                item.status = AlibabaSdk.HttpRequest.GetEnumDesc<AlibabaSdk.EnumOrderStatus>((AlibabaSdk.EnumOrderStatus)log.OrderStatus);
                item.statusDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                item.statusDesc = string.Empty;

                list.Add(item);
            }
            #endregion

            //分页更新
            var totalCount = list.Count;
            int numb = 10;
            int size = (int)Math.Ceiling((decimal)totalCount / numb);

            for (int i = 1; i <= size; i++)
            {
                var qList = list.Skip((i - 1) * numb).Take(numb).ToList();
                var batchUpdateResult = AlibabaSdk.OrderBusiness.BatchUpdateBulkList(qList, token);
                #region 批量更新打样订单失败
                if (batchUpdateResult.error_code > 0)
                {
                    //token失效
                    if (batchUpdateResult.error_code == 401)
                    {
                        //通过refreshToken获取用户token
                        var tokenResult = AlibabaSdk.OauthBusiness.GetTokenByRefreshToken(refreshToken);
                        if (!string.IsNullOrEmpty(tokenResult.access_token))
                        {
                            token = tokenResult.access_token;
                            AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanToken(clientID, tokenResult.access_token, refreshToken);

                            batchUpdateResult = AlibabaSdk.OrderBusiness.BatchUpdateBulkList(qList, token);
                        }
                        else
                        {
                            AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanStatus(clientID, AlibabaSdk.AliOrderPlanStatus.RefreshTokenError);
                            return false;
                        }

                    }
                    else
                    {
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanStatus(clientID, AlibabaSdk.AliOrderPlanStatus.Exception);
                        return false;
                    }
                }
                #endregion

                //更新订单更新日志的更新状态成功
                List<string> succeseGodesCodeList = batchUpdateResult.succeseGodesCodeList;
                AliOrderBusiness.BaseBusiness.UpdateAllAliOrderUpdateLogStatus(string.Join(",", succeseGodesCodeList), 1);

                //更新订单更新日志的更新状态失败
                if (succeseGodesCodeList.Count < list.Count)
                {
                    List<string> failGodesCodeList = new List<string>();
                    foreach (var item in list)
                    {
                        if (!succeseGodesCodeList.Contains(item.bulkGoodsCode))
                        {
                            failGodesCodeList.Add(item.bulkGoodsCode);
                        }

                    }
                    AliOrderBusiness.BaseBusiness.UpdateAllAliOrderUpdateLogStatus(string.Join(",", failGodesCodeList), 0);
                }


            }

            return true;
        }


        #region 阿里订单下载计划
        /// <summary>
        /// 获取阿里订单下载计划列表
        /// </summary>
        /// <param name="status">计划状态 1：正常；2：token失效；3；系统异常</param>
        /// <returns></returns>
        public List<AliOrderDownloadPlan> GetAliOrderDownloadPlans()
        {
            List<AliOrderDownloadPlan> list = new List<AliOrderDownloadPlan>();
            DataTable dt = AliOrderDAL.BaseProvider.GetAliOrderDownloadPlans();

            foreach (DataRow dr in dt.Rows)
            {
                AliOrderDownloadPlan item = new AliOrderDownloadPlan();
                item.FillData(dr);

                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 获取阿里订单下载计划详情
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public AliOrderDownloadPlan GetAliOrderDownloadPlanDetail(string clientID)
        {
            DataTable dt = AliOrderDAL.BaseProvider.GetAliOrderDownloadPlanDetail(clientID);
            if (dt.Rows.Count == 1)
            {
                AliOrderDownloadPlan item = new AliOrderDownloadPlan();
                item.FillData(dt.Rows[0]);

                return item;
            }
            else
                return null;

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
            bool flag = AliOrderDAL.BaseProvider.UpdateAliOrderDownloadPlanToken(clientID, token, refreshToken);

            return flag;
        }

        public bool UpdateAliOrderDownloadPlanStatus(string clientID,AlibabaSdk.AliOrderPlanStatus status)
        {
            bool flag = AliOrderDAL.BaseProvider.UpdateAliOrderDownloadPlanStatus(clientID, (int)status);

            return flag;
        }

        public bool UpdateAliOrderDownloadPlanSuccessTime(string agentID, EnumOrderType orderType, DateTime successTime)
        {
            bool flag = AliOrderDAL.BaseProvider.UpdateAliOrderDownloadPlanSuccessTime(agentID, (int)orderType, successTime);

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
        public List<AliOrderUpdateLog> GetAliOrderDownloadLogs(EnumOrderType orderType, int isSuccess, int downType, DateTime startTime, DateTime endTime, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentID, string clientID)
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
        public bool AddAliOrderDownloadLog(EnumOrderType orderType, bool isSuccess,AlibabaSdk.AliOrderDownType downType, DateTime startTime, DateTime endTime, int successCount, int totalCount, string agentID, string clientID)
        {
            bool flag = AliOrderDAL.BaseProvider.AddAliOrderUpdateLog((int)orderType, isSuccess?1:0,(int)downType,
                startTime, endTime, successCount, totalCount,
                agentID, clientID);

            return flag;
        }
        #endregion

        #region 阿里订单更新日志
        /// <summary>
        /// 获取订单更新日志列表
        /// </summary>
        /// <param name="orderType">订单类型</param>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public List<AliOrderUpdateLog> GetAliOrderUpdateLogs(EnumOrderType orderType, string clientID)
        {
            List<AliOrderUpdateLog> list = new List<AliOrderUpdateLog>();
            DataTable dt = AliOrderDAL.BaseProvider.GetAliOrderUpdateLogs((int)orderType, clientID);

            foreach (DataRow dr in dt.Rows)
            {
                AliOrderUpdateLog item = new AliOrderUpdateLog();
                item.FillData(dr);

                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 更新订单更新日志的更新状态
        /// </summary>
        /// <param name="aliOrderCodes"></param>
        /// <param name="status">0:未更新；1：已更新；2：更新失败</param>
        /// <returns></returns>
        public bool UpdateAllAliOrderUpdateLogStatus(string aliOrderCodes, int status)
        {
            bool flag = AliOrderDAL.BaseProvider.UpdateAllAliOrderUpdateLogStatus(aliOrderCodes, status);

            return flag;

        }
        #endregion

    }
}
