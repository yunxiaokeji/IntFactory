using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class ShoppingCartDAL : BaseDAL
    {
        public static DataTable GetShoppingCart(int ordertype, string guid, string userid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderType",ordertype),
                                     new SqlParameter("@GUID" , guid),
                                     new SqlParameter("@UserID" , userid)
                                   };
            return GetDataTable("P_GetShoppingCart", paras, CommandType.StoredProcedure);
        }

        public static bool AddShoppingCart(int ordertype, string productid, string detailsid, decimal quantity, string unitid, string depotid, string remark, string guid, string userid, string operateip)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderType",ordertype),
                                     new SqlParameter("@ProductDetailID",detailsid),
                                     new SqlParameter("@ProductID" , productid),
                                     new SqlParameter("@Quantity" , quantity),
                                     new SqlParameter("@UnitID" , unitid),
                                     new SqlParameter("@DepotID" , depotid),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@GUID" , guid),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip)
                                   };
            return ExecuteNonQuery("P_AddShoppingCart", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool UpdateCartQuantity(string autoid, double quantity, string guid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Quantity",quantity),
                                     new SqlParameter("@GUID" , guid)
                                   };
            return ExecuteNonQuery("P_UpdateCartQuantity", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool UpdateCartPrice(string autoid, decimal price, string guid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@Price",price),
                                     new SqlParameter("@GUID" , guid)
                                   };
            return ExecuteNonQuery("P_UpdateCartPrice", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool DeleteCart(string autoid, string guid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@GUID" , guid)
                                   };
            return ExecuteNonQuery("P_DeleteCart", paras, CommandType.StoredProcedure) > 0;
        }

    }
}
