using IntFactoryBusiness.Manage;
using IntFactoryDAL;
using IntFactoryEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryBusiness
{
    public class OrderRPTBusiness
    {
        public static OrderRPTBusiness BaseBusiness = new OrderRPTBusiness();

        public List<OrderProductionRptEntity> GetOrderProductionRPT(int timeType, int entrustType, string begintime, string endtime, string keyWords, string userid, string teamid, string clientid)
        {
            if (string.IsNullOrEmpty(begintime))
            {
                begintime = "1990-1-1 00:00:00";
            }
            else
            {
                begintime = Convert.ToDateTime(begintime).ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (string.IsNullOrEmpty(endtime))
            {
                endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            DataTable dt = RPTDAL.BaseProvider.GetOrderProductionRPT(timeType, entrustType, begintime, endtime, keyWords, userid, teamid, clientid);

            List<OrderProductionRptEntity> list = new List<OrderProductionRptEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                OrderProductionRptEntity model = new OrderProductionRptEntity();
                model.FillData(dr);
                model.UserName = OrganizationBusiness.GetUserCacheByUserID(model.OwnerID, clientid).Name;

                if (!string.IsNullOrEmpty(model.EntrustClientID) && model.EntrustClientID.ToLower() == clientid.ToLower())
                {
                    model.IsEntrustClient = true;
                    var client = ClientBusiness.GetClientDetailBase(model.ClientID);
                    model.CustomerID = client.ClientID;
                    model.CustomerName = client.CompanyName + "（委托）";
                }

                list.Add(model);
            }
            return list;
        }

        public List<UserWorkLoadRptEntity> GetUserLoadReport(string begintime, string endtime, int docType, string keyWords, string userid, string teamid, string clientid)
        {
            if (string.IsNullOrEmpty(begintime))
            {
                begintime = "1990-1-1 00:00:00";
            }
            else
            {
                begintime = Convert.ToDateTime(begintime).ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (string.IsNullOrEmpty(endtime))
            {
                endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            DataTable dt = RPTDAL.BaseProvider.GetUserLoadReport(begintime, endtime, docType, keyWords, userid, teamid, clientid);

            List<UserWorkLoadRptEntity> list = new List<UserWorkLoadRptEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                UserWorkLoadRptEntity model = new UserWorkLoadRptEntity();
                model.FillData(dr);
                model.UserName = OrganizationBusiness.GetUserCacheByUserID(model.UserID, clientid).Name;

                if (!string.IsNullOrEmpty(model.EntrustClientID) && model.EntrustClientID.ToLower() == clientid.ToLower())
                {
                    model.IsEntrustClient = true;
                    var client = ClientBusiness.GetClientDetailBase(model.ClientID);
                    model.CustomerID = client.ClientID;
                    model.CustomerName = client.CompanyName + "（委托）";
                }
                list.Add(model);
            }
            return list;
        }

        public List<UserWorkLoadRptEntity> GetUserLoadDetailByOrderID(string begintime, string endtime, int docType, string userid, string orderid)
        {
            if (string.IsNullOrEmpty(begintime))
            {
                begintime = "1990-1-1 00:00:00";
            }
            else
            {
                begintime = Convert.ToDateTime(begintime).ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (string.IsNullOrEmpty(endtime))
            {
                endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            DataTable dt = RPTDAL.BaseProvider.GetUserLoadDetailByOrderID(begintime, endtime, docType, userid, orderid);

            List<UserWorkLoadRptEntity> list = new List<UserWorkLoadRptEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                UserWorkLoadRptEntity model = new UserWorkLoadRptEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<OrderTabCountEntity> GetOrderTabCount(string userid, string clientid)
        {
            DataTable dt = SalesRPTDAL.BaseProvider.GetOrderTabCount(userid, clientid);

            List<OrderTabCountEntity> list = new List<OrderTabCountEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                OrderTabCountEntity model = new OrderTabCountEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }
    }
}
