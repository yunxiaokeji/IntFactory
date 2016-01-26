using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class StorageDocAction
    {
        public string CreateUserID { get; set; }

        public DateTime CreateTime { get; set; }

        public string Remark { get; set; }

        public string OperateIP { get; set; }

        public Users CreateUser { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
