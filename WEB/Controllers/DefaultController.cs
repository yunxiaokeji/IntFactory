using IntFactoryBusiness;
using IntFactoryBusiness.Manage;
using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    public class DefaultController : BaseController
    {
        //
        // GET: /Default/

        public ActionResult Index()
        {
            if (CurrentUser.Client.GuideStep == 1)
            {
                return Redirect("/Default/SetProcess");
            }
            else if (CurrentUser.Client.GuideStep == 2)
            {
                return Redirect("/Default/SetCategory");
            }
            else if (CurrentUser.Client.GuideStep == 3)
            {
                return Redirect("/Default/BindMobile");
            }
            return Redirect("/Home/Index");
        }

        public ActionResult SetProcess()
        {
            if (CurrentUser.Client.GuideStep != 1)
            {
                return Redirect("/Default/Index");
            }
            
            ViewBag.Categorys = SystemBusiness.BaseBusiness.GetProcessCategorys();

            var mobile = OrganizationBusiness.IsExistAccountType(EnumAccountType.Mobile, CurrentUser.UserID);
            ViewBag.IsExists = mobile ? 1 : 0;
            return View();
        }

        public ActionResult SetCategory()
        {
            if (CurrentUser.Client.GuideStep != 2)
            {
                return Redirect("/Default/Index");
            }
            var list = new ProductsBusiness().GetChildCategorysByID("", IntFactoryEnum.EnumCategoryType.Order);
            foreach (var item in list)
            {
                if (item.ChildCategory == null || item.ChildCategory.Count == 0)
                {
                    item.ChildCategory = new ProductsBusiness().GetChildCategorysByID(item.CategoryID, IntFactoryEnum.EnumCategoryType.Order);
                }
            }
            ViewBag.Items = list;
            return View();
        }

        public ActionResult BindMobile()
        {
            if (CurrentUser.Client.GuideStep == 3)
            {
                if (OrganizationBusiness.IsExistAccountType(EnumAccountType.Mobile, CurrentUser.UserID))
                {
                    bool bl = ClientBusiness.FinishInitSetting(CurrentUser.ClientID);
                    if (bl)
                    {
                        CurrentUser.Client.GuideStep = 0;
                        Session["ClientManager"] = CurrentUser;
                        return Redirect("/Default/SettingHelp");
                    }
                }

                var existName = OrganizationBusiness.IsExistAccountType(EnumAccountType.UserName, CurrentUser.UserID);
                ViewBag.IsShow = existName ? 1 : 0;
                return View();
            }
            else
            {
                return Redirect("/Default/Index");
            }               
        }

        public ActionResult SettingHelp()
        {
            if (CurrentUser.Client.GuideStep != 0)
            {
                return Redirect("/Default/Index");
            }
            return View();
        }


        #region Ajax

        public JsonResult FinishInitSetting()
        {
            bool bl = ClientBusiness.FinishInitSetting(CurrentUser.ClientID);
            JsonDictionary.Add("result", bl);
            if (bl) 
            {
                CurrentUser.Client.GuideStep = 0;
                Session["ClientManager"] = CurrentUser;
            }

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AccountBindMobile(string BindMobile,string pwd)
        {
            bool bl = OrganizationBusiness.UpdateAccountBindMobile(BindMobile, pwd, true, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("result", bl);
            if (bl)
            {
                CurrentUser.MobilePhone = BindMobile;
                CurrentUser.Client.GuideStep = 0;
                Session["ClientManager"] = CurrentUser;
                Common.Common.ClearMobilePhoneCode(BindMobile);
            }

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SetClientProcess(string ids)
        {
            int guideStep = ClientBusiness.SetClientProcess(ids, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("value", guideStep);
            CurrentUser.Client.GuideStep = guideStep;
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SetClientCategory(string ids)
        {
            int guideStep = ClientBusiness.SetClientCategory(ids, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("value", guideStep);
            CurrentUser.Client.GuideStep = guideStep;
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult IsExistLoginName(string loginName)
        {
            bool bl = OrganizationBusiness.IsExistLoginName(loginName);
            JsonDictionary.Add("result", bl);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
