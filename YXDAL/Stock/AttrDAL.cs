using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL
{
    public class AttrDAL : BaseDAL
    {
        public static AttrDAL BaseProvider = new AttrDAL();

        #region 属性

        public DataTable GetAttrs()
        {
            DataTable dt = GetDataTable("select * from ProductAttr where  Status<>9");
            return dt;
        }

        public DataSet GetAttrList(string categoryid, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@CategoryID", categoryid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetAttrList", paras, CommandType.StoredProcedure, "Attrs|Values");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;

        }

        public DataTable GetAttrsByCategoryID(string categoryid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@CategoryID", categoryid)
                                   };

            return GetDataTable("P_GetAttrsByCategoryID", paras, CommandType.StoredProcedure);

        }

        public DataSet GetAttrByID(string attrid)
        {
            SqlParameter[] paras = { new SqlParameter("@AttrID", attrid) };
            DataSet ds = GetDataSet("P_GetAttrByID", paras, CommandType.StoredProcedure, "Attrs|Values");
            return ds;
        }

        public DataTable GetAttrValuesByAttrID(string attrid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@AttrID", attrid)
                                   };
            DataTable dt = GetDataTable("select * from AttrValue where Status<>9 and AttrID=@AttrID Order by Sort asc ,AutoID desc", paras, CommandType.Text);
            return dt;
        }

        public DataSet GetTaskPlateAttrByCategoryID(string categoryid)
        {
            SqlParameter[] paras = { new SqlParameter("@CategoryID", categoryid) };
            DataSet ds = GetDataSet("P_GetTaskPlateAttrByCategoryID", paras, CommandType.StoredProcedure, "Attrs|Values");
            return ds;
        }

        public bool AddAttr(string attrid, string attrname, string description, string categoryid, int type, string operateid)
        {
            string sqlText = "P_AddAttr";
            SqlParameter[] paras = { 
                                     new SqlParameter("@AttrID" , attrid),
                                     new SqlParameter("@AttrName" , attrname),
                                     new SqlParameter("@Description" , description),
                                     new SqlParameter("@CategoryID" , categoryid),
                                     new SqlParameter("@Type" , type),
                                     new SqlParameter("@CreateUserID" , operateid)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.StoredProcedure) > 0;
        }

        public bool AddAttrValue(string valueid, string valuename, string attrid, string operateid)
        {
            string sqlText = "INSERT INTO AttrValue([ValueID] ,[ValueName],[Status],[AttrID],CreateUserID) "
                                             + "values(@ValueID ,@ValueName,1,@AttrID,@CreateUserID) ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ValueID" , valueid),
                                     new SqlParameter("@ValueName" , valuename),
                                     new SqlParameter("@AttrID" , attrid),
                                     new SqlParameter("@CreateUserID" , operateid)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool UpdateProductAttr(string attrID, string attrName, string description)
        {
            string sqlText = "Update ProductAttr set [AttrName]=@AttrName,[Description]=@Description  where [AttrID]=@AttrID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@AttrID",attrID),
                                     new SqlParameter("@AttrName" , attrName),
                                     new SqlParameter("@Description" , description),
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool UpdateAttrValue(string ValueID, string ValueName)
        {
            string sqlText = "Update AttrValue set [ValueName]=@ValueName  where [ValueID]=@ValueID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ValueID",ValueID),
                                     new SqlParameter("@ValueName" , ValueName),
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool UpdateProductAttrStatus(string attrid, int status)
        {
            string sqlText = "Update ProductAttr set Status=@Status,UpdateTime=getdate()  where [AttrID]=@AttrID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@AttrID",attrid),
                                     new SqlParameter("@Status" , status)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool UpdateAttrValueStatus(string valueid, int status)
        {
            string sqlText = "Update AttrValue set Status=@Status,UpdateTime=getdate()  where [ValueID]=@ValueID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ValueID",valueid),
                                     new SqlParameter("@Status" , status)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool UpdateCategoryAttrStatus(string categoryid, string attrid, int status, int type)
        {
            string sqlText = "Update CategoryAttr set Status=@Status,UpdateTime=getdate()  where [AttrID]=@AttrID and CategoryID=@CategoryID and Type=@Type";
            SqlParameter[] paras = { 
                                     new SqlParameter("@CategoryID",categoryid),
                                     new SqlParameter("@AttrID",attrid),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@Type" , type)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        #endregion
    }
}
