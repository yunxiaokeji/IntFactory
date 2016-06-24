using System;
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

        public DataSet GetUserByMDUserID(string userid)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@MDUserID",userid)
                                   };
            return GetDataSet("GetUserByMDUserID", paras, CommandType.StoredProcedure, "User|Permission");//|Department|Role


        }

        public DataSet GetUserByAliMemberID(string aliMemberID)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@AliMemberID",aliMemberID)
                                   };
            return GetDataSet("P_GetUserByAliMemberID", paras, CommandType.StoredProcedure, "User|Permission");//|Department|Role


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

        public DataTable CreateUser(string userid, string loginname, string loginpwd, string name, string mobile, string email, string citycode, string address, string jobs,
                               string roleid, string departid, string parentid, string agentid, string clientid, string mduserid, string mdprojectid, int isAppAdmin, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@UserID",userid),
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
                                       new SqlParameter("@MDUserID",mduserid),
                                       new SqlParameter("@MDProjectID",mdprojectid),
                                       new SqlParameter("@IsAppAdmin",isAppAdmin),
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
        public bool UpdateUserAccount(string userid, string loginName, string loginPwd)
        {
            if (string.IsNullOrEmpty(loginPwd))
            {
                string sql = "update users set LoginName=@LoginName where UserID=@UserID";

                SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@LoginName",loginName)
                                   };

                return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
            }
            else
            {
                string sql = "update users set LoginName=@LoginName,LoginPwd=@LoginPwd where UserID=@UserID";

                SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@LoginName",loginName),
                                       new SqlParameter("@LoginPwd",loginPwd)
                                   };

                return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
            }
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
            string sql = "update users set LoginPwd=@LoginPwd where BindMobilePhone=@LoginName and Status<>9 ";

            SqlParameter[] paras = { 
                                       new SqlParameter("@LoginName",loginName),
                                       new SqlParameter("@LoginPwd",loginPwd)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
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

        public bool UpdateAccountBindMobile(string userid, string bindMobile)
        {
            string sql = "update users set BindMobilePhone=@BindMobile where UserID=@UserID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@BindMobile",bindMobile)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool AccountBindMobile(string userid, string bindMobile,string pwd,string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@BindMobile",bindMobile),
                                       new SqlParameter("@Pwd",pwd),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery("P_AccountBindMobile", paras, CommandType.StoredProcedure) > 0;
        }

        public bool BindAccountAliMember(string userid, string memberid)
        {
            string sql = "update users set AliMemberID=@AliMemberID where UserID=@UserID";

            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AliMemberID",memberid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool ClearAccountBindMobile(string userid)
        {
            string sql = "update users set BindMobilePhone='' where UserID=@UserID";

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
