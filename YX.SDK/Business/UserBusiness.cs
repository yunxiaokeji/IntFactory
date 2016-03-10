using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace AlibabaSdk
{
    public class UserBusiness
    {
        public static UserResult GetMemberDetail(string token, string memberId)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("memberId", memberId);

            var result = HttpRequest.RequestServer(ApiOption.memberDetail, paras);
            return JsonConvert.DeserializeObject<UserResult>(result);
        }

    }
}
