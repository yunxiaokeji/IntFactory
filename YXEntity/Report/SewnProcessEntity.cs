using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class SewnProcessRptEntity
    {
        public string UserID { get; set; }

        public string UserName { get; set; }

        public List<SewnProcessItemEntity> ProcessItems { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }

    public class SewnProcessItemEntity
    {
        public string OwnerID { get; set; }

        public string ProcessID { get; set; }

        public string ProcessName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int ReturnQuantity { get; set; }

        public decimal ReturnPrice { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
    public class ProcessItemEntity
    {

        public string ProcessID { get; set; }

        public string ProcessName { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
