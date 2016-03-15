using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity.Manage;
using  IntFactoryDAL.Manage;
using System.Data;
namespace IntFactoryBusiness.Manage
{
    public class ExpressCompanyBusiness
    {

        #region Cache

        private static Dictionary<string, ExpressCompany> _expresscompany;

        private static Dictionary<string, ExpressCompany> ExpressCompanys
        {
            get 
            {
                if (_expresscompany == null)
                {
                    _expresscompany = new Dictionary<string, ExpressCompany>();
                }
                return _expresscompany;
            }
            set { _expresscompany = value; }
        }

        #endregion

        #region  增
        public static bool InsertExpressCompany(ExpressCompany model)
        {
            return ExpressCompanyDAL.BaseProvider.InsertExpressCompany(model.Name, model.Website, model.CreateUserID);
        }
        #endregion

        #region  删
        public static bool DeleteExpressCompany(string id)
        {
            return ExpressCompanyDAL.BaseProvider.DeleteExpressCompany(id);
        }
        #endregion

        #region  查
        public static List<ExpressCompany> GetExpressCompanys()
        {
            DataTable dt = ExpressCompanyDAL.BaseProvider.GetExpressCompanys();
            List<ExpressCompany> list = new List<ExpressCompany>();
            ExpressCompany model;
            foreach (DataRow item in dt.Rows)
            {
                model = new ExpressCompany();
                model.FillData(item);
                list.Add(model);
            }

            return list;
        }

        public static List<ExpressCompany> GetExpressCompanys(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string sqlWhere = "Status<>9";
            if (!string.IsNullOrEmpty(keyWords))
                sqlWhere += " and Name like '%"+keyWords+"%'";

            DataTable dt = CommonBusiness.GetPagerData("ExpressCompany", "*", sqlWhere, "AutoID", pageSize, pageIndex, out totalCount, out pageCount);
            List<ExpressCompany> list = new List<ExpressCompany>();
            ExpressCompany model;
            foreach (DataRow item in dt.Rows)
            {
                model = new ExpressCompany();
                model.FillData(item);
                list.Add(model);
            }

            return list;
        }

        public static ExpressCompany GetExpressCompanyDetail(string id)
        {
            id = id.ToLower();
            if (ExpressCompanys.ContainsKey(id))
            {
                return ExpressCompanys[id];
            }
            ExpressCompany model = new ExpressCompany();

            DataTable dt = ExpressCompanyDAL.BaseProvider.GetExpressCompanyDetail(id);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);

                ExpressCompanys.Add(id, model);
            }

            return model;
        }
        #endregion

        #region  改
        public static bool UpdateExpressCompany(ExpressCompany model)
        {
            return ExpressCompanyDAL.BaseProvider.UpdateExpressCompany(model.ExpressID, model.Name, model.Website);
        }
        #endregion
    }
}
