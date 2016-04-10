using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IntFactory.Sdk
{
    public class TaskEntity
    {
        /// <summary>
        /// 打样单编码
        /// </summary>
        public string taskID { get; set; }

        /// <summary>
        /// 大货订单编码
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 款式编码
        /// </summary>
        public int mark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public int colorMark { get; set; }


        /// <summary>
        /// 产品标题
        /// </summary>
        public int finishStatus { get; set; }

        /// <summary>
        /// 状态编码  Yang｜check｜final
        /// </summary>
        public int orderType { get; set; }

        /// <summary>
        /// 工厂打样的价格(单元分)
        /// </summary>
        public string orderImg { get; set; }

        /// <summary>
        /// 工厂最终报的核价价格(单元分)
        /// </summary>
        public DateTime acceptTime { get; set; }

        /// <summary>
        /// 预计大货数量
        /// </summary>
        public DateTime endTime { get; set; }

        /// <summary>
        /// 买方会员ID
        /// </summary>
        public DateTime completeTime { get; set; }


        /// <summary>
        /// 买方会员名称
        /// </summary>
        public DateTime createTime { get; set; }

        
    }
}
