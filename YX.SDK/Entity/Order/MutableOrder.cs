using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlibabaSdk
{
    public class MutableOrder
    {
        /// <summary>
        /// 打样订单编码
        /// </summary>
        public string fentGoodsCode { get; set; }

        /// <summary>
        /// 大货订单编码
        /// </summary>
        public string bulkGoodsCode { get; set; }

        /// <summary>
        /// 产品标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 状态编码
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 状态发生时间
        /// </summary>
        public DateTime statusDate { get; set; }

        /// <summary>
        /// 状态说明
        /// </summary>
        public string statusDesc { get; set; }

        /// <summary>
        /// 工厂最终报的核价价格(单元分)
        /// </summary>
        public long bulkPrice { get; set; }

        /// <summary>
        /// 工厂最终报的核价价格(单元分)
        /// </summary>
        public string bomList { get; set; }

    }
}
