using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXin.Sdk
{
    public class TokenEntity
    {
        public string openid { get; set; }

        public string access_token { get; set; }

        public string unionid { get; set; }

        public string errcode { get; set; }
    }
}
