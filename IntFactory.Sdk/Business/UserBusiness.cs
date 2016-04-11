using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace IntFactory.Sdk
{
    public class UserBusiness
    {
        /// <summary>
        /// 获取用户详情
        /// </summary>
        public static MemberResult GetMemberDetail(string token, string memberId)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("memberId", memberId);

            return HttpRequest.RequestServer<MemberResult>(ApiOption.memberDetail, paras);

        }

    }
}
