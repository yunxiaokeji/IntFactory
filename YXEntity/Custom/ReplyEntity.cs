using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ReplyEntity
    {
        [Property("Lower")]
        public string ReplyID { get; set; }

        [Property("Lower")]
        public string GUID { get; set; }

        public string Content { get; set; }

        [Property("Lower")]
        public string StageID { get; set; }

        public int Mark { get; set; }

        public int Status { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public Users CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string FromReplyID { get; set; }

        [Property("Lower")]
        public string FromReplyUserID { get; set; }

        [Property("Lower")]
        public string FromReplyAgentID { get; set; }

        public Users FromReplyUser { get; set; }

        public List<Attachment> Attachments { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

    }
}
