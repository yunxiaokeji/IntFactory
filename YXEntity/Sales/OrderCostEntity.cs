using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class OrderCostEntity
    {
        public int AutoID { get; set; }

        [Property("Lower")]
        public string OrderID { get; set; }

        public decimal Price { get; set; }

        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }

        public string ProcessName { get; set; }

        public string ProcessID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
