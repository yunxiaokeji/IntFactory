using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity.Manage
{
    public class FeedBack
    {
        public int AutoID { get; set; }

        public string Title { get; set; }

        public string ContactName { get; set; }

        public string MobilePhone { get; set; }

        public int Type { get; set; }

        public int Status { get; set; }

        public string FilePath { get; set; }

        public string Remark { get; set; }

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
