using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness.Manage;
using System.Web.Script.Serialization;
using IntFactoryEntity;
using IntFactoryEntity.Manage;
using IntFactoryBusiness;
namespace YXManage.Controllers
{
     [YXManage.Common.UserAuthorize]
    public class SystemController : BaseController
    {
        #region view
         public ActionResult ClearCache()
         {
             return View();
         }

         public ActionResult Admin()
        {
            return View();
        }

         public ActionResult Users()
         {
             return View();
         }

         public ActionResult Menu()
         {
             return View();
         }

         public ActionResult MenuDetail(string id)
         {
             var model = new SystemMenu();
             var menuCode = string.Empty;
             var pCodeName=string.Empty;

             if (!string.IsNullOrEmpty(id))
             {
                 model = ManageSystemBusiness.GetSystemMenu(id);
                 menuCode = model.MenuCode;
                 pCodeName = model.PCodeName;
             }
             ViewBag.Model = model;
             ViewBag.MenuCode = menuCode;
             ViewBag.PCodeName = pCodeName;
             ViewBag.Layer = 1;
             ViewBag.PCode = string.Empty;

             return View();
         }

         public ActionResult AddSubMenu(string id)
         {
             var model = new SystemMenu();
             var menuCode = string.Empty;
             var pCode = string.Empty;
             var pCodeName = string.Empty;
             var layer = 0;

             model = ManageSystemBusiness.GetSystemMenu(id);
             pCode = model.MenuCode;
             pCodeName = model.Name;
             layer = model.Layer+1;

             ViewBag.PCode = pCode;
             ViewBag.PCodeName = pCodeName;
             ViewBag.Layer = layer;
             ViewBag.Model = new SystemMenu();
             ViewBag.MenuCode = menuCode;
             return View("MenuDetail");
         }

         public ActionResult Roles()
         {
             return View();
         }

         public ActionResult RolePermission(string id)
         {
             ViewBag.Model = ManageSystemBusiness.GetRoleByID(id);
             ViewBag.Menus = CommonBusiness.ManageMenus.Where(m => m.PCode == ExpandClass.CLIENT_TOP_CODE).ToList();
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


        public JsonResult GetSystemMenus()
        {
            var list = ManageSystemBusiness.GetSystemMenus();

            JsonDictionary.Add("Items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetSystemMenu(string menuCode)
        {
            var item = ManageSystemBusiness.GetSystemMenu(menuCode);
            JsonDictionary.Add("Item", item);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet 
            };
        }

        public JsonResult SaveSystemMenu(string menu, string menucode)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            SystemMenu model = serializer.Deserialize<SystemMenu>(menu);
            bool flag = false;

            if (!string.IsNullOrEmpty(model.MenuCode))
            {
                flag = ManageSystemBusiness.UpdateSystemMenu(model);
            }
            else
            {
                if (string.IsNullOrEmpty(model.PCode))
                    model.PCode = "100000000";
                model.MenuCode = menucode;
                model.Type = 2;
                flag = ManageSystemBusiness.AddSystemMenu(model);
            }

            JsonDictionary.Add("Result",flag?1:0);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteSystemMenu(string menuCode)
        {
            bool flag = false;

            flag = ManageSystemBusiness.DeleteSystemMenu(menuCode);


            JsonDictionary.Add("Result", flag ? 1 : 0);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRoles()
        {
            var list = ManageSystemBusiness.GetRoles();
            JsonDictionary.Add("items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetRoleByID(string id)
        {
            var model = ManageSystemBusiness.GetRoleByID(id);
            JsonDictionary.Add("model", model);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveRole(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Role model = serializer.Deserialize<Role>(entity);

            if (string.IsNullOrEmpty(model.RoleID))
            {
                model.RoleID = new ManageSystemBusiness().CreateRole(model.Name, model.ParentID, model.Description, string.Empty);
            }
            else
            {
                bool bl = new ManageSystemBusiness().UpdateRole(model.RoleID, model.Name, model.Description,string.Empty);
                if (!bl)
                {
                    model.RoleID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public JsonResult DeleteRole(string roleid)
        {
            int result = 0;
            bool bl = new ManageSystemBusiness().DeleteRole(roleid, CurrentUser.UserID, OperateIP, out result);
            JsonDictionary.Add("status", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public JsonResult SaveRolePermission(string roleid, string permissions)
        {
            if (permissions.Length > 0)
            {
                permissions = permissions.Substring(0, permissions.Length - 1);

            }
            bool bl = new ManageSystemBusiness().UpdateRolePermission(roleid, permissions, CurrentUser.UserID, OperateIP);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUsers(string keyWords,int pageIndex)
        {
            int totalCount = 0, pageCount = 0;
            var list =M_UsersBusiness.GetUsers(keyWords,string.Empty, PageSize, pageIndex, ref totalCount, ref pageCount);

            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserDetail(string id)
        {
            var item = M_UsersBusiness.GetUserDetail(id);

            JsonDictionary.Add("Item", item);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

         /// <summary>
         /// 清理系统缓存
         /// </summary>
        public JsonResult ClearSystemCache(int type)
        {
            string path = string.Empty;
            switch (type) {

                case 1: {
                    path = "/Api/Cache/ClearCategoryCache";
                    break;
                }
                case 2:
                {
                    path = "/Api/Cache/ClearAttrsCache";
                    break;
                }
                case 3:
                {
                    path = "/Api/Cache/ClearUnitCache";
                    break;
                }
            }

            string resultStr= YXManage.Common.Common.RequestServer(path);

            JsonDictionary.Add("result",!string.IsNullOrEmpty(resultStr)? 1:0);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
