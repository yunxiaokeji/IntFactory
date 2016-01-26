using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class UpcomingsEntity
    {
        public int DocType { get; set; }

        public int Status { get; set; }

        public int SendStatus { get; set; }

        public int ReturnStatus { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
