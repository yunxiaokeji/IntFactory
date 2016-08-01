using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL.Manage
{
    public class ModulesProductDAL:BaseDAL
    {
        public static ModulesProductDAL BaseProvider = new ModulesProductDAL();
        public DataTable GetModulesProductDetail(int id)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@AutoID",id),
                                   };
            return GetDataTable("select * from ModulesProduct where AutoID=@AutoID and Status<>9", paras, CommandType.Text);
        }

        public bool InsertModulesProduct(int period, int periodQuantity, int userQuantity, decimal price, 
                                   string description,int type,int userType,  string userid)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@Period",period),
                                       new SqlParameter("@PeriodQuantity",periodQuantity),
                                       new SqlParameter("@UserQuantity",userQuantity),
                                       new SqlParameter("@Price",price),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@UserType",userType),
                                       new SqlParameter("@CreateUserID",userid)
                                   };

            string cmdTxt = "insert into ModulesProduct(ProductID,Period,PeriodQuantity,UserQuantity,Price,Description,Type,userType,CreateUserID,CreateTime,ModulesID) values(NewID(),@Period,@PeriodQuantity,@UserQuantity,@Price,@Description,@Type,@UserType,@CreateUserID,getdate(),'')";

            return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
        }

       public bool UpdateModulesProduct(int autoID, int period, int periodQuantity, int userQuantity, decimal price, 
                                   string description,int type,int userType)
        {
            SqlParameter[] parms = {
                                       new SqlParameter("@AutoID",autoID),
                                       new SqlParameter("@Period",period),
                                       new SqlParameter("@PeriodQuantity",periodQuantity),
                                       new SqlParameter("@UserQuantity",userQuantity),
                                       new SqlParameter("@Price",price),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@UserType",userType)
                                   };

            string cmdTxt = "update ModulesProduct set Period=@Period,PeriodQuantity=@PeriodQuantity,UserQuantity=@UserQuantity,Price=@Price,Description=@Description,type=@Type,userType=@UserType where AutoID=@AutoID";

            return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
        }

       public bool DeleteModulesProduct(int id)
       {
           SqlParameter[] parms = {
                                       new SqlParameter("@AutoID",id),
                                   };

           string cmdTxt = "update ModulesProduct set status=9 where AutoID=@AutoID";

           return ExecuteNonQuery(cmdTxt, parms, CommandType.Text) > 0;
       }
    }
}
