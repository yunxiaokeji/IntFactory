using IntFactoryBusiness;
using IntFactoryEntity;
using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXERP.Models;

namespace YXERP.Controllers
{
    public class OrganizationController : BaseController
    {
        //
        // GET: /Organization/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Department()
        {
            return View();
        }

        public ActionResult Roles()
        {
            return View();
        }

        public ActionResult RolePermission(string id)
        {
            ViewBag.Model = OrganizationBusiness.GetRoleByID(id, CurrentUser.ClientID);
            ViewBag.Menus = CommonBusiness.ClientMenus.Where(m => m.PCode == ExpandClass.CLIENT_TOP_CODE).ToList();
            return View();
        }

        public ActionResult Users()
        {
            ViewBag.MDToken = string.Empty;
            ViewBag.Roles = OrganizationBusiness.GetRoles(CurrentUser.ClientID);
            ViewBag.Departments = OrganizationBusiness.GetDepartments(CurrentUser.ClientID);

            ViewBag.IsSysAdmin = CurrentUser.Role.IsDefault == 1 ? true : false;
            
            return View();
        }

        public ActionResult CreateUser()
        {
            ViewBag.Roles = OrganizationBusiness.GetRoles(CurrentUser.ClientID);
            ViewBag.Departments = OrganizationBusiness.GetDepartments(CurrentUser.ClientID);
            return View();
        }

        public ActionResult Structure()
        {
            var list = OrganizationBusiness.GetStructureByParentID("6666666666", CurrentUser.ClientID);
            if (list.Count > 0)
            {
                ViewBag.Model = list[0];
            }
            else
            {
                ViewBag.Model = new Users();
            }
            return View();
        }

