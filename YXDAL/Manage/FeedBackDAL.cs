using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL.Manage
{
    public class FeedBackDAL:BaseDAL
    {
        public static FeedBackDAL BaseProvider = new FeedBackDAL();

        #region 添加

        public bool InsertFeedBack(string title, string contactName, string mobilePhone, int type, string filePath, string remark,
                                   string createUserID)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@Title",title),
                                       new SqlParameter("@ContactName",contactName),
                                       new SqlParameter("@MobilePhone",mobilePhone),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@FilePath",filePath),
                                       new SqlParameter("@Remark",remark),
                                       new SqlParameter("@CreateUserID",createUserID)
                                   };

            string cmdText = "insert into feedback(Title,ContactName,MobilePhone,Type,FilePath,Remark,CreateUserID) values(@Title,@ContactName,@MobilePhone,@Type,@FilePath,@Remark,@CreateUserID)";
            return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }
        #endregion

        public DataTable GetFeedBackDetail(string id)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@AutoID",id)
                                   };
            return GetDataTable("select * from FeedBack where AutoID=@AutoID", paras, CommandType.Text);
        }

        public bool UpdateFeedBackStatus(string id, int status)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@AutoID",id),
                                    new SqlParameter("@Status",status),
                                   };
            return ExecuteNonQuery("update  FeedBack set Status=@Status where AutoID=@AutoID", paras, CommandType.Text) > 0;
        }
    }
}
