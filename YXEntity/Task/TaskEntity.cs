using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Task
{
    public class TaskEntity
    {
        public int AutoID { get; set; }

        [Property("Lower")]
        public string TaskID { get; set; }

        [Property("Lower")]
        public string OrderID { get; set; }

        [Property("Lower")]
        public string OrderCode { get; set; }

        public string OrderImg { get; set; }

        [Property("Lower")]
        public string ProcessID { get; set; }

        [Property("Lower")]
        public string StageID { get; set; }

        public OrderStageEntity Stage { get; set; }

        [Property("Lower")]
        public string OwnerID { get; set; }

        public Users Owner { get; set; }

        public DateTime EndTime { get; set; }

        public int Sort { get; set; }

        public int Status { get; set; }

        public int FinishStatus { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        [Property("Lower")]
        public string AgentID { get; set; }

        public string TaskName { get; set; }
        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
