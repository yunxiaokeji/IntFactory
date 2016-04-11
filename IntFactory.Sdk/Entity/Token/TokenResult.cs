using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactory.Sdk
{
    public class TokenResult
    {
        public string access_token { get; set; }

        public string expires_in { get; set; }

        public string refresh_token { get; set; }

        public string aliId { get; set; }

        public string memberId { get; set; }

        public string resource_owner { get; set; }

        public int error_code = 0;

        public string error_message
        {
            get;
            set;
        }
    }
}
