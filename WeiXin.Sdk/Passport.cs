using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace WeiXin.Sdk
{
     public class Passport
    {
         public static PassportEntity GetUserInfo(string access_token, string openid)
         {
             Dictionary<string, object> paras = new Dictionary<string, object>();
             paras.Add("access_token", access_token);
             paras.Add("openid", openid);

             var result= HttpRequest.RequestServer(ApiOption.userinfo, paras, RequestType.Post);
             return JsonConvert.DeserializeObject<PassportEntity>(result);
         }
    }
}
