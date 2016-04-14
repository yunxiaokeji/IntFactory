using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
namespace IntFactoryDAL
{
    public class ApiModuleDAL:BaseDAL
    {
        public static ApiModuleDAL BaseProvider = new ApiModuleDAL();

        public DataSet GetApiModules() {
            string sqlStr = "select * from O_ApiModule where status<>9; select * from O_ApiDetail where status<>9";

            DataSet ds = GetDataSet(sqlStr);

            return ds;
        }
    }
}
