using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using IntFactoryEntity.Manage;
using IntFactoryDAL.Manage;


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
            }

            //记录登录日志
            LogBusiness.AddLoginLog(loginname, model != null, IntFactoryEnum.EnumSystemType.Manage, operateip, "", "", "");

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
