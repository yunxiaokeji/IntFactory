using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL.Manage
{
    public class ClientOrderAccountDAL:BaseDAL
    {
        public static ClientOrderAccountDAL BaseProvider = new ClientOrderAccountDAL();

        #region 查询
        public DataTable ClientOrderAccountByClientIDDAL(string clientID )
        {
            SqlParameter[] paras = {  new SqlParameter("@ClientID",clientID) };
            string sql = @"select * from ClientOrderAccountDAL  where ClientID=@ClientID ";
            return GetDataTable(sql, paras, CommandType.Text);
        }
        public DataTable ClientOrderAccountByOrderIDDAL( string orderID)
        {
            SqlParameter[] paras = { new SqlParameter("@OrderID", orderID) };
            string sql = @"select * from ClientOrderAccountDAL  where OrderID=@OrderID ";
            return GetDataTable(sql, paras, CommandType.Text);
        }
        public DataTable ClientOrderAccountByAutoIDDAL(string autoID)
        {
            SqlParameter[] paras = { new SqlParameter("@AutoID", autoID) };
            string sql = @"select * from ClientOrderAccountDAL  where AutoID=@AutoID ";
            return GetDataTable(sql, paras, CommandType.Text);
        }
        #endregion

        #region 修改
        public int UpdateClientOrderAccountStatus(string autoID, int status)
        {
            int result = 0;
            SqlParameter[] parms = { new SqlParameter("@Result",result),
                                       new SqlParameter("@PayStatus",status),
                                       new SqlParameter("@AutoID",autoID)
                                   };
            parms[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("M_UpdateClientOrderAccountStatus ", parms, CommandType.StoredProcedure);
            result = Convert.ToInt32(parms[0].Value);
            return result ;
        }
        #endregion

        #region 新增
        public int InsertClientOrderAccount(string orderID, int payType, decimal realAmount, int type, string clientID, string userID,string remark)
        {
            int result = 0;
            SqlParameter[] parms = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@OrderID",orderID),
                                       new SqlParameter("@PayType",payType),
                                       new SqlParameter("@RealAmount",realAmount),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@CreateUserID",userID),
                                        new SqlParameter("@Remark",remark==null?"":remark)
                                   };
            parms[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("M_AddClientOrderAccount ", parms, CommandType.StoredProcedure);
            result = Convert.ToInt32(parms[0].Value);
            return result ;
        }

        #endregion
    }
}
