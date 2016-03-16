using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlibabaSdk
{
    public class CacheBusiness
    {
        private static Dictionary<string, int> _successOrderCountCache;

        public static Dictionary<string, int> SuccessOrderCountCache
        {
            get
            {
                if (_successOrderCountCache == null)
                {
                    _successOrderCountCache = new Dictionary<string, int>();
                }

                return _successOrderCountCache;
            }
        }
    }
}
