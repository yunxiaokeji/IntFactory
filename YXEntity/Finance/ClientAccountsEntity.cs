using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ClientAccountsEntity
    {
        public int Mark { get; set; }

        public int SubjectID { get; set; }

        public decimal HappenMoney { get; set; }

        public decimal EndMoney { get; set; }

        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }
        [Property("Lower")] 
        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        [Property("Lower")] 
        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
