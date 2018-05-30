using IntFactoryBusiness;
using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IntfactoryH5.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return Redirect("/Home/Login");
        }

        public ActionResult Login(string ReturnUrl)
        {
            if (Session["ClientManager"] != null)
            {
                return Redirect("/Task/List");
            }
            else
            {
                HttpCookie userinfo = Request.Cookies["m_intfactory_userinfo"];
                if (userinfo != null)
                {
                    string operateip = GetRequestIP();
                    int result;
                    IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(userinfo["username"], userinfo["pwd"], out result, operateip);
                    if (model != null)
                    {
                        Session["ClientManager"] = model;
                        return Redirect("/Task/List");
                    }
                }
            }
            ReturnUrl = ReturnUrl ?? string.Empty;
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        public ActionResult Logout()
        {
            Session["ClientManager"] = null;
            HttpCookie mycookie = Request.Cookies["m_intfactory_userinfo"];
            if (mycookie != null)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, 0); //时间跨度
                mycookie.Expires = DateTime.Now.Add(ts); //立即过期
                Response.Cookies.Remove("m_intfactory_userinfo");//清除
                Response.Cookies.Add(mycookie); //写入立即过期的*/
                Response.Cookies["m_intfactory_userinfo"].Expires = DateTime.Now.AddDays(-1);
            }

            return Redirect("/Home/Login");
        }

        //跳转微信公众号授权
        public ActionResult WeiXinMPLogin(string returnUrl)
        {
            string url = "/Task/List";
            if (Session["ClientManager"] == null)
            {
                HttpCookie userinfo = Request.Cookies["m_intfactory_userinfo"];
                if (userinfo != null)
                {
                    string operateip = GetRequestIP();
                    int result;
                    IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(userinfo["username"], userinfo["pwd"], out result, operateip);
                    if (model != null)
                    {
                        Session["ClientManager"] = model;
                        return Redirect("/Task/List");
                    }
                }
                else
                {
                    return Redirect(WeiXin.Sdk.Token.GetMPAuthorizeUrl(returnUrl));
                }
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                url = returnUrl;
            }

            return Redirect(url);
        }

        //微信公众号授权回调地址
        public ActionResult WeiXinMPCallBack(string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var userToken = WeiXin.Sdk.Token.GetAccessToken(code);
                if (string.IsNullOrEmpty(userToken.errcode))
                {
                    string operateip = GetRequestIP();
                    var model = OrganizationBusiness.GetUserByOtherAccount(EnumAccountType.WeiXin, userToken.unionid, operateip);
                    if (model != null)
                    {
                        //未注销
                        if (model.Status.Value == 1)
                        {
                            Session["ClientManager"] = model;

                            if (string.IsNullOrEmpty(state))
                            {
                                return Redirect("/Task/List");
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
                        }
                    }
                }
            }
            return Redirect("/Home/Login");
        }

        #region ajax
        public JsonResult UserLogin(string userName, string pwd)
        {
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            string operateip = GetRequestIP();
            int result;
            IntFactoryEntity.Users model = IntFactoryBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, out result, operateip);
            if (model != null)
            {
                Session["ClientManager"] = model;
                //保持登录状态
                HttpCookie cook = new HttpCookie("m_intfactory_userinfo");
                cook["username"] = userName;
                cook["pwd"] = pwd;
                cook.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Add(cook);
            }

            return new JsonResult()
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}