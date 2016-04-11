using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IntFactory.Sdk
{
    public class ProcessEntity
    {
        /// <summary>
        /// 打样单编码
        /// </summary>
        public string processID { get; set; }

        /// <summary>
        /// 大货订单编码
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 款式编码
        /// </summary>
        public string processName { get; set; }

        
        
    }
}
