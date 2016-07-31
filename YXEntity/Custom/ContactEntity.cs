using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ContactEntity
    {
        [Property("Lower")]
        public string ContactID { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public string CityCode { get; set; }

        public CityEntity City { get; set; }

        public string Address { get; set; }

        public string MobilePhone { get; set; }

        public string OfficePhone { get; set; }

        public string Email { get; set; }

        public string Jobs { get; set; }

        public DateTime Birthday { get; set; }

        public int Age { get; set; }

        public int Sex { get; set; }

        public int Education { get; set; }

        public string Description { get; set; }

        [Property("Lower")]
        public string OwnerID { get; set; }

        public Users Owner { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        [Property("Lower")]
        public string CustomerID { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
