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

        public int ActivityCount { get; set; }

        public int ProductCount { get; set; }

        public int UsersCount { get; set; }

        public int AgentCount { get; set; }

        public int OpportunityCount { get; set; }

        public int PurchaseCount { get; set; }

        public int WarehousingCount { get; set; }

        public string AgentID { get; set; }

        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
