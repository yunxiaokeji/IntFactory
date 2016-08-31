using CloudSales.Sdk.Common;
using CloudSales.Sdk.EDJEntity;
using CloudSales.Sdk.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudSales.Sdk.Business
{
    public class TaskOrderBusiness
    {
        public static TaskOrderBusiness BaseBusiness = new TaskOrderBusiness();
        /// <summary>
        /// 生成二当家入库单
        /// </summary>
        /// <param name="jasonParas"> remarks he nums 结尾是英文逗号 不能去除
        /// {   "orderid":"二当家采购单ID",
        ///     "remarks":"智能工厂产品ID1:规格1,规格2,智能工厂产品ID2:规格1,规格2,",
        ///     "nums":"数量1,数量2,数量3,","userid":""
        /// }
        /// </param>
        /// <returns></returns>
        public OperateResult AddStockPartIn(string jasonParas)
        {
            var paras=JsonConvert.DeserializeObject<Dictionary<string, object>>(jasonParas);      
            return HttpRequest.RequestServer<OperateResult>(ApiOption.addStockPartIn, paras);
        }
    }
}
