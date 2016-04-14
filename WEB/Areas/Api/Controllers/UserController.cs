using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
namespace YXERP.Areas.Api.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /Api/User/

        public JsonResult UserLogin(string userName, string pwd)
        {
            int result = 0;
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            YXERP.Common.PwdErrorUserEntity pwdErrorUser = null;

            if (Common.Common.CachePwdErrorUsers.ContainsKey(userName)) pwdErrorUser = Common.Common.CachePwdErrorUsers[userName];

            if (pwdErrorUser == null || (pwdErrorUser.ErrorCount < 3 && pwdErrorUser.ForbidTime < DateTime.Now))
            {
                string operateip = Common.Common.GetRequestIP();
                int outResult;
                IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, out outResult, operateip);
                if (model != null)
                {
                    //保持登录状态
                    HttpCookie cook = new HttpCookie("cloudsales");
                    cook["username"] = userName;
                    cook["pwd"] = pwd;
                    cook.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cook);

                    if (outResult == 3)
                    {
                        if (pwdErrorUser == null)
                        {
                            pwdErrorUser = new Common.PwdErrorUserEntity();
                        }
                        else
                        {
                            if (pwdErrorUser.ErrorCount > 2)
                            {
                                pwdErrorUser.ErrorCount = 0;
                            }
                        }

                        pwdErrorUser.ErrorCount += 1;
                        if (pwdErrorUser.ErrorCount > 2)
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
                else
                {
                    int forbidTime = (int)(pwdErrorUser.ForbidTime - DateTime.Now).TotalMinutes;
                    resultObj.Add("forbidTime", forbidTime);
                    result = -1;
                }


                
            }

            resultObj.Add("result", result);
            return new JsonResult
            {
                Data = resultObj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };


        }

    }
}
