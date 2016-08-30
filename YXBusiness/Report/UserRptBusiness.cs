﻿using System;
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
