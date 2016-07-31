using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class AgentOrderDAL : BaseDAL
    {
        public static AgentOrderDAL BaseProvider = new AgentOrderDAL();

        public bool InvalidApplyReturnProduct(string orderid, string userid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_InvalidApplyReturnProduct", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public bool AuditApplyReturnProduct(string orderid, string wareid, string userid,string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                       new SqlParameter("@WareID",wareid),
                                       new SqlParameter("@OrderID",orderid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_AuditApplyReturnProduct", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }
    }
}
