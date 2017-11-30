using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class UserTaskQuantityRptEntity
    {
        public string UserID { get; set; }

        public string UserName { get; set; }

        public int NoBeginQuantity { get; set; }

        public int Processing { get; set; }

        public int ExpiredProcessing { get; set; }

        public int OverQuantity { get; set; }

        public int ExpiredOver { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
