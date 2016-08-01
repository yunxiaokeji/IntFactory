using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity;
using IntFactoryDAL.Agents;

namespace IntFactoryBusiness
{
    public class AgentsBusiness
    {

        public static List<Report_AgentAction_Day> GetAgentActionReport(string keyword,string startDate,string endDate ,string clientID) 
        {
            DataTable dt = AgentsDAL.BaseProvider.GetAgentActionReport(keyword,startDate,endDate,clientID);
            List<Report_AgentAction_Day> list = new List<Report_AgentAction_Day>();
            
            foreach (DataRow dr in dt.Rows)
            {
                Report_AgentAction_Day model = new Report_AgentAction_Day();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }
        public static List<Report_AgentAction_Day> GetAgentActionReport(string keyword, string startDate, string endDate,int type,string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            DataTable dt = AgentsDAL.BaseProvider.GetAgentActionReportPageList(keyword, startDate, endDate, type,orderBy,pageSize, pageIndex, ref totalCount, ref pageCount);
            List<Report_AgentAction_Day> list = new List<Report_AgentAction_Day>();

            foreach (DataRow dr in dt.Rows)
            {
                Report_AgentAction_Day model = new Report_AgentAction_Day();
                model.FillData(dr);
                list.Add(model);
            }

            return list;
        }
    }
}
