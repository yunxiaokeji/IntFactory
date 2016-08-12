﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ReplyJson
    {
        [Property("Lower")]
        public string replyID { get; set; }

        [Property("Lower")]
        public string taskID { get; set; }

        public string content { get; set; }


        [Property("Lower")]
        public string fromReplyID { get; set; }

        [Property("Lower")]
        public string fromReplyUserID { get; set; }

        [Property("Lower")]
        public string fromReplyAgentID { get; set; }

        public List<Attachment> attachments { get; set; }
    }
}
