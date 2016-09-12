using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using IntFactoryBusiness;
using IntFactoryEntity;
using IntFactoryBusiness.Manage;
using IntFactoryEntity.Manage;
using System.Web.Script.Serialization;
using IntFactoryEnum;
using YXERP.Common;
namespace YXERP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string orderLevel = "0";
            string taskLevel = "0";
            if (Session["ClientManager"] == null)
            {
                return Redirect("/Home/Login");
            }
            else
            {
                var currentUser = (IntFactoryEntity.Users)Session["ClientManager"];

                if (currentUser.Client.GuideStep != 0)
                {
                    return Redirect("/Default/Index");
                }

                var client = ClientBusiness.GetClientDetail(currentUser.ClientID);
                ViewBag.RemainDay = Math.Ceiling((client.EndTime - DateTime.Now).TotalDays);
                ViewBag.RemainDate = client.EndTime.Date.ToString("yyyy-MM-dd");

                ViewBag.BuyPeople = client.UserQuantity;
                ViewBag.UsePeople = OrganizationBusiness.GetUsers(client.ClientID).FindAll(m => m.Status != 9).Count;

                if (currentUser.Role != null)
                {
                    //所有订单
                    if (ExpandClass.IsExistMenu("102010300"))
                    {
                        orderLevel = "-1";
                    }
                    else if (ExpandClass.IsExistMenu("102010601") || ExpandClass.IsExistMenu("102010701") || ExpandClass.IsExistMenu("102010801"))
                    {
                        orderLevel = "-1";
                    }
                    else if (ExpandClass.IsExistMenu("102010600") || ExpandClass.IsExistMenu("102010700") || ExpandClass.IsExistMenu("102010800"))
                    {
                        orderLevel = "1";
                    }

                    //所有任务
                    if (ExpandClass.IsExistMenu("109010200"))
                    {
                        taskLevel = "-1";
                    }
                    else
                    {
                        taskLevel = "1";
                    } 
                }
                ViewBag.OrderMarks = SystemBusiness.BaseBusiness.GetLableColor(currentUser.ClientID, EnumMarkType.Orders);
                ViewBag.TaskMarks = SystemBusiness.BaseBusiness.GetLableColor(currentUser.ClientID, EnumMarkType.Tasks);
                ViewBag.UserID = currentUser.UserID;
                ViewBag.orderLevel = orderLevel;
                ViewBag.taskLevel = taskLevel;
            }

            return View();
        }

        public ActionResult Register(string source)
        {
            string loginUrl = "/home/login";
            string successUrl = "/home/index";
            if (!string.IsNullOrEmpty(source)) {
                source=source.ToLower();
                if (source== "app") {
                    successUrl=loginUrl = YXERP.Common.Common.IntFactoryAppUrl + "/home/login";
                }
                else if (source == "wxmp") {
                    successUrl=loginUrl = YXERP.Common.Common.IntFactoryAppUrl + "/home/WeiXinMPLogin";
                }
            }
            ViewBag.LoginUrl = loginUrl;
            ViewBag.SuccessUrl = successUrl;

            return View();
        }

        public ActionResult FindPassword(string source)
        {
            string IntFactoryAppUrl = YXERP.Common.Common.IntFactoryAppUrl;
            string loginUrl = "/home/login";
            string registerUrl ="/home/register";
            
            if (!string.IsNullOrEmpty(source))
            {
                source = source.ToLower();
                if (source == "app")
                {
                    loginUrl = IntFactoryAppUrl + "/home/login";
                    registerUrl = "/home/register?source=app";
                }
                else if (source == "wxmp")
                {
                    loginUrl = IntFactoryAppUrl + "/home/WeiXinMPLogin";
                    registerUrl = "/home/register?source=wxmp";
                }
            }
            ViewBag.LoginUrl = loginUrl;
            ViewBag.RegisterUrl = registerUrl;

            return View();
        }

        public ActionResult BindAccount()
        {
            if (Session["AliTokenInfo"] == null)
            {
                return Redirect("/Home/Login");
            }

            return View();
        }

        public ActionResult Logout(int Status = 0)
        {
            HttpCookie cook = Request.Cookies["intfactory_system"];
            if (cook != null)
            {
                cook["status"] = "0";
                Response.Cookies.Add(cook);
            }
            Session["ClientManager"] = null;

            return Redirect("/Home/Login?Status=" + Status);
        }

        public ActionResult InfoPage() 
        {
            return View();
        }

        public ActionResult Terms() 
        {
            return View();
        }

        public ActionResult UseExpired()
        {
            if (Session["ClientManager"] != null)
            {
                var currentUser = (IntFactoryEntity.Users)Session["ClientManager"];
                var client = ClientBusiness.GetClientDetail(currentUser.ClientID);
                ViewBag.EndTime = client.EndTime.ToString("yyyy-MM-dd");
            }

            return View();
        }

        public JsonResult GetSign(string redirect_uri)
        {
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            resultObj.Add("sign", Signature.GetSignature(Common.Common.YXAppKey, Common.Common.YXAppSecret, redirect_uri));

            return new JsonResult
            {
                Data = resultObj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult Authorize(string sign, string redirect_uri)
        {
            if (!string.IsNullOrEmpty(sign) && !string.IsNullOrEmpty(redirect_uri))
            {
                if (sign.Equals(Signature.GetSignature(Common.Common.YXAppKey, Common.Common.YXAppSecret, redirect_uri), StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.Status = 0;
                    ViewBag.ReturnUrl = redirect_uri ?? string.Empty;
                    ViewBag.BindAccountType = 10000;

                    if (Session["ClientManager"] != null)
                    {
                        ViewBag.CurrentUser = (IntFactoryEntity.Users)Session["ClientManager"];
                    }

                    return View();
                }
            }

            Response.Write("<script>alert('参数有误');location.href='http://edj.yunxiaokeji.com';</script>");
            Response.End();
            return View();
        }

        public ActionResult Login(string ReturnUrl, int Status = 0, int BindAccountType=0)
        {
            if (Session["ClientManager"] != null)
            {
                return Redirect("/Home/Index");
            }
            HttpCookie cook = Request.Cookies["intfactory_system"];
            if (cook != null)
            {
                if (cook["status"] == "1")
                {
                    string operateip = Common.Common.GetRequestIP();
                    int result;
                    IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(cook["username"], cook["pwd"], out result, operateip);
                    if (model != null)
                    {
                        Session["ClientManager"] = model;
                        return Redirect("/Home/Index");
                    }
                }
                else
                {
                    ViewBag.UserName = cook["username"];
                }
            }
            ViewBag.Status = Status;
            ViewBag.ReturnUrl = ReturnUrl ?? string.Empty;
            ViewBag.BindAccountType = BindAccountType;

            return View();
        }

        //阿里账户选择进入方式
        public ActionResult AliSelectLogin()
        {
            if (Session["AliTokenInfo"] == null)
            {
                return Redirect("/Home/Login");
            }

            return View();
        }

       //阿里授权地址
        public ActionResult AlibabaLogin(string ReturnUrl)
        {
            return Redirect(AlibabaSdk.OauthBusiness.GetAuthorizeUrl(ReturnUrl??string.Empty));
        }

        //阿里巴巴回调地址
        public ActionResult AlibabaCallBack(string code, string state)
        {
            string operateip = Common.Common.GetRequestIP();
            var userToken = AlibabaSdk.OauthBusiness.GetUserToken(code);

            if (userToken.error_code <= 0)
            {
                var model = OrganizationBusiness.GetUserByOtherAccount(EnumAccountType.Ali, userToken.memberId, operateip);
                //已注册
                if (model != null)
                {
                    //未注销
                    if (model.Status.Value ==1)
                    {
                        Session["ClientManager"] = model;
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanToken(model.ClientID, userToken.access_token, userToken.refresh_token);

                        if (string.IsNullOrEmpty(state)){
                            return Redirect("/Home/Index");
                        }
                        else {
                            return Redirect(state);
                        }
                    }
                    else
                    {
                        if (model.Status.Value == 9)
                        {
                            Response.Write("<script>alert('您的账户已注销,请切换其他账户登录');location.href='/Home/login';</script>");
                            Response.End();
                        }
                        else{
                            return Redirect("/Home/Login");
                        }
                    }
                }
                else
                {
                    Session["AliTokenInfo"] = userToken.access_token + "|" + userToken.refresh_token + "|" + userToken.memberId;
                    return Redirect("/Home/AliSelectLogin");
                }
            }

            return Redirect("/Home/Login");
        }

        public ActionResult MDCallBack(string code, string state)
        {
            string operateip = Common.Common.GetRequestIP();
            var userToken = AlibabaSdk.OauthBusiness.GetUserToken(code);

            if (userToken.error_code <= 0)
            {
                var model = OrganizationBusiness.GetUserByOtherAccount(EnumAccountType.Ali, userToken.memberId, operateip);
                //已注册
                if (model != null)
                {
                    //未注销
                    if (model.Status.Value == 1)
                    {
                        Session["ClientManager"] = model;
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanToken(model.ClientID, userToken.access_token, userToken.refresh_token);

                        if (string.IsNullOrEmpty(state))
                        {
                            return Redirect("/Home/Index");
                        }
                        else
                        {
                            return Redirect(state);
                        }
                    }
                    else
                    {
                        if (model.Status.Value == 9)
                        {
                            Response.Write("<script>alert('您的账户已注销,请切换其他账户登录');location.href='/Home/login';</script>");
                            Response.End();
                        }
                        else
                        {
                            return Redirect("/Home/Login");
                        }
                    }
                }
                else
                {
                    Session["AliTokenInfo"] = userToken.access_token + "|" + userToken.refresh_token + "|" + userToken.memberId;
                    return Redirect("/Home/AliSelectLogin");
                }
            }

            return Redirect("/Home/Login");
        }

        //阿里账户注册
        public ActionResult AliRegisterMember()
        {
            string operateip = Common.Common.GetRequestIP();
            int result;
            if (Session["AliTokenInfo"] != null)
            {
                string tokenInfo = Session["AliTokenInfo"].ToString();
                string[] tokenArr = tokenInfo.Split('|');
                if (tokenArr.Length == 3)
                {
                    string access_token = tokenArr[0];
                    string refresh_token = tokenArr[1];
                    string memberId = tokenArr[2];

                    var memberResult = AlibabaSdk.UserBusiness.GetMemberDetail(access_token, memberId);
                    var member = memberResult.result.toReturn[0];

                    string userid = "";

                    string clientid = ClientBusiness.InsertClient(EnumRegisterType.Ali, EnumAccountType.Ali, memberId, "", member.companyName, member.sellerName, member.mobilePhone, member.email, "", "", "", "", "", "", out result, out userid);

                    if (!string.IsNullOrEmpty(clientid))
                    {
                        var current = OrganizationBusiness.GetUserByOtherAccount(EnumAccountType.Ali, member.memberId, operateip);
                        AliOrderBusiness.BaseBusiness.AddAliOrderDownloadPlan(current.UserID, member.memberId, access_token, refresh_token, current.ClientID);

                        Session.Remove("AliTokenInfo");
                        Session["ClientManager"] = current;

                        return Redirect("/Home/Index");
                    }
                }
            }

            return Redirect("/Home/Login");
        }

        //微信账户选择进入方式
        public ActionResult WeiXinSelectLogin()
        {
            if (Session["WeiXinTokenInfo"] == null)
            {
                return Redirect("/Home/Login");
            }
            return View();
        }

        //微信授权地址
        public ActionResult WeiXinLogin(string ReturnUrl)
        {
            return Redirect(WeiXin.Sdk.Token.GetAuthorizeUrl(Server.UrlEncode(WeiXin.Sdk.AppConfig.CallBackUrl), "", false));
        }

        //微信回调地址
        public ActionResult WeiXinCallBack(string code, string state) {
            string operateip = Common.Common.GetRequestIP();
            var userToken = WeiXin.Sdk.Token.GetAccessToken(code);

            if (string.IsNullOrEmpty(userToken.errcode))
            {
                var model = OrganizationBusiness.GetUserByOtherAccount(EnumAccountType.WeiXin, userToken.unionid, operateip);
                //已注册
                if (model != null)
                {
                    //未注销
                    if (model.Status.Value == 1)
                    {
                        Session["ClientManager"] = model;

                        if (string.IsNullOrEmpty(state))
                        {
                            return Redirect("/Home/Index");
                        }
                        else
                        {
                            return Redirect(state);
                        }
                    }
                    else
                    {
                        if (model.Status.Value == 9)
                        {
                            Response.Write("<script>alert('您的账户已注销,请切换其他账户登录');location.href='/Home/login';</script>");
                            Response.End();
                        }
                        else
                        {
                            return Redirect("/Home/Login");
                        }
                    }
                }
                else
                {
                    //Response.Write("<script>alert('您的账户还没绑定微信,请在账户设置里绑定微信后登录');location.href='/Home/login';</script>");
                    //Response.End();
                    Session["WeiXinTokenInfo"] = userToken.access_token + "|" + userToken.openid + "|" + userToken.unionid;
                    return Redirect("/Home/WeiXinSelectLogin");

                }
            }

            return Redirect("/Home/Login");
        }

        //微信账户注册
        public ActionResult WinXinRegisterMember()
        {
            string operateip = Common.Common.GetRequestIP();
            int result;
            if (Session["WeiXinTokenInfo"] != null)
            {
                string tokenInfo = Session["WeiXinTokenInfo"].ToString();
                string[] tokenArr = tokenInfo.Split('|');
                if (tokenArr.Length == 3)
                {
                    string access_token = tokenArr[0];
                    string openid = tokenArr[1];
                    var memberResult = WeiXin.Sdk.Passport.GetUserInfo(access_token, openid);

                    string userid = "";
                    var clientid = ClientBusiness.InsertClient(EnumRegisterType.WeiXin, EnumAccountType.WeiXin, memberResult.unionid, "", memberResult.nickname, memberResult.nickname, 
                                                                "", "", "", "", "", "", "", "", out result, out userid);

                    if (!string.IsNullOrEmpty(clientid))
                    {
                        var current = OrganizationBusiness.GetUserByOtherAccount(EnumAccountType.WeiXin, memberResult.unionid, operateip);
                        Session.Remove("WeiXinTokenInfo");
                        Session["ClientManager"] = current;

                        return Redirect("/Home/Index");
                    }
                }
            }
            return Redirect("/Home/Login");
        }

        //微信绑定回调地址
        public ActionResult WeiXinLoginCallBack(string code, string state)
        {
            string operateip = Common.Common.GetRequestIP();
            var userToken = WeiXin.Sdk.Token.GetAccessToken(code);

            if (string.IsNullOrEmpty(userToken.errcode))
            {
                var model = OrganizationBusiness.GetUserByOtherAccount(EnumAccountType.WeiXin, userToken.unionid, operateip);
                //未绑定
                if (model == null)
                {
                    model = (Users)Session["ClientManager"];
                    bool flag = OrganizationBusiness.BindOtherAccount(EnumAccountType.WeiXin, model.UserID, userToken.unionid, model.ClientID);
                }
                else
                {
                    if (model.Status.Value == 1)
                    {
                        Session["ClientManager"] = model;
                        Response.Write("<script>alert('您的账户已绑定过微信');location.href='/Home/login';</script>");
                        Response.End();
                    }
                    else
                    {
                        if (model.Status.Value == 9)
                        {
                            Response.Write("<script>alert('您的账户已注销,请切换其他账户登录');location.href='/Home/login';</script>");
                            Response.End();
                        }
                        else
                        {
                            return Redirect("/Home/Login");
                        }
                    }
                }
            }

            return View();
        }

        //登录
        public JsonResult UserLogin(string userName, string pwd, string remember, int bindAccountType)
        {
            int result = 0;
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            YXERP.Common.PwdErrorUserEntity pwdErrorUser = null;

            if (Common.Common.CachePwdErrorUsers.ContainsKey(userName)) pwdErrorUser = Common.Common.CachePwdErrorUsers[userName];

            if (pwdErrorUser == null || (pwdErrorUser.ErrorCount < 10 && pwdErrorUser.ForbidTime<DateTime.Now) )
            {
                string operateip = Common.Common.GetRequestIP();
                int outResult;
                IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, out outResult, operateip);
                if (model != null)
                {
                    if (model.Status.Value ==1)
                    {
                        //保持登录状态
                        HttpCookie cook = new HttpCookie("intfactory_system");
                        cook["username"] = userName;
                        cook["pwd"] = pwd;
                        if (remember == "1")
                        {
                            cook["status"] = remember;
                        }
                        cook.Expires = DateTime.Now.AddDays(7);
                        Response.Cookies.Add(cook);

                        //将阿里账户绑定到已有账户
                        if (bindAccountType == 1) 
                        {
                            result = BindAliMember(model);
                        }
                        //将微信账户绑定到已有账户
                        else if (bindAccountType == 2) 
                        {
                            result = BindWeiXin(model);
                        }
                        else if (bindAccountType == 10000)
                        {
                            result = 1;
                            resultObj.Add("userid",model.UserID);
                            resultObj.Add("clientid", model.ClientID);
                            resultObj.Add("sign", Signature.GetSignature(Common.Common.YXAppKey, Common.Common.YXAppSecret, model.UserID));
                        }
                        else
                        {
                            Session["ClientManager"] = model;
                            result = 1;
                        }

                        Common.Common.CachePwdErrorUsers.Remove(userName);
                    }
                    else
                    {
                        if (model.Status.Value == 9)
                        {
                            result = 9;
                        }
                    }
                }
                else
                {
                    //密码错误
                    if (outResult==3)
                    {
                        if (pwdErrorUser == null){
                            pwdErrorUser = new Common.PwdErrorUserEntity();
                        }
                        else
                        {
                            if (pwdErrorUser.ErrorCount > 9)
                            {
                                pwdErrorUser.ErrorCount = 0;
                            }
                        }

                        pwdErrorUser.ErrorCount++;
                        if (pwdErrorUser.ErrorCount > 9)
                        {
                            pwdErrorUser.ForbidTime = DateTime.Now.AddHours(2);
                            result = 2;
                        }
                        else
                        {
                            resultObj.Add("errorCount", pwdErrorUser.ErrorCount);
                            result = 3;
                        }

                        Common.Common.CachePwdErrorUsers[userName] = pwdErrorUser;
                    }
                }
            }
            else
            {
                int forbidTime =(int)(pwdErrorUser.ForbidTime - DateTime.Now).TotalMinutes;
                resultObj.Add("forbidTime", forbidTime);
                result = -1;
            }
            resultObj.Add("result",result);

            return new JsonResult
            {
                Data = resultObj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //绑定阿里账户
        public int BindAliMember(Users model)
        {
            int result = 0;
            if (Session["AliTokenInfo"] != null)
            {
                var client = ClientBusiness.GetClientDetail(model.ClientID);
                if (string.IsNullOrEmpty(client.AliMemberID))
                {

                    string tokenInfo = Session["AliTokenInfo"].ToString();
                    string[] tokenArr = tokenInfo.Split('|');
                    if (tokenArr.Length == 3)
                    {
                        string access_token = tokenArr[0];
                        string refresh_token = tokenArr[1];
                        string memberId = tokenArr[2];

                        bool flag = AliOrderBusiness.BaseBusiness.AddAliOrderDownloadPlan(model.UserID, memberId, access_token, refresh_token,  model.ClientID);
                        if (flag)
                        {
                            flag = OrganizationBusiness.BindOtherAccount(EnumAccountType.Ali, model.UserID, memberId, model.ClientID);
                            if (flag)
                            {
                                Session["ClientManager"] = model;
                                Session.Remove("AliTokenInfo");
                                result = 1;
                            }
                        }
                        else
                        {
                            AliOrderBusiness.BaseBusiness.DeleteAliOrderDownloadPlan(model.ClientID);
                        }
                    }
                }
                else{
                    result = 4;
                }
            }
            else {
                result = 5;
            }

            return result;
        }

        //绑定微信账户
        public int BindWeiXin(Users model)
        {
            int result = 0;
            if (Session["WeiXinTokenInfo"] != null)
            {
                string tokenInfo = Session["WeiXinTokenInfo"].ToString();
                string[] tokenArr = tokenInfo.Split('|');
                if (tokenArr.Length == 3)
                {
                    string access_token = tokenArr[0];
                    string unionid = tokenArr[2];
                    bool flag = OrganizationBusiness.BindOtherAccount(EnumAccountType.WeiXin, model.UserID, unionid, model.ClientID);
                    if (flag)
                    {
                        Session["ClientManager"] = model;
                        Session.Remove("WeiXinTokenInfo");
                        result = 1;
                    }
                }
            }
            else{
                result = 5;
            }

            return result;
        }

        public JsonResult IsExistLoginName(string loginName)
        {
            bool bl = OrganizationBusiness.IsExistLoginName(loginName);
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JsonDictionary.Add("Result", bl?1:0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult RegisterClient(string name, string companyName, string loginName, string loginPWD,string code)
        {
            int result = 0;
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

            bool bl = OrganizationBusiness.IsExistLoginName(loginName);
            if (bl){
                result = 2;
            }
            else
            {
                bl = Common.Common.ValidateMobilePhoneCode(loginName, code);
                if (!bl){
                    result = 3;
                }
                else
                {
                    string userid = "";
                    ClientBusiness.InsertClient(EnumRegisterType.Self, EnumAccountType.Mobile, loginName, loginPWD, companyName, name, loginName, "", "", "", "", "", "", string.Empty, out result, out userid);

                    if (result == 1)
                    {
                        string operateip = Common.Common.GetRequestIP();
                        int outResult;
                        IntFactoryEntity.Users user = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(loginName, loginPWD, out outResult, operateip);
                        if (user != null)
                        {
                            Session["ClientManager"] = user;
                        }

                        Common.Common.ClearMobilePhoneCode(loginName);
                    }
                    else
                    {
                        result = 0;
                    }
                }
            }

            JsonDictionary.Add("Result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SendMobileMessage(string mobilePhone)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            Random rd = new Random();
            int code = rd.Next(100000, 1000000);

            bool flag = Common.MessageSend.SendMessage(mobilePhone, code);
            JsonDictionary.Add("Result", flag ? 1 : 0);

            if (flag)
            {
                Common.Common.SetCodeSession(mobilePhone, code.ToString());

                Common.Common.WriteAlipayLog(mobilePhone + " : " + code.ToString());
            }

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ValidateMobilePhoneCode(string mobilePhone, string code)
        {
            bool bl = Common.Common.ValidateMobilePhoneCode(mobilePhone, code);
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JsonDictionary.Add("Result", bl ? 1 : 0);
            
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateUserPwd(string loginName, string loginPwd, string code)
        {
            int result = 0;
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

            bool bl = OrganizationBusiness.IsExistLoginName(loginName);
            if (bl)
            {
                bl = Common.Common.ValidateMobilePhoneCode(loginName, code);
                if (!bl)
                {
                    result = 3;
                }
                else
                {
                    bl = OrganizationBusiness.UpdateUserAccountPwd(loginName, loginPwd);
                    result = bl ? 1 : 0;

                    if (bl)
                    {
                        Common.Common.CachePwdErrorUsers.Remove(loginName);
                        Common.Common.ClearMobilePhoneCode(loginName);
                    }
                }
                
            }
            else
            {
                result = 2;
            }

            JsonDictionary.Add("Result",result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetAuthorizeInfo()
        {

            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            double remainderDays = 0;
            int authorizeType = 0;
            string clientCode = string.Empty;

            if (Session["ClientManager"] != null)
            {
                var CurrentUser = (IntFactoryEntity.Users)Session["ClientManager"];
                var client = ClientBusiness.GetClientDetail(CurrentUser.ClientID);
                remainderDays = Math.Ceiling((client.EndTime - DateTime.Now).TotalDays);
                authorizeType = client.AuthorizeType;
            }
            JsonDictionary.Add("remainderDays", remainderDays);
            JsonDictionary.Add("authorizeType", authorizeType);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        public ActionResult FeedBack()
        {
            if (Session["ClientManager"] != null)
            {
                var userInfo = Session["ClientManager"];
                ViewBag.userInfo = userInfo;
            }
            else
            {
              return  Redirect("/home/login");
            }
            return View();
        }

        public JsonResult GetOrdersOrTasksReportData(int orderType, int filterTimeType, int moduleType, int taskType, string userID)
        {
            Dictionary<string, Object> resultObj = new Dictionary<string, object>();
            int result = 0;

            if (Session["ClientManager"] != null)
            {
                var currentUser = (IntFactoryEntity.Users)Session["ClientManager"];
                var nowDate=DateTime.Now;
                string beginTime = "";
                string endTime = "";
                if (filterTimeType == 0)
                {
                    beginTime = nowDate.Date.AddDays(-15).ToString();
                    endTime = nowDate.Date.AddDays(-1).ToString();
                    nowDate = nowDate.Date.AddDays(-15);
                }
                else if (filterTimeType == 1)
                {
                    beginTime = nowDate.Date.ToString();
                    endTime = nowDate.Date.AddDays(14).ToString();
                }
                else if (filterTimeType == 2)
                {
                    beginTime = nowDate.Date.AddDays(15).ToString();
                    endTime = nowDate.Date.AddDays(29).ToString();
                    nowDate = nowDate.Date.AddDays(15);
                }

                List<OrderEntity> orderItems = new List<OrderEntity>();
                List<IntFactoryEntity.Task.TaskEntity> taskItems = new List<IntFactoryEntity.Task.TaskEntity>();

                int getTotalCount=0;
                int pageCount = 0;

                if (moduleType == 1)
                {
                    bool isDYAll = ExpandClass.IsExistMenu("102010701");
                    bool isDHAll = ExpandClass.IsExistMenu("102010801");
                    string orderWhere = "";

                    //所有订单
                    if (ExpandClass.IsExistMenu("102010300"))
                    {

                    }
                    else if (!isDYAll && isDHAll)
                    {
                        orderWhere = " and (o.OrderType=2 or ((o.OwnerID='" + currentUser.UserID + "' or o.CreateUserID='" + currentUser.UserID + "'))) ";
                    }
                    else if (isDYAll && !isDHAll)
                    {
                        orderWhere = " and (o.OrderType=1 or ((OwnerID='" + currentUser.UserID + "' or o.CreateUserID='" + currentUser.UserID + "'))) ";
                    }
                    else
                    {
                        userID = currentUser.UserID;
                    }

                    orderItems = IntFactoryBusiness.OrdersBusiness.BaseBusiness.GetOrdersByPlanTime(beginTime, endTime, orderType, -1, -1,
                                        userID, currentUser.ClientID, orderWhere, int.MaxValue, 1, ref getTotalCount, ref pageCount);
                }
                else
                {
                    //所有任务
                    if (!ExpandClass.IsExistMenu("109010200"))
                    {
                        userID = currentUser.UserID;
                    }

                    taskItems = IntFactoryBusiness.TaskBusiness.GetTasksByEndTime(beginTime, endTime,
                    orderType, -1, -1, -1, taskType,
                    userID, currentUser.ClientID, int.MaxValue, 1, ref getTotalCount, ref pageCount);
                }

                var totalExceedCount = 0;
                var totalWarnCount = 0;
                var totalWorkCount = 0;
                var totalFinishCount = 0;
                var totalSumCount = 0;
                var reportArr =new  List<Dictionary<string, Object>>();
                for (var i = 0; i < 15; i++) 
                {
                    var report = new Dictionary<string, Object>();
                    var nextDate = nowDate.AddDays(i);
                    
                    var exceedCount = 0;
                    var warnCount = 0;
                    var workCount = 0;
                    var finishCount = 0;
                    var totalCount = 0;

                    //订单操作
                    if (moduleType == 1)
                    {
                        var orderList = orderItems.FindAll(m => m.PlanTime.Date == nextDate.Date);
                        exceedCount = orderList.FindAll(m => m.PlanTime < DateTime.Now && m.OrderStatus == 1).Count;
                        for (var j = 0; j < orderList.Count; j++)
                        {
                            var order = orderList[j];
                            if (order.PlanTime > DateTime.Now && order.OrderStatus == 1)
                            {
                                if ((order.PlanTime - DateTime.Now).TotalHours * 3 < (order.PlanTime - order.OrderTime).TotalHours)
                                {
                                    warnCount++;
                                }
                                else
                                {
                                    workCount++;
                                }
                            }
                        }
                        finishCount = orderList.FindAll(m => m.OrderStatus == 2).Count;
                    }
                    //任务操作
                    else
                    {
                        var taskList = taskItems.FindAll(m => m.EndTime.Date == nextDate.Date);
                        exceedCount = taskList.FindAll(m => m.EndTime < DateTime.Now && m.FinishStatus == 1).Count;
                        for (var j = 0; j < taskList.Count; j++)
                        {
                            var task = taskList[j];
                            if (task.EndTime > DateTime.Now && task.FinishStatus == 1)
                            {
                                if ((task.EndTime - DateTime.Now).TotalHours * 3 < (task.EndTime - task.AcceptTime).TotalHours)
                                {
                                    warnCount++;
                                }
                                else
                                {
                                    workCount++;
                                }
                            }
                        }
                        finishCount = taskList.FindAll(m => m.FinishStatus == 2).Count;
                    }
                    report.Add("dateTime", nextDate.Date.ToString("yyyy.MM.dd"));
                    report.Add("date", nextDate.Date.ToString("MM.dd"));
                    report.Add("warnCount", warnCount);
                    report.Add("finishCount", finishCount);
                    report.Add("workCount", workCount);
                    report.Add("exceedCount", exceedCount);
                    totalCount=warnCount + finishCount + workCount + exceedCount;
                    report.Add("totalCount", totalCount);

                    totalExceedCount += exceedCount;
                    totalFinishCount += finishCount;
                    totalWarnCount += warnCount;
                    totalWorkCount += workCount;
                    totalSumCount += totalCount;
                    reportArr.Add(report);
                }

                result = 1;
                resultObj.Add("items", reportArr);
                resultObj.Add("totalExceedCount", totalExceedCount);
                resultObj.Add("totalFinishCount", totalFinishCount);
                resultObj.Add("totalWarnCount", totalWarnCount);
                resultObj.Add("totalWorkCount", totalWorkCount);
                resultObj.Add("totalSumCount", totalSumCount);
            }
            resultObj.Add("result", result);

            return new JsonResult()
            {
                Data=resultObj,
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrdersOrTasksDataList(int filterType, string filterTime,
           int moduleType, int orderType, int taskType, string userID,
           int pageSize, int pageIndex, int preFinishStatus) 
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            var currentUser = (IntFactoryEntity.Users)Session["ClientManager"];

            string startTime = string.Empty;
            int orderStatus = 0;
            int finishStatus = 0;
            if (!string.IsNullOrEmpty(filterTime))
            {
                startTime = filterTime;
                orderStatus = -1;
                finishStatus = -1;
            }
            else
            {
                if (filterType != -1)
                {
                    orderStatus = -1;
                    finishStatus = -1;
                }
            }

            int getTotalCount = 0;
            int pageCount = 0;

            if (moduleType == 1)
            {
               
                string orderWhere = "";
                //所有订单
                if (!ExpandClass.IsExistMenu("102010300"))
                {
                    if (orderStatus == 0)
                    {
                        if (!ExpandClass.IsExistMenu("102010601"))
                        {
                            userID = currentUser.UserID;
                        }
                    }
                    else
                    {
                        bool isDYAll = ExpandClass.IsExistMenu("102010701");
                        bool isDHAll = ExpandClass.IsExistMenu("102010801");

                        if (!isDYAll && isDHAll)
                        {
                            orderWhere = " and (o.OrderType=2 or ((o.OwnerID='" + currentUser.UserID + "' or o.CreateUserID='" + currentUser.UserID + "'))) ";
                        }
                        else if (isDYAll && !isDHAll)
                        {
                            orderWhere = " and (o.OrderType=1 or ((OwnerID='" + currentUser.UserID + "' or o.CreateUserID='" + currentUser.UserID + "'))) ";
                        }
                        else
                        {
                            userID = currentUser.UserID;
                        }
                    }
                }

                var list = IntFactoryBusiness.OrdersBusiness.BaseBusiness.GetOrdersByPlanTime(startTime, startTime, orderType, filterType, orderStatus,
                   userID, currentUser.ClientID, orderWhere, pageSize, pageIndex, ref getTotalCount, ref pageCount);
                JsonDictionary.Add("items", list);
            }
            else
            {
                //所有任务
                if (!ExpandClass.IsExistMenu("109010200"))
                {
                    userID = currentUser.UserID;
                }

                var list = IntFactoryBusiness.TaskBusiness.GetTasksByEndTime(startTime, startTime,
                    orderType, filterType, finishStatus, preFinishStatus, taskType,
                    userID, currentUser.ClientID, pageSize, pageIndex, ref getTotalCount, ref pageCount);
                JsonDictionary.Add("items", list);
            }

            JsonDictionary.Add("getNeedTotalCount", getTotalCount);
            JsonDictionary.Add("pageCount", pageCount);
            if (!string.IsNullOrEmpty(filterTime))
            {
                JsonDictionary.Add("showTime", filterTime.Replace(".", "-") + "/" + YXERP.Common.Common.Week("周", (int)Convert.ToDateTime(filterTime).DayOfWeek));
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskOrOrderEcceedCount(int moduleType, int orderType, string userID)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            var currentUser = (IntFactoryEntity.Users)Session["ClientManager"];
            var total = 0;
            
            if (moduleType == 1)
            {
                bool isDYAll = ExpandClass.IsExistMenu("102010701");
                bool isDHAll = ExpandClass.IsExistMenu("102010801");
                string orderWhere = "";
                //订单权限控制
                if (orderType == -1 )
                {
                    //所有订单
                    if (ExpandClass.IsExistMenu("102010300"))
                    {
                        orderWhere = "-1";
                    }
                    else if (isDYAll && isDHAll) //所有大货单和打样单
                    {
                        orderWhere = "-1";
                    }
                    else if (!isDYAll && isDHAll)
                    {
                        orderWhere = " and (OrderType=2 or ((OwnerID='" + currentUser.UserID + "' or CreateUserID='" + currentUser.UserID + "'))) ";
                    }
                    else if (isDYAll && !isDHAll)
                    {
                        orderWhere = " and (OrderType=1 or ((OwnerID='" + currentUser.UserID + "' or CreateUserID='" + currentUser.UserID + "'))) ";
                    }
                    else
                    {
                        userID = currentUser.UserID;
                    }
                }
                else if (orderType == 1)
                {
                    if (!isDYAll)
                    {
                        userID = currentUser.UserID;
                    }
                    orderWhere = " and OrderType=1 ";
                }
                else if (orderType == 2)
                {
                    if (!isDHAll)
                    {
                        userID = currentUser.UserID;
                    }
                    orderWhere = " and OrderType=2 ";
                }

                total = IntFactoryBusiness.OrdersBusiness.BaseBusiness.GetExceedOrderCount(userID, orderWhere, currentUser.ClientID);   
            }
            else
            {
                //任务权限控制
                if (!ExpandClass.IsExistMenu("109010200"))
                {
                    userID = currentUser.UserID;
                }

                total = IntFactoryBusiness.TaskBusiness.GetExceedTaskCount(userID, orderType, currentUser.ClientID);
            }

            JsonDictionary.Add("result", total);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
