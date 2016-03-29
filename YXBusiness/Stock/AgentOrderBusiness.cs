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

        #region 查询

        public List<AgentOrderEntity> GetAgentOrders(string searchagentid, EnumOrderStageStatus status, EnumSendStatus sendstatus, EnumReturnStatus returnstatus, string keywords, string begintime, string endtime, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            DataSet ds = AgentOrderDAL.BaseProvider.GetAgentOrders(searchagentid, (int)status, (int)sendstatus, (int)returnstatus, keywords, begintime, endtime, pageSize, pageIndex, ref totalCount, ref pageCount, agentid, clientid);

            List<AgentOrderEntity> list = new List<AgentOrderEntity>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AgentOrderEntity model = new AgentOrderEntity();
                model.FillData(dr);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                model.SendStatusStr = CommonBusiness.GetEnumDesc((EnumSendStatus)model.SendStatus);

                model.ExpressTypeStr = CommonBusiness.GetEnumDesc((EnumExpressType)model.ExpressType);

                if(!string.IsNullOrEmpty(model.ExpressID))
                {
                    model.ExpressCompany = Manage.ExpressCompanyBusiness.GetExpressCompanyDetail(model.ExpressID);
                }

                model.City = CommonBusiness.GetCityByCode(model.CityCode);
                if (model.City != null) 
                {
                    model.Address = model.City.Province + model.City.City + model.City.Counties + model.Address;
                }

                list.Add(model);
            }
            return list;
        }

        public AgentOrderEntity GetAgentOrderByID(string orderid, string agentid, string clientid)
        {
            DataSet ds = AgentOrderDAL.BaseProvider.GetAgentOrderByID(orderid, agentid, clientid);
            AgentOrderEntity model = new AgentOrderEntity();
            if (ds.Tables["Order"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Order"].Rows[0]);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStageStatus)model.Status);

                model.ExpressTypeStr = CommonBusiness.GetEnumDesc((EnumExpressType)model.ExpressType);

                model.City = CommonBusiness.GetCityByCode(model.CityCode);
                if (model.City != null)
                {
                    model.Address = model.City.Province + model.City.City + model.City.Counties + model.Address;
                }

                model.Details = new List<AgentOrderDetail>();
                if (ds.Tables["Details"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Details"].Rows)
                    {
                        AgentOrderDetail detail = new AgentOrderDetail();
                        detail.FillData(dr);
                        model.Details.Add(detail);
                    }
                }
            }
            return model;
        }

        #endregion

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
