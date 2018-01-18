﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryDAL;
using IntFactoryEntity;

namespace IntFactoryBusiness
{
    public class TaskRPTBusiness
    {
        public static TaskRPTBusiness BaseBusiness = new TaskRPTBusiness();

        public List<UserTaskQuantityRptEntity> GetUserTaskQuantity(string begintime, string endtime, string userid, string teamid, string clientid)
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
            DataTable dt = RPTDAL.BaseProvider.GetUserTaskQuantity(begintime, endtime, userid, teamid, clientid);

            List<UserTaskQuantityRptEntity> list = new List<UserTaskQuantityRptEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                UserTaskQuantityRptEntity model = new UserTaskQuantityRptEntity();
                model.FillData(dr);
                model.UserName = OrganizationBusiness.GetUserCacheByUserID(model.UserID, clientid).Name;
                list.Add(model);
            }
            return list;
        }

        public List<UserWorkLoadRptEntity> GetUserLoadReport(string begintime, string endtime, int docType, string userid, string teamid, string clientid)
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
            DataTable dt = RPTDAL.BaseProvider.GetUserLoadReport(begintime, endtime, docType, userid, teamid, clientid);

            List<UserWorkLoadRptEntity> list = new List<UserWorkLoadRptEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                UserWorkLoadRptEntity model = new UserWorkLoadRptEntity();
                model.FillData(dr);
                model.UserName = OrganizationBusiness.GetUserCacheByUserID(model.UserID, clientid).Name;
                list.Add(model);
            }
            return list;
        }

        public List<SewnProcessItemEntity> GetUserSewnProcessReport(string begintime, string endtime, string userid, string teamid, string clientid)
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
            DataTable dt = RPTDAL.BaseProvider.GetUserSewnProcessReport(begintime, endtime, userid, teamid, clientid);

            List<SewnProcessItemEntity> list = new List<SewnProcessItemEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                SewnProcessItemEntity model = new SewnProcessItemEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<OrderProductionRptEntity> GetOrderProductionRPT(int timeType, string begintime, string endtime, string keyWords, string userid, string teamid, string clientid)
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
            DataTable dt = RPTDAL.BaseProvider.GetOrderProductionRPT(timeType, begintime, endtime, keyWords, userid, teamid, clientid);

            List<OrderProductionRptEntity> list = new List<OrderProductionRptEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                OrderProductionRptEntity model = new OrderProductionRptEntity();
                model.FillData(dr);
                model.UserName = OrganizationBusiness.GetUserCacheByUserID(model.OwnerID, clientid).Name;
                list.Add(model);
            }
            return list;
        }

        public List<CustomerRateRptEntity> GetCustomerRateRPT(string begintime, string endtime, string keyWords, string clientid)
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
            DataTable dt = RPTDAL.BaseProvider.GetCustomerRateRPT(begintime, endtime, keyWords, clientid);

            List<CustomerRateRptEntity> list = new List<CustomerRateRptEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                CustomerRateRptEntity model = new CustomerRateRptEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }
        
    }
}
