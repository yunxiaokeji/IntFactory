using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class AliOrderUpdateLog
    {
        public int AutoID { get; set; }

        public string LogID { get; set; }

        public string OrderID { get; set; }

        public string AliOrderCode { get; set; }

        public int OrderType { get; set; }

        public int Status { get; set; }

        public int OrderStatus { get; set; }

        public decimal OrderPrice { get; set; }

        public int FailCount { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime CreateTime { get; set; }

        public string Remark { get; set; }

        public string AgentID { get; set; }

        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
