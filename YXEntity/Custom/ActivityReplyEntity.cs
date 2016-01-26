using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ActivityReplyEntity
    {
        [Property("Lower")]
        public string ReplyID { get; set; }

        [Property("Lower")]
        public string ActivityID { get; set; }

        public string Msg { get; set; }

        public int Status { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        [Property("Lower")]
        public string AgentID { get; set; }

        public Users CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string FromReplyID { get; set; }

        [Property("Lower")]
        public string FromReplyUserID { get; set; }

        [Property("Lower")]
        public string FromReplyAgentID { get; set; }

        public Users FromReplyUser { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

    }
}
