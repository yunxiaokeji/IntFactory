using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class UserWorkLoadRptEntity
    {
        public string UserID { get; set; }

        public string UserName { get; set; }

        public int CutQuantity { get; set; }

        public int SewnQuantity { get; set; }

        public int SewnReturn { get; set; }

        public int DocType { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
