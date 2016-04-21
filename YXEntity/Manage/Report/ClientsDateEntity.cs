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
    public class ClientsLoginEntity
    {
        public string Name { get; set; }

        public List<ClientsLoginItem> Items{ get; set; }
    }
    public class ClientsLoginItem 
    { 
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
