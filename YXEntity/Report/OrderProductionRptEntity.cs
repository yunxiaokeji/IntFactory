using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class OrderProductionRptEntity
    {
        public string OwnerID { get; set; }

        public string OrderID { get; set; }

        public string CustomerID { get; set; }

        public string UserName { get; set; }

        public string IntGoodsCode { get; set; }

        public string CustomerName { get; set; }

        public int OrderQuantity { get; set; }

        public int CutQuantity { get; set; }

        public int Complete { get; set; }

        public int SendQuantity { get; set; }

        public decimal FinalPrice { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
