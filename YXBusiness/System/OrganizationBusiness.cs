using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using IntFactoryEntity;
using IntFactoryDAL;
using IntFactoryEnum;


namespace IntFactoryBusiness
{
    public class OrganizationBusiness
    {

        #region Cache

        private static Dictionary<string, List<Users>> _cacheUsers;
        private static Dictionary<string, List<Department>> _cacheDeparts;
        private static Dictionary<string, List<Role>> _cacheRoles;

        private static Dictionary<string, List<Users>> Users
        {
            get 
            {
                if (_cacheUsers == null)
                {
                    _cacheUsers = new Dictionary<string, List<Users>>();
                }
                return _cacheUsers;
            }
            set
            {
                _cacheUsers = value;
            }
        }

        private static Dictionary<string, List<Department>> Departments
        {
            get
            {
                if (_cacheDeparts == null)
                {
                    _cacheDeparts = new Dictionary<string, List<Department>>();
                }
                return _cacheDeparts;
            }
            set
            {
                _cacheDeparts = value;
            }
        }

        private static Dictionary<string, List<Role>> Roles
        {
            get
            {
                if (_cacheRoles == null)
                {
                    _cacheRoles = new Dictionary<string, List<Role>>();
                }
                return _cacheRoles;
            }
            set
            {
                _cacheRoles = value;
            }
        }

        #endregion

        #region 查询

        public static bool IsExistAccountType(EnumAccountType type, string userid)
        {
            string where = " UserID='" + userid + "' and AccountType =" + (int)type;

            object count = CommonBusiness.Select("UserAccounts", "count(0)", where);
            return Convert.ToInt32(count) > 0;
        }

        public static bool IsExistLoginName(string loginName)
        {
            if (string.IsNullOrEmpty(loginName))
            {
                return false;
            }

            object count = CommonBusiness.Select("UserAccounts", "count(0)", " AccountName='" + loginName + "' and AccountType in(1,2) ");
            return Convert.ToInt32(count) > 0;
        }

        public static bool IsExistOtherAccount(EnumAccountType type, string account, string companyid)
        {
            if (string.IsNullOrEmpty(account))
            {
                return false;
            }

            string where = " AccountName='" + account + "' and AccountType =" + (int)type;

            object count = CommonBusiness.Select("UserAccounts", "count(0)", where);
            return Convert.ToInt32(count) > 0;
        }

        public static Users GetUserByUserName(string loginname, string pwd, out int result, string operateip)
        {
            pwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd, loginname);
            DataSet ds = new OrganizationDAL().GetUserByUserName(loginname, pwd, out result);
            Users model = null;
            if (ds.Tables.Contains("User") && ds.Tables["User"].Rows.Count > 0)
            {
                model = new Users();
                model.FillData(ds.Tables["User"].Rows[0]);

                model.LogGUID = Guid.NewGuid().ToString();

                model.Department = GetDepartmentByID(model.DepartID, model.ClientID);
                model.Role = GetRoleByIDCache(model.RoleID, model.ClientID);
                
                //处理缓存
                if (!Users.ContainsKey(model.ClientID))
                {
                    GetUsers(model.ClientID);
                }
                if (Users[model.ClientID].Where(u => u.UserID == model.UserID).Count() == 0)
                {
                    Users[model.ClientID].Add(model);
                }
                else
                {
                    var user = Users[model.ClientID].Where(u => u.UserID == model.UserID).FirstOrDefault();
                    user.LogGUID = model.LogGUID;
                }

                model.Client = Manage.ClientBusiness.GetClientDetail(model.ClientID);

                //权限
                if (model.Role != null && model.Role.IsDefault == 1)
                {
                    model.Menus = CommonBusiness.ClientMenus;
                }
                else
                {
                    model.Menus = new List<Menu>();
                    foreach (DataRow dr in ds.Tables["Permission"].Rows)
                    {
                        Menu menu = new Menu();
                        menu.FillData(dr);
                        model.Menus.Add(menu);
                    }
                }
            }

            //记录登录日志
            if (model != null)
            {
                LogBusiness.AddLoginLog(loginname, true, IntFactoryEnum.EnumSystemType.Client, operateip, model.UserID, model.ClientID);
            }
            else
            {
                LogBusiness.AddLoginLog(loginname, false, IntFactoryEnum.EnumSystemType.Client, operateip, "", "");
            }

            return model;
        }

