using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlibabaSdk
{
    public enum ApiOption
    {
        oauth2_access_token=0,

        user_all=1,
        user_detail=2,
        passport_detail=3,

        post_v2_all=4,
        post_v2_detail=5,
        post_update=6,

        group_my_joined=7,

        message_create_sys=8,

        task_v4_addTask=9,

        calendar_create=10,

        app_is_admin = 11


    }
}
