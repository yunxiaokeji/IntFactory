using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ApiModuleEntity
    {
        public int AutoID;

        public string ModuleID;

        public string Name;

        public string Remark;

        public string Icon;

        public int Status;

        public int Sort;

        public DateTime CreateTime;

        public List<ApiDetailEntity> Details;

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
