using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IntFactoryEnum;
using IntFactoryEntity;

namespace YXERP.Models
{
    [Serializable]
    public class FilterTasks
    {
        public string keyWords = string.Empty;

        public int filtertype = 1;

        public string userID { get; set; }

        public int taskType = -1;

        public int colorMark = -1;

        public int status = 1;

        public int finishStatus = 0;

        public int invoiceStatus = -1;

        public string beginDate = string.Empty;

        public string endDate = string.Empty;

        public int orderType = -1;

        public string orderProcessID = "-1";

        public string orderStageID = "-1";

        public int taskOrderColumn = 0;

        public int isAsc = 0;

        public int pageSize = 10;

        public int pageIndex = 1;

    }
}