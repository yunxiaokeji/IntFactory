using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ProductUseLogEntity
    {
        public string PersonName { get; set; }

        public string OrderCode { get; set; }

        public decimal Quantity { get; set; }

        public DateTime CreateTime { get; set; }

        public string Remark { get; set; }

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
