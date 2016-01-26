using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IntFactoryEnum;
using IntFactoryEntity;

namespace YXERP.Models
{
    [Serializable]
    public class FilterProduct
    {
        public string CategoryID { get; set; }

        public string BeginPrice { get; set; }

        public string EndPrice { get; set; }

        public string Keywords { get; set; }

        public string OrderBy { get; set; }

        public int DocType { get; set; }

        public bool IsAsc { get; set; }

        public int PageIndex { get; set; }

        public string ProviderID { get; set; }

        public List<FilterAttr> Attrs { get; set; }

    }
}