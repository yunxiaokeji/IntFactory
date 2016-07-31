using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using IntFactoryEntity;
using IntFactoryDAL;
using IntFactoryEnum;


namespace IntFactoryBusiness
{
    public class AgentOrderBusiness
    {
        public static AgentOrderBusiness BaseBusiness = new AgentOrderBusiness();

        #region 编辑

        public bool ConfirmAgentOrderOut(string orderid, string wareid, int issend, string expressid, string expresscode, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            bool bl = AgentOrderDAL.BaseProvider.ConfirmAgentOrderOut(orderid, wareid, issend, expressid, expresscode, userid, agentid, clientid, ref result, ref errinfo);
            if (bl)
            {
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.StockOut, EnumLogType.Create, "", userid, agentid, clientid);
            }
            return bl;
        }

        public bool ConfirmAgentOrderSend(string orderid, string expressid, string expresscode, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return AgentOrderDAL.BaseProvider.ConfirmAgentOrderSend(orderid, expressid, expresscode, userid, agentid, clientid, ref result, ref errinfo);
        }

        public bool InvalidApplyReturn(string orderid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return AgentOrderDAL.BaseProvider.InvalidApplyReturn(orderid, userid, agentid, clientid, ref result, ref errinfo);
        }

        public bool AuditApplyReturn(string orderid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return AgentOrderDAL.BaseProvider.AuditApplyReturn(orderid, userid, agentid, clientid, ref result, ref errinfo);
        }

        public bool InvalidApplyReturnProduct(string orderid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return AgentOrderDAL.BaseProvider.InvalidApplyReturnProduct(orderid, userid, agentid, clientid, ref result, ref errinfo);
        }

        public bool AuditApplyReturnProduct(string orderid, string wareid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return AgentOrderDAL.BaseProvider.AuditApplyReturnProduct(orderid, wareid, userid, agentid, clientid, ref result, ref errinfo);
        }

        #endregion
    }
}
