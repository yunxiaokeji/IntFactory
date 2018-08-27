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

        public string TaskCode { get; set; }

        public string Title { get; set; }

        [Property("Lower")]
        public string OrderID { get; set; }

        public OrderEntity Order { get; set; }

        public int OrderType { get; set; }

        public string ProductName { get; set; }

        [Property("Lower")]
        public string OrderCode { get; set; }

        public string GoodsName { get; set; }

        public string OrderImg { get; set; }

        [Property("Lower")]
        public string ProcessID { get; set; }

        [Property("Lower")]
        public string StageID { get; set; }

        [Property("Lower")]
        public string OwnerID { get; set; }

        [Property("Lower")]
        public string OrderOwnerID { get; set; }

        public CacheUserEntity Owner { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime AcceptTime { get; set; }

        public DateTime CompleteTime { get; set; }

        public int Sort { get; set; }

        public int LockStatus { get; set; }

        public int Status { get; set; }

        public int Mark { get; set; }

        public int ColorMark { get; set; }

        public string Remark { get; set; }

        public int MaxHours { get; set; }

        public int FinishStatus { get; set; }

        public int PreFinishStatus { get; set; }

        public string PreTitle { get; set; }

        public DateTime PEndTime { get; set; }

        public DateTime PCompleteTime { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public string TaskName { get; set; }

        public string Members { get; set; }

        public List<TaskMember> TaskMembers { get; set; }

        /// <summary>
        /// 预警状态 0：正常 1：快到期 2：已超期
        /// </summary>
        public int WarningStatus { get; set; }

        public string WarningTime { get; set; }

        public int WarningDays { get; set; }

        public int UseDays { get; set; }

        public string OpenID { get; set; }

        public int IsTop { get; set; }
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
