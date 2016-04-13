using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class ApiDetailEntity
    {
        public int AutoID;

        public string ApiID;

        public string ModuleID;

        public string Name;

        public string Remark;

        public string Detail;

        public int Status;

        public int Sort;

        public DateTime CreateTime;

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
