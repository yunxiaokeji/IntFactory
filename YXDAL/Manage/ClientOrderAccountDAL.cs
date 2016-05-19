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
        #endregion

        #region 修改
        
        #endregion

        #region 新增
        public bool InsertClientOrderAccount(string orderID, int payType, decimal realAmount, int type, string clientID, string userID)
        { 
            SqlParameter[] parms = { 
                                       new SqlParameter("@OrderID",orderID),
                                       new SqlParameter("@PayType",payType),
                                       new SqlParameter("@RealAmount",realAmount),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@CreateUserID",userID)
                                   };
            parms[0].Direction = ParameterDirection.Output;
            int result= ExecuteNonQuery("insert into ClientOrderAccount (OrderID,PayType,Type,RealAmount,ClientID,CreateUserID) values(@OrderID,@PayType,@Type,@RealAmount,@ClientID,@CreateUserID) ", parms, CommandType.Text);
            return result>0?true:false;
        }

        #endregion
    }
}
