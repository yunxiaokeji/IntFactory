using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Manage
{
    public class ClientOrder
    {
        public int AutoID { get; set; }

        [Property("Lower")]
        public string OrderID { get; set; }

        public int UserQuantity { get; set; }

        public int Years { get; set; }

        public decimal Amount { get; set; }

        public decimal RealAmount { get; set; }

        public int PayType { get; set; }

        public int Type { get; set; }

        public int SystemType { get; set; }

        public int Status { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public string ClientCode { get; set; }

        public string CompanyName { get; set; }

        public decimal PayFee { get; set; }

        public decimal RefundFee { get; set; }

        public int PayStatus { get; set; }

        public int SourceType { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        [Property("Lower")]
        public string CheckUserID { get; set; }

        public DateTime CheckTime { get; set; }
   
        public Users CheckUser { get; set; }       

        public List<ClientOrderDetail> Details { get; set; }
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
