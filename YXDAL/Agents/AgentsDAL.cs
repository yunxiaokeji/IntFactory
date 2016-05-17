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
        public DataTable GetAgentDetail(string agentID)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@AgentID",agentID),
                                   };
            return GetDataTable("select * from Agents where AgentID=@AgentID", paras, CommandType.Text);
        }

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
        public DataTable GetAgentActionReportPageList(string keyword, string beginDate, string endDate,string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = { 
                                    new SqlParameter("@totalCount",SqlDbType.Int),
                                    new SqlParameter("@pageCount",SqlDbType.Int),
                                    new SqlParameter("@Keyword",keyword),
                                    new SqlParameter("@PageSize",pageSize),
                                    new SqlParameter("@pageIndex",pageIndex), 
                                    new SqlParameter("@BeginDate",beginDate),
                                    new SqlParameter("@EndDate",endDate),
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

        #region 编辑

        public bool ClientAgentAuthorize(string agentID, int userQuantity, DateTime endTime)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@UserQuantity",userQuantity),
                                       new SqlParameter("@EndTime",endTime)
                                   };

            string cmdText = "update Agents set  UserQuantity=@UserQuantity,EndTime=@EndTime where AgentID=@AgentID";

            return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }

        public bool AddClientAgentUserQuantity(string agentID, int quantity)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@UserQuantity",quantity)
                                   };

            string cmdText = "update Agents set  UserQuantity+=@UserQuantity where  AgentID=@AgentID";

            return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }

        public bool SetClientAgentEndTime(string agentID, DateTime endTime)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@EndTime",endTime)
                                   };

            string cmdText = "update Agents set EndTime=@EndTime where  AgentID=@AgentID";

            return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }

        public bool UpdateAgentKey(string agentID, string key)
        {
            SqlParameter[] parms = { 
                                       new SqlParameter("@AgentID",agentID),
                                       new SqlParameter("@Key",key)
                                   };

            string cmdText = "update Agents set AgentKey=@Key where  AgentID=@AgentID";

            return ExecuteNonQuery(cmdText, parms, CommandType.Text) > 0;
        }
        #endregion
    }
}
