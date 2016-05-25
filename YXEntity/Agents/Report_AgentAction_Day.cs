using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class Report_AgentAction_Day
    {
        public int AutoID { get; set; }

        public int CustomerCount { get; set; }

        public int OrdersCount { get; set; }

        public int ProductCount { get; set; }

        public int UsersCount { get; set; }

        public int AgentCount { get; set; }

        public int OpportunityCount { get; set; }

        public int PurchaseCount { get; set; }

        public int WarehousingCount { get; set; }
        public int TaskCount { get; set; }
        public int DownOrderCount { get; set; }
        public int ProductOrderCount { get; set; }
        public int UserNum { get; set; }
        public decimal Vitality { get; set; }
        public string AgentID { get; set; }

        public string ClientID { get; set; }

        public string CompanyName { get; set; }
        public string ClientCode { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
