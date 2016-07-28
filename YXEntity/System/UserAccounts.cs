using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class UserAccounts
    {
        [Property("Lower")]
        public string UserID
        {
            set;
            get;
        }

        public string AccountName
        {
            set;
            get;
        }

        public int AccountType
        {
            set;
            get;
        }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
