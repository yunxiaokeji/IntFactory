using IntFactoryBusiness.Manage;
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

        public static List<StorageDoc> GetStorageDocList(string userid, EnumDocType type, EnumDocStatus status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            DataSet ds = StockDAL.GetStorageDocList(userid, (int)type, (int)status, keywords, begintime, endtime, wareid, providerid, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<StorageDoc> list = new List<StorageDoc>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StorageDoc model = new StorageDoc();
                model.FillData(dr);
                model.UserName = OrganizationBusiness.GetUserByUserID(model.CreateUserID, clientID).Name;
                model.StatusStr = GetDocStatusStr(model.DocType, model.Status);

                model.Details = new List<StorageDetail>();
                if (ds.Tables.Contains("Details"))
                {
                    foreach (DataRow detail in ds.Tables["Details"].Select("DocID='" + model.DocID + "'"))
                    {
                        StorageDetail dModel = new StorageDetail();
                        dModel.FillData(detail);
                        if ((int)type == 3 || (int)type == 4)
                        {
                            if (!string.IsNullOrEmpty(dModel.DepotID) && !string.IsNullOrEmpty(dModel.WareID))
                            {
                                dModel.DepotCode = SystemBusiness.BaseBusiness.GetDepotByID(dModel.DepotID, dModel.WareID, clientID).DepotCode;
                            }
                        }
                        model.Details.Add(dModel);
                    }
                }

                list.Add(model);
            }
            return list;
        }

        public static List<StorageDoc> GetStorageDocDetails(string docid,string clientid)
        {
            DataSet ds = StockDAL.BaseProvider.GetStorageDocDetails(docid);

            List<StorageDoc> list = new List<StorageDoc>();
            foreach (DataRow dr in ds.Tables["Doc"].Rows)
            {
                StorageDoc model = new StorageDoc();
                model.FillData(dr);

                List<StorageDetail> details = new List<StorageDetail>();
                foreach (DataRow detailDR in ds.Tables["Details"].Select("DocID = '" + model.DocID + "'"))
                {
                    StorageDetail detail = new StorageDetail();
                    detail.FillData(detailDR);
                    details.Add(detail);
                }
                model.Details = details;
                var user = OrganizationBusiness.GetUserByUserID(model.CreateUserID, clientid);
                model.UserName = user != null ? user.Name : "";

                list.Add(model);
            }
            return list;
        }

        public static List<GoodsDoc> GetGoodsDocByOrderID(string orderid,string taskid, EnumDocType type, string clientid)
        {
            DataSet ds = StockDAL.BaseProvider.GetGoodsDocByOrderID(orderid,taskid, (int)type, clientid);

            List<GoodsDoc> list = new List<GoodsDoc>();
            foreach (DataRow dr in ds.Tables["Doc"].Rows)
            {
                GoodsDoc model = new GoodsDoc();
                model.FillData(dr);
                if (!string.IsNullOrEmpty(model.ExpressID))
                {
                    model.Express = ExpressCompanyBusiness.GetExpressCompanyDetail(model.ExpressID);
                }
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, clientid);
                model.StatusStr = GetDocStatusStr(model.DocType, model.Status);
                model.Details = new List<GoodsDocDetail>();
                if (ds.Tables.Contains("Details"))
                {
                    foreach (DataRow detail in ds.Tables["Details"].Select("DocID='" + model.DocID + "'"))
                    {
                        GoodsDocDetail goodsDetailModel = new GoodsDocDetail();
                        goodsDetailModel.FillData(detail);
                        model.Details.Add(goodsDetailModel);
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

                if (!string.IsNullOrEmpty(model.ExpressID))
                {
                    model.Express = ExpressCompanyBusiness.GetExpressCompanyDetail(model.ExpressID);
                }

                model.WareHouse = SystemBusiness.BaseBusiness.GetWareByID(model.WareID, model.ClientID);
                model.Details = new List<StorageDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    StorageDetail details = new StorageDetail();
                    details.FillData(item);
                    if(!string.IsNullOrEmpty(details.UnitID))
                    {
                        details.UnitName = new ProductsBusiness().GetUnitByID(details.UnitID).UnitName;
                    }
                    if (!string.IsNullOrEmpty(details.ProviderID))
                    {
                        details.Providers = new ProvidersBusiness().GetProviderByID(details.ProviderID);
                    }
                    model.Details.Add(details);
                }
            }

            return model;
        }

        public static GoodsDoc GetGoodsDocDetail(string docid, string clientid)
        {
            DataSet ds = StockDAL.GetGoodsDocDetail(docid, clientid);
            GoodsDoc model = new GoodsDoc();
            if (ds.Tables.Contains("Doc") && ds.Tables["Doc"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Doc"].Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, clientid);
                model.StatusStr = GetDocStatusStr(model.DocType, model.Status);

                model.DocTypeStr = CommonBusiness.GetEnumDesc<EnumGoodsDocType>((EnumGoodsDocType)model.DocType);

                if (!string.IsNullOrEmpty(model.ExpressID))
                {
                    model.Express = ExpressCompanyBusiness.GetExpressCompanyDetail(model.ExpressID);
                }

                //model.WareHouse = SystemBusiness.BaseBusiness.GetWareByID(model.WareID, model.ClientID);
                model.Details = new List<GoodsDocDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    GoodsDocDetail details = new GoodsDocDetail();
                    details.FillData(item);

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
                        : doctype == 2 ? "已发货"
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

        public List<Products> GetProductStocks(string keywords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
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

        public List<ProductDetail> GetProductDetailStocks(string productid,  string clientid)
        {
            DataTable dt = StockDAL.BaseProvider.GetProductDetailStocks(productid, clientid);

            List<ProductDetail> list = new List<ProductDetail>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductDetail model = new ProductDetail();
                model.FillData(dr);

                list.Add(model);
            }
            return list;
        }

        public List<ProductStock> GetDetailStocks(string wareid, string keywords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
        {
            DataSet ds = StockDAL.BaseProvider.GetDetailStocks(wareid, keywords, pageSize, pageIndex, ref totalCount, ref pageCount, clientid);

            List<ProductStock> list = new List<ProductStock>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductStock model = new ProductStock();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<ProductStock> GetProductsByKeywords(string wareid, string keywords, string clientid)
        {
            DataSet ds = StockDAL.BaseProvider.GetProductsByKeywords(wareid, keywords, clientid);

            List<ProductStock> list = new List<ProductStock>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductStock model = new ProductStock();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<ProductStock> GetProductByDetailID(string detailid)
        {
            DataTable dt = StockDAL.BaseProvider.GetProductByDetailID(detailid);
            List<ProductStock> list = new List<ProductStock>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductStock model = new ProductStock();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }
        #endregion

        #region 添加



        public static bool CreateStorageDoc(string wareid, string remark, string userid, string operateip, string clientid)
        {

            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.AddStorageDoc(guid, (int)EnumDocType.RK, 0, remark, wareid, userid, operateip, clientid);
            if (bl)
            {
                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.StockIn, EnumLogType.Create, "", userid, clientid);
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
        public bool SubmitHandOutDoc(string wareid, string remark, string userid, string operateip, string clientid)
        {
            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.SubmitDamagedDoc(guid, (int)EnumDocType.SGCK, 0, remark, wareid, userid, operateip, clientid);
            if (bl)
            {
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.StockOut, EnumLogType.Create, "", userid, clientid);
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

        public bool AuditStorageIn(string docid, int doctype, int isover, string details, string remark, string userid, string operateip,string clientid, ref int result, ref string errinfo)
        {
            bool bl = new StockDAL().AuditStorageIn(docid, doctype, isover, details, remark, userid, operateip, clientid, ref result, ref errinfo);
            return bl;
        }

        public bool AuditReturnIn(string docid, string userid,string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.BaseProvider.AuditReturnIn(docid, userid,  clientid, ref result, ref errinfo);
        }

        public bool AuditDamagedDoc(string docid, string userid, string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.BaseProvider.AuditDamagedDoc(docid, userid,  clientid, ref result, ref errinfo);
        }

        public bool AuditOverflowDoc(string docid, string userid, string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.BaseProvider.AuditOverflowDoc(docid, userid, clientid, ref result, ref errinfo);
        }

        #endregion
    }
}
