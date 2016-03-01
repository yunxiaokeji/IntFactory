using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
namespace IntFactoryDAL.Manage
{
    public class SystemDAL : BaseDAL
    {
        public static SystemDAL BaseProvider = new SystemDAL();

        public DataTable  GetSystemMenus()
        {
            return GetDataTable("select * from Menu  where type=2 and IsHide=0");
        }

        public DataTable GetSystemMenu(string menuCode)
        {
            string sqlStr="select * from Menu  where type=2 and MenuCode=@MenuCode";
            SqlParameter[] paras = { 
                                    new SqlParameter("@MenuCode",menuCode),
                                   };

            return GetDataTable(sqlStr, paras, CommandType.Text);
        }


        public bool AddSystemMenu(string menuCode, string name, string controller, string view, string pCode, int sort, int layer, int type)
        {

            string sqlStr = "insert into menu(MenuCode,Name,Controller,[View],PCode,Sort,Layer,Type) values(@MenuCode,@Name,@Controller,@View,@PCode,@Sort,@Layer,@Type)";
            SqlParameter[] paras = { 
                                    new SqlParameter("@MenuCode",menuCode),
                                    new SqlParameter("@Name",name),
                                    new SqlParameter("@Controller",controller),
                                    new SqlParameter("@View",view),
                                    new SqlParameter("@PCode",pCode),
                                    new SqlParameter("@Sort",sort),
                                    new SqlParameter("@Layer",layer),
                                    new SqlParameter("@Type",type)
                                   };

            return ExecuteNonQuery(sqlStr,paras,CommandType.Text)>0;
        }

        public bool UpdateSystemMenu(string menuCode, string name, string controller, string view, int sort)
        {

            string sqlStr = "update  menu set MenuCode=@MenuCode,Name=@Name,Controller=@Controller,[View]=@View,Sort=@Sort where MenuCode=@MenuCode";
            SqlParameter[] paras = { 
                                    new SqlParameter("@MenuCode",menuCode),
                                    new SqlParameter("@Name",name),
                                    new SqlParameter("@Controller",controller),
                                    new SqlParameter("@View",view),
                                    new SqlParameter("@Sort",sort)
                                   };

            return ExecuteNonQuery(sqlStr, paras, CommandType.Text) > 0;
        }

        public bool DeleteSystemMenu(string menuCode)
        {

            string sqlStr = "update  menu set IsHide=1 where MenuCode=@MenuCode";
            SqlParameter[] paras = { 
                                    new SqlParameter("@MenuCode",menuCode)
                                   };

            return ExecuteNonQuery(sqlStr, paras, CommandType.Text) > 0;
        }
    }
}
