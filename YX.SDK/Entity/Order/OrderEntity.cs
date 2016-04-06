using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

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
        public DateTime gmtReleasedExpect { get; set; }

        /// <summary>
        /// 打样下单时间
        /// </summary>
        public DateTime gmtFent { get; set; }

        /// <summary>
        /// 工厂最终报价时间
        /// </summary>
        public DateTime gmtFinalQuote { get; set; }

        /// <summary>
        /// 买家接受最终报价时间
        /// </summary>
        public DateTime gmtFinalAccept { get; set; }

        /// <summary>
        /// 大货下单时间
        /// </summary>
        public DateTime gmtBulk { get; set; }

        /// <summary>
        /// 实际交货时间
        /// </summary>
        public DateTime gmtReleased { get; set; }

        /// <summary>
        /// 确认收到时间
        /// </summary>
        public DateTime gmtReceived { get; set; }

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
        /// 买方地址
        /// </summary>
        public string buyerAddress { get; set; }

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
