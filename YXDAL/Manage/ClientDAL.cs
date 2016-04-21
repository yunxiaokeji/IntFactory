using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace IntFactoryDAL.Manage
{
    public class ClientDAL : BaseDAL
    {
        public static ClientDAL BaseProvider = new ClientDAL();

        #region 查询
        public DataTable GetClientDetail(string clientID)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@ClientID",clientID),
                                   };
            return GetDataTable("select * from Clients where ClientID=@ClientID", paras, CommandType.Text);
        }

        public DataTable GetClientsGrow(int type, string begintime, string endtime)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@DateType",type), 
                                    new SqlParameter("@BeginTime",begintime),
                                    new SqlParameter("@EndTime",endtime)
                                   };
            return GetDataTable("R_GetClientsGrowDate", paras, CommandType.StoredProcedure);
        }

        public DataSet GetClientsLoginReport(int type, string begintime, string endtime)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@DateType",type), 
                                    new SqlParameter("@BeginTime",begintime),
                                    new SqlParameter("@EndTime",endtime)
                                   };
            return GetDataSet("R_GetClientsAgentLogin_Day", paras, CommandType.StoredProcedure);
        }
        #endregion

        #region 添加

        public string InsertClient(string companyName, string loginname, string contactName, string mobilePhone, string industry, string cityCode, string address,
                                   string description, string bindMobilePhone, string loginPwd, string email, string mduserid, string mdprojectid, string userid, string aliMemberID, out int result)
        {
            string clientid = Guid.NewGuid().ToString();
            result = 0;
            SqlParameter[] parms = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@LoginName",loginname),
                                       new SqlParameter("@ClientCode",GetCode(6)),
                                       new SqlParameter("@CompanyName",companyName),
                                       new SqlParameter("@MobilePhone",mobilePhone),
                                       new SqlParameter("@Industry",industry),
                                       new SqlParameter("@CityCode",cityCode),
                                       new SqlParameter("@Address",address),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@ContactName",contactName),
                                       new SqlParameter("@BindMobilePhone",bindMobilePhone),
                                       new SqlParameter("@LoginPWD",loginPwd),
                                       new SqlParameter("@Email",email),
                                       new SqlParameter("@MDUserID",mduserid),
                                       new SqlParameter("@MDprojectID",mdprojectid),
                                       new SqlParameter("@AliMemberID",aliMemberID),
                                       new SqlParameter("@CreateUserID",userid)
                                   };
            parms[0].Direction = ParameterDirection.Output;

            ExecuteNonQuery("M_InsertClient", parms, CommandType.StoredProcedure);

            result = Convert.ToInt32(parms[0].Value);
            return clientid;
        }

        public bool BindClientAliMember(string clientID,string userID, string memberID)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@ClientiD",clientID),
                                       new SqlParameter("@UserID",userID),
                                       new SqlParameter("@AliMemberID",memberID)
                                   };

            return ExecuteNonQuery("M_BindClientAliMember", parms, CommandType.StoredProcedure) > 0;
        }

        public bool InsertClientAuthorizeLog(string clientID, string agentID, string orderID, int userQuantity, DateTime? beginTime, DateTime? endTime, int type)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@ClientiD",clientID),
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@OrderID",orderID),
                                       new SqlParameter("@UserQuantity",userQuantity),
                                       new SqlParameter("@BeginTime",beginTime),
                                       new SqlParameter("@EndTime",endTime),
                                       new SqlParameter("@Type",type),
                                   };
            string cmdTxt = "insert into ClientAuthorizeLog(ClientiD,AgentID,OrderID,UserQuantity,BeginTime,EndTime,Type) values(@ClientiD,@AgentID,@OrderID,@UserQuantity,@BeginTime,@EndTime,@Type)";

            return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
        }
        #endregion

        #region 编辑
        public bool UpdateClient(string clientID, string companyName, string contactName, string mobilePhone, string industry, string cityCode, string address,string description,string logo,string officePhone,string userid)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@ClientiD",clientID),
                                       new SqlParameter("@CompanyName",companyName),
                                       new SqlParameter("@MobilePhone",mobilePhone),
                                       new SqlParameter("@Industry",industry),
                                       new SqlParameter("@CityCode",cityCode),
                                       new SqlParameter("@Address",address),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@ContactName",contactName),
                                       new SqlParameter("@Logo",logo),
                                       new SqlParameter("@OfficePhone",officePhone),
                                       new SqlParameter("@CreateUserID",userid)
                                   };

            return ExecuteNonQuery("M_UpdateClient", parms, CommandType.StoredProcedure) > 0;
        }

        public bool ClientAgentAuthorize(string agentID, int userQuantity, DateTime endTime)
        { 
        SqlParameter[] parms = { 
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@UserQuantity",userQuantity),
                                       new SqlParameter("@EndTime",endTime)
                                   };

        string cmdText = "update Agents set  UserQuantity=@UserQuantity,EndTime=@EndTime where AgentID=@AgentID";

        return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }

        public bool AddClientAgentUserQuantity(string agentID, int quantity)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@UserQuantity",quantity)
                                   };

            string cmdText = "update Agents set  UserQuantity+=@UserQuantity where  AgentID=@AgentID";

            return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }

        public bool SetClientAgentEndTime(string agentID, DateTime endTime)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@EndTime",endTime)
                                   };

            string cmdText = "update Agents set EndTime=@EndTime where  AgentID=@AgentID";

            return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }
            #endregion
    }
}
