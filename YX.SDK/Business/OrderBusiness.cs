using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlibabaSdk
{
    public class OrderBusiness
    {
        public static List<string> pullFentGoodsCodes() { 
            List<string> list=new List<string>();

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
