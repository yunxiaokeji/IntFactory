using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class StageCustomerEntity
    {
        public string GUID { get; set; }

        public string Name { get; set; }

        public string PID { get; set; }

        public List<StageCustomerItem> Stages { get; set; }

        public List<StageCustomerEntity> ChildItems { get; set; }
    }

    public class StageCustomerItem
    {
        public string StageID { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public decimal Money { get; set; }
    }
}
