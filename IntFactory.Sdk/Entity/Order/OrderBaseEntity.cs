using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactory.Sdk
{
    public class OrderBaseEntity
    {
        public string orderID;

        public string platemaking;

        public string plateRemark;

        List<ProductBaseEntity> details;
    }
}
