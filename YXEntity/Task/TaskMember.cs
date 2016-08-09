using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Task
{
    public class TaskMember
    {
        public int AutoID { get; set; }

        [Property("Lower")]
        public string MemberID { get; set; }

        public CacheUserEntity Member{ get; set; }

        [Property("Lower")]
        public string TaskID { get; set; }

        public int Status { get; set; }

        public int PermissionType { get; set; }

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
