using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using IntFactoryEntity.Manage;
using IntFactoryDAL.Manage;
using IntFactoryEntity;


namespace IntFactoryBusiness
{
   

    public class M_UsersBusiness
    {
         #region 查询
        /// <summary>
        /// 根据账号密码获取信息
        /// </summary>
        /// <param name="loginname">账号</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static M_Users GetM_UserByUserName(string loginname, string pwd, string operateip)
        {
            pwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd, loginname);
            DataTable dt = new M_UsersDAL().GetM_UserByUserName(loginname, pwd);
            M_Users model = null;
            if (dt.Rows.Count > 0)
            {
                model = new M_Users();
                model.FillData(dt.Rows[0]); 
                if(!string.IsNullOrEmpty(model.RoleID))
                    model.Role = ManageSystemBusiness.GetRoleByIDCache(model.RoleID);
                //权限
                if (model.Role != null && model.IsAdmin != 1)
                {
                    model.Menus = CommonBusiness.ManageMenus;
                }
                else
                {
                    model.Menus = new List<Menu>();
                }
            }
            return model;
        }
        /// <summary>
        /// 根据账号密码获取信息（登录）
        /// </summary>
        /// <param name="loginname"></param>
        /// <param name="pwd"></param>
        /// <param name="operateip"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static M_Users GetM_UserByProUserName(string loginname, string pwd, string operateip, out int result)
        {
            pwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd, loginname);
            DataSet ds = new M_UsersDAL().GetM_UserByProUserName(loginname, pwd, out result);
            M_Users model = null;
            if (ds.Tables.Contains("M_User") && ds.Tables["M_User"].Rows.Count > 0)
            {
                model = new M_Users();
                model.FillData(ds.Tables["M_User"].Rows[0]);
                if (!string.IsNullOrEmpty(model.RoleID))
                    model.Role = ManageSystemBusiness.GetRoleByIDCache(model.RoleID);
                //权限
                if ( model.IsAdmin == 1)
                {
                    model.Menus = CommonBusiness.ManageMenus;
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
            LogBusiness.AddLoginLog(loginname, model != null, IntFactoryEnum.EnumSystemType.Manage, operateip, "", "", "");

            return model;
        }
        public static int GetM_UserCountByLoginName(string loginname)
        {
            DataTable dt = new M_UsersDAL().GetM_UserByLoginName(loginname);
            return dt.Rows.Count;
        }
        public static List<M_Users> GetUsers(string keyWords, string roleID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string whereSql = " Status<>9";

            if (!string.IsNullOrEmpty(keyWords))
                whereSql += " and ( Name like '%" + keyWords + "%' or MobilePhone like '%" + keyWords + "%' or Email like '%" + keyWords + "%')";


            if (!string.IsNullOrEmpty(roleID))
                whereSql += " and RoleID='" + roleID + "'";

            DataTable dt = CommonBusiness.GetPagerData("M_Users", "*", whereSql, "AutoID", pageSize, pageIndex, out totalCount, out pageCount);
            List<M_Users> list = new List<M_Users>();
            M_Users model;
            foreach (DataRow item in dt.Rows)
            {
                model = new M_Users();
                model.FillData(item);

                if (!string.IsNullOrEmpty(model.RoleID))
                    model.Role = ManageSystemBusiness.GetRoleByIDCache(model.RoleID);

                list.Add(model);
            }

            return list;
        }

        public static M_Users GetUserDetail(string userID)
        {
            DataTable dt = M_UsersDAL.BaseProvider.GetUserDetail(userID);

            M_Users model=null;
            if (dt.Rows.Count == 1)
            {
                model = new M_Users();
                model.FillData(dt.Rows[0]);
            }

            return model;
        }
        #endregion

        #region 改
        /// <summary>
        /// 修改管理员账户
        /// </summary>
        public static bool SetAdminAccount(string userid, string loginname, string pwd)
        {
            pwd = CloudSalesTool.Encrypt.GetEncryptPwd(pwd, loginname);

            return M_UsersDAL.BaseProvider.SetAdminAccount(userid, loginname, pwd);
        }
        /// <summary>
        /// 新增或修改用户信息
        /// </summary>
        public string CreateM_User(M_Users musers)
        {
            string userid = Guid.NewGuid().ToString();
            musers.LoginPWD = CloudSalesTool.Encrypt.GetEncryptPwd(musers.LoginPWD, musers.LoginName);
            bool bl = M_UsersDAL.BaseProvider.CreateM_User(userid, musers.LoginName, musers.LoginPWD,musers.Name,musers.IsAdmin,musers.RoleID,musers.Email,musers.MobilePhone,musers.OfficePhone,musers.Jobs,musers.Avatar,musers.Description,musers.CreateUserID);
            if (bl)
            {
                return userid;
            }

            return "";
        }
        /// <summary>
        /// 修改用户户信息
        /// </summary>
        public bool UpdateM_User(string userid,string name, string roleid, string email, string mobilephone, string officephone, string jobs, string avatar, string description)
        {
            bool bl = M_UsersDAL.BaseProvider.UpdateM_User(userid, name, roleid, email, mobilephone, officephone, jobs, avatar, description);
            return bl;
        }
        public bool DeleteM_User(string userid, int status) {
            return M_UsersDAL.BaseProvider.DeleteM_User(userid, status);
        }
        #endregion
    }

    

    
}
