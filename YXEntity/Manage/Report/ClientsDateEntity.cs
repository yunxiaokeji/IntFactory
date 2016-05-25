using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Manage.Report
{
    public class ClientsDateEntity
    {
        public string Name { get; set; }

        public int Value { get; set; }
    }
    public class ClientsBaseEntity
    {
        public string Name { get; set; }

        public List<ClientsItem> Items{ get; set; }
    }
    public class ClientsItem 
    { 
        public string Name { get; set; }

        public int Value { get; set; }
    }

    public class ClientVitalityItem
    {
        public string Name { get; set; }

        public decimal Value { get; set; }
    }
    public class ClientVitalityEntity
    {
        public string Name { get; set; }

        public List<ClientVitalityItem> Items { get; set; }
    }
}
