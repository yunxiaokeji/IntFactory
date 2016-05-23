using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Manage
{
    public class ClientOrderAccount
    {
        public int AutoID { get; set; }

        public string OrderID { get; set; }

        public string ClientID { get; set; } 

        public decimal RealAmount { get; set; }

        public int PayType { get; set; }

        public int Type { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserID { get; set; }

        public DateTime CheckTime { get; set; }

        public string CheckUserID { get; set; }

        public string AlipayNo { get; set; }

        public string Remark { get; set; }


        public M_Users CheckerUser { get; set; }
        public Users CreateUser { get; set; }
        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
