using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity;
using IntFactoryDAL;
using IntFactoryEnum;


namespace IntFactoryBusiness
{
    public class ProvidersBusiness
    {
        public static ProvidersBusiness BaseBusiness = new ProvidersBusiness();

        private ProvidersDAL dal = new ProvidersDAL();

        public List<ProvidersEntity> GetProviders(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
        {
            var dal = new StockDAL();

            string where = " ClientID='" + clientid + "' and Status<>9";
            if (!string.IsNullOrEmpty(keyWords))
            {
                where += " and (Name like '%" + keyWords + "%' or Contact like '%" + keyWords + "%' or MobileTele like '%" + keyWords + "%')";
            }
            DataTable dt = CommonBusiness.GetPagerData("Providers", "*", where, "AutoID", pageSize, pageIndex, out totalCount, out pageCount);

            List<ProvidersEntity> list = new List<ProvidersEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                ProvidersEntity model = new ProvidersEntity();
                model.FillData(dr);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }
            return list;
        }

        public List<ProvidersEntity> GetProviders(string clientid)
        {
            DataTable dt = dal.GetProviders(clientid);

            List<ProvidersEntity> list = new List<ProvidersEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                ProvidersEntity model = new ProvidersEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public ProvidersEntity GetProviderByID(string providerid)
        {
            DataTable dt = dal.GetProviderByID(providerid);

            ProvidersEntity model = new ProvidersEntity();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
            }
            return model;
        }

        public string AddProviders(string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateid, string agentid, string clientid)
        {
            return dal.AddProviders(name, contact, mobile, email, cityCode, address, remark, operateid, agentid, clientid);
        }

        public bool UpdateProvider(string providerid, string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateid, string agentid, string clientid)
        {
            return dal.UpdateProvider(providerid, name, contact, mobile, email, cityCode, address, remark, operateid, agentid, clientid);
        }

        public bool UpdateProviderStatus(string providerid, EnumStatus status, string ip, string operateid)
        {
            return CommonBusiness.Update("Providers", "Status", (int)status, "ProviderID='" + providerid + "'");
        }
    }
}
