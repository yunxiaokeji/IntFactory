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

        public DataTable GetAgentActionReport(string keyword, string beginDate, string endDate)
        {

            SqlParameter[] paras = { 
                                    new SqlParameter("@Keyword",keyword),
                                    new SqlParameter("@BeginDate",beginDate),
                                    new SqlParameter("@EndDate",endDate)
                                   };
            return GetDataTable("M_Get_Report_AgentAction_Day", paras, CommandType.StoredProcedure);
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
