﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class OrganizationDAL :BaseDAL
    {
        public static OrganizationDAL BaseProvider = new OrganizationDAL();

        #region 查询

        public DataSet GetUserByUserName(string loginname, string pwd,out int result)
        {
            result = 0;
            SqlParameter[] paras = {
                                    new SqlParameter("@Result",result),
                                    new SqlParameter("@LoginName",loginname),
                                    new SqlParameter("@LoginPwd",pwd)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            DataSet ds= GetDataSet("P_GetUserToLogin", paras, CommandType.StoredProcedure, "User|Permission");//|Department|Role
            result =Convert.ToInt32( paras[0].Value);

            return ds;
        }

        public DataSet GetUserByOtherAccount(int accountType, string account)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@AccountType",accountType),
                                       new SqlParameter("@Account",account)
                                   };
            return GetDataSet("P_GetUserByOtherAccount", paras, CommandType.StoredProcedure, "User|Permission");//|Department|Role


        }

        public DataTable GetUsers(string agentid)
        {
            string sql = "select * from Users where AgentID=@AgentID";

            SqlParameter[] paras = { 
                                    new SqlParameter("@AgentID",agentid)
                                   };

            return GetDataTable(sql, paras, CommandType.Text);
        }

        public DataTable GetUserByUserID(string userid)
        {
            string sql = "select * from Users where UserID=@UserID ";

            SqlParameter[] paras = { 
                                    new SqlParameter("@UserID",userid)
                                   };

            return GetDataTable(sql, paras, CommandType.Text);
        }

        public DataTable GetDepartments(string agentid)
        {
            string sql = "select * from Department where AgentID=@AgentID and Status<>9";

            SqlParameter[] paras = { 
                                    new SqlParameter("@AgentID",agentid)
                                   };

            return GetDataTable(sql, paras, CommandType.Text);
        }

        public DataTable GetRoles(string agentid)
        {
            string sql = "select * from Role where AgentID=@AgentID and Status<>9";

            SqlParameter[] paras = { 
                                    new SqlParameter("@AgentID",agentid)
                                   };

            return GetDataTable(sql, paras, CommandType.Text);
        }

        public DataSet GetRoleByID(string roleid, string agentid)
        {
            string sql = "select * from Role where RoleID=@RoleID and AgentID=@AgentID and Status<>9; select * from RolePermission where RoleID=@RoleID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@AgentID",agentid)
                                   };

            return GetDataSet(sql, paras, CommandType.Text, "Role|Menus");
        }

        #endregion

        #region 添加

        public bool CreateDepartment(string departid, string name, string parentid, string description, string operateid, string agentid, string clientid)
        {
            string sql = "insert into Department(DepartID,Name,ParentID,Status,Description,CreateUserID,AgentID,ClientID) "+
                        " values(@DepartID,@Name,@ParentID,1,@Description,@CreateUserID,@AgentID,@ClientID)";

            SqlParameter[] paras = { 
                                       new SqlParameter("@DepartID",departid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@ParentID",parentid),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool CreateRole(string roleid, string name, string parentid, string description, string operateid, string agentid, string clientid)
        {
            string sql = "insert into Role(RoleID,Name,ParentID,Status,IsDefault,Description,CreateUserID,AgentID,ClientID) " +
                        " values(@RoleID,@Name,@ParentID,1,0,@Description,@CreateUserID,@AgentID,@ClientID)";

            SqlParameter[] paras = { 
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@ParentID",parentid),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public DataTable CreateUser(int accountType, string userid, string loginname, string loginpwd, string name, string mobile, string email, string citycode, string address, string jobs,
                               string roleid, string departid, string parentid, string agentid, string clientid, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AccountType",accountType),
                                       new SqlParameter("@LoginName",loginname),
                                       new SqlParameter("@LoginPwd",loginpwd),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Mobile",mobile),
                                       new SqlParameter("@Email",email),
                                       new SqlParameter("@CityCode",citycode),
                                       new SqlParameter("@Address",address),
                                       new SqlParameter("@Jobs",jobs),
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@DepartID",departid),
                                       new SqlParameter("@ParentID",parentid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            paras[0].Direction = ParameterDirection.Output;

            DataTable dt = GetDataTable("P_InsterUser", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);

            return dt;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateUserAccount(string userid, string loginName, string loginPwd, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@LoginName",loginName),
                                       new SqlParameter("@LoginPwd",loginPwd),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_UpdateUserAccount", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateUserPass(string userid, string loginPwd)
        {
            string sql = "update users set LoginPwd=@LoginPwd where UserID=@UserID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@LoginPwd",loginPwd)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateUserAccountPwd(string loginName, string loginPwd)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@LoginName",loginName),
                                       new SqlParameter("@LoginPwd",loginPwd)
                                   };

            return ExecuteNonQuery("P_UpdateUserAccountPwd", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateUserInfo(string userid, string name, string jobs, DateTime birthday, int age, string departID, string email, string mobilePhone, string officePhone)
        {
            string sql = "update users set Name=@Name,Jobs=@Jobs, Birthday=@Birthday, Age=@Age,DepartID=@DepartID,Email=@Email,MobilePhone=@MobilePhone, OfficePhone=@OfficePhone where UserID=@UserID";
            DateTime date = birthday.ToString("yyyy/MM/dd") == "0001/01/01" ? Convert.ToDateTime("1900/01/01") : birthday;
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Jobs",jobs),
                                       new SqlParameter("@Birthday",date),
                                       new SqlParameter("@Age",age),
                                       new SqlParameter("@DepartID",departID),
                                       new SqlParameter("@Email",email),
                                       new SqlParameter("@MobilePhone",mobilePhone),
                                       new SqlParameter("@OfficePhone",officePhone)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateAccountAvatar(string userid, string avatar) {
            string sql = "update users set Avatar=@Avatar where UserID=@UserID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@Avatar",avatar)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateAccountBindMobile(string userid, string bindMobile, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@LoginName",bindMobile),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_UpdateAccountBindMobile", paras, CommandType.StoredProcedure) > 0;
        }

        public bool AccountBindMobile(string userid, string bindMobile, bool isFirst, string pwd, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@BindMobile",bindMobile),
                                       new SqlParameter("@Pwd",pwd),
                                       new SqlParameter("@IsFirst",isFirst?1:0),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_AccountBindMobile", paras, CommandType.StoredProcedure) > 0;
        }

        public bool BindOtherAccount(int accountType, string userid, string account, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@AccountType",accountType),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@Account",account),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_BindOtherAccount", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UnBindOtherAccount(int accountType, string userid, string account, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@AccountType",accountType),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@Account",account),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_UnBindOtherAccount", paras, CommandType.StoredProcedure) > 0;
        }

        public bool ClearAccountBindMobile(string userid)
        {
            string sql = "Delete from UserAccounts where UserID=@UserID and AccountType=2 ";

            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateDepartment(string departid, string name, string description, string agentid)
        {
            string sql = "update Department set Name=@Name,Description=@Description where DepartID=@DepartID and AgentID=@AgentID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@DepartID",departid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@AgentID",agentid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateRole(string roleid, string name, string description, string agentid)
        {
            string sql = "update Role set Name=@Name,Description=@Description where RoleID=@RoleID and AgentID=@AgentID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@AgentID",agentid)
                                   };
            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool DeleteRole(string roleid, string agentid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@AgentID",agentid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            bool bl = ExecuteNonQuery("P_DeleteRole", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            return bl;
        }

        public bool UpdateRolePermission(string roleid, string permissions, string userid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@Permissions",permissions)
                                   };
            return ExecuteNonQuery("P_UpdateRolePermission", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateUserParentID(string userid, string parentid, string agentid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@ParentID",parentid),
                                       new SqlParameter("@AgentID",agentid)
                                   };
            bool bl = ExecuteNonQuery("P_UpdateUserParentID", paras, CommandType.StoredProcedure) > 0;
            return bl;
        }

        public bool ChangeUsersParentID(string userid, string olduserid, string agentid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@OldUserID",olduserid),
                                       new SqlParameter("@AgentID",agentid)
                                   };
            bool bl = ExecuteNonQuery("P_ChangeUsersParentID", paras, CommandType.StoredProcedure) > 0;
            return bl;
        }

        public bool DeleteUserByID(string userid, string agentid, out int result)
        {

            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            bool bl = ExecuteNonQuery("P_DeleteUserByID", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            return bl;
        }

        public bool UpdateUserRole(string userid, string roleid, string agentid,string operateid)
        {

            SqlParameter[] paras = { 
                                       new SqlParameter("@RoleID",roleid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@OpreateID",operateid)
                                   };
            bool bl = ExecuteNonQuery("P_UpdateUserRole", paras, CommandType.StoredProcedure) > 0;
            return bl;
        }

        #endregion

    }
}
