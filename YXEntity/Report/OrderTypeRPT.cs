using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class TypeOrderEntity
    {
        public string GUID { get; set; }

        public string Name { get; set; }

        public string PID { get; set; }

        public List<TypeOrderItem> Types { get; set; }

        public List<TypeOrderEntity> ChildItems { get; set; }
    }

    public class TypeOrderItem
    {
        public string TypeID { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public decimal Money { get; set; }
    }
}
