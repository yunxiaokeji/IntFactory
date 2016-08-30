using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class UserRPTDAL : BaseDAL
    {
        public static UserRPTDAL BaseProvider = new UserRPTDAL();

        public DataTable GetUserLoadReport(string begintime, string endtime, string UserID, string TeamID, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetUserWorkloadDate", paras, CommandType.StoredProcedure);
            return dt;
        }
    }
}
