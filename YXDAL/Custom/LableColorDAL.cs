﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL.Custom
{
    public class LableColorDAL : BaseDAL
    {
        public static LableColorDAL BaseProvider = new LableColorDAL();


        public DataTable GetLableColor(string tableName, string clientid, int colorid = 0)
        {
            string sqlText = "select  *  from " + tableName + " where status <>9 and ClientID=@ClientID ";
            SqlParameter[] paras = { new SqlParameter("@ClientID", clientid), };
            if (colorid > 0)
            {
                paras.SetValue(new SqlParameter("@ColorID", colorid), paras.Length);
                sqlText += " and ColorID=@ColorID";
            }
            sqlText += "   order by ColorID asc ";
            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        public bool ExistLableColor(string tableName, string clientid, int colorid)
        {
            string sqlText = string.Empty;
            if (tableName.ToLower() == "ordertask")
            {
                sqlText = "select count(ColorMark)  from " + tableName + " where status <>9 and ClientID=@ClientID and ColorMark=@ColorID";
            }
            else
            { 
                sqlText = "select count(Mark)  from " + tableName + " where status <>9 and ClientID=@ClientID and Mark=@ColorID";
            }

            SqlParameter[] paras = { new SqlParameter("@ClientID", clientid),
                                   new SqlParameter("@ColorID", colorid)
                                   };

            return (int)ExecuteScalar(sqlText, paras, CommandType.Text)>0;
        }

        public int InsertLableColor(string procName, string colorName, string colorValue, string clientid, string userid, int status = 0)
        {
            int result = 0;
            SqlParameter[] paras = {  new SqlParameter("@Result",result),
                                     new SqlParameter("@ColorName",colorName),
                                     new SqlParameter("@ColorValue",colorValue), 
                                     new SqlParameter("@CreateUserID" , userid), 
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;            
            ExecuteNonQuery(procName, paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result;
        } 

        public bool UpdateLableColor(string tableName, string clientid, int colorid, string colorName, string colorValue, string updateUserId)
        {
            SqlParameter[] paras = {  
                                     new SqlParameter("@ColorID",colorid),
                                     new SqlParameter("@ColorName",colorName),
                                     new SqlParameter("@ColorValue" , colorValue),
                                     new SqlParameter("@UpdateTime" , DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")), 
                                     new SqlParameter("@UpdateUserID" , updateUserId),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            string updateSql ="update "+tableName+" set ColorName=@ColorName,ColorValue=@ColorValue,UpdateUserID=@UpdateUserID,UpdateTime=@UpdateTime where  ClientID=@ClientID and ColorID=@ColorID";
            return ExecuteNonQuery(updateSql, paras, CommandType.Text) > 0;
        }

        public bool DeleteLableColor(string tableName, int status, int colorid, string clientid, string updateuserid)
        {
            SqlParameter[] paras = {  
                                     new SqlParameter("@ColorID",colorid),
                                     new SqlParameter("@UpdateTime" , DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")), 
                                     new SqlParameter("@UpdateUserID" , updateuserid),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            string updateSql =
                "update " + tableName + " set Status=@Status,UpdateUserID=@UpdateUserID,UpdateTime=@UpdateTime where ClientID=@ClientID and ColorID=@ColorID";
            return ExecuteNonQuery(updateSql, paras, CommandType.Text) > 0;
        }
    }
}
