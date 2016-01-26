using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class CustomSourceEntity
    {
        [Property("Lower")] 
        public string SourceID { get; set; }

        public string SourceCode { get; set; }

        public string SourceName { get; set; }

        public int IsSystem { get; set; }

        public int IsChoose { get; set; }

        public int Status { get; set; }

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
