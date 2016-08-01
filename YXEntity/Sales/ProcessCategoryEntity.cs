using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ProcessCategoryEntity
    {
        [Property("Lower")]
        public string CategoryID { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public List<CategoryItemsEntity> CategoryItems { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }

    public class CategoryItemsEntity
    {
        [Property("Lower")]
        public string ItemID { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public int OrderType { get; set; }

        public int Sort { get; set; }

        public int Mark { get; set; }

        [Property("Lower")]
        public string CategoryID { get; set; }

        public string Remark { get; set; }

        public string Desc { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
