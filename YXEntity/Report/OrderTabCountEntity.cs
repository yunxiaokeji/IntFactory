using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class OrderTabCountEntity
    {
        public int OrderType { get; set; }

        public int OrderStatus { get; set; }

        public int OrderQuantity { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
