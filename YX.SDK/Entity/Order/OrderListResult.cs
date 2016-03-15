using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlibabaSdk
{
    public class OrderListResult
    {
        public List<OrderEntity> bulkOrderList
        {
            set;
            get;
        }

        public List<OrderEntity> fentOrderList
        {
            set;
            get;
        }

        public int error_code = 0;

    }
}
