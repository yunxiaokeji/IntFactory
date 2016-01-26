using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness.Manage;
using System.Web.Script.Serialization;
using IntFactoryEntity;
using IntFactoryEntity.Manage;
namespace YXManage.Controllers
{
     [YXManage.Common.UserAuthorize]
    public class SystemController : BaseController
    {
        #region view
         public ActionResult Admin()
        {
            return View();
        }

        #endregion

        #region ajax
        public JsonResult GetAdminDetail()
        {
            JsonDictionary.Add("LoginName", CurrentUser.LoginName);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ConfirmAdminPwd(string pwd)
        {
            IntFactoryEntity.Manage.M_Users model = IntFactoryBusiness.M_UsersBusiness.GetM_UserByUserName(CurrentUser.LoginName, pwd, string.Empty);
            JsonDictionary.Add("Result", model!=null?1:0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SetAdminAccount(string loginName, string newPwd)
        {
            bool flag = IntFactoryBusiness.M_UsersBusiness.SetAdminAccount(CurrentUser.UserID, loginName, newPwd);
            
            JsonDictionary.Add("Result", flag ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