        public static Users GetUserByOtherAccount(EnumAccountType accountType, string account, string operateip)
        {
            DataSet ds = new OrganizationDAL().GetUserByOtherAccount((int)accountType, account);
            Users model = null;
            if (ds.Tables.Contains("User") && ds.Tables["User"].Rows.Count > 0)
            {
                model = new Users();
                model.FillData(ds.Tables["User"].Rows[0]);

                model.LogGUID = Guid.NewGuid().ToString();

                model.Department = GetDepartmentByID(model.DepartID, model.ClientID);
                model.Role = GetRoleByIDCache(model.RoleID, model.ClientID);
                model.Client = Manage.ClientBusiness.GetClientDetail(model.ClientID);
                //处理缓存
                if (!Users.ContainsKey(model.ClientID))
                {
                    GetUsers(model.ClientID);
                }
                if (Users[model.ClientID].Where(u => u.UserID == model.UserID).Count() == 0)
                {
                    Users[model.ClientID].Add(model);
                }
                else
                {
                    var user = Users[model.ClientID].Where(u => u.UserID == model.UserID).FirstOrDefault();
                    user.LogGUID = model.LogGUID;
                }

                //权限
                if (model.Role != null && model.Role.IsDefault == 1)
                {
                    model.Menus = CommonBusiness.ClientMenus;
                }
                else
                {
                    model.Menus = new List<Menu>();
                    foreach (DataRow dr in ds.Tables["Permission"].Rows)
                    {
                        Menu menu = new Menu();
                        menu.FillData(dr);
                        model.Menus.Add(menu);
                    }
                }
            }
            if (string.IsNullOrEmpty(operateip))
            {
                operateip = "";
            }

            //记录登录日志
            if (model != null)
            {
                LogBusiness.AddLoginLog(account, true, IntFactoryEnum.EnumSystemType.Client, operateip, model.UserID, model.ClientID);
            }
            else
            {
                LogBusiness.AddLoginLog(account, false, IntFactoryEnum.EnumSystemType.Client, operateip, "", "");
            }
            return model;
        }

        public static CacheUserEntity GetUserCacheByUserID(string userid, string clientid)
        {
            var user = GetUserByUserID(userid, clientid);
            CacheUserEntity model = new CacheUserEntity();
            model.UserID = userid;
            if (user != null && !string.IsNullOrEmpty(user.UserID))
            {
                model.Name = user.Name;
                model.Avatar = user.Avatar;
            }
            return model;
        }

