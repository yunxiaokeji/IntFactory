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
        public static UsetList GetUserAll(string token,string keywords, int pageindex = 0, int pagesize = 20)
        {
            var paras=new Dictionary<string,object>();
            paras.Add("access_token", token);
            paras.Add("pageindex",pageindex);
            paras.Add("pagesize", pagesize);
            paras.Add("keywords", keywords);
            var result = HttpRequest.RequestServer(ApiOption.user_all, paras);

            return JsonConvert.DeserializeObject<UsetList>(result);
        }

        public static UserJson GetUserDetail(string token, string userID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("access_token", token);
            paras.Add("u_id", userID);
            var result = HttpRequest.RequestServer(ApiOption.user_detail, paras);

            return JsonConvert.DeserializeObject<UserJson>(result);
        }

        public static UserResult GetMemberDetail(string token, string memberId)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("memberId", memberId);

            var result = HttpRequest.RequestServer(ApiOption.passport_detail, paras,token);
            return JsonConvert.DeserializeObject<UserResult>(result);

            //JObject resultObj =(JObject) JsonConvert.DeserializeObject(result);
            //JObject resultObj2 = (JObject)resultObj["result"];
            //JArray returnArr = (JArray)resultObj2["toReturn"];

            //return JsonConvert.DeserializeObject<UserResult>(JsonConvert.SerializeObject(returnArr[0]));
        }

    }
}
