using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace IntFactoryDAL.Manage
{
    public class IndustryDAL : BaseDAL
    {
        #region 查询

        public DataTable GetIndustrys(string clientid = "")
        {

            //SqlParameter[] paras = { 
            //                        new SqlParameter("@ClientID",clientid)
            //                       };
            return GetDataTable("select * from Industry where Status<>9");
        }

        public DataTable GetIndustryDetail(string id)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@IndustryID",id)
                                   };
            return GetDataTable("select * from Industry where IndustryID=@IndustryID and Status<>9",paras,CommandType.Text);
        }
        #endregion

        #region 添加

        public string InsertIndustry(string name, string description, string userid, string clientid)
        {
            string industryID = Guid.NewGuid().ToString();
            string sql = "Insert into Industry(IndustryID,Name,Description,CreateUserID,ClientID) values(@IndustryID,@Name,@Description,@CreateUserID,@ClientID)";
            SqlParameter[] paras = { 
                                       new SqlParameter("@IndustryID",industryID),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",string.IsNullOrEmpty(userid) ? (object)DBNull.Value : userid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0 ? industryID : "";
        }

        #endregion

        #region 编辑

        public bool UpdateIndustry(string id,string name, string description)
        {
            string sql = "update Industry set name=@Name,Description=@Description where IndustryID=@IndustryID";
            SqlParameter[] paras = { 
                                       new SqlParameter("@IndustryID",id),
                                       new SqlParameter("@Name",name),
                                       new SqlParameter("@Description",description)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.Text)>0;
        }

        #endregion
    }
}
