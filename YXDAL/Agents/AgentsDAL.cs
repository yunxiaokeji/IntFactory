using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryDAL.Agents
{
    public class AgentsDAL:BaseDAL
    {
        public static AgentsDAL BaseProvider = new AgentsDAL();

        #region 查询

        public DataTable GetAgentActionReport(string keyword, string beginDate, string endDate,string clientID)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@Keyword",keyword),
                                    new SqlParameter("@BeginDate",beginDate),
                                    new SqlParameter("@EndDate",endDate),
                                    new SqlParameter("@ClientID",clientID)
                                   };
            return GetDataTable("M_Get_Report_AgentAction_Day", paras, CommandType.StoredProcedure);
        }

        public DataTable GetAgentActionReportPageList(string keyword, string beginDate, string endDate,int type,string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@totalCount",SqlDbType.Int),
                                    new SqlParameter("@pageCount",SqlDbType.Int),
                                    new SqlParameter("@Keyword",keyword),
                                    new SqlParameter("@PageSize",pageSize),
                                    new SqlParameter("@pageIndex",pageIndex), 
                                    new SqlParameter("@BeginDate",beginDate),
                                    new SqlParameter("@EndDate",endDate),
                                    new SqlParameter("@Type",type),
                                    new SqlParameter("@OrderBy",orderBy)
                                   
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;
            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;


            DataTable dt = GetDataTable("M_Get_Report_AgentActionDayPageList", paras, CommandType.StoredProcedure);

            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return dt;
        }
        
        #endregion


    }
}
