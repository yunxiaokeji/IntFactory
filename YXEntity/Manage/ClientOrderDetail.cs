using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Manage
{
    public class ClientOrderDetail
    {
        public int AutoID { get; set; }

        public string OrderID { get; set; }

        public string ProductID { get; set; }

        public decimal Price { get; set; }

        public int Qunatity { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserID { get; set; }

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
