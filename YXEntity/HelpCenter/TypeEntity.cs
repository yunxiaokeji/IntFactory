using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryEntity.HelpCenter
{
    public class TypeEntity
    {
        public int AutoID{ get; set; }

        public string TypeID { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }

        public string Icon{ get; set; }

        public int ModuleType{ get; set; }

        public string CreateUserID{ get; set; }

        public int Status { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
