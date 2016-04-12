using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using IntFactoryEntity;
using IntFactoryDAL;
namespace IntFactoryBusiness
{
    public class ApiModuleBusiness
    {
        public static ApiModuleBusiness BaseBusiness = new ApiModuleBusiness();

        private static List<ApiModuleEntity> _apiModulesCache;
        private static List<ApiModuleEntity> ApiModulesCache
        {
            get {
                if (_apiModulesCache == null) {
                    _apiModulesCache = new List<ApiModuleEntity>();
                }

                return _apiModulesCache;
            }
            set {
                _apiModulesCache = value;
            }
        }

        public List<ApiModuleEntity> GetApiModules() {
            if (ApiModulesCache.Count > 0)
            {
                return ApiModulesCache;
            }

            List<ApiModuleEntity> list = new List<ApiModuleEntity>();
            DataSet ds = ApiModuleDAL.BaseProvider.GetApiModules();
            DataTable apiModuleTB=ds.Tables[0];
            DataTable apiDetailTB=ds.Tables[1];

            foreach (DataRow dr in apiModuleTB.Rows) {
                ApiModuleEntity item = new ApiModuleEntity();
                item.FillData(dr);

                item.Details = new List<ApiDetailEntity>();
                foreach (DataRow d in apiDetailTB.Select("ModuleID='" + dr["ModuleID"] + "'", "sort asc"))
                {
                    ApiDetailEntity detail = new ApiDetailEntity();
                    detail.FillData(d);

                    item.Details.Add(detail);
                }
                list.Add(item);
            }

            ApiModulesCache = list;

            return list;
        }

        public ApiDetailEntity GetApiDetail(string moduleID, string apiID)
        {
            var apiModule = ApiModulesCache.Find(m => m.ModuleID.ToLower() == moduleID.ToLower());
            if (apiModule != null)
            {
                var apiDetail = apiModule.Details.Find(m => m.ApiID.ToLower() == apiID.ToLower());
                if (apiDetail != null) {
                    return apiDetail;
                }
            }

            return null;

        }

    }
}
