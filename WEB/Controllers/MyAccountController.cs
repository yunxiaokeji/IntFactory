using IntFactoryBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using IntFactoryEntity.Manage;
using IntFactoryBusiness.Manage;
using Qiniu.Auth;
using Qiniu.IO;
using Qiniu.IO.Resumable;
using Qiniu.RS;
using Qiniu.RPC;
using IntFactoryEnum;
using Qiniu.Conf;
namespace YXERP.Controllers
{
    
    public class MyAccountController :BaseController
    {
        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public static string FilePath = CloudSalesTool.AppSettings.Settings["UploadFilePath"];
        public static string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];
        String Bucket = System.Configuration.ConfigurationManager.AppSettings["QN-Bucket"] ?? "zngc-intfactory"; 

        #region view

        public ActionResult Index(string id)
        {
            ViewBag.Departments = OrganizationBusiness.GetDepartments(CurrentUser.ClientID);

            return View();
        }

        public ActionResult SetAvatar()
        {
            return View();
        }
        public ActionResult SetPassWord()
        {
            return View();
        }
        public ActionResult Account()
        {
            var BindMobilePhone = string.Empty;
            var UserAccounts = OrganizationBusiness.GetUserAccountsByUserID(CurrentUser.UserID, CurrentUser.ClientID);
            var UserAccount = UserAccounts.Find(m => m.AccountType ==(int) EnumAccountType.Mobile);
            if (UserAccount != null)
            {
                BindMobilePhone = UserAccount.AccountName;
            }
            ViewBag.BindMobilePhone = BindMobilePhone;

            var WeiXinID = string.Empty;
            UserAccount = UserAccounts.Find(m => m.AccountType == (int)EnumAccountType.WeiXin);
            if (UserAccount != null)
            {
                WeiXinID = UserAccount.AccountName;
            }
            ViewBag.WeiXinID = WeiXinID;

            return View();
        }
        public ActionResult FeedBackList()
        {
            return View();
        }

        public ActionResult WXLogin()
        {
            ViewBag.redirect_uri = System.Configuration.ConfigurationManager.AppSettings["WeiXinLoginCallBack"] ?? "http://localhost:9999/Home/WeiXinLoginCallBack"; 
            return View();
        }

