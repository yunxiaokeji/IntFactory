using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Manage
{
    public class ClientOrderDetail
    {
        public int AutoID { get; set; }

        [Property("Lower")]
        public string OrderID { get; set; }

        [Property("Lower")]
        public string ProductID { get; set; }

        public decimal Price { get; set; }

        public int Qunatity { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public string Name { get; set; }
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
