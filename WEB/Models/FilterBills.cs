using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IntFactoryEnum;
using IntFactoryEntity;

namespace YXERP.Models
{
    [Serializable]
    public class FilterBills
    {

        public int PayStatus { get; set; }

        public int InvoiceStatus { get; set; }

        public int ReturnStatus { get; set; }

        public string Keywords { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

    }
}