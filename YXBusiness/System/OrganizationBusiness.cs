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

        /// <summary>
        /// 缓存用户信息
        /// </summary>
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

        /// <summary>
        /// 缓存部门信息
        /// </summary>
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

        /// <summary>
        /// 缓存角色信息
        /// </summary>
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

        public static bool IsExistLoginName(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) return false;

            object count = CommonBusiness.Select("Users", "count(0)", " Status=1 and (LoginName='" + loginName + "'  or BindMobilePhone='" + loginName + "')");
            return Convert.ToInt32(count) > 0;
        }

        public static Users GetUserByUserName(string loginname, string pwd,out int result, string operateip)
        {
            pwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd, loginname);
            DataSet ds = new OrganizationDAL().GetUserByUserName(loginname, pwd, out result);
            Users model = null;
            if (ds.Tables.Contains("User") && ds.Tables["User"].Rows.Count > 0)
            {
                model = new Users();
                model.FillData(ds.Tables["User"].Rows[0]);

                model.LogGUID = Guid.NewGuid().ToString();

                model.Department = GetDepartmentByID(model.DepartID, model.AgentID);
                model.Role = GetRoleByIDCache(model.RoleID, model.AgentID);
                
                //处理缓存
                if (!Users.ContainsKey(model.AgentID))
                {
                    GetUsers(model.AgentID);
                }
                if (Users[model.AgentID].Where(u => u.UserID == model.UserID).Count() == 0)
                {
                    Users[model.AgentID].Add(model);
                }
                else
                {
                    var user = Users[model.AgentID].Where(u => u.UserID == model.UserID).FirstOrDefault();
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
                LogBusiness.AddLoginLog(loginname, true,Manage.ClientBusiness.GetClientDetail(model.ClientID).AgentID == model.AgentID ? IntFactoryEnum.EnumSystemType.Client : IntFactoryEnum.EnumSystemType.Agent, operateip, model.UserID, model.AgentID, model.ClientID);
            }
            else
            {
                LogBusiness.AddLoginLog(loginname, false, IntFactoryEnum.EnumSystemType.Client, operateip, "", "", "");
            }

            return model;
        }

        public static bool ConfirmLoginPwd(string loginname, string pwd)
        {
            pwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd, loginname);
            int result;
            DataSet ds = new OrganizationDAL().GetUserByUserName(loginname, pwd,out result);

            if (ds.Tables.Contains("User") && ds.Tables["User"].Rows.Count > 0)
            {
                return true;
            }

            return false;
        }

        public static Users GetUserByMDUserID(string mduserid, string operateip)
        {
            DataSet ds = new OrganizationDAL().GetUserByMDUserID(mduserid);
            Users model = null;
            if (ds.Tables.Contains("User") && ds.Tables["User"].Rows.Count > 0)
            {
                model = new Users();
                model.FillData(ds.Tables["User"].Rows[0]);

                model.LogGUID = Guid.NewGuid().ToString();

                model.Department = GetDepartmentByID(model.DepartID, model.AgentID);
                model.Role = GetRoleByIDCache(model.RoleID, model.AgentID);
                model.Client = Manage.ClientBusiness.GetClientDetail(model.ClientID);
                //处理缓存
                if (!Users.ContainsKey(model.AgentID))
                {
                    GetUsers(model.AgentID);
                }
                if (Users[model.AgentID].Where(u => u.UserID == model.UserID).Count() == 0)
                {
                    Users[model.AgentID].Add(model);
                }
                else
                {
                    var user = Users[model.AgentID].Where(u => u.UserID == model.UserID).FirstOrDefault();
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
                LogBusiness.AddLoginLog(mduserid, true, Manage.ClientBusiness.GetClientDetail(model.ClientID).AgentID == model.AgentID ? IntFactoryEnum.EnumSystemType.Client : IntFactoryEnum.EnumSystemType.Agent, operateip, model.UserID, model.AgentID, model.ClientID);
            }
            else
            {
                LogBusiness.AddLoginLog(mduserid, false, IntFactoryEnum.EnumSystemType.Client, operateip, "", "", "");
            }
            return model;
        }

        public static Users GetUserByAliMemberID(string aliMemberID, string operateip)
        {
            DataSet ds = new OrganizationDAL().GetUserByAliMemberID(aliMemberID);
            Users model = null;
            if (ds.Tables.Contains("User") && ds.Tables["User"].Rows.Count > 0)
            {
                model = new Users();
                model.FillData(ds.Tables["User"].Rows[0]);

                model.LogGUID = Guid.NewGuid().ToString();

                model.Department = GetDepartmentByID(model.DepartID, model.AgentID);
                model.Role = GetRoleByIDCache(model.RoleID, model.AgentID);
                model.Client = Manage.ClientBusiness.GetClientDetail(model.ClientID);
                //处理缓存
                if (!Users.ContainsKey(model.AgentID))
                {
                    GetUsers(model.AgentID);
                }
                if (Users[model.AgentID].Where(u => u.UserID == model.UserID).Count() == 0)
                {
                    Users[model.AgentID].Add(model);
                }
                else
                {
                    var user = Users[model.AgentID].Where(u => u.UserID == model.UserID).FirstOrDefault();
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
                LogBusiness.AddLoginLog(aliMemberID, true, Manage.ClientBusiness.GetClientDetail(model.ClientID).AgentID == model.AgentID ? IntFactoryEnum.EnumSystemType.Client : IntFactoryEnum.EnumSystemType.Agent, operateip, model.UserID, model.AgentID, model.ClientID);
            }
            else
            {
                LogBusiness.AddLoginLog(aliMemberID, false, IntFactoryEnum.EnumSystemType.Client, operateip, "", "", "");
            }
            return model;
        }

        public static Users GetUserByUserID(string userid, string agentid)
        {
            
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(agentid))
            {
                return null;
            }
            userid = userid.ToLower();
            var list = GetUsers(agentid);
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

                    if (agentid == model.AgentID)
                    {
                        model.Department = GetDepartmentByID(model.DepartID, agentid);
                        model.Role = GetRoleByIDCache(model.RoleID, agentid);
                        Users[agentid].Add(model);
                    }
                }
                return model;
            }
        }

        public static List<Users> GetUsers(string keyWords, string departID, string roleID, string agentid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string whereSql = "AgentID='" + agentid + "' and Status<>9";

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

                model.CreateUser = GetUserByUserID(model.CreateUserID, model.AgentID);
                model.Department = GetDepartmentByID(model.DepartID, model.AgentID);
                model.Role = GetRoleByIDCache(model.RoleID, model.AgentID);

                list.Add(model);
            }

            return list;
        }

        public static List<Users> GetUsers(string agentid)
        {
            if (string.IsNullOrEmpty(agentid))
            {
                return new List<Users>();
            }
            if (!Users.ContainsKey(agentid))
            {
                List<Users> list = new List<IntFactoryEntity.Users>();
                DataTable dt = OrganizationDAL.BaseProvider.GetUsers(agentid);
                foreach (DataRow dr in dt.Rows)
                {
                    Users model = new Users();
                    model.FillData(dr);

                    model.Department = GetDepartmentByID(model.DepartID, agentid);
                    model.Role = GetRoleByIDCache(model.RoleID, agentid);

                    list.Add(model);
                }
                Users.Add(agentid, list);
                return list;
            }
            return Users[agentid].ToList();
        }

        public static List<Users> GetUsersByParentID(string parentid, string agentid)
        {
            var users = GetUsers(agentid).Where(m => m.ParentID == parentid && m.Status == 1).ToList();
            return users;
        }

        public static List<Users> GetStructureByParentID(string parentid, string agentid)
        {
            var users = GetUsersByParentID(parentid, agentid);
            foreach (var user in users)
            {
                user.ChildUsers = GetStructureByParentID(user.UserID, agentid);
            }
            return users;
        }

        public static List<Department> GetDepartments(string agentid)
        {
            if (!Departments.ContainsKey(agentid))
            {
                DataTable dt = new OrganizationDAL().GetDepartments(agentid);
                List<Department> list = new List<Department>();
                foreach (DataRow dr in dt.Rows)
                {
                    Department model = new Department();
                    model.FillData(dr);
                    list.Add(model);
                }
                Departments.Add(agentid, list);
                return list;
            }
            return Departments[agentid].Where(m => m.Status == 1).ToList();
        }

        public static Department GetDepartmentByID(string departid, string agendid)
        {
            return GetDepartments(agendid).Where(d => d.DepartID == departid).FirstOrDefault();
        }

        public static List<Role> GetRoles(string agentid)
        {
            if (!Roles.ContainsKey(agentid))
            {
                DataTable dt = new OrganizationDAL().GetRoles(agentid);
                List<Role> list = new List<Role>();
                foreach (DataRow dr in dt.Rows)
                {
                    Role model = new Role();
                    model.FillData(dr);
                    list.Add(model);
                }
                Roles.Add(agentid, list);
                return list;
            }
            return Roles[agentid].Where(m => m.Status == 1).ToList();
        }

        public static Role GetRoleByIDCache(string roleid, string agentid)
        {
            return GetRoles(agentid).Where(r => r.RoleID == roleid).FirstOrDefault();
        }

        public static Role GetRoleByID(string roleid, string agentid)
        {
            Role model = null;
            DataSet ds = OrganizationDAL.BaseProvider.GetRoleByID(roleid, agentid);
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

        public string CreateDepartment(string name, string parentid, string description, string operateid, string agentid, string clientid)
        {
            string departid = Guid.NewGuid().ToString();
            bool bl = OrganizationDAL.BaseProvider.CreateDepartment(departid, name, parentid, description, operateid, agentid, clientid);
            if (bl)
            {
                //处理缓存
                var departs = GetDepartments(agentid);
                departs.Add(new Department()
                {
                    DepartID = departid,
                    Name = name,
                    Description = description,
                    CreateTime = DateTime.Now,
                    CreateUserID = operateid,
                    Status = 1,
                    AgentID = agentid,
                    ClientID = clientid
                });
                Departments[agentid] = departs;
                return departid;
            }
            return "";
        }

        public string CreateRole(string name, string parentid, string description, string operateid, string agentid, string clientid)
        {
            string roleid = Guid.NewGuid().ToString();
            bool bl = OrganizationDAL.BaseProvider.CreateRole(roleid, name, parentid, description, operateid, agentid, clientid);
            if (bl)
            {
                //处理缓存
                var roles = GetRoles(agentid);
                roles.Add(new Role()
                {
                    RoleID = roleid,
                    Name = name,
                    Description = description,
                    CreateTime = DateTime.Now,
                    CreateUserID = operateid,
                    Status = 1,
                    IsDefault = 0,
                    AgentID = agentid,
                    ClientID = clientid
                });
                Roles[agentid] = roles;
                return roleid;
            }
            return "";
        }

        public static Users CreateUser(string loginname, string loginpwd, string name, string mobile, string email, string citycode, string address, string jobs,
                               string roleid, string departid, string parentid, string agentid, string clientid, string mduserid, string mdprojectid, int isAppAdmin, string operateid, out int result)
        {
            string userid = Guid.NewGuid().ToString();

            loginpwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginpwd, loginname);

            Users user = null;

            DataTable dt = OrganizationDAL.BaseProvider.CreateUser(userid, loginname, loginpwd, name, mobile, email, citycode, address, jobs, roleid, departid, parentid, agentid, clientid, mduserid, mdprojectid, isAppAdmin, operateid, out result);
            if (dt.Rows.Count > 0)
            {
                user = new Users();
                user.FillData(dt.Rows[0]);

                var cache = GetUsers(user.AgentID).Where(m => m.UserID == user.UserID).FirstOrDefault();
                if (cache == null || string.IsNullOrEmpty(cache.UserID))
                {
                    user.Role = GetRoleByID(user.RoleID, user.AgentID);
                    user.Department = GetDepartmentByID(user.DepartID, user.AgentID);
                    Users[user.AgentID].Add(user);
                }
                else 
                {
                    cache.Status = 1;
                }

                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.User, EnumLogType.Create, "", operateid, user.AgentID, user.ClientID);
            }
            return user;
        }

        #endregion

        #region 编辑/删除

        public static bool UpdateUserAccount(string userid, string loginName, string loginPwd, string agentid)
        {
            loginPwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginPwd, loginName);
            bool flag = OrganizationDAL.BaseProvider.UpdateUserAccount(userid, loginName, loginPwd);

            if (flag)
            {
                if (Users.ContainsKey(agentid))
                {
                    List<Users> users = Users[agentid];
                    Users u = users.Find(m => m.UserID == userid);
                    u.LoginName = loginName;
                }
            }
            return flag;
        }

        public static bool UpdateUserPass(string userid, string loginPwd, string agentid)
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

        public static bool UpdateUserInfo(string userid, string name, string jobs, DateTime birthday, int age, string departID, string email, string mobilePhone, string officePhone, string agentid)
        {
            bool flag = OrganizationDAL.BaseProvider.UpdateUserInfo(userid, name, jobs, birthday, age, departID, email, mobilePhone, officePhone);

           //清除缓存
           if (flag)
           {
               if (Users.ContainsKey(agentid))
               {
                   List<Users> users = Users[agentid];
                   Users u = users.Find(m => m.UserID == userid);
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

        public static bool UpdateAccountAvatar(string userid, string avatar, string agentid)
        {
            bool flag = OrganizationDAL.BaseProvider.UpdateAccountAvatar(userid, avatar);

            //清除缓存
            if (flag)
            {
                if (Users.ContainsKey(agentid))
                {
                    List<Users> users = Users[agentid];
                    Users u = users.Find(m => m.UserID == userid);
                    u.Avatar = avatar;
                }
            }
            return flag;
        }

        public static bool AccountBindMobile(string bindMobile, string userid, string agentid, string clientid)
        {
            string loginpwd = CloudSalesTool.Encrypt.GetEncryptPwd(bindMobile, bindMobile);
            bool flag = OrganizationDAL.BaseProvider.AccountBindMobile(userid, bindMobile, loginpwd, clientid);

            //清除缓存
            if (flag)
            {
                Users u = OrganizationBusiness.GetUserByUserID(userid, agentid);
                u.BindMobilePhone = bindMobile;
                u.MobilePhone = bindMobile;

                Agents agent = AgentsBusiness.GetAgentDetail(agentid);
                agent.EndTime = agent.EndTime.AddMonths(2);

                IntFactoryEntity.Manage.Clients client = Manage.ClientBusiness.GetClientDetail(clientid);
                client.MobilePhone = bindMobile;
            }
            return flag;
        }

        public static bool UpdateAccountBindMobile(string userid, string bindMobile, string agentid)
        {
            bool flag = OrganizationDAL.BaseProvider.UpdateAccountBindMobile(userid, bindMobile);

            //清除缓存
            if (flag)
            {
                if (Users.ContainsKey(agentid))
                {
                    List<Users> users = Users[agentid];
                    Users u = users.Find(m => m.UserID == userid);
                    u.BindMobilePhone = bindMobile;
                }
            }
            return flag;
        }

        public static bool BindAccountAliMember(string userid, string memberid, string agentid)
        {

            bool flag = OrganizationDAL.BaseProvider.BindAccountAliMember(userid, memberid);

            //清除缓存
            if (flag)
            {
                if (Users.ContainsKey(agentid))
                {
                    List<Users> users = Users[agentid];
                    Users u = users.Find(m => m.UserID == userid);
                    u.AliMemberID = memberid;
                }
            }
            return flag;
        }

        public static bool ClearAccountBindMobile(string userid, string agentid)
        {
            bool flag = OrganizationDAL.BaseProvider.ClearAccountBindMobile(userid);

            //清除缓存
            if (flag)
            {
                if (Users.ContainsKey(agentid))
                {
                    List<Users> users = Users[agentid];
                    Users u = users.Find(m => m.UserID == userid);
                    u.BindMobilePhone = string.Empty;
                }
            }
            return flag;
        }

        public bool UpdateDepartment(string departid, string name, string description, string operateid, string operateip, string agentid)
        {
            var dal = new OrganizationDAL();
            bool bl = dal.UpdateDepartment(departid, name, description, agentid);
            if (bl)
            {
                //处理缓存
                var model = GetDepartments(agentid).Where(d => d.DepartID == departid).FirstOrDefault();
                model.Name = name;
                model.Description = description;
            }
            return bl;
        }

        public EnumResultStatus UpdateDepartmentStatus(string departid, EnumStatus status, string operateid, string operateip, string agentid)
        {
            if (status == EnumStatus.Delete)
            {
                object count = CommonBusiness.Select("Users", "count(0)", "DepartID='" + departid + "' and Status=1");
                if (Convert.ToInt32(count) > 0)
                {
                    return EnumResultStatus.Exists;
                }
            }
            if (CommonBusiness.Update("Department", "Status", (int)status, "DepartID='" + departid + "' and AgentID='" + agentid + "'"))
            {
                var model = GetDepartments(agentid).Where(d => d.DepartID == departid).FirstOrDefault();
                model.Status = (int)status;
                return EnumResultStatus.Success;
            }
            else
            {
                return EnumResultStatus.Failed;
            }
        }

        public bool UpdateRole(string roleid, string name, string description, string operateid, string ip, string agentid)
        {
            bool bl = OrganizationDAL.BaseProvider.UpdateRole(roleid, name, description, agentid);
            if (bl)
            {
                //处理缓存
                var model = GetRoles(agentid).Where(d => d.RoleID == roleid).FirstOrDefault();
                model.Name = name;
                model.Description = description;
            }
            return bl;
        }

        public bool DeleteRole(string roleid, string operateid, string ip, string agentid, out int result)
        {
            bool bl = OrganizationDAL.BaseProvider.DeleteRole(roleid, agentid, out result);
            if (bl)
            {
                var model = GetRoles(agentid).Where(d => d.RoleID == roleid).FirstOrDefault();
                model.Status = 9;
            }
            return bl;
        }

        public bool UpdateRolePermission(string roleid, string permissions, string operateid, string ip)
        {
            return OrganizationDAL.BaseProvider.UpdateRolePermission(roleid, permissions, operateid);
        }

        public bool UpdateUserParentID(string userid, string parentid, string agentid, string operateid, string ip)
        {
            bool bl = OrganizationDAL.BaseProvider.UpdateUserParentID(userid, parentid, agentid);
            if (bl)
            {
                var user = GetUserByUserID(userid, agentid);
                user.ParentID = parentid;
            }
            return bl;
        }

        public bool ChangeUsersParentID(string userid, string olduserid, string agentid, string operateid, string ip)
        {
            bool bl = OrganizationDAL.BaseProvider.ChangeUsersParentID(userid, olduserid, agentid);
            if (bl)
            {
                //新员工
                var user = GetUserByUserID(userid, agentid);
                //被替换员工
                var oldUser = GetUserByUserID(olduserid, agentid);

                user.ParentID = oldUser.ParentID;
                oldUser.ParentID = "";
                var list = GetUsersByParentID(olduserid, agentid);
                foreach (var model in list)
                {
                    model.ParentID = userid;
                }

            }
            return bl;
        }

        public bool DeleteUserByID(string userid, string agentid, string operateid, string ip, out int result)
        {
            bool bl = OrganizationDAL.BaseProvider.DeleteUserByID(userid, agentid, out result);
            if (bl)
            {
                var user = GetUserByUserID(userid, agentid);
                user.Status = 9;
                user.ParentID = "";

                var list = GetUsersByParentID(userid, agentid);
                foreach (var model in list)
                {
                    model.ParentID = "";
                }

            }
            return bl;
        }

        public bool UpdateUserRole(string userid, string roleid, string agentid, string operateid, string ip)
        {
            var user = GetUserByUserID(userid, agentid);
            if (user.RoleID.ToLower() != roleid.ToLower())
            {
                bool bl = OrganizationDAL.BaseProvider.UpdateUserRole(userid, roleid, agentid, operateid);
                if (bl)
                {
                    user.RoleID = roleid;
                    user.Role = GetRoleByID(roleid, agentid);
                }
                return bl;
            }
            return true;
        }

        #endregion
    }
}
