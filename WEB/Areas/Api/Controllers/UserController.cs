using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEnum;
namespace YXERP.Areas.Api.Controllers
{
    public class UserController : BaseAPIController
    {

        public JsonResult UserLogin(string userName, string pwd)
        {
            int result = 0;
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            YXERP.Common.PwdErrorUserEntity pwdErrorUser = null;
            if (Common.Common.CachePwdErrorUsers.ContainsKey(userName))
            {
                pwdErrorUser = Common.Common.CachePwdErrorUsers[userName];
            }

            if (pwdErrorUser == null || (pwdErrorUser.ErrorCount < 10 && pwdErrorUser.ForbidTime < DateTime.Now))
            {
                string operateip = Common.Common.GetRequestIP();
                IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, out result, operateip);
                if (model != null)
                {
                    if (result == 1)
                    {
                        Dictionary<string, object> userObj = new Dictionary<string, object>();
                        string domainUrl = Request.Url.Scheme + "://" + Request.Url.Host;
                        userObj.Add("userID", model.UserID);
                        userObj.Add("clientID", model.ClientID);
                        userObj.Add("companyName", model.Client.CompanyName);
                        userObj.Add("name", model.Name);
                        userObj.Add("avatar", domainUrl + model.Avatar);
                        resultObj.Add("user", userObj);
                    }
                }
                else
                {
                    if (result == 3)
                    {
                        if (pwdErrorUser == null)
                        {
                            pwdErrorUser = new Common.PwdErrorUserEntity();
                        }
                        else
                        {
                            if (pwdErrorUser.ErrorCount > 9)
                            {
                                pwdErrorUser.ErrorCount = 0;
                            }
                        }

                        pwdErrorUser.ErrorCount += 1;
                        if (pwdErrorUser.ErrorCount > 9)
                        {
                            pwdErrorUser.ForbidTime = DateTime.Now.AddHours(2);
                            result = 2;
                        }
                        else
                        {
                            result = 3;
                            resultObj.Add("errorCount", pwdErrorUser.ErrorCount);
                        }
                        Common.Common.CachePwdErrorUsers[userName] = pwdErrorUser;
                    }
                }
            }
            else
            {
                int forbidTime = (int)(pwdErrorUser.ForbidTime - DateTime.Now).TotalMinutes;
                resultObj.Add("forbidTime", forbidTime);
                result = -1;
            }
            resultObj.Add("result", result);

            return new JsonResult
            {
                Data = resultObj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserByWeiXinMP(string unionid, string openid)
        {
            int result = 0;
            string operateip = Common.Common.GetRequestIP();
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            var model = OrganizationBusiness.GetUserByOtherAccount(EnumAccountType.WeiXin, unionid, operateip, openid);
            if (model != null)
            {
                //未注销
                if (model.Status.Value == 1)
                {
                    result = 1;
                    Dictionary<string, object> userObj = new Dictionary<string, object>();
                    //string domainUrl = Request.Url.Scheme + "://" + Request.Url.Host;
                    userObj.Add("userID", model.UserID);
                    userObj.Add("clientID", model.ClientID);
                    userObj.Add("name", model.Name);
                    userObj.Add("avatar", model.Avatar);
                    resultObj.Add("user", userObj);
                }
                else {
                    result = 2;
                }
            }
            resultObj.Add("result", result);

            return new JsonResult
            {
                Data = resultObj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public  JsonResult BindWeiXinMP(string unionid, string openid, string userID, string clientID)
        {
            string operateip = Common.Common.GetRequestIP();
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            bool flag = OrganizationBusiness.BindOtherAccount(EnumAccountType.WeiXin, userID, unionid, clientID, openid);
            resultObj.Add("result", flag?1:0);

            return new JsonResult
            {
                Data = resultObj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [YXERP.Common.ApiAuthorize]
        public JsonResult GetUserByUserID(string userID, string clientID) {
            var user = OrganizationBusiness.GetUserByUserID(userID, clientID);
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("userID", user.UserID);
            obj.Add("clientID", user.ClientID);
            obj.Add("name", user.Name);
            obj.Add("avatar", user.Avatar);
            obj.Add("mobilePhone", user.MobilePhone);
            obj.Add("isSystemAdmin", user.Role.IsDefault == 1);
            JsonDictionary.Add("user", obj);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
