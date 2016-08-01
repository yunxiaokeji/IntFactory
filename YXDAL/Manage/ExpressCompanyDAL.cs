using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL.Manage
{
    public class ExpressCompanyDAL: BaseDAL
    {
        public static ExpressCompanyDAL BaseProvider = new ExpressCompanyDAL();

        public DataTable GetExpressCompanys()
        {
            return GetDataTable("select * from ExpressCompany where Status<>9");
        }

        public DataTable GetExpressCompanyDetail(string id)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@ExpressID",id),
                                   };
            return GetDataTable("select * from ExpressCompany where ExpressID=@ExpressID and Status<>9", paras, CommandType.Text);
        }

        public bool InsertExpressCompany(string name, string website,string userid)
        {
            string expressID = Guid.NewGuid().ToString();
            SqlParameter[] parms = { 
                                       new SqlParameter("@ExpressID",expressID),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Website",website),
                                       new SqlParameter("@CreateUserID",userid)
                                   };

            string cmdTxt = "insert into ExpressCompany(ExpressID,Name,Website,CreateUserID,CreateTime) values(@ExpressID,@Name,@Website,@CreateUserID,getdate())";

            return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
        }

        public bool UpdateExpressCompany(string id,string name, string website)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@ExpressID",id),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Website",website)
                                   };

            string cmdTxt = "update ExpressCompany set Name=@Name,Website=@Website where ExpressID=@ExpressID";

            return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
        }

       public bool DeleteExpressCompany(string id)
       {
           SqlParameter[] parms = {
                                       new SqlParameter("@ExpressID",id),
                                   };

           string cmdTxt = "update ExpressCompany set status=9 where ExpressID=@ExpressID";

           return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
       }

    }
}
