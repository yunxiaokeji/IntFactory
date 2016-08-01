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


        public bool InvalidApplyReturnProduct(string orderid, string userid, string clientid, ref int result, ref string errinfo)
        {
            return AgentOrderDAL.BaseProvider.InvalidApplyReturnProduct(orderid, userid, clientid, ref result, ref errinfo);
        }

        public bool AuditApplyReturnProduct(string orderid, string wareid, string userid, string clientid, ref int result, ref string errinfo)
        {
            return AgentOrderDAL.BaseProvider.AuditApplyReturnProduct(orderid, wareid, userid, clientid, ref result, ref errinfo);
        }

        #endregion
    }
}
