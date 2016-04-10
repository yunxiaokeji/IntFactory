using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IntFactory.Sdk
{
    public class TaskLogEntity
    {
        /// <summary>
        /// 打样单编码
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 买方会员名称
        /// </summary>
        public DateTime createTime { get; set; }

        public UserBase createUser { get; set; }

    }
}
