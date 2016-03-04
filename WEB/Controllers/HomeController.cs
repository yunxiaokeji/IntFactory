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

            Users CurrentUser = (Users)Session["ClientManager"];
            ViewBag.UserCount = OrganizationBusiness.GetUsers(CurrentUser.AgentID).Count;
            var agent = AgentsBusiness.GetAgentDetail(CurrentUser.AgentID);
            ViewBag.RemainderDays = (agent.EndTime - DateTime.Now).Days;
            ViewBag.UserQuantity = agent.UserQuantity;
            return View();
        }

        public ActionResult Register(string code)
        {
            ViewBag.Msg = "";
            //if (!string.IsNullOrEmpty(code))
            //    ViewBag.Msg = AlibabaSdk.Business.OauthBusiness.GetUserInfo(code);

            return View();
        }

        public ActionResult FindPassword()
        {
            return View();
        }

        public ActionResult Login(string ReturnUrl, int Status = 0)
        {
            //return Redirect(AlibabaSdk.Business.OauthBusiness.GetAuthorizeUrl());

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
            var list = new ProductsBusiness().GetChildCategorysByID("", EnumCategoryType.Order);
            ViewBag.Items = list;
            return View();
        }


        public JsonResult GetAgentActions()
        {
            IntFactoryEntity.Users CurrentUser = (IntFactoryEntity.Users)Session["ClientManager"];
            var model = LogBusiness.BaseBusiness.GetAgentActions(CurrentUser.AgentID);

            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JsonDictionary.Add("model", model);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 明道登录
        /// </summary>
        /// <returns></returns>
        public ActionResult MDLogin(string ReturnUrl)
        {
            if(string.IsNullOrEmpty(ReturnUrl))
            return Redirect(AlibabaSdk.Business.OauthBusiness.GetAuthorizeUrl());
            else
                return Redirect(AlibabaSdk.Business.OauthBusiness.GetAuthorizeUrl() + "&state=" + ReturnUrl);
        }

        /// <summary>
        /// 员工登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonResult UserLogin(string userName, string pwd, string remember)
        {
            int result = 0;
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            YXERP.Common.PwdErrorUserEntity pwdErrorUser = null;

            if (Common.Common.CachePwdErrorUsers.ContainsKey(userName)) pwdErrorUser = Common.Common.CachePwdErrorUsers[userName];

            if (pwdErrorUser == null || (pwdErrorUser.ErrorCount < 3 && pwdErrorUser.ForbidTime<DateTime.Now) )
            {
                string operateip = string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];
                int outResult;
                IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, out outResult, operateip);
                if (model != null)
                {
                    //保持登录状态
                    HttpCookie cook = new HttpCookie("cloudsales");
                    cook["username"] = userName;
                    cook["pwd"] = pwd;
                    cook["status"] = remember;
                    cook.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cook);

                    Session["ClientManager"] = model;
                    Common.Common.CachePwdErrorUsers.Remove(userName);
                    result = 1;
                }
                else
                {
                    if (outResult == 3)
                    {
                        if (pwdErrorUser == null)
                            pwdErrorUser = new Common.PwdErrorUserEntity();
                        else
                        {
                            if (pwdErrorUser.ErrorCount > 2)
                                pwdErrorUser.ErrorCount = 0;
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

                    if(bl)
                        Common.Common.ClearMobilePhoneCode(loginName);
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

        public JsonResult GetAgentInfo()
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
            var list = new ProductsBusiness().GetChildCategorysByID(categoryid, EnumCategoryType.Order);
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

            string orderid = OrdersBusiness.BaseBusiness.CreateOrder(model.CustomerID, model.GoodsCode, model.Title, model.PersonName, model.MobileTele, model.OrderType, model.BigCategoryID, model.CategoryID, model.PlanPrice, model.PlanQuantity,
                                                                     model.OrderImage, model.CityCode, model.Address, model.Remark, "", model.AgentID, model.ClientID);
            JsonDictionary.Add("id", orderid);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