        public JsonResult GetFeedBacks(string keyWords, string beginDate, string endDate, int type, int status, int pageSize, int pageIndex)
        {
            int totalCount = 0;
            int pageCount = 0;
            List<FeedBack> feedBacks = FeedBackBusiness.GetFeedBacks(keyWords, CurrentUser.UserID, beginDate, endDate, type, status, pageSize, pageIndex, out totalCount, out pageCount);
            JsonDictionary.Add("items", feedBacks);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region ajax

        #region 基本信息
        /// <summary>
        /// 获取当前用户详情
        /// </summary>
        public JsonResult GetAccountDetail() 
        {
            JsonDictionary.Add("LoginName", CurrentUser.LoginName);
            JsonDictionary.Add("Name", CurrentUser.Name);
            JsonDictionary.Add("Jobs", CurrentUser.Jobs);
            JsonDictionary.Add("Birthday", CurrentUser.Birthday);
            JsonDictionary.Add("Age", CurrentUser.Age);
            JsonDictionary.Add("DepartID", CurrentUser.DepartID);
            JsonDictionary.Add("DepartmentName", CurrentUser.Department != null ? CurrentUser.Department.Name : string.Empty);
            JsonDictionary.Add("MobilePhone", CurrentUser.MobilePhone);
            JsonDictionary.Add("OfficePhone", CurrentUser.OfficePhone);
            JsonDictionary.Add("Email", CurrentUser.Email);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存用户基本信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveAccountInfo(string entity, string departmentName)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            IntFactoryEntity.Users model = serializer.Deserialize<IntFactoryEntity.Users>(entity);

            bool flag = OrganizationBusiness.UpdateUserInfo(CurrentUser.UserID, model.Name, model.Jobs, model.Birthday, 0, model.DepartID, model.Email, model.MobilePhone, model.OfficePhone, CurrentUser.ClientID);
            JsonDictionary.Add("result", flag?1:0);

            if (flag)
            {
                CurrentUser.Name = model.Name;
                CurrentUser.Jobs = model.Jobs;
                CurrentUser.Birthday = model.Birthday;
                CurrentUser.Age = model.Age;
                if (CurrentUser.DepartID != model.DepartID)
                {
                    CurrentUser.DepartID = model.DepartID;
                    CurrentUser.Department = OrganizationBusiness.GetDepartmentByID(model.DepartID, CurrentUser.ClientID);
                }
                CurrentUser.Email = model.Email;
                CurrentUser.MobilePhone = model.MobilePhone;
                CurrentUser.OfficePhone = model.OfficePhone;
                Session["ClientManager"] = CurrentUser;
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region 设置头像
        /// <summary>
        /// 设置用户头像
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public JsonResult SaveAccountAvatar(string avatar)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(avatar))
            {
                avatar = avatar.Split(',')[1];             
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(avatar));
                Bitmap img = new Bitmap(stream);

                var id = CurrentUser.UserID;
                string localFile = TempPath + id + ".png";
                img.Save(Server.MapPath(localFile));

                string key = TempPath + "User/" + id + ".png";
                UploadAttachment(key, localFile);
                avatar = (Bucket == "zngc-intfactory" ? "http://o9h6bx3r4.bkt.clouddn.com/" : "http://o9vwxv40j.bkt.clouddn.com/") + key;
                bool flag = OrganizationBusiness.UpdateAccountAvatar(CurrentUser.UserID, avatar, CurrentUser.ClientID);
                if (flag)
                {
                    result = 1;
                    CurrentUser.Avatar = avatar;
                    Session["ClientManager"] = CurrentUser;
                }
            }
            JsonDictionary.Add("result",result);
            JsonDictionary.Add("avatar", avatar);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public bool UploadAttachment(string key,string localFile)
        {
            Config.Init();
            //设置上传的空间

            IOClient target = new IOClient();
            PutExtra extra = new PutExtra();
            //设置上传的空间
            String bucket = Bucket;

            //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
            PutPolicy put = new PutPolicy(bucket + ":" + key, 3600);

            //调用Token()方法生成上传的Token
            string upToken = put.Token();

            localFile = Server.MapPath(localFile);
            //调用PutFile()方法上传
            PutRet ret = target.PutFile(upToken, key, localFile, extra);
            return ret.OK;
        }
        #endregion

        #region 账户管理
        /// <summary>
        /// 账号是否存在
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 验证账号密码是否正确
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public JsonResult ConfirmLoginPwd(string loginName, string loginPwd)
        {
            if (string.IsNullOrEmpty(loginName)) {
                loginName = CurrentUser.LoginName;
            }

            bool bl = OrganizationBusiness.ConfirmLoginPwd(CurrentUser.UserID, loginName, loginPwd);
            JsonDictionary.Add("result", bl);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        ///编辑用户账户
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public JsonResult UpdateUserAccount(string loginName, string loginPwd)
        {
            bool bl = OrganizationBusiness.UpdateUserAccount(CurrentUser.UserID, loginName, loginPwd,  CurrentUser.ClientID);
            JsonDictionary.Add("result", bl);

            if (bl) {
                CurrentUser.LoginName = loginName;
                Session["ClientManager"] = CurrentUser;
            }

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateUserPass(string loginPwd)
        {
            bool bl = OrganizationBusiness.UpdateUserPass(CurrentUser.UserID, loginPwd);
            JsonDictionary.Add("result", bl);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UnBindWeiXin()
        {
            bool bl = OrganizationBusiness.UnBindOtherAccount(IntFactoryEnum.EnumAccountType.WeiXin, CurrentUser.UserID, "", CurrentUser.ClientID);
            JsonDictionary.Add("result", bl);
            if (bl) 
            {
                Session["ClientManager"] = CurrentUser;
            }

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region 绑定手机
        public JsonResult SaveAccountBindMobile(string bindMobile, int option,string code)
        {
            bool flag = false;
            int result = 0;
            if (!string.IsNullOrEmpty( CurrentUser.LoginName) )
            {
                bool bl = Common.Common.ValidateMobilePhoneCode(bindMobile, code);
                if (!bl)
                {
                    result = 2;
                }
                else
                {
                    if (option == 1)
                    {
                        flag = OrganizationBusiness.UpdateAccountBindMobile(bindMobile, "", false, CurrentUser.UserID, CurrentUser.ClientID);
                    }
                    else
                    {
                        flag = OrganizationBusiness.ClearAccountBindMobile(CurrentUser.UserID);
                    }

                    if (flag)
                    {
                        Session["ClientManager"] = CurrentUser;
                        Common.Common.ClearMobilePhoneCode(bindMobile);
                        result = 1;
                    }
                }
            }
            else 
            {
                result = 3;
            }
            

            JsonDictionary.Add("result",result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion


        #endregion

    }
}
