using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YXERP.Models
{
    public class OrderGoodsModel
    {
        public string OrderID { get; set; }

        public List<IntFactoryEntity.ProductDetail> OrderGoods { get; set; }
    }
}