using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryDAL;
using IntFactoryEntity;
using IntFactoryEntity.Manage.Report;
using IntFactoryBusiness.Manage;

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

        public List<KanbanRptEntity> GetKanbanRPT(string begintime, string endtime,int dateType, string clientid)
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

            string todaybegin, todayend, lastbegin, lastend;
            if (dateType == 1)
            {
                todaybegin = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
                todayend= DateTime.Today.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                lastbegin = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                lastend = DateTime.Today.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (dateType == 2)
            {
                var day = (int)DateTime.Today.DayOfWeek;
                day = day == 0 ? 6 : day - 1;
                todaybegin = DateTime.Today.AddDays(-day).ToString("yyyy-MM-dd HH:mm:ss");
                todayend = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                lastbegin = DateTime.Today.AddDays(-day).AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
                lastend = DateTime.Today.AddDays(-day).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else  
            {
                var day = DateTime.Today.Day - 1;
                todaybegin = DateTime.Today.AddDays(-day).ToString("yyyy-MM-dd HH:mm:ss");
                todayend = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                lastbegin = DateTime.Today.AddDays(-day).AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss");
                lastend = DateTime.Today.AddDays(-day).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            }

            DataTable dt = RPTDAL.BaseProvider.GetKanbanRPT(begintime, endtime, todaybegin, todayend, lastbegin, lastend, clientid);

            List<KanbanRptEntity> list = new List<KanbanRptEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                KanbanRptEntity model = new KanbanRptEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public static List<ClientVitalityItem> GetKanbanItemRPT(int dateType, string itemType,string begintime, string endtime,string clientid)
        {
            if(string.IsNullOrEmpty(begintime))
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
            List<ClientVitalityItem> list = new List<ClientVitalityItem>();

            DataTable dt = RPTDAL.BaseProvider.GetKanbanItemRPT(itemType, dateType, begintime, endtime, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                ClientVitalityItem model = new ClientVitalityItem();
                model.Name = dr["CreateTime"].ToString();
                model.Value = Convert.ToDecimal(dr["TotalNum"]);
                list.Add(model);
            }
            return list;
        }

        public List<OrderTabCountEntity> GetTaskTabCount(string userid, int searchType, string clientid)
        {
            DataTable dt = RPTDAL.BaseProvider.GetTaskTabCount(userid, searchType, clientid);

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
