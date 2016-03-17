using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class AliOrderDownloadPlan
    {
        public int AutoID { get; set; }

        public string PlanID { get; set; }

        public string UserID { get; set; }

        public string MemberID { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public int Status { get; set; }

        public DateTime FentSuccessEndTime { get; set; }

        public DateTime BulkSuccessEndTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserID { get; set; }

        public string AgentID { get; set; }

        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
