using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WeiXin.Sdk
{
    public enum ApiOption
    {
        [Description("/sns/oauth2/access_token")]
        access_token,

        [Description("/sns/userinfo")]
        userinfo
    }
}
