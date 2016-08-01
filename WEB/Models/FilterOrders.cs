using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IntFactoryEnum;
using IntFactoryEntity;

namespace YXERP.Models
{
    [Serializable]
    public class FilterOrders
    {
        public EnumOrderSearchType SearchOrderType { get; set; }

        public EnumSearchType SearchType { get; set; }

        public string TypeID { get; set; }

        public int Status { get; set; }

        public int PayStatus { get; set; }

        public int WarningStatus { get; set; }

        public int OrderStatus { get; set; }

        public int ReturnStatus { get; set; }

        public int SourceType { get; set; }

        public string UserID { get; set; }

        public string AgentID { get; set; }

        public string TeamID { get; set; }

        public int Mark { get; set; }

        public string Keywords { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public string StageID { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public bool IsAsc { get; set; }

        public string OrderBy { get; set; }

        public string EntrustType { get; set; }

    }
}