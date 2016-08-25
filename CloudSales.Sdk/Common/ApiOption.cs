using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSales.Sdk.Common
{
    public enum ApiOption
    {
        [Description("getToken")]
        getToken,

        [Description("/api/stock/addstockpartin")]
        addStockPartIn
        
    }
}
