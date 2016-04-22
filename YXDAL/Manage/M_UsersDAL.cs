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
            return GetDataTable("select * from M_Users where LoginName=@UserName and LoginPwd=@LoginPwd and Status=1", paras, CommandType.Text);
        }
        public DataSet GetM_UserByProUserName(string loginname, string pwd, out int result)
        {
            result = 0;
            SqlParameter[] paras = {
                                    new SqlParameter("@Result",result),
                                    new SqlParameter("@LoginName",loginname),
                                    new SqlParameter("@LoginPwd",pwd)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            DataSet ds = GetDataSet("M_GetM_UserToLogin", paras, CommandType.StoredProcedure, "M_User|Permission");
            result = Convert.ToInt32(paras[0].Value);

            return ds;
        }
        public DataTable GetM_UserByLoginName(string loginname)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@LoginName",loginname)
                                   };
            return GetDataTable("select * from M_Users where LoginName=@LoginName and Status=1", paras, CommandType.Text);
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

        public bool CreateM_User(string userid, string loginname, string loginpwd,string name,int? isadmin,string roleid,string email,string mobilephone,string officephone,string jobs,string avatar, string description, string operateid)
        {
            string sql = "INSERT INTO M_Users(UserID,LoginName ,LoginPWD,Name,Email,MobilePhone,OfficePhone,Jobs ,Avatar ,IsAdmin ,Status  ,Description ,CreateUserID ,RoleID) " +
                        " values(@UserID,@LoginName,@LoginPWD,@Name,@Email,@MobilePhone,@OfficePhone,@Jobs,@Avatar,@IsAdmin,1,@Description,@CreateUserID,@RoleID)";

            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@LoginName",loginname),
                                       new SqlParameter("@LoginPWD",loginpwd),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Email",email),
                                       new SqlParameter("@MobilePhone",mobilephone),
                                       new SqlParameter("@OfficePhone",officephone),
                                       new SqlParameter("@Jobs",jobs),
                                       new SqlParameter("@Avatar",avatar),
                                       new SqlParameter("@IsAdmin",isadmin),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@RoleID",roleid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateM_User(string userid,string name, string roleid, string email, string mobilephone, string officephone, string jobs, string avatar, string description)
        {
            string sql = "update M_Users set Name=@Name,Email=@Email,MobilePhone=@MobilePhone,OfficePhone=@OfficePhone,Jobs=@Jobs ,Avatar=@Avatar ,Description=@Description ,RoleID=@RoleID where UserID=@UserID ";

            SqlParameter[] paras = {  
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Email",email),
                                       new SqlParameter("@MobilePhone",mobilephone),
                                       new SqlParameter("@OfficePhone",officephone),
                                       new SqlParameter("@Jobs",jobs),
                                       new SqlParameter("@Avatar",avatar), 
                                       new SqlParameter("@Description",description), 
                                       new SqlParameter("@RoleID",roleid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }
        public bool DeleteM_User(string userid, int status)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@userID",userid),
                                    new SqlParameter("@Status",status),
                                   };

            return ExecuteNonQuery("update M_Users set Status=@Status where userID=@userID", paras, CommandType.Text) > 0;
        }
    }
}
