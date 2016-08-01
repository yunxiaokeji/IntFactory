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
            string sql = @"select * from Clients  where ClientID=@ClientID ";

            return GetDataTable(sql, paras, CommandType.Text);
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

        public DataSet GetClientsAgentActionReport(int type, string begintime, string endtime, string clientId)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@DateType",type), 
                                    new SqlParameter("@BeginTime",begintime),
                                    new SqlParameter("@EndTime",endtime),
                                    new SqlParameter("@ClientID",clientId)
                                   };
            return GetDataSet("R_GetClientsAgentAction", paras, CommandType.StoredProcedure);
        }

        public DataSet GetClientsVitalityReport(int type, string begintime, string endtime, string clientId)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@DateType",type), 
                                    new SqlParameter("@BeginTime",begintime),
                                    new SqlParameter("@EndTime",endtime),
                                    new SqlParameter("@ClientID",clientId)
                                   };
            return GetDataSet("R_GetClientsActiveReprot", paras, CommandType.StoredProcedure, "ClientReport|SystemReport");
        }
        #endregion

        #region 添加

        public string InsertClient(int registerType, int accountType, string account, string loginPwd, string companyName, string contactName, string mobile, string email, string industry, string cityCode, string address, string remark,
                                    string companyid, string operateid, out int result, out string userid)
        {
            string clientid = Guid.NewGuid().ToString().ToLower();
            userid = Guid.NewGuid().ToString().ToLower();
            result = 0;
            SqlParameter[] parms = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@ClientCode",GetCode(6)),
                                       new SqlParameter("@RegisterType",registerType),
                                       new SqlParameter("@AccountType",accountType),
                                       new SqlParameter("@Account",account),
                                       new SqlParameter("@LoginPWD",loginPwd),
                                       new SqlParameter("@CompanyName",companyName),
                                       new SqlParameter("@ContactName",contactName),
                                       new SqlParameter("@MobilePhone",mobile),
                                       new SqlParameter("@Email",email),
                                       new SqlParameter("@Industry",industry),
                                       new SqlParameter("@CityCode",cityCode),
                                       new SqlParameter("@Address",address),
                                       new SqlParameter("@CompanyID",companyid),
                                       new SqlParameter("@Description",remark),
                                       new SqlParameter("@CreateUserID",operateid)
                                   };
            parms[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("M_InsertClient", parms, CommandType.StoredProcedure);

            result = Convert.ToInt32(parms[0].Value);
            if (result == 1)
            {
                return clientid;
            }
            else
            {
                userid = "";
                return "";
            }

        }

        public bool InsertClientAuthorizeLog(string clientID, string orderID, int userQuantity, DateTime? beginTime, DateTime? endTime, int type)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@ClientiD",clientID),
                                       new SqlParameter("@OrderID",orderID),
                                       new SqlParameter("@UserQuantity",userQuantity),
                                       new SqlParameter("@BeginTime",beginTime),
                                       new SqlParameter("@EndTime",endTime),
                                       new SqlParameter("@Type",type),
                                   };
            string cmdTxt = "insert into ClientAuthorizeLog(ClientiD,OrderID,UserQuantity,BeginTime,EndTime,Type) values(@ClientiD,@OrderID,@UserQuantity,@BeginTime,@EndTime,@Type)";

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

        public bool ClientClientAuthorize(string clientid, int userQuantity, DateTime endTime)
        { 
        SqlParameter[] parms = { 
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@UserQuantity",userQuantity),
                                       new SqlParameter("@EndTime",endTime)
                                   };

        string cmdText = "update Clients set  UserQuantity=@UserQuantity,EndTime=@EndTime where ClientID=@ClientID";

        return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }

        public bool AddClientUserQuantity(string clientid, int quantity)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@UserQuantity",quantity)
                                   };

            string cmdText = "update Clients set  UserQuantity+=@UserQuantity where  ClientID=@ClientID";

            return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }

        public bool SetClientEndTime(string clientid, DateTime endTime)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@EndTime",endTime)
                                   };

            string cmdText = "update Clients set EndTime=@EndTime where  ClientID=@ClientID";

            return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }

        public bool SetClientProcess(string ids, string userid, string clientid)
        {
            int result = 0;
            SqlParameter[] parms = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@IDS",ids),
                                       new SqlParameter("@UserID",userid)
                                   };

            parms[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("M_SetClientProcess", parms, CommandType.StoredProcedure);
            return Convert.ToInt32(parms[0].Value) == 1;
        }

        public bool SetClientCategory(string ids, string userid, string clientid)
        {
            int result = 0;
            SqlParameter[] parms = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@IDS",ids),
                                       new SqlParameter("@UserID",userid)
                                   };

            parms[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("M_SetClientCategory", parms, CommandType.StoredProcedure);
            return Convert.ToInt32(parms[0].Value) == 1;
        }

        public bool FinishInitSetting(string clientid)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@ClientID",clientid)
                                   };

          return  ExecuteNonQuery("Update Clients set GuideStep=0 where ClientID=@ClientID", parms, CommandType.Text)>0;
        }
        #endregion
    }
}
