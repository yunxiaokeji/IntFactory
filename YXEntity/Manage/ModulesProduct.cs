using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Manage
{
    public class ModulesProduct
    {
        public int AutoID { get; set; }

        public string ProductID { get; set; }

        public string ModulesID { get; set; }

        public string  Name { get; set; }

        public int Period { get; set; }

        public int UserQuantity { get; set; }

        public int PeriodQuantity { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserID { get; set; }

        public int Type { get; set; }

        public int IsChild { get; set; }

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
