using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace IntFactoryDAL.Manage
{
    public class M_UsersDAL : BaseDAL
    {
        public static M_UsersDAL BaseProvider = new M_UsersDAL();

        public DataTable GetM_UserByUserName(string loginname, string pwd)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@UserName",loginname),
                                    new SqlParameter("@LoginPwd",pwd)
                                   };
            return GetDataTable("select * from M_Users where LoginName=@UserName and LoginPwd=@LoginPwd", paras, CommandType.Text);
        }

        public DataTable GetUserDetail(string userID)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@UserID",userID)
                                   };

            return GetDataTable("select * from M_Users where UserID=@UserID", paras, CommandType.Text);
        }

        public bool SetAdminAccount(string userid,string loginname, string pwd)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@userID",userid),
                                    new SqlParameter("@UserName",loginname),
                                    new SqlParameter("@LoginPwd",pwd)
                                   };

            return ExecuteNonQuery("update M_Users set LoginName=@UserName , LoginPwd=@LoginPwd where userID=@userID", paras, CommandType.Text) > 0;
        }
    }
}