        public static Users GetUserByUserID(string userid, string clientid)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(clientid))
            {
                return null;
            }
            userid = userid.ToLower();
            var list = GetUsers(clientid);
            if (list.Where(u => u.UserID == userid).Count() > 0)
            {
                return list.Where(u => u.UserID == userid).FirstOrDefault();
            }
            else
            {
                DataTable dt = new OrganizationDAL().GetUserByUserID(userid);
                Users model = new Users();
                if (dt.Rows.Count > 0)
                {
                    model.FillData(dt.Rows[0]);

                    if (clientid == model.ClientID)
                    {
                        model.Department = GetDepartmentByID(model.DepartID, clientid);
                        model.Role = GetRoleByIDCache(model.RoleID, clientid);
                        Users[clientid].Add(model);
                    }
                }
                return model;
            }
        }

        public static List<UserAccounts> GetUserAccountsByUserID(string userid, string clientid)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(clientid))
            {
                return null;
            }

            DataTable dt = new OrganizationDAL().GetUserAccountsByUserID(userid, clientid);
            List <UserAccounts > list = new List<UserAccounts>();
            UserAccounts model;
            foreach (DataRow item in dt.Rows)
            {
                model = new UserAccounts();
                model.FillData(item);

                list.Add(model);
            }

            return list;
        }

        public static bool ConfirmLoginPwd(string userid, string loginname, string pwd)
        {
            pwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd, loginname);

            object obj = CommonBusiness.Select("Users", "Count(0)", " UserID='" + userid + "' and LoginPWD='" + pwd + "' ");

            return Convert.ToInt32(obj) > 0;
        }

        public static List<Users> GetUsers(string keyWords, string departID, string roleID, string clientid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string whereSql = "ClientID='" + clientid + "' and Status<>9";

            if (!string.IsNullOrEmpty(keyWords))
                whereSql += " and ( Name like '%" + keyWords + "%' or MobilePhone like '%" + keyWords + "%' or Email like '%" + keyWords + "%')";

            if (!string.IsNullOrEmpty(departID))
                whereSql += " and DepartID='" + departID + "'";

            if (!string.IsNullOrEmpty(roleID))
                whereSql += " and RoleID='" + roleID + "'";

            DataTable dt = CommonBusiness.GetPagerData("Users", "*", whereSql, "AutoID", pageSize, pageIndex, out totalCount, out pageCount);
            List<Users> list = new List<Users>();
            Users model;
            foreach (DataRow item in dt.Rows)
            {
                model = new Users();
                model.FillData(item);

                model.CreateUser = GetUserCacheByUserID(model.CreateUserID, model.ClientID);
                model.Department = GetDepartmentByID(model.DepartID, model.ClientID);
                model.Role = GetRoleByIDCache(model.RoleID, model.ClientID);

                list.Add(model);
            }

            return list;
        }

        public static List<Users> GetUsers(string clientid)
        {
            if (string.IsNullOrEmpty(clientid))
            {
                return new List<Users>();
            }
            if (!Users.ContainsKey(clientid))
            {
                List<Users> list = new List<IntFactoryEntity.Users>();
                DataTable dt = OrganizationDAL.BaseProvider.GetUsers(clientid);
                foreach (DataRow dr in dt.Rows)
                {
                    Users model = new Users();
                    model.FillData(dr);

                    model.Department = GetDepartmentByID(model.DepartID, clientid);
                    model.Role = GetRoleByIDCache(model.RoleID, clientid);

                    list.Add(model);
                }
                Users.Add(clientid, list);
                return list;
            }
            return Users[clientid].ToList();
        }

        public static List<Users> GetUsersByParentID(string parentid, string clientid)
        {
            var users = GetUsers(clientid).Where(m => m.ParentID == parentid && m.Status == 1).ToList();
            return users;
        }

        public static List<Users> GetStructureByParentID(string parentid, string clientid)
        {
            var users = GetUsersByParentID(parentid, clientid);
            foreach (var user in users)
            {
                user.ChildUsers = GetStructureByParentID(user.UserID, clientid);
            }
            return users;
        }

        public static List<Department> GetDepartments(string clientid)
        {
            if (!Departments.ContainsKey(clientid))
            {
                DataTable dt = new OrganizationDAL().GetDepartments(clientid);
                List<Department> list = new List<Department>();
                foreach (DataRow dr in dt.Rows)
                {
                    Department model = new Department();
                    model.FillData(dr);
                    list.Add(model);
                }
                Departments.Add(clientid, list);
                return list;
            }
            return Departments[clientid].Where(m => m.Status == 1).ToList();
        }

        public static Department GetDepartmentByID(string departid, string clientid)
        {
            return GetDepartments(clientid).Where(d => d.DepartID == departid).FirstOrDefault();
        }

        public static List<Role> GetRoles(string clientid)
        {
            if (!Roles.ContainsKey(clientid))
            {
                DataTable dt = new OrganizationDAL().GetRoles(clientid);
                List<Role> list = new List<Role>();
                foreach (DataRow dr in dt.Rows)
                {
                    Role model = new Role();
                    model.FillData(dr);
                    list.Add(model);
                }
                Roles.Add(clientid, list);
                return list;
            }
            return Roles[clientid].Where(m => m.Status == 1).ToList();
        }

        public static Role GetRoleByIDCache(string roleid, string clientid)
        {
            return GetRoles(clientid).Where(r => r.RoleID == roleid).FirstOrDefault();
        }

        public static Role GetRoleByID(string roleid, string clientid)
        {
            Role model = null;
            DataSet ds = OrganizationDAL.BaseProvider.GetRoleByID(roleid);
            if (ds.Tables.Contains("Role") && ds.Tables["Role"].Rows.Count > 0)
            {
                model = new Role();
                model.FillData(ds.Tables["Role"].Rows[0]);
                model.Menus = new List<Menu>();
                foreach (DataRow dr in ds.Tables["Menus"].Rows)
                {
                    Menu menu = new Menu();
                    menu.FillData(dr);
                    model.Menus.Add(menu);
                }
            }
            return model;
        }

        #endregion

        #region 添加

        public string CreateDepartment(string name, string parentid, string description, string operateid, string clientid)
        {
            string departid = Guid.NewGuid().ToString().ToLower();
            bool bl = OrganizationDAL.BaseProvider.CreateDepartment(departid, name, parentid, description, operateid, clientid);
            if (bl)
            {
                //处理缓存
                var departs = GetDepartments(clientid);
                departs.Add(new Department()
                {
                    DepartID = departid,
                    Name = name,
                    Description = description,
                    CreateTime = DateTime.Now,
                    CreateUserID = operateid,
                    Status = 1,
                    ClientID = clientid
                });
                Departments[clientid] = departs;
                return departid;
            }
            return "";
        }

        public string CreateRole(string name, string parentid, string description, string operateid, string clientid)
        {
            string roleid = Guid.NewGuid().ToString().ToLower();
            bool bl = OrganizationDAL.BaseProvider.CreateRole(roleid, name, parentid, description, operateid, clientid);
            if (bl)
            {
                //处理缓存
                var roles = GetRoles(clientid);
                roles.Add(new Role()
                {
                    RoleID = roleid,
                    Name = name,
                    Description = description,
                    CreateTime = DateTime.Now,
                    CreateUserID = operateid,
                    Status = 1,
                    IsDefault = 0,
                    ClientID = clientid
                });
                Roles[clientid] = roles;
                return roleid;
            }
            return "";
        }

        public static Users CreateUser(EnumAccountType accountType, string loginname, string loginpwd, string name, string mobile, string email, string citycode, string address, string jobs,
                               string roleid, string departid, string parentid, string clientid, string operateid, out int result)
        {
            string userid = Guid.NewGuid().ToString().ToLower();

            loginpwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginpwd, loginname);

            Users user = null;

            DataTable dt = OrganizationDAL.BaseProvider.CreateUser((int)accountType, userid, loginname, loginpwd, name, mobile, email, citycode, address, jobs, roleid, departid, parentid, clientid, operateid, out result);
            if (dt.Rows.Count > 0)
            {
                user = GetUserByUserID(userid, clientid);
                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.User, EnumLogType.Create, "", operateid, user.ClientID);
            }
            return user;
        }

        #endregion

        #region 编辑/删除

        public static bool UpdateUserAccount(string userid, string loginName, string loginPwd, string clientid)
        {
            loginPwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginPwd, loginName);
            bool flag = OrganizationDAL.BaseProvider.UpdateUserAccount(userid, loginName, loginPwd, clientid);

            if (flag)
            {
            }
            return flag;
        }

        public static bool UpdateUserPass(string userid, string loginPwd)
        {
            loginPwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginPwd, "");
            bool flag = OrganizationDAL.BaseProvider.UpdateUserPass(userid, loginPwd);

            return flag;
        }

        public static bool UpdateUserAccountPwd(string loginName, string loginPwd)
        {
            loginPwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginPwd, loginName);
            bool flag = OrganizationDAL.BaseProvider.UpdateUserAccountPwd(loginName, loginPwd);

            return flag;
        }

        public static bool UpdateUserInfo(string userid, string name, string jobs, DateTime birthday, int age, string departID, string email, string mobilePhone, string officePhone, string clientid)
        {
            bool flag = OrganizationDAL.BaseProvider.UpdateUserInfo(userid, name, jobs, birthday, age, departID, email, mobilePhone, officePhone);

           //清除缓存
           if (flag)
           {
               if (Users.ContainsKey(clientid))
               {
                   Users u = GetUserByUserID(userid, clientid);
                   u.Name = name;
                   u.Jobs = jobs;
                   u.Birthday = birthday;
                   u.Age = age;
                   u.DepartID = departID;
                   u.Email = email;
                   u.MobilePhone = mobilePhone;
                   u.OfficePhone = officePhone;
               }
           }
           return flag;
        }

        public static bool UpdateAccountAvatar(string userid, string avatar, string clientid)
        {
            bool flag = OrganizationDAL.BaseProvider.UpdateAccountAvatar(userid, avatar);

            //清除缓存
            if (flag)
            {
                if (Users.ContainsKey(clientid))
                {
                    Users u = GetUserByUserID(userid, clientid);
                    u.Avatar = avatar;
                }
            }
            return flag;
        }

        public static bool UpdateAccountBindMobile(string mobile, string pwd, bool isFirst, string userid, string clientid)
        {
            string loginpwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd, pwd);
            bool flag = OrganizationDAL.BaseProvider.AccountBindMobile(userid, mobile, isFirst, loginpwd, clientid);

            //清除缓存
            if (flag)
            {
                Users u = OrganizationBusiness.GetUserByUserID(userid, clientid);
                u.MobilePhone = mobile;

                if (isFirst)
                {
                    var client = Manage.ClientBusiness.GetClientDetail(clientid);
                    client.MobilePhone = mobile;
                    client.GuideStep = 0;
                }

            }
            return flag;
        }

        public static bool BindOtherAccount(EnumAccountType accountType, string userid, string account, string clientid)
        {
            bool flag = OrganizationDAL.BaseProvider.BindOtherAccount((int)accountType, userid, account, clientid);

            //清除缓存
            if (flag)
            {
                if (accountType == EnumAccountType.Ali)
                {
                    var client = Manage.ClientBusiness.GetClientDetail(clientid);
                    client.AliMemberID = account;
                }
            }
            return flag;
        }

        public static bool UnBindOtherAccount(EnumAccountType accountType, string userid, string account, string clientid)
        {
            bool flag = OrganizationDAL.BaseProvider.UnBindOtherAccount((int)accountType, userid, account, clientid);

            //清除缓存
            if (flag)
            {
            }
            return flag;
        }

        public static bool ClearAccountBindMobile(string userid)
        {
            bool flag = OrganizationDAL.BaseProvider.ClearAccountBindMobile(userid);

            return flag;
        }

        public bool UpdateDepartment(string departid, string name, string description, string operateid, string operateip, string clientid)
        {
            var dal = new OrganizationDAL();
            bool bl = dal.UpdateDepartment(departid, name, description);
            if (bl)
            {
                //处理缓存
                var model = GetDepartmentByID(departid, clientid);
                model.Name = name;
                model.Description = description;
            }
            return bl;
        }

        public EnumResultStatus UpdateDepartmentStatus(string departid, EnumStatus status, string operateid, string operateip, string clientid)
        {
            if (status == EnumStatus.Delete)
            {
                object count = CommonBusiness.Select("Users", "count(0)", "DepartID='" + departid + "' and Status=1");
                if (Convert.ToInt32(count) > 0)
                {
                    return EnumResultStatus.Exists;
                }
            }
            if (CommonBusiness.Update("Department", "Status", (int)status, "DepartID='" + departid + "'"))
            {
                var model = GetDepartments(clientid).Where(d => d.DepartID == departid).FirstOrDefault();
                model.Status = (int)status;
                return EnumResultStatus.Success;
            }
            else
            {
                return EnumResultStatus.Failed;
            }
        }

        public bool UpdateRole(string roleid, string name, string description, string operateid, string ip, string clientid)
        {
            bool bl = OrganizationDAL.BaseProvider.UpdateRole(roleid, name, description);
            if (bl)
            {
                //处理缓存
                var model = GetRoles(clientid).Where(d => d.RoleID == roleid).FirstOrDefault();
                model.Name = name;
                model.Description = description;
            }
            return bl;
        }

        public bool DeleteRole(string roleid, string operateid, string ip, string clientid, out int result)
        {
            bool bl = OrganizationDAL.BaseProvider.DeleteRole(roleid, out result);
            if (bl)
            {
                var model = GetRoles(clientid).Where(d => d.RoleID == roleid).FirstOrDefault();
                model.Status = 9;
            }
            return bl;
        }

        public bool UpdateRolePermission(string roleid, string permissions, string operateid, string ip)
        {
            return OrganizationDAL.BaseProvider.UpdateRolePermission(roleid, permissions, operateid);
        }

        public bool UpdateUserParentID(string userid, string parentid, string clientid, string operateid, string ip)
        {
            bool bl = OrganizationDAL.BaseProvider.UpdateUserParentID(userid, parentid, clientid);
            if (bl)
            {
                var user = GetUserByUserID(userid, clientid);
                user.ParentID = parentid;
            }
            return bl;
        }

        public bool ChangeUsersParentID(string userid, string olduserid, string clientid, string operateid, string ip)
        {
            bool bl = OrganizationDAL.BaseProvider.ChangeUsersParentID(userid, olduserid, clientid);
            if (bl)
            {
                //新员工
                var user = GetUserByUserID(userid, clientid);
                //被替换员工
                var oldUser = GetUserByUserID(olduserid, clientid);

                user.ParentID = oldUser.ParentID;
                oldUser.ParentID = "";
                var list = GetUsersByParentID(olduserid, clientid);
                foreach (var model in list)
                {
                    model.ParentID = userid;
                }

            }
            return bl;
        }

        public bool DeleteUserByID(string userid, string clientid, string operateid, string ip, out int result)
        {
            bool bl = OrganizationDAL.BaseProvider.DeleteUserByID(userid, clientid, out result);
            if (bl)
            {
                var user = GetUserByUserID(userid, clientid);
                user.Status = 9;
                user.ParentID = "";

                var list = GetUsersByParentID(userid, clientid);
                foreach (var model in list)
                {
                    model.ParentID = "";
                }

            }
            return bl;
        }

        public bool UpdateUserRole(string userid, string roleid, string clientid, string operateid, string ip)
        {
            var user = GetUserByUserID(userid, clientid);
            if (user.RoleID.ToLower() != roleid.ToLower())
            {
                bool bl = OrganizationDAL.BaseProvider.UpdateUserRole(userid, roleid, clientid, operateid);
                if (bl)
                {
                    user.RoleID = roleid;
                    user.Role = GetRoleByIDCache(roleid, clientid);
                }
                return bl;
            }
            return true;
        }

        #endregion
    }
}
