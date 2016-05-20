using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Task
{
    public class PlateMaking
    {
        public int AutoID { get; set; }

        [Property("Lower")]
        public string PlateID { get; set; }

        [Property("Lower")]
        public string OrderID { get; set; }

        [Property("Lower")]
        public string TaskID { get; set; }

        public string Title { get; set; }

        public string Remark { get; set; }

        public string Icon { get; set; }

        public int Status { get; set; }

        [Property("Lower")]
        public string AgentID { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public DateTime CreateTime { get; set; }

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
