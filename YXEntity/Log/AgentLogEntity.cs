using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class AgentActionEntity
    {
        public string AgentID { get; set; }

        public string Date { get; set; }

        public decimal TotalMoney { get; set; }

        public int OrderCount { get; set; }

        public int CustomerCount { get; set; }

        public List<ActionTypeEntity> Actions { get; set; }

    }

    public class ActionTypeEntity
    {
        public int ObjectType { get; set; }

        public int OrderType { get; set; }

        public int Status { get; set; }

        public int OrderCount { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
