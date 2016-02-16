using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    [Serializable]
    public class OrderStageEntity
    {
        [Property("Lower")]
        public string StageID { get; set; }

        public string StageName { get; set; }

        public decimal Probability { get; set; }

        public int Sort { get; set; }

        public int Mark { get; set; }

        public int Status { get; set; }

        [Property("Lower")]
        public string ProcessID { get; set; }

        [Property("Lower")]
        public string PID { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        public string OwnerID { get; set; }

        public Users Owner { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public List<StageItemEntity> StageItem { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
