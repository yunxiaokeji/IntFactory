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
        /// 获取打样订单编码
        /// </summary>
        public static GoodsCodesResult pullFentGoodsCodes(DateTime gmtFentStart, DateTime gmtFentEnd, string token)
        { 
            var paras = new Dictionary<string, object>();
            paras.Add("gmtFentStart", gmtFentEnd.ToString("yyyyMMddHHmmssfffzzz").Replace(":", ""));
            paras.Add("gmtFentEnd", gmtFentEnd.ToString("yyyyMMddHHmmssfffzzz").Replace(":", ""));
            paras.Add("access_token", token);

            string resultStr = HttpRequest.RequestServer(ApiOption.pullFentGoodsCodes, paras, RequestType.Get);
            return JsonConvert.DeserializeObject<GoodsCodesResult>(resultStr);
        }

        /// <summary>
        /// 根据订单编码获取打样订单列表
        /// </summary>
        public static List<OrderEntity> pullFentDataList(List<string> goodsCodes, string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("bulkGoodsCodes", string.Join(",", goodsCodes.ToArray()));
            paras.Add("access_token", token);

            string resultStr = HttpRequest.RequestServer(ApiOption.pullFentDataList, paras, RequestType.Post);
            var format = "yyyyMMddHHmmssfffzzz"; // your datetime format
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
            OrderListResult result = JsonConvert.DeserializeObject<OrderListResult>(resultStr, dateTimeConverter);

            return result.fentOrderList;
        }

        /// <summary>
        /// 更新打样订单
        /// </summary>
        public static GoodsCodesResult batchUpdateFent(List<MutableOrder> list, string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("access_token", token);
            paras.Add("fentOrderList", JsonConvert.SerializeObject(list));

            var resultStr = HttpRequest.RequestServer(ApiOption.batchUpdateBulk, paras, RequestType.Post);
            return JsonConvert.DeserializeObject<GoodsCodesResult>(resultStr);
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
        public static List<OrderEntity> pullBulkDataList(List<string> goodsCodes,string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("bulkGoodsCodes", string.Join(",", goodsCodes.ToArray()));
            paras.Add("access_token", token);

            string resultStr = HttpRequest.RequestServer(ApiOption.pullBulkDataList, paras, RequestType.Post);
            var format = "yyyyMMddHHmmssfffzzz"; // your datetime format
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
            OrderListResult result = JsonConvert.DeserializeObject<OrderListResult>(resultStr, dateTimeConverter);

            return result.bulkOrderList;
        }

        /// <summary>
        /// 更新大货订单
        /// </summary>
        public static GoodsCodesResult batchUpdateBulk(List<MutableOrder> list, string token)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("access_token",token);
            paras.Add("bulkOrderList", JsonConvert.SerializeObject(list));

            var resultStr = HttpRequest.RequestServer(ApiOption.batchUpdateBulk, paras, RequestType.Post);
            return JsonConvert.DeserializeObject<GoodsCodesResult>(resultStr);
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
