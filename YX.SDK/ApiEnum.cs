using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlibabaSdk
{
    public enum FentOrderStatus {
        [DescriptionAttribute("FENT")]
        FENT,
        [DescriptionAttribute("PRICING")]
        PRICING,
        [DescriptionAttribute("SEALED")]
        SEALED,
        [DescriptionAttribute("CLOSE")]
        CLOSE
    }

    public enum BulkOrderStatus
    {
        [DescriptionAttribute("PRODUCING")]
        PRODUCING,
        [DescriptionAttribute("PRODUCED")]
        PRODUCED,
        [DescriptionAttribute("CLOSE")]
        CLOSE
    }
}
