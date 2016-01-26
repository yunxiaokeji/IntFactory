using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ActivityEntity
    {
        [Property("Lower")] 
        public string ActivityID { get; set; }

        public string Name { get; set; }

        public string Poster { get; set; }

        public string ActivityCode { get; set; }

        public string Remark { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Address { get; set; }
        /// <summary>
        /// 状态1正常 2结束 9删除
        /// </summary>
        public int Status { get; set; }

        [Property("Lower")] 
        public string OwnerID { get; set; }
        public Users Owner { get; set; }

        public string MemberID { get; set; }
        public List<Users> Members { get; set; }


        public int CustomerQuantity { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")] 
        public string  CreateUserID { get; set; }

        [Property("Lower")] 
        public string AgentID { get; set; }

        [Property("Lower")] 
        public string ClientID { get; set; }


        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

    }
}
