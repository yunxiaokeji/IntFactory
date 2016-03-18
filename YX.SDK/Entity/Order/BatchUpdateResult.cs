using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlibabaSdk
{
    public class BatchUpdateResult
    {
        public List<string> succeseGodesCodeList
        {
            get;
            set;
        }

        public int error_code = 0;
    }
}
