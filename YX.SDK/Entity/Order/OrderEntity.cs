using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlibabaSdk
{
    public class OrderEntity
    {
        /// <summary>
        /// 打样单编码
        /// </summary>
        public string fentGoodsCode { get; set; }

        /// <summary>
        /// 大货订单编码
        /// </summary>
        public string bulkGoodsCode { get; set; }

        /// <summary>
        /// 款式编码
        /// </summary>
        public string productCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime gmtCreate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime gmtModified { get; set; }

        /// <summary>
        /// 期望交货时间
        /// </summary>
        public DateTime gmtDeliveryExpect { get; set; }

        /// <summary>
        /// 封样时间
        /// </summary>
        public DateTime gmtSample { get; set; }

        /// <summary>
        /// 实际交货时间
        /// </summary>
        public DateTime gmtDelivery { get; set; }

        /// <summary>
        /// 产品标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 状态编码  Yang｜check｜final
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 工厂打样的价格(单元分)
        /// </summary>
        public long fentPrice { get; set; }

        /// <summary>
        /// 工厂最终报的核价价格(单元分)
        /// </summary>
        public long bulkPrice { get; set; }

        /// <summary>
        /// 预计大货数量
        /// </summary>
        public int  bulkCount { get; set; }

        /// <summary>
        /// 买方会员ID
        /// </summary>
        public string buyerMemberId { get; set; }


        /// <summary>
        /// 买方会员名称
        /// </summary>
        public string buyerName { get; set; }

        /// <summary>
        /// 买方联系手机
        /// </summary>
        public string buyerMobile { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        public string catName { get; set; }

        /// <summary>
        /// 样图url列表
        /// </summary>
        public List<string> samplePicList { get; set; }
        
    }
}