        #region Ajax

        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDepartments()
        {
            var list = OrganizationBusiness.GetDepartments(CurrentUser.ClientID);
            foreach (var item in list)
            {
                if (item.CreateUser == null && !string.IsNullOrEmpty(item.CreateUserID))
                {
                    var user = OrganizationBusiness.GetUserByUserID(item.CreateUserID, CurrentUser.ClientID);
                    item.CreateUser = new Users() { Name = user.Name };
                }
            }
            JsonDictionary.Add("items", list);
            return new JsonResult() 
            {
                Data = JsonDictionary,
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetDepartmentByID(string id)
        {
            var model = OrganizationBusiness.GetDepartmentByID(id, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveDepartment(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Department model = serializer.Deserialize<Department>(entity);

            if (string.IsNullOrEmpty(model.DepartID))
            {
                model.DepartID = new OrganizationBusiness().CreateDepartment(model.Name, model.ParentID, model.Description, CurrentUser.UserID,  CurrentUser.ClientID);
            }
            else
            {
                bool bl = new OrganizationBusiness().UpdateDepartment(model.DepartID, model.Name, model.Description, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
                if (!bl)
                {
                    model.DepartID = "";
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
        /// 删除部门
        /// </summary>
        /// <param name="departid"></param>
        /// <returns></returns>
        public JsonResult DeleteDepartment(string departid)
        {
            var status = new OrganizationBusiness().UpdateDepartmentStatus(departid, IntFactoryEnum.EnumStatus.Delete, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", (int)status);
            return new JsonResult
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
            var list = OrganizationBusiness.GetRoles(CurrentUser.ClientID);
            foreach (var item in list)
            {
                if (item.CreateUser == null && !string.IsNullOrEmpty(item.CreateUserID))
                {
                    var user = OrganizationBusiness.GetUserByUserID(item.CreateUserID, CurrentUser.ClientID);
                    item.CreateUser = new Users() { Name = user.Name };
                }
            }
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetRoleByID(string id)
        {
            var model = OrganizationBusiness.GetRoleByID(id, CurrentUser.ClientID);
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
                model.RoleID = new OrganizationBusiness().CreateRole(model.Name, model.ParentID, model.Description, CurrentUser.UserID,CurrentUser.ClientID);
            }
            else
            {
                bool bl = new OrganizationBusiness().UpdateRole(model.RoleID, model.Name, model.Description, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
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
            bool bl = new OrganizationBusiness().DeleteRole(roleid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public JsonResult UpdateUserPwd(string userID,string loginPwd) 
        {
            bool bl = OrganizationBusiness.UpdateUserPass(userID, loginPwd);
            JsonDictionary.Add("status",bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 重置手机号
        /// </summary>
        /// <param name="bindMobile"></param>
        /// <param name="option"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public JsonResult UpdateMobilePhone(string userID)
        {
            bool flag = false;
            int  result = 0;
            if (OrganizationBusiness.IsExistAccountType(EnumAccountType.UserName, userID))
            {
                Users item = IntFactoryBusiness.OrganizationBusiness.GetUserByUserID(userID, CurrentUser.ClientID);
                if (OrganizationBusiness.IsExistAccountType(EnumAccountType.Mobile, CurrentUser.UserID))
                {
                    flag = OrganizationBusiness.ClearAccountBindMobile(userID);
                    if (flag)
                    {
                        result = 1;
                        if (!string.IsNullOrEmpty(item.MobilePhone))
                        {
                            result = 2;
                        }
                    }
                }
                else 
                {
                    result = 3;
                }
            }

            JsonDictionary.Add("result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 修改员工基本信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public JsonResult UpdateUserBaseInfo(string entity, string userID)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(userID))
            {
                bool flag = false;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                IntFactoryEntity.Users newItem = serializer.Deserialize<IntFactoryEntity.Users>(entity);
                IntFactoryEntity.Users item = OrganizationBusiness.GetUserByUserID(userID, CurrentUser.ClientID);
                flag = OrganizationBusiness.UpdateUserInfo(userID, newItem.Name, item.Jobs, item.Birthday, item.Age.Value, newItem.DepartID,
                                                            newItem.Email, newItem.MobilePhone, item.OfficePhone, CurrentUser.ClientID);
                result = flag ? 1 : 0;
            }
            JsonDictionary.Add("result", result);
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
            bool bl = new OrganizationBusiness().UpdateRolePermission(roleid, permissions, CurrentUser.UserID, OperateIP);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveUser(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Users model = serializer.Deserialize<Users>(entity);

            int result = 0;

            var user = OrganizationBusiness.CreateUser(EnumAccountType.UserName, model.LoginName, model.LoginPWD, model.Name, model.MobilePhone, model.Email, model.CityCode, model.Address, model.Jobs, model.RoleID, model.DepartID, "",
               CurrentUser.ClientID, CurrentUser.UserID, out result);

            JsonDictionary.Add("model", user);
            JsonDictionary.Add("result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult IsExistLoginName(string loginname)
        {
            var bl = OrganizationBusiness.IsExistLoginName(loginname);
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 根据上级获取用户列表
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public JsonResult GetUsersByParentID(string parentid = "")
        {
            var list = OrganizationBusiness.GetUsersByParentID(parentid, CurrentUser.ClientID).OrderBy(m => m.FirstName).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUsers(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterUser model = serializer.Deserialize<FilterUser>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = OrganizationBusiness.GetUsers(model.Keywords, model.DepartID, model.RoleID, CurrentUser.ClientID, PageSize, model.PageIndex, ref totalCount, ref pageCount);

            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserAll()
        {
            var list = OrganizationBusiness.GetUsers(CurrentUser.ClientID).Where(m => m.Status == 1).OrderBy(m => m.FirstName).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserNoTeam() 
        {
            var list = OrganizationBusiness.GetUsers(CurrentUser.ClientID).Where(m => m.TeamID == "");
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        /// <summary>
        /// 编辑组织架构上级
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public JsonResult UpdateUserParentID(string ids, string parentid)
        {
            bool bl = false;//
            string[] list = ids.Split(',');
            foreach (var userid in list)
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    if (new OrganizationBusiness().UpdateUserParentID(userid, parentid, CurrentUser.ClientID, CurrentUser.UserID, OperateIP))
                    {
                        bl = true;
                    }
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ClearUserParentID(string ids, string parentid)
        {
            bool bl = false;//
            string[] list = ids.Split(',');
            foreach (var userid in list)
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    if (new OrganizationBusiness().UpdateUserParentID(userid, parentid, CurrentUser.ClientID, CurrentUser.UserID, OperateIP))
                    {
                        bl = true;
                    }
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 组织架构替换人员
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="olduserid"></param>
        /// <returns></returns>
        public JsonResult ChangeUsersParentID(string userid, string olduserid)
        {

            bool bl = new OrganizationBusiness().ChangeUsersParentID(userid, olduserid, CurrentUser.ClientID, CurrentUser.UserID, OperateIP);
           
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public JsonResult DeleteUserByID(string userid)
        {
            int result = 0;
            bool bl = new OrganizationBusiness().DeleteUserByID(userid, CurrentUser.ClientID, CurrentUser.UserID, OperateIP, out result);
            
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //编辑员工角色
        public JsonResult UpdateUserRole(string userid,string roleid)
        {
            bool bl = new OrganizationBusiness().UpdateUserRole(userid, roleid, CurrentUser.ClientID, CurrentUser.UserID, OperateIP);

            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
