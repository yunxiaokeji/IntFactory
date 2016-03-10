using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
namespace AlibabaSdk
{
    public class OrderBusiness
    {
        static string token = "1450d672-9abe-4d14-b87d-f75d0d19e256";

        public static List<string> pullFentGoodsCodes() { 
            List<string> list=new List<string>();
            var paras = new Dictionary<string, object>();
            paras.Add("gmtFentStart",DateTime.Parse( "2016-01-01"));
            paras.Add("gmtFentEnd",DateTime.Parse( "2016-03-07"));

            string aaa = HttpRequest.RequestServer(ApiOption.pullBulkGoodsCodes, paras, RequestType.Post);
            return list;
        }

        public static List<OrderEntity> pullFentDataList()
        {
            List<OrderEntity> list = new List<OrderEntity>();

            return list;
        }

        public static bool batchUpdateFent(List<MutableOrder> list,string token)
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

        public static bool batchUpdateBulk(List<MutableOrder> list,string token)
        {
            bool flag = false;
            var paras = new Dictionary<string, object>();
            paras.Add("access-token",token);
            paras.Add("bulkOrderList", JsonConvert.SerializeObject(list));

            return flag;
        }

        public static bool pushProductionPlan(List<MutableOrder> list)
        {
            bool flag = false;

            return flag;
        }
    }
}
