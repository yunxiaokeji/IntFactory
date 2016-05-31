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

namespace YXERP.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (Session["ClientManager"] == null)
            {
                return Redirect("/Home/Login");
            }

            return View();
        }

        public ActionResult NewIndex() {
            return View();
        }
        //public ActionResult Register()
        //{
        //    return View();
        //}

        public ActionResult FindPassword()
        {
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

        public ActionResult SelectLogin()
        {
            if (Session["AliTokenInfo"] == null)
            {
                return Redirect("/Home/Login");
            }

            return View();
        }

        public ActionResult Login(string ReturnUrl, int Status = 0)
        {
            if (Session["ClientManager"] != null)
            {
                return Redirect("/Home/Index");
            }
            HttpCookie cook = Request.Cookies["cloudsales"];
            if (cook != null)
            {
                if (cook["status"] == "1")
                {
                    string operateip = Common.Common.GetRequestIP();
                    int result;
                    IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(cook["username"], cook["pwd"],out result, operateip);
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
            return View();
        }
        
        public ActionResult Logout(int Status = 0)
        {
            HttpCookie cook = Request.Cookies["cloudsales"];
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
                var agent = IntFactoryBusiness.AgentsBusiness.GetAgentDetail(currentUser.AgentID);
                ViewBag.EndTime = agent.EndTime.ToString("yyyy-MM-dd");
            }

            return View();
        }

        public ActionResult SelfOrder(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Home/Login");
            }
            var model = ClientBusiness.GetClientDetail(id);
            if (model == null || string.IsNullOrEmpty(model.ClientID))
            {
                return Redirect("/Home/Login");
            }
            ViewBag.Model = model;
            var list = new ProductsBusiness().GetClientCategorysByPID("", EnumCategoryType.Order, id);
            ViewBag.Items = list;
            return View();
        }

        public ActionResult OrderSuccess(string id)
        {
            var order = OrdersBusiness.BaseBusiness.GetOrderByID(id);
            if (order == null || string.IsNullOrEmpty(order.OrderID))
            {
                return Redirect("/Home/Login");
            }
            var model = ClientBusiness.GetClientDetail(order.ClientID);
            ViewBag.Model = model;
            ViewBag.Order = order;
            return View();
        }

        public JsonResult GetAgentActionData()
        {
            int customercount = 0, ordercount = 0;
            decimal totalmoney = 0;
            IntFactoryEntity.Users CurrentUser = (IntFactoryEntity.Users)Session["ClientManager"];
            var model = LogBusiness.BaseBusiness.GetClientActions(CurrentUser.ClientID, ref customercount, ref ordercount, ref totalmoney);

            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JsonDictionary.Add("model", model);

            int myNeedOrders = 0;
            int cooperationNeedOrders = 0;
            int delegateNeedOrders = 0;

            int myFentOrder = 0;
            int doMyFentOrder = 0;
            int cooperationFentOrders = 0;
            int doCooperationFentOrders = 0;
            int delegateFentOrders = 0;
            int doDelegateFentOrders = 0;

            int myBulkOrder = 0;
            int doMyBulkOrder = 0;
            int cooperationBulkOrders = 0;
            int doCooperationBulkOrders = 0;
            int delegateBulkOrders = 0;
            int doDelegateBulkOrders = 0;
            foreach (var action in model.Actions) {
                if (action.OrderType == 1)
                {
                    if (action.Status == 0)
                    {
                        if (action.ObjectType == 6)
                        {
                            myNeedOrders += action.OrderCount;
                        }
                        else if (action.ObjectType == 5)
                        {
                            cooperationNeedOrders += action.OrderCount;
                        }
                        else if (action.ObjectType == 4)
                        {
                            delegateNeedOrders += action.OrderCount;
                        }
                    }
                    else
                    {

                        if (action.ObjectType == 6)
                        {
                            myFentOrder += action.OrderCount;
                            if (action.Status == 2)
                            {
                                doMyFentOrder += action.OrderCount;
                            }
                        }
                        else if (action.ObjectType == 5)
                        {
                            cooperationFentOrders += action.OrderCount;
                            if (action.Status == 2)
                            doCooperationFentOrders += action.OrderCount;
                        }
                        else if (action.ObjectType == 4)
                        {
                            delegateFentOrders += action.OrderCount;
                            if (action.Status == 2)
                            doDelegateFentOrders += action.OrderCount;
                        }

                    }
                }
                else
                {
                    if (action.Status == 0)
                    {
                        if (action.ObjectType == 6)
                        {
                            myNeedOrders += action.OrderCount;
                        }
                        else if (action.ObjectType == 5)
                        {
                            cooperationNeedOrders += action.OrderCount;
                        }
                        else if (action.ObjectType == 4)
                        {
                            delegateNeedOrders += action.OrderCount;
                        }
                    }
                    else
                    {
                        if (action.ObjectType == 6)
                        {
                            myBulkOrder += action.OrderCount;
                            if (action.Status == 2)
                            {
                                doMyBulkOrder += action.OrderCount;
                            }
                        }
                        else if (action.ObjectType == 5)
                        {
                            cooperationBulkOrders += action.OrderCount;
                            if (action.Status == 2)
                                doCooperationBulkOrders += action.OrderCount;
                        }
                        else if (action.ObjectType == 4)
                        {
                            delegateBulkOrders += action.OrderCount;
                            if (action.Status == 2)
                                doDelegateBulkOrders += action.OrderCount;
                        }
                    }
                }
            }

            JsonDictionary.Add("customercount", model.CustomerCount);
            JsonDictionary.Add("ordercount", model.OrderCount);
            JsonDictionary.Add("totalmoney", model.TotalMoney.ToString("C"));
            JsonDictionary.Add("myOrders", myNeedOrders);
            JsonDictionary.Add("cooperationOrders", cooperationNeedOrders);
            JsonDictionary.Add("delegateOrders", delegateNeedOrders);

            JsonDictionary.Add("myFentOrder", myFentOrder);
            JsonDictionary.Add("doMyFentOrder",doMyFentOrder);
            JsonDictionary.Add("cooperationFentOrders", cooperationFentOrders);
            JsonDictionary.Add("doCooperationFentOrders",doCooperationFentOrders);
            JsonDictionary.Add("delegateFentOrders", delegateFentOrders);
            JsonDictionary.Add("doDelegateFentOrders",doDelegateFentOrders);

            JsonDictionary.Add("myBulkOrder", myBulkOrder);
            JsonDictionary.Add("doMyBulkOrder",doMyBulkOrder);
            JsonDictionary.Add("cooperationBulkOrders", cooperationBulkOrders);
            JsonDictionary.Add("doCooperationBulkOrders",doCooperationBulkOrders);
            JsonDictionary.Add("delegateBulkOrders", delegateBulkOrders);
            JsonDictionary.Add("doDelegateBulkOrders",doDelegateBulkOrders);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 阿里账户登录
        /// </summary>
        /// <returns></returns>
        public ActionResult MDLogin(string ReturnUrl)
        {
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                return Redirect(AlibabaSdk.OauthBusiness.GetAuthorizeUrl() );
            }
            else
            {
                return Redirect(AlibabaSdk.OauthBusiness.GetAuthorizeUrl(ReturnUrl) );
            }
        }

        //明道登录回掉
        public ActionResult MDCallBack(string code, string state)
        {
            string operateip = Common.Common.GetRequestIP();
            var userToken = AlibabaSdk.OauthBusiness.GetUserToken(code);

            if (userToken.error_code <= 0)
            {
                var model = OrganizationBusiness.GetUserByAliMemberID(userToken.memberId, operateip);
                //已注册云销账户
                if (model != null)
                {
                    //未注销
                    if (model.Status.Value ==1)
                    {
                        model.AliToken = userToken.access_token;
                        Session["ClientManager"] = model;
                        AliOrderBusiness.BaseBusiness.UpdateAliOrderDownloadPlanToken(model.ClientID, userToken.access_token, userToken.refresh_token);

                        if (string.IsNullOrEmpty(state))
                            return Redirect("/Home/Index");
                        else
                            return Redirect(state);
                    }
                    else {
                        if (model.Status.Value == 9)
                        {
                            Response.Write("<script>alert('您的账户已注销,请切换其他账户登录');location.href='/Home/login';</script>");
                            Response.End();
                        }
                        else {
                            return Redirect("/Home/Login");
                        }

                    }
                }
                else
                {
                    Session["AliTokenInfo"] = userToken.access_token + "|" + userToken.refresh_token + "|" + userToken.memberId;
                    return Redirect("/Home/SelectLogin");
                }
            }

            return Redirect("/Home/Login");
        }

        /// <summary>
        /// 员工登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonResult UserLogin(string userName, string pwd, string remember, int fromBindAccount)
        {
            int result = 0;
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            YXERP.Common.PwdErrorUserEntity pwdErrorUser = null;

            if (Common.Common.CachePwdErrorUsers.ContainsKey(userName)) pwdErrorUser = Common.Common.CachePwdErrorUsers[userName];

            if (pwdErrorUser == null || (pwdErrorUser.ErrorCount < 3 && pwdErrorUser.ForbidTime<DateTime.Now) )
            {
                string operateip = Common.Common.GetRequestIP();
                int outResult;
                IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, out outResult, operateip);
                if (model != null)
                {
                    if (model.Status.Value ==1)
                    {
                        //保持登录状态
                        HttpCookie cook = new HttpCookie("cloudsales");
                        cook["username"] = userName;
                        cook["pwd"] = pwd;
                        cook["status"] = remember;
                        cook.Expires = DateTime.Now.AddDays(7);
                        Response.Cookies.Add(cook);

                        //将阿里账户绑定到现有账户
                        if (fromBindAccount == 1)
                        {
                            result=BindAliMember(model);
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
                            if (pwdErrorUser.ErrorCount > 2)
                            {
                                pwdErrorUser.ErrorCount = 0;
                            }
                        }

                        pwdErrorUser.ErrorCount++;
                        if (pwdErrorUser.ErrorCount > 2)
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

                        bool flag = AliOrderBusiness.BaseBusiness.AddAliOrderDownloadPlan(model.UserID, memberId, access_token, refresh_token, model.AgentID, model.ClientID);
                        if (flag)
                        {
                            flag = ClientBusiness.BindClientAliMember(model.ClientID, model.UserID, memberId);
                            if (flag)
                            {
                                model.AliToken = access_token;
                                model.AliMemberID = memberId;
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
                else
                {
                    result = 4;
                }

            }
            else
            {
                result = 5;
            }

            return result;
        }

        public ActionResult AliRegisterMember() {

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

                    var memberResult = AlibabaSdk.UserBusiness.GetMemberDetail(access_token,memberId);
                    var member = memberResult.result.toReturn[0];

                    Clients clientModel = new Clients();
                    clientModel.CompanyName = member.companyName??string.Empty;
                    clientModel.ContactName = member.sellerName??string.Empty;
                    clientModel.MobilePhone = string.Empty;

                    var clientid = ClientBusiness.InsertClient(clientModel, "", "", "", "", out result,
                        member.email,string.Empty,string.Empty,
                        member.memberId);

                    if (!string.IsNullOrEmpty(clientid))
                    {
                        var current = OrganizationBusiness.GetUserByAliMemberID(member.memberId, operateip);
                        AliOrderBusiness.BaseBusiness.AddAliOrderDownloadPlan(current.UserID, member.memberId, access_token, refresh_token, current.AgentID, current.ClientID);

                        current.MDToken = access_token;
                        Session.Remove("AliTokenInfo");
                        Session["ClientManager"] = current;

                        return Redirect("/Home/Index");
                    }
                }
            }

            return Redirect("/Home/Login");
        }
        /// <summary>
        /// 账号是否存在
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 主动注册客户端
        /// </summary>
        /// <param name="name"></param>
        /// <param name="companyName"></param>
        /// <param name="loginName"></param>
        /// <param name="loginPWD"></param>
        /// <returns></returns>
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
                    Clients client = new Clients() { CompanyName = companyName, ContactName = name, MobilePhone = loginName };
                    ClientBusiness.InsertClient(client, "", loginName, loginPWD, string.Empty, out result);

                    if (result == 1)
                    {
                        string operateip = Common.Common.GetRequestIP();
                        int outResult;
                        IntFactoryEntity.Users user = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(loginName, loginPWD, out outResult, operateip);
                        if (user != null){
                            Session["ClientManager"] = user;
                        }

                        Common.Common.ClearMobilePhoneCode(loginName);
                    }
                    else
                        result = 0;
                }
            }

            JsonDictionary.Add("Result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <returns></returns>
        public JsonResult SendMobileMessage(string mobilePhone)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            Random rd = new Random();
            int code=rd.Next(100000, 1000000);

            bool flag = Common.MessageSend.SendMessage(mobilePhone, code);
            JsonDictionary.Add("Result",flag?1:0);

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

        /// <summary>
        /// 验证手机验证码
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
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
            int remainderDays = 0;
            int authorizeType = 0;

            if (Session["ClientManager"] != null)
            {
                var CurrentUser = (IntFactoryEntity.Users)Session["ClientManager"];
                var agent = AgentsBusiness.GetAgentDetail(CurrentUser.AgentID);

                remainderDays = (agent.EndTime - DateTime.Now).Days;
                authorizeType = agent.AuthorizeType;

            }

            JsonDictionary.Add("remainderDays", remainderDays);
            JsonDictionary.Add("authorizeType", authorizeType);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        public JsonResult GetChildOrderCategorysByID(string categoryid, string clientid)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            var list = new ProductsBusiness().GetClientCategorysByPID(categoryid, EnumCategoryType.Order, clientid);
            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrder(string entity)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            IntFactoryEntity.OrderEntity model = serializer.Deserialize<IntFactoryEntity.OrderEntity>(entity);

            string orderid = OrdersBusiness.BaseBusiness.CreateOrder(model.CustomerID, model.GoodsCode, model.Title, model.PersonName, model.MobileTele, EnumOrderSourceType.SelfOrder,
                                                                    (EnumOrderType)model.OrderType, model.BigCategoryID, model.CategoryID, model.PlanPrice, model.PlanQuantity, model.PlanTime,
                                                                     model.OrderImage, model.CityCode, model.Address, model.ExpressCode, model.Remark, "", model.AgentID, model.ClientID);
            JsonDictionary.Add("id", orderid);
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

        public JsonResult GetOrdersByPlanTime(string userID)
        {
            Dictionary<string, Object> resultObj = new Dictionary<string, object>();
            int result = 0;
            if (Session["ClientManager"] != null)
            {
                var currentUser = (IntFactoryEntity.Users)Session["ClientManager"];
                var nowDate=DateTime.Now;
                var list= IntFactoryBusiness.OrdersBusiness.BaseBusiness.GetOrdersByPlanTime(nowDate.Date.ToString(), 
                    nowDate.Date.AddDays(14).ToString(), string.Empty, currentUser.ClientID);

                var totalExceedCount = 0;
                var totalFinishCount = 0;
                var totalWarnCount = 0;
                var totalWorkCount = 0;
                var totalSumCount = 0;
                var reportArr =new  List<Dictionary<string, Object>>();
                for (var i = 0; i < 15; i++) {
                    var report = new Dictionary<string, Object>();
                    var nextDate = nowDate.AddDays(i);
                    var orderList = list.FindAll(m => m.PlanTime.Date == nextDate.Date);
                    
                    var exceedCount = 0;
                    var finishCount = 0;
                    var warnCount = 0;
                    var workCount = 0;
                    var totalCount = 0;
                    if (i > 0)
                    {
                        nextDate = nextDate.Date;
                    }
                    finishCount = orderList.FindAll(m => m.EndTime.ToString("yyyy-MM-dd") != "0001-01-01").Count;
                    exceedCount = orderList.FindAll(m => m.PlanTime < nextDate && m.EndTime.ToString("yyyy-MM-dd") == "0001-01-01").Count;
                    for (var j = 0; j < orderList.Count; j++) { 
                        var order=orderList[j];
                        if (order.PlanTime > nowDate && order.EndTime.ToString("yyyy-MM-dd") == "0001-01-01")
                        {
                            if ((order.PlanTime - nowDate).TotalHours * 3 < (order.PlanTime - order.OrderTime).TotalHours)
                            {
                                warnCount++;
                            }
                            else
                            {
                                workCount++;
                            }
                        }
                    }
                    report.Add("date", nextDate.Date.ToString("MM.dd"));
                    report.Add("warnCount", warnCount);
                    report.Add("finishCount", finishCount);
                    report.Add("workCount", workCount);
                    report.Add("exceedCount", exceedCount);
                    totalCount=warnCount + finishCount + workCount + exceedCount;
                    report.Add("totalCount", totalCount);
                    if (totalCount > 0)
                    {
                        totalExceedCount += exceedCount;
                        totalFinishCount += finishCount;
                        totalWarnCount += warnCount;
                        totalWorkCount += workCount;
                        totalSumCount += totalCount;
                        reportArr.Add(report);
                    }
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


    }
}
