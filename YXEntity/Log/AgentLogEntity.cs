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

        public List<ActionTypeEntity> Actions { get; set; }

    }

    public class ActionTypeEntity
    {
        public int ObjectType { get; set; }

        public decimal DayValue { get; set; }

        public decimal WeekValue { get; set; }

        public decimal MonthValue { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
