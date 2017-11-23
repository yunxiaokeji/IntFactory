using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryDAL;
using IntFactoryEntity;

namespace IntFactoryBusiness
{
    public class UserRptBusiness
    {
        public static UserRptBusiness BaseBusiness = new UserRptBusiness();

        public List<UserWorkLoadRptEntity> GetUserLoadReport(string begintime, string endtime, string userid, string teamid, string clientid)
        {
            if (string.IsNullOrEmpty(begintime))
            {
                begintime = "1990-1-1 00:00:00";
            }
            else
            {
                begintime = Convert.ToDateTime(begintime).ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (string.IsNullOrEmpty(endtime))
            {
                endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            DataTable dt = UserRPTDAL.BaseProvider.GetUserLoadReport(begintime, endtime, userid, teamid, clientid);

            List<UserWorkLoadRptEntity> list = new List<UserWorkLoadRptEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                UserWorkLoadRptEntity model = new UserWorkLoadRptEntity();
                model.FillData(dr);
                model.UserName = OrganizationBusiness.GetUserCacheByUserID(model.UserID, clientid).Name;
                list.Add(model);
            }
            return list;
        }
        
    }
}
