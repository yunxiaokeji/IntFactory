using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlibabaSdk
{
    public class MutableProductionPlan
    {
        /// <summary>
        /// 当前日期
        /// </summary>
        public DateTime planDate { get; set; }

        /// <summary>
        /// 工厂产能每天最大产量（件）
        /// </summary>
        public int productionCapacity { get; set; }

        /// <summary>
        /// 当天产能占用数量
        /// </summary>
        public int productionOccupy { get; set; }

        /// <summary>
        /// 工厂产能单元
        /// </summary>
        public string productionUnit { get; set; }

        /// <summary>
        /// 任务数
        /// </summary>
        public string taskCount { get; set; }

        /// <summary>
        /// 任务清单
        /// </summary>
        public List<string> taskList { get; set; }
    }
}
