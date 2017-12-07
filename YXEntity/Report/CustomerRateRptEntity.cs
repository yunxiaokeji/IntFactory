using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class CustomerRateRptEntity
    {
        public string CustomerName { get; set; }

        public int DemandCount { get; set; }

        public int DYCount { get; set; }

        public int DHCount { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
