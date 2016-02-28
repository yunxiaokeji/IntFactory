using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace AlibabaSdk.Business
{
    public class OauthBusiness
    {
        public static string GetAuthorizeUrl()
        {
            Dictionary<string, string> paras = new Dictionary<string, string>();
            paras.Add("client_id", AppConfig.AppKey);
            paras.Add("site", "china");
            paras.Add("redirect_uri", AppConfig.CallBackUrl);

            string sign = HttpRequest.sign(paras);

            return string.Format("{0}/auth/authorize.htm?client_id={1}&site=china&redirect_uri={2}&_aop_signature={3}",
                AppConfig.AlibabaApiUrl, AppConfig.AppKey, AppConfig.CallBackUrl, sign);
        }

        public static string GetUserToken(string code)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("code", code);
            paras.Add("redirect_uri", AppConfig.CallBackUrl);
            paras.Add("grant_type", "authorization_code");
            paras.Add("need_refresh_token", true);
            paras.Add("client_id", AppConfig.AppKey);
            paras.Add("client_secret", AppConfig.AppSecret);

            return HttpRequest.RequestServer(ApiOption.accessToken, paras, string.Empty,RequestType.Post);
        }

        public static UserResult GetUserInfo(string code)
        {
            var result = GetUserToken(code);
            var tokenEntity = JsonConvert.DeserializeObject<TokenEntity>(result);

            var model = UserBusiness.GetMemberDetail(tokenEntity.access_token, tokenEntity.memberId);


            return model;
        }

    }
}
