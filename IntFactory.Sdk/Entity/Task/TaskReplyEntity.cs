using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IntFactory.Sdk
{
    public class TaskReplyEntity
    {
        /// <summary>
        /// 打样单编码
        /// </summary>
        public string replyID { get; set; }

        public string orderID { get; set; }

        public string stageID { get; set; }


        /// <summary>
        /// 款式编码
        /// </summary>
        public int mark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 买方会员名称
        /// </summary>
        public DateTime createTime { get; set; }

        public UserBase createUser { get; set; }

        public UserBase fromReplyUser { get; set; }
    }
}
