using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class OrderPriceRange
    {
        public int AutoID { get; set; }

        [Property("Lower")] 
        public string RangeID { get; set; }

        [Property("Lower")] 
        public string OrderID { get; set; }

        public int MinQuantity { get; set; }

        public int MaxQuantity { get; set; }

        public decimal Price { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")] 
        public string ClientID { get; set; }

        [Property("Lower")] 
        public string CreateUserID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
