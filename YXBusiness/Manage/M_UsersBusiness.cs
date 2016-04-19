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
        /// 根据账号密码获取信息（登录）
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
                  
                    model.Menus = CommonBusiness.ManageMenus;
                    //  model.Menus = new List<Menu>();
                    //DataTable ds = new M_UsersDAL().GetM_UserByUserName(loginname, pwd);
                    //foreach (DataRow dr in ds.Tables["Permission"].Rows)
                    //{
                    //    Menu menu = new Menu();
                    //    menu.FillData(dr);
                    //    model.Menus.Add(menu);
                    //}
                }
            }

            //记录登录日志
            LogBusiness.AddLoginLog(loginname, model != null, IntFactoryEnum.EnumSystemType.Manage, operateip, "", "", "");

            return model;
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
        #endregion
    }

    

    
}
