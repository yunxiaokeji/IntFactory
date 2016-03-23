using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class ProvidersDAL : BaseDAL
    {
        public DataTable GetProviders(string clientid)
        {
            SqlParameter[] paras = { new SqlParameter("@ClientID", clientid) };
            DataTable dt = GetDataTable("select ProviderID,Name from Providers where ClientID=@ClientID and Status<>9", paras, CommandType.Text);
            return dt;

        }

        public DataTable GetProviderByID(string providerid)
        {
            SqlParameter[] paras = { new SqlParameter("@ProviderID", providerid) };
            DataTable dt = GetDataTable("select * from Providers where ProviderID=@ProviderID", paras, CommandType.Text);
            return dt;
        }

        public string AddProviders(string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            string sqlText = "insert into Providers(ProviderID,Name,Contact,MobileTele,Email,Website,CityCode,Address,Remark,CreateTime,CreateUserID,AgentID,ClientID)"
                                      + "values(@ProviderID ,@Name,@Contact ,@MobileTele,@Email,'',@CityCode,@Address,@Remark,getdate(),@CreateUserID,@AgentID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ProviderID" , id),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@Contact" , contact),
                                     new SqlParameter("@MobileTele" , mobile),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0 ? id : "";

        }

        public bool UpdateProvider(string providerid, string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateid, string agentid, string clientid)
        {
            string sqlText = "Update Providers set [Name]=@Name,[Contact]=@Contact ,[MobileTele]=@MobileTele,[CityCode]=@CityCode,[Address]=@Address,[Remark]=@Remark where [ProviderID]=@ProviderID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@Contact" , contact),
                                     new SqlParameter("@MobileTele" , mobile),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@ProviderID" , providerid),
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }
    }
}
