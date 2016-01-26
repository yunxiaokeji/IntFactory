﻿using System;
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
        public EnumSearchType SearchType { get; set; }

        public string TypeID { get; set; }

        public int Status { get; set; }

        public int PayStatus { get; set; }

        public int InvoiceStatus { get; set; }

        public int ReturnStatus { get; set; }

        public string UserID { get; set; }

        public string AgentID { get; set; }

        public string TeamID { get; set; }

        public string Keywords { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public string StageID { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

    }
}