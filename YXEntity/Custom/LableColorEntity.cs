﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class LableColorEntity
    {
        public int AutoID { get; set; }

        public int ColorID { get; set; }

        public string ColorName { get; set; }

        public string ColorValue { get; set; }

        public int Status { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        [Property("Lower")]
        public string UpdateUserID { get; set; }

        public Users UpdateUser { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
