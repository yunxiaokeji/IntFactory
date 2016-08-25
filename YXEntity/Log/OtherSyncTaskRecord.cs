using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class OtherSyncTaskRecord
    {
        public int AutoID { get; set; }

        public int Type { get; set; }

        public int Status { get; set; } 

        public string OrderID { get; set; }

        public string OtherSysID { get; set; }

        public string ErrorMsg { get; set; }

        public string Content { get; set; }

        public string ClientID { get; set; }

        public string Operater { get; set; }

        public string Reamrk { get; set; }

        public string CreateUserID { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime SyncTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
