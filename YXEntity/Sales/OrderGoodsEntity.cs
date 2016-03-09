using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class OrderGoodsEntity
    {
        public string OrderID { get; set; }

        public string ExpressCode { get; set; }

        public List<ProductDetail> OrderGoods { get; set; }
    }
}
