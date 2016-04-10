using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IntFactory.Sdk
{
    public class ProcessStageEntity
    {
        public string stageID { get; set; }

        /// <summary>
        /// 打样单编码
        /// </summary>
        public string processID { get; set; }

        /// <summary>
        /// 款式编码
        /// </summary>
        public string stageName { get; set; }

        
        
    }
}
