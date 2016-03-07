using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlibabaSdk
{
    public class OrderBusiness
    {
        static string token = "e67ddef5-e0ee-4094-bdde-d623354e8d1d";
        public static List<string> pullFentGoodsCodes() { 
            List<string> list=new List<string>();
            var paras = new Dictionary<string, object>();

            string aaa = HttpRequest.RequestServer(ApiOption.pullBulkGoodsCodes, paras, token, RequestType.Post);
            return list;
        }

        public static List<OrderEntity> pullFentDataList()
        {
            List<OrderEntity> list = new List<OrderEntity>();

            return list;
        }

        public static bool batchUpdateFent(List<MutableOrder> list)
        {
            bool flag = false;

            return flag;
        }

        public static List<string> pullBulkGoodsCodes()
        {
            List<string> list = new List<string>();

            return list;
        }

        public static List<OrderEntity> pullBulkDataList()
        {
            List<OrderEntity> list = new List<OrderEntity>();

            return list;
        }

        public static bool batchUpdateBulk(List<MutableOrder> list)
        {
            bool flag = false;

            return flag;
        }

        public static bool pushProductionPlan(List<MutableOrder> list)
        {
            bool flag = false;

            return flag;
        }
    }
}
