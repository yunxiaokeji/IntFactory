using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class KanbanRptEntity
    {
        public string DataType { get; set; }

        public decimal Total { get; set; }

        public decimal Today { get; set; }

        public decimal LastDay { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
