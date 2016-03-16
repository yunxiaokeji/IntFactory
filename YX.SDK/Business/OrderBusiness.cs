using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace AlibabaSdk
{
    public class OrderBusiness
    {
        #region 打样
        /// <summary>
        /// 下载阿里打样订单列表
        /// </summary>
        /// <returns></returns>
        public static List<OrderEntity> DownFentOrders(DateTime gmtFentStart, string token)
        {
            GoodsCodesResult goodsCodesResult = pullFentGoodsCodes(gmtFentStart, DateTime.Now, token);

            if (goodsCodesResult.error_code > 0)
            {

            }
            else
            {
                var List = goodsCodesResult.goodsCodeList;
                int numb = 10;
                int size = (int)Math.Ceiling((decimal)List.Count / numb);
                for (int i = 1; i <= size; i++)
                {
                    var qList = List.Skip( (i - 1) * numb ).Take(numb).ToList();

                    OrderListResult orderListResult = pullFentDataList(qList, token);
                    if (orderListResult.error_code > 0)
                    {

                    }
                    else
                    {
                        int total = orderListResult.fentOrderList.Count;

                        return orderListResult.fentOrderList;
                    }
                }

               
            }
            return new List<OrderEntity>();
        }
        /// <summary>
        /// 获取打样订单编码
        /// </summary>
        public static GoodsCodesResult pullFentGoodsCodes(DateTime gmtFentStart, DateTime gmtFentEnd, string token)
        { 
            var paras = new Dictionary<string, object>();
            paras.Add("gmtFentStart", gmtFentStart.ToString("yyyyMMddHHmmssfffzzz").Replace(":", ""));
            paras.Add("gmtFentEnd", gmtFentEnd.ToString("yyyyMMddHHmmssfffzzz").Replace(":", ""));
            paras.Add("access_token", token);

            string resultStr = HttpRequest.RequestServer(ApiOption.pullFentGoodsCodes, paras, RequestType.Get);
            return JsonConvert.DeserializeObject<GoodsCodesResult>(resultStr);
        }

        /// <summary>
        /// 根据订单编码获取打样订单列表
        /// </summary>
        public static OrderListResult pullFentDataList(List<string> goodsCodes, string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("fentGoodsCodes", string.Join(",", goodsCodes.ToArray()));
            paras.Add("access_token", token);

            string resultStr = HttpRequest.RequestServer(ApiOption.pullFentDataList, paras, RequestType.Post);
            var format = "yyyyMMddHHmmssfffzzz"; // your datetime format
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

            return JsonConvert.DeserializeObject<OrderListResult>(resultStr, dateTimeConverter);
        }

        /// <summary>
        /// 批量更新打样订单
        /// </summary>
        public static GoodsCodesResult batchUpdateFentList(List<MutableOrder> list, string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("access_token", token);
            paras.Add("fentOrderList", JsonConvert.SerializeObject(list));

            var resultStr = HttpRequest.RequestServer(ApiOption.batchUpdateFent, paras, RequestType.Post);
            return JsonConvert.DeserializeObject<GoodsCodesResult>(resultStr);
        }

        /// <summary>
        /// 更新打样订单
        /// </summary>
        public static GoodsCodesResult batchUpdateFent(string fentGoodsCode, FentOrderStatus fentOrderStatus, string statusDesc, string token)
        {
            List<AlibabaSdk.MutableOrder> list = new List<AlibabaSdk.MutableOrder>();
            AlibabaSdk.MutableOrder order = new AlibabaSdk.MutableOrder();
            order.fentGoodsCode = fentGoodsCode;
            order.status = HttpRequest.GetEnumDesc<FentOrderStatus>(fentOrderStatus);
            order.statusDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            order.statusDesc = statusDesc;
            list.Add(order);

            return batchUpdateFentList(list, token);
        }
        #endregion

        #region 大货
        /// <summary>
        /// 获取大货订单编码
        /// </summary>
        public static GoodsCodesResult pullBulkGoodsCodes(DateTime gmtBulkStart, DateTime gmtBulkEnd, string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("gmtBulkStart", gmtBulkStart.ToString("yyyyMMddHHmmssfffzzz").Replace(":", ""));
            paras.Add("gmtBulkEnd", gmtBulkEnd.ToString("yyyyMMddHHmmssfffzzz").Replace(":", ""));
            paras.Add("access_token", token);

            string resultStr= HttpRequest.RequestServer(ApiOption.pullBulkGoodsCodes, paras, RequestType.Get);
            return JsonConvert.DeserializeObject<GoodsCodesResult>(resultStr);
        }

        /// <summary>
        /// 根据订单编码获取大货列表
        /// </summary>
        public static OrderListResult pullBulkDataList(List<string> goodsCodes, string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("bulkGoodsCodes", string.Join(",", goodsCodes.ToArray()));
            paras.Add("access_token", token);

            string resultStr = HttpRequest.RequestServer(ApiOption.pullBulkDataList, paras, RequestType.Post);
            var format = "yyyyMMddHHmmssfffzzz"; // your datetime format
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

            return JsonConvert.DeserializeObject<OrderListResult>(resultStr, dateTimeConverter);
        }

        /// <summary>
        /// 批量更新大货订单
        /// </summary>
        public static GoodsCodesResult batchUpdateBulkList(List<MutableOrder> list, string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("access_token",token);
            paras.Add("bulkOrderList", JsonConvert.SerializeObject(list));

            var resultStr = HttpRequest.RequestServer(ApiOption.batchUpdateBulk, paras, RequestType.Post);
            return JsonConvert.DeserializeObject<GoodsCodesResult>(resultStr);
        }

        /// <summary>
        /// 更新大货订单
        /// </summary>
        public static GoodsCodesResult batchUpdateBulk(string bulkGoodsCode, BulkOrderStatus bulkOrderStatus, string statusDesc, string token)
        {
            List<AlibabaSdk.MutableOrder> list = new List<AlibabaSdk.MutableOrder>();
            AlibabaSdk.MutableOrder order = new AlibabaSdk.MutableOrder();
            order.bulkGoodsCode = bulkGoodsCode;
            order.status = HttpRequest.GetEnumDesc<BulkOrderStatus>(bulkOrderStatus);
            order.statusDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            order.statusDesc = statusDesc;
            list.Add(order);

            return batchUpdateBulkList(list, token);
        }

        /// <summary>
        /// 批量推送生产计划
        /// </summary>
        public static bool pushProductionPlan(List<MutableProductionPlan> list, string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("access_token", token);
            paras.Add("planList", JsonConvert.SerializeObject(list));

            var result = HttpRequest.RequestServer(ApiOption.pushProductionPlan, paras, RequestType.Post);

            return false;
        }
        #endregion
    }
}
