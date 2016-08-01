using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class AliOrderDownloadLog
    {
        public int AutoID { get; set; }

        public int OrderType { get; set; }

        public int IsSuccess { get; set; }

        public int DownType { get; set; }

        public int TotalCount { get; set; }

        public int SuccessCount { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CreateTime { get; set; }

        public string Remark { get; set; }

        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
