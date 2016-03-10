using IntFactoryDAL;
using IntFactoryEntity;
using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryBusiness
{
    public class StockBusiness
    {
        public static StockBusiness BaseBusiness = new StockBusiness();

        #region 查询


        public List<ProvidersEntity> GetProviders(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new StockDAL();

            string where = " ClientID='" + clientID + "' and Status<>9";

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

        public List<ProvidersEntity> GetProviders(string clientID)
        {
            var dal = new StockDAL();
            DataTable dt = dal.GetProviders(clientID);

            List<ProvidersEntity> list = new List<ProvidersEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                ProvidersEntity model = new ProvidersEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public ProvidersEntity GetProviderByID(string providerID)
        {
            var dal = new StockDAL();
            DataTable dt = dal.GetProviderByID(providerID);

            ProvidersEntity model = new ProvidersEntity();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
            }
            return model;
        }

        public static List<StorageDoc> GetStorageDocList(string userid, EnumDocType type, EnumDocStatus status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            DataSet ds = StockDAL.GetStorageDocList(userid, (int)type, (int)status, keywords, begintime, endtime, wareid, providerid, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<StorageDoc> list = new List<StorageDoc>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StorageDoc model = new StorageDoc();
                model.FillData(dr);
                model.StatusStr = GetDocStatusStr(model.DocType, model.Status);
                if (!string.IsNullOrEmpty(model.ProviderID))
                {
                    model.ProviderName = BaseBusiness.GetProviderByID(model.ProviderID).Name;
                }
                model.Details = new List<StorageDetail>();
                if (ds.Tables.Contains("Details"))
                {
                    foreach (DataRow detail in ds.Tables["Details"].Select("DocID='" + model.DocID + "'"))
                    {
                        StorageDetail dModel = new StorageDetail();
                        dModel.FillData(detail);
                        model.Details.Add(dModel);
                    }
                }

                list.Add(model);
            }
            return list;
        }

        public static StorageDoc GetStorageDetail(string docid, string clientid)
        {
            DataSet ds = StockDAL.GetStorageDetail(docid, clientid);
            StorageDoc model = new StorageDoc();
            if (ds.Tables.Contains("Doc") && ds.Tables["Doc"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Doc"].Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, clientid);
                model.StatusStr = GetDocStatusStr(model.DocType, model.Status);

                model.DocTypeStr = CommonBusiness.GetEnumDesc<EnumDocType>((EnumDocType)model.DocType);

                //model.WareHouse = SystemBusiness.BaseBusiness.GetWareByID(model.WareID, model.ClientID);
                model.Details = new List<StorageDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    StorageDetail details = new StorageDetail();
                    details.FillData(item);

                    if (!string.IsNullOrEmpty(details.ProdiverID))
                    {
                        details.Providers = BaseBusiness.GetProviderByID(details.ProdiverID);
                    }
                    model.Details.Add(details);
                }
            }

            return model;
        }

        private static string GetDocStatusStr(int doctype, int status)
        {
            string str = "";
            switch (status)
            {
                case 0:
                    str = doctype == 6 ? "待入库"
                        : doctype == 2 ? "待上架"
                        : "待审核";
                    break;
                case 1:
                    str = doctype == 1 ? "部分入库"
                        : doctype == 2 ? "部分出库"
                        : "部分审核";
                    break;
                case 2:
                    str = doctype == 1 ? "已入库"
                        : doctype == 2 ? "已出库"
                        : doctype == 6 ? "已入库"
                        : "已审核";
                    break;
                case 4:
                    str = "已作废";
                    break;
                case 9:
                    str = "已删除";
                    break;
            }
            return str;
        }

        public static List<StorageDocAction> GetStorageDocAction(string docid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid)
        {
            DataTable dt = CommonBusiness.GetPagerData("StorageDocAction", "*", "DocID='" + docid + "'", "AutoID", pageSize, pageIndex, out totalCount, out pageCount);

            List<StorageDocAction> list = new List<StorageDocAction>();
            foreach (DataRow dr in dt.Rows)
            {
                StorageDocAction model = new StorageDocAction();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);

                list.Add(model);
            }
            return list;
        }

        public List<Products> GetProductStocks(string keywords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            DataSet ds = StockDAL.BaseProvider.GetProductStocks(keywords, pageSize, pageIndex, ref totalCount, ref pageCount, clientid);

            List<Products> list = new List<Products>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Products model = new Products();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<ProductDetail> GetProductDetailStocks(string productid, string agentid, string clientid)
        {
            DataTable dt = StockDAL.BaseProvider.GetProductDetailStocks(productid, clientid);

            List<ProductDetail> list = new List<ProductDetail>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductDetail model = new ProductDetail();
                model.FillData(dr);
                model.SaleAttrValueString = "";
                if (!string.IsNullOrEmpty(model.SaleAttrValue)) 
                {
                    string[] attrs = model.SaleAttrValue.Split(',');
                    foreach (string attrid in attrs)
                    {
                        if (!string.IsNullOrEmpty(attrid))
                        {
                            var attr = new ProductsBusiness().GetAttrByID(attrid.Split(':')[0]);
                            var value = attr.AttrValues.Where(m => m.ValueID == attrid.Split(':')[1]).FirstOrDefault();
                            if (attr != null && value != null)
                            {
                                model.SaleAttrValueString += attr.AttrName + "：" + value.ValueName + "，";
                            }
                        }
                    }
                    if (model.SaleAttrValueString.Length > 0)
                    {
                        model.SaleAttrValueString = model.SaleAttrValueString.Substring(0, model.SaleAttrValueString.Length - 1);
                    }
                }

                list.Add(model);
            }
            return list;
        }

        public List<ProductStock> GetDetailStocks(string wareid, string keywords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            DataSet ds = StockDAL.BaseProvider.GetDetailStocks(wareid, keywords, pageSize, pageIndex, ref totalCount, ref pageCount, clientid);

            List<ProductStock> list = new List<ProductStock>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductStock model = new ProductStock();
                model.FillData(dr);
                model.SaleAttrValueString = "";
                if (!string.IsNullOrEmpty(model.SaleAttrValue))
                {
                    string[] attrs = model.SaleAttrValue.Split(',');
                    foreach (string attrid in attrs)
                    {
                        if (!string.IsNullOrEmpty(attrid))
                        {
                            var attr = new ProductsBusiness().GetAttrByID(attrid.Split(':')[0]);
                            var value = attr.AttrValues.Where(m => m.ValueID == attrid.Split(':')[1]).FirstOrDefault();
                            if (attr != null && value != null)
                            {
                                model.SaleAttrValueString += attr.AttrName + "：" + value.ValueName + "，";
                            }
                        }
                    }
                    if (model.SaleAttrValueString.Length > 0)
                    {
                        model.SaleAttrValueString = model.SaleAttrValueString.Substring(0, model.SaleAttrValueString.Length - 1);
                    }
                }
                list.Add(model);
            }
            return list;
        }

        public List<ProductStock> GetProductsByKeywords(string wareid, string keywords, string agentid, string clientid)
        {
            DataSet ds = StockDAL.BaseProvider.GetProductsByKeywords(wareid, keywords, clientid);

            List<ProductStock> list = new List<ProductStock>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductStock model = new ProductStock();
                model.FillData(dr);
                model.SaleAttrValueString = "";
                if (!string.IsNullOrEmpty(model.SaleAttrValue))
                {
                    string[] attrs = model.SaleAttrValue.Split(',');
                    foreach (string attrid in attrs)
                    {
                        if (!string.IsNullOrEmpty(attrid))
                        {
                            var attr = new ProductsBusiness().GetAttrByID(attrid.Split(':')[0]);
                            var value = attr.AttrValues.Where(m => m.ValueID == attrid.Split(':')[1]).FirstOrDefault();
                            if (attr != null && value != null)
                            {
                                model.SaleAttrValueString += attr.AttrName + "：" + value.ValueName + "，";
                            }
                        }
                    }
                    if (model.SaleAttrValueString.Length > 0)
                    {
                        model.SaleAttrValueString = model.SaleAttrValueString.Substring(0, model.SaleAttrValueString.Length - 1);
                    }
                }
                list.Add(model);
            }
            return list;
        }


        #endregion

        #region 添加

        public string AddProviders(string name, string contact, string mobile,string email, string cityCode, string address,string remark, string operateID, string agentid, string clientID)
        {
            return new StockDAL().AddProviders(name, contact, mobile, email, cityCode, address, remark, operateID, agentid, clientID);
        }


        public static bool CreateStorageDoc(string wareid, string remark, string userid, string operateip, string agentid, string clientid)
        {

            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.AddStorageDoc(guid, (int)EnumDocType.RK, 0, remark, wareid, userid, operateip, clientid);
            if (bl)
            {
                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.StockIn, EnumLogType.Create, "", userid, agentid, clientid);
            }
            return bl;
        }

        public bool SubmitDamagedDoc(string wareid, string remark, string userid, string operateip, string clientid)
        {
            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.SubmitDamagedDoc(guid, (int)EnumDocType.BS, 0, remark, wareid, userid, operateip, clientid);
            return bl;
        }

        /// <summary>
        /// 出库按报损逻辑
        /// </summary>
        public bool SubmitHandOutDoc(string wareid, string remark, string userid, string operateip, string agentid, string clientid)
        {
            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.SubmitDamagedDoc(guid, (int)EnumDocType.SGCK, 0, remark, wareid, userid, operateip, clientid);
            if (bl)
            {
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.StockOut, EnumLogType.Create, "", userid, agentid, clientid);
            }
            return bl;
        }

        public bool SubmitOverflowDoc(string wareid, string remark, string userid, string operateip, string clientid)
        {
            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.SubmitOverflowDoc(guid, (int)EnumDocType.BY, 0, remark, wareid, userid, operateip, clientid);
            return bl;
        }

        #endregion

        #region 编辑、删除


        public bool UpdateProvider(string providerid, string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateID, string agentid, string clientID)
        {
            var dal = new StockDAL();
            return dal.UpdateProvider(providerid, name, contact, mobile, email, cityCode, address, remark, operateID, agentid, operateID);
        }

        public bool UpdateProviderStatus(string providerid, EnumStatus status, string ip, string operateid)
        {
            return CommonBusiness.Update("Providers", "Status", (int)status, "ProviderID='" + providerid + "'");
        }

        public bool DeleteDoc(string docid, string userid, string operateip, string clientid)
        {
            return new StockDAL().UpdateStorageStatus(docid, (int)EnumDocStatus.Delete, "删除单据", userid, operateip, clientid);
        }

        public bool InvalidDoc(string docid, string userid, string operateip, string clientid)
        {
            return new StockDAL().UpdateStorageStatus(docid, (int)EnumDocStatus.Invalid, "作废单据", userid, operateip, clientid);
        }

        public bool UpdateStorageDetailWare(string docid, string autoid, string wareid, string depotid, string userid, string operateip, string clientid)
        {
            return new StockDAL().UpdateStorageDetailWare(docid, autoid, wareid, depotid);
        }

        public bool UpdateStorageDetailBatch(string docid, string autoid, string batch, string userid, string operateip, string clientid)
        {
            return new StockDAL().UpdateStorageDetailBatch(docid, autoid, batch);
        }

        public bool AuditStorageIn(string docid, string userid, string operateip, string agentid, string clientid, ref int result, ref string errinfo)
        {
            bool bl = new StockDAL().AuditStorageIn(docid, userid, operateip, agentid, clientid, ref result, ref errinfo);
            return bl;
        }

        public bool AuditReturnIn(string docid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.BaseProvider.AuditReturnIn(docid, userid, agentid, clientid, ref result, ref errinfo);
        }

        public bool AuditDamagedDoc(string docid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.BaseProvider.AuditDamagedDoc(docid, userid, agentid, clientid, ref result, ref errinfo);
        }

        public bool AuditOverflowDoc(string docid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.BaseProvider.AuditOverflowDoc(docid, userid, agentid, clientid, ref result, ref errinfo);
        }

        #endregion
    }
}
