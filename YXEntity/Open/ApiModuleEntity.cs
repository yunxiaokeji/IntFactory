using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ApiModuleEntity
    {
        public int AutoID{get;set;}

        public string ModuleID { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public string Icon { get; set; }

        public int Status { get; set; }

        public int Sort { get; set; }

        public DateTime CreateTime { get; set; }

        public List<ApiDetailEntity> Details { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
