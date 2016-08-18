using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;


using IntFactoryDAL;
using IntFactoryEntity;
using IntFactoryEnum;

namespace IntFactoryBusiness
{
    public class ProductsBusiness
    {
        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public string FILEPATH = CloudSalesTool.AppSettings.Settings["UploadFilePath"] + "Product/" + DateTime.Now.ToString("yyyyMM") + "/";
        public string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];


        public static ProductsBusiness BaseBusiness = new ProductsBusiness();
        public static object SingleLock = new object();

        #region Cache

        private static List<ProductUnit> _units;
        private static List<ProductUnit> CacheUnits
        {
            get
            {
                if (_units == null)
                {
                    _units = new List<ProductUnit>();
                }
                return _units;
            }
            set { _units = value; }
        }

        private static List<ProductAttr> _attrs;
        private static List<ProductAttr> CacheAttrs
        {
            get 
            {
                if (_attrs == null)
                {
                    _attrs = new List<ProductAttr>();
                }
                return _attrs;
            }
            set { _attrs = value; }
        }

        private static List<Category> _category;
        private static List<Category> CacheCategory
        {
            get 
            {
                if (_category == null)
                {
                    _category = new List<Category>();
                }
                return _category;
            }
            set
            {
                _category = value;
            }
        }

        #endregion

        #region 单位

        public List<ProductUnit> GetUnits()
        {
            if (CacheUnits.Count() > 0)
            {
                return CacheUnits;
            }

            foreach (DataRow dr in ProductsDAL.BaseProvider.GetUnits().Rows)
            {
                ProductUnit model = new ProductUnit();
                model.FillData(dr);
                CacheUnits.Add(model);
            }
            return CacheUnits;
        }

        public ProductUnit GetUnitByID(string unitid)
        {
            var list = GetUnits();
            if (list.Where(m => m.UnitID.ToLower() == unitid.ToLower()).Count() > 0)
            {
                return list.Where(m => m.UnitID.ToLower() == unitid.ToLower()).FirstOrDefault();
            }

            var dal = new ProductsDAL();
            DataTable dt = dal.GetUnitByID(unitid);

            ProductUnit model = new ProductUnit();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                list.Add(model);
            }
            return model;
        }

        public string AddUnit(string unitname, string description, string operateid)
        {
            var dal = new ProductsDAL();
            var id = dal.AddUnit(unitname, description, operateid);
            if (!string.IsNullOrEmpty(id))
            {
                var list = GetUnits();
                list.Add(new ProductUnit() { UnitID = id, UnitName = unitname });
            }
            return id;
        }

        public bool UpdateUnit(string unitid, string unitname, string desciption, string operateid)
        {
            var dal = new ProductsDAL();
            bool bl = dal.UpdateUnit(unitid, unitname, desciption);
            if (bl)
            {
                var model = GetUnitByID(unitid);
                model.UnitName = unitname;
            }
            return bl;
        }

        public bool DeleteUnit(string unitid, string operateip, string operateid)
        {
            bool bl = ProductsDAL.BaseProvider.DeleteUnit(unitid);
            if (bl)
            {
                var model = GetUnitByID(unitid);
                CacheUnits.Remove(model);
            }
            return bl;
        }

        public static void ClearUnitCache()
        {
            CacheUnits = null;

        }
        #endregion

        #region 属性

        public List<ProductAttr> GetAttrs()
        {
            if (CacheAttrs.Count > 0)
            {
                return CacheAttrs;
            }
            DataSet ds = ProductsDAL.BaseProvider.GetAttrs();
            foreach (DataRow dr in ds.Tables["Attrs"].Rows)
            {
                ProductAttr model = new ProductAttr();
                model.FillData(dr);
                model.AttrValues = new List<AttrValue>();
                foreach (DataRow item in ds.Tables["Values"].Select(" AttrID='" + model.AttrID + "' "))
                {
                    AttrValue attrValue = new AttrValue();
                    attrValue.FillData(item);
                    model.AttrValues.Add(attrValue);
                }
                CacheAttrs.Add(model);
            }
            return CacheAttrs;
        }

        public List<ProductAttr> GetAttrList(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetAttrList(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount);

            List<ProductAttr> list = new List<ProductAttr>();
            if (ds.Tables.Contains("Attrs"))
            {
                foreach (DataRow dr in ds.Tables["Attrs"].Rows)
                {
                    ProductAttr model = new ProductAttr();
                    model.FillData(dr);
                    list.Add(model);
                }
            }
            return list;
        }

        public ProductAttr GetAttrByID(string attrid)
        {
            var list = GetAttrs();

            if (list.Where(m => m.AttrID.ToLower() == attrid.ToLower()).Count() > 0)
            {
                return list.Where(m => m.AttrID.ToLower() == attrid.ToLower()).FirstOrDefault();
            }

            DataSet ds = new ProductsDAL().GetAttrByID(attrid);

            ProductAttr model = new ProductAttr();
            if (ds.Tables.Contains("Attrs") && ds.Tables["Attrs"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Attrs"].Rows[0]);
                model.AttrValues = new List<AttrValue>();
                foreach (DataRow item in ds.Tables["Values"].Rows)
                {
                    AttrValue attrValue = new AttrValue();
                    attrValue.FillData(item);
                    model.AttrValues.Add(attrValue);
                }
                list.Add(model);
            }

            return model;
        }

        public string AddAttr(string attrname, string description, string categoryid, int type, string operateid)
        {
            var attrid = Guid.NewGuid().ToString().ToLower();
            if (ProductsDAL.BaseProvider.AddAttr(attrid, attrname, description, categoryid, type, operateid))
            {
                CacheAttrs.Add(new ProductAttr()
                {
                    AttrID = attrid,
                    AttrName = attrname,
                    Description = description,
                    AttrValues = new List<AttrValue>()
                });
                return attrid;
            }
            return string.Empty;
        }

        public string AddAttrValue(string valueName, int sort, string attrid, string operateid)
        {
            var valueid = Guid.NewGuid().ToString().ToLower();
            if (ProductsDAL.BaseProvider.AddAttrValue(valueid, valueName, sort, attrid, operateid))
            {
                var model = GetAttrByID(attrid);
                model.AttrValues.Add(new AttrValue()
                {
                    ValueID = valueid,
                    ValueName = valueName,
                    AttrID = attrid,
                    Sort = sort
                });
                return valueid;
            }
            return string.Empty;
        }

        public bool UpdateProductAttr(string attrID, string attrName, string description, string operateIP, string operateID)
        {
            var bl = ProductsDAL.BaseProvider.UpdateProductAttr(attrID, attrName, description);
            if (bl)
            {
                var model = GetAttrByID(attrID);
                model.AttrName = attrName;
                model.Description = description;
            }
            return bl;
        }

        public bool UpdateAttrValue(string valueid, string attrid, string valueName, int sort, string operateIP, string operateID)
        {
            var bl = ProductsDAL.BaseProvider.UpdateAttrValue(valueid, attrid, valueName, sort);
            if (bl)
            {
                var model = GetAttrByID(attrid);
                model.AttrValues = new List<AttrValue>();
                foreach (DataRow item in ProductsDAL.BaseProvider.GetAttrValuesByAttrID(attrid).Rows)
                {
                    AttrValue attrValue = new AttrValue();
                    attrValue.FillData(item);
                    model.AttrValues.Add(attrValue);
                }
            }
            return bl;
        }

        public bool DeleteProductAttr(string attrid, string operateIP, string operateID)
        {
            var bl = ProductsDAL.BaseProvider.DeleteProductAttr(attrid);
            if (bl)
            {
                var model = GetAttrByID(attrid);
                CacheAttrs.Remove(model);
            }
            return bl;
        }

        public bool UpdateAttrValueStatus(string valueid, string attrid, EnumStatus status, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            bool bl = dal.UpdateAttrValueStatus(valueid, (int)status);
            if (bl)
            {
                var model = GetAttrByID(attrid);
                var value = model.AttrValues.Where(m => m.ValueID.ToLower() == valueid.ToLower()).FirstOrDefault();
                model.AttrValues.Remove(value);
            }
            return bl;
        }

        public static void ClearAttrsCache()
        {
            CacheAttrs = null;
        }
        #endregion

        #region 分类

        public List<Category> GetCategorys()
        {
            if (CacheCategory.Count() > 0)
            {
                return CacheCategory;
            }

            DataSet ds = ProductsDAL.BaseProvider.GetCategorys();

            foreach (DataRow dr in ds.Tables["Category"].Rows)
            {
                Category model = new Category();
                model.FillData(dr);
                model.ChildCategory = new List<Category>();
                model.AttrLists = new List<ProductAttr>();
                model.SaleAttrs = new List<ProductAttr>();
                foreach (DataRow item in ds.Tables["Attr"].Select(" CategoryID='" + model.CategoryID + "' "))
                {
                    ProductAttr attr = new ProductAttr();
                    attr.FillData(item);
                    if (attr.Type == 1)
                    {
                        model.AttrLists.Add(GetAttrByID(attr.AttrID));
                    }
                    else
                    {
                        model.SaleAttrs.Add(GetAttrByID(attr.AttrID));
                    }
                }
                CacheCategory.Add(model);
            }
            foreach (var model in CacheCategory)
            {
                model.ChildCategory = CacheCategory.Where(m => m.PID == model.CategoryID).ToList();
            }
            return CacheCategory;

        }

        public List<Category> GetChildCategorysByID(string categoryid, EnumCategoryType type)
        {
            var list = GetCategorys();
            if (list.Where(m => m.PID.ToLower() == categoryid.ToLower() && m.CategoryType == (int)type).Count() > 0)
            {
                return list.Where(m => m.PID.ToLower() == categoryid.ToLower() && m.CategoryType == (int)type && m.Status == 1).ToList();
            }
            return new List<Category>();
        }

        public Category GetCategoryByID(string categoryid)
        {
            var cacheList = GetCategorys();
            if (cacheList.Where(m => m.CategoryID.ToLower() == categoryid.ToLower()).Count() > 0)
            {
                return cacheList.Where(m => m.CategoryID.ToLower() == categoryid.ToLower()).FirstOrDefault();
            }

            DataSet ds = ProductsDAL.BaseProvider.GetCategoryByID(categoryid);

            Category model = new Category();
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.FillData(ds.Tables[0].Rows[0]);
                model.ChildCategory = new List<Category>();
                model.AttrLists = new List<ProductAttr>();
                model.SaleAttrs = new List<ProductAttr>();
                foreach (DataRow item in ds.Tables["Attr"].Rows)
                {
                    ProductAttr attr = new ProductAttr();
                    attr.FillData(item);
                    if (attr.Type == 1)
                    {
                        model.AttrLists.Add(GetAttrByID(attr.AttrID));
                    }
                    else
                    {
                        model.SaleAttrs.Add(GetAttrByID(attr.AttrID));
                    }
                }
                CacheCategory.Add(model);
            }

            return model;
        }

        public List<ProductAttr> GetAttrsByCategoryID(string categoryid)
        {
            if (string.IsNullOrEmpty(categoryid))
            {
                return GetAttrs();
            }
            var model = GetCategoryByID(categoryid);

            List<ProductAttr> list = new List<ProductAttr>();
            foreach (var attr in model.AttrLists)
            {
                list.Add(new ProductAttr() { AttrID = attr.AttrID, AttrName = attr.AttrName, Description = attr.Description, Type = 1, CategoryID = categoryid });
            }
            foreach (var attr in model.SaleAttrs)
            {
                list.Add(new ProductAttr() { AttrID = attr.AttrID, AttrName = attr.AttrName, Description = attr.Description, Type = 2, CategoryID = categoryid });
            }
            return list;
        }

        public ProductAttr GetTaskPlateAttrByCategoryID(string categoryid)
        {
            var model = GetCategoryByID(categoryid);
            if (model.AttrLists.Count > 0)
            {
                return model.AttrLists[0];
            }
            return null;
        }

        public List<ProcessCategory> GetClientProcessCategorys(string clientid)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetClientProcessCategorys(clientid);

            List<ProcessCategory> list = new List<ProcessCategory>();

            foreach (DataRow dr in dt.Rows)
            {
                ProcessCategory model = new ProcessCategory();
                model.FillData(dr);
                list.Add(model);

            }
            return list;
        }

        public string AddCategory(string categoryCode, string categoryName, string pid, int type, int status, List<string> attrlist, List<string> saleattr, string description, string operateid)
        {
            var dal = new ProductsDAL();
            string guid = dal.AddCategory(categoryCode, categoryName, pid, type, status, string.Join(",", attrlist), string.Join(",", saleattr), description, operateid);
            if (!string.IsNullOrEmpty(guid))
            {
                if (string.IsNullOrEmpty(pid))
                {
                    CacheCategory.Add(new Category()
                    {
                        CategoryID = guid.ToLower(),
                        CategoryCode = categoryCode,
                        CategoryName = categoryName,
                        CategoryType = type,
                        Layers = 1,
                        PID = pid,
                        Status = status,
                        Description = description,
                        ChildCategory = new List<Category>(),
                        AttrLists = new List<ProductAttr>(),
                        SaleAttrs = new List<ProductAttr>()
                    });
                }
                else
                {
                    var PModel = GetCategoryByID(pid);
                    var model = new Category()
                    {
                        CategoryID = guid.ToLower(),
                        CategoryCode = categoryCode,
                        CategoryName = categoryName,
                        CategoryType = type,
                        Layers = PModel.Layers + 1,
                        PID = pid,
                        Status = status,
                        Description = description,
                        ChildCategory = new List<Category>(),
                        AttrLists = new List<ProductAttr>(),
                        SaleAttrs = new List<ProductAttr>()
                    };
                    CacheCategory.Add(model);
                    PModel.ChildCategory.Add(model);
                }
                
            }
            return guid;
        }

        public bool UpdateCategory(string categoryid, string categoryName, int status, List<string> attrlist, List<string> saleattr, string description, string operateid)
        {
            var dal = new ProductsDAL();
            bool bl = dal.UpdateCategory(categoryid, categoryName, status, string.Join(",", attrlist), string.Join(",", saleattr), description, operateid);
            if (bl)
            {
                var model = GetCategoryByID(categoryid);
                model.CategoryName = categoryName;
                model.Description = description;
            }
            return bl;
        }

        public bool AddCategoryAttr(string categoryid, string attrid, int type, string operateIP, string operateID)
        {
            var bl = ProductsDAL.BaseProvider.AddCategoryAttr(categoryid, attrid, type, operateID);
            if (bl)
            {
                var model = GetCategoryByID(categoryid);
                if (type == 1 && model.AttrLists.Where(m => m.AttrID.ToLower() == attrid.ToLower()).Count()<=0)
                {
                    var attr = GetAttrByID(attrid);
                    model.AttrLists.Add(attr);
                }
                else if (type == 2 && model.SaleAttrs.Where(m => m.AttrID.ToLower() == attrid.ToLower()).Count() <= 0)
                {
                    var attr = GetAttrByID(attrid);
                    model.SaleAttrs.Add(attr);
                }
            }
            return bl;
        }

        public bool DeleteCategoryAttr(string categoryid, string attrid, int type, string operateIP, string operateID)
        {
            var bl = ProductsDAL.BaseProvider.UpdateCategoryAttrStatus(categoryid, attrid, type);
            if (bl)
            {
                var model = GetCategoryByID(categoryid);
                if (type == 1)
                {
                    var attr = model.AttrLists.Where(m => m.AttrID.ToLower() == attrid.ToLower()).FirstOrDefault();
                    model.AttrLists.Remove(attr);
                }
                else
                {
                    var attr = model.SaleAttrs.Where(m => m.AttrID.ToLower() == attrid.ToLower()).FirstOrDefault();
                    model.SaleAttrs.Remove(attr);
                }
            }
            return bl;
        }

        public bool DeleteCategory(string categoryid, string operateid, string ip, out int result)
        {
            var dal = new ProductsDAL();
            bool bl = dal.DeleteCategory(categoryid, operateid, out result);
            if (bl)
            {
                var model = GetCategoryByID(categoryid);
                if (!string.IsNullOrEmpty(model.PID))
                {
                    var PModel = GetCategoryByID(model.PID);
                    if (PModel.ChildCategory.Where(m => m.CategoryID.ToLower() == model.CategoryID.ToLower()).Count() > 0)
                    {
                        PModel.ChildCategory.Remove(model);
                    }
                }
                CacheCategory.Remove(model);
            }
            return bl;
        }

        public static void ClearCategoryCache() 
        {
            CacheCategory = null;
        }

        #endregion

        #region 产品

        public List<Products> GetProductList(string categoryid, string prodiverid, string beginprice, string endprice, string keyWords, string orderby, bool isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductList(categoryid, prodiverid, beginprice, endprice, keyWords, orderby, isasc ? 1 : 0, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<Products> list = new List<Products>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Products model = new Products();
                model.FillData(dr);
                if (!string.IsNullOrEmpty(model.CategoryID))
                {
                    model.CategoryName = GetCategoryByID(model.CategoryID).CategoryName;
                }

                if (!string.IsNullOrEmpty(model.UnitID))
                {
                    model.UnitName = GetUnitByID(model.UnitID).UnitName;
                }

                model.IsPublicStr = CommonBusiness.GetEnumDesc((EnumProductPublicStatus)model.IsPublic);
                
                list.Add(model);
                
            }
            return list;
        }

        public List<Products> GetProductsAll(string categoryid, string prodiverid, string beginprice, string endprice, int ispublic, string keyWords, string orderby, bool isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductsAll(categoryid, prodiverid, beginprice, endprice, ispublic, keyWords, orderby, isasc ? 1 : 0, pageSize, pageIndex, ref totalCount, ref pageCount);

            List<Products> list = new List<Products>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Products model = new Products();
                model.FillData(dr);
                if (!string.IsNullOrEmpty(model.UnitID))
                {
                    model.UnitName = GetUnitByID(model.UnitID).UnitName;
                }
                model.IsPublicStr = CommonBusiness.GetEnumDesc((EnumProductPublicStatus)model.IsPublic);
                list.Add(model);
            }
            return list;
        }

        public Products GetProductByID(string productid, string clientid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductByID(productid);

            Products model = new Products();
            if (ds.Tables.Contains("Product") && ds.Tables["Product"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Product"].Rows[0]);
                model.Category = GetCategoryByID(model.CategoryID);
                model.SmallUnit = GetUnitByID(model.UnitID);

                if (!string.IsNullOrEmpty(model.ProviderID))
                {
                    model.Providers = ProvidersBusiness.BaseBusiness.GetProviderByID(model.ProviderID);
                }

                model.ProductDetails = new List<ProductDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    //子产品
                    ProductDetail detail = new ProductDetail();
                    detail.FillData(item);

                    model.ProductDetails.Add(detail);
                }
            }

            return model;
        }

        public bool IsExistProductCode(string code, string clientid)
        {
            object obj = CommonBusiness.Select("Products", " Count(0) ", "ClientID='" + clientid + "' and ProductCode='" + code + "'");
            return Convert.ToInt32(obj) > 0;
        }

        public List<ProductDetail> GetFilterProducts(string categoryid, List<FilterAttr> Attrs, int doctype, string beginprice, string endprice, int ispublic, string keyWords, string orderby, bool isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new ProductsDAL();
            StringBuilder attrbuild = new StringBuilder();
            StringBuilder salebuild = new StringBuilder();
            foreach (var attr in Attrs)
            {
                if (attr.Type == EnumAttrType.Parameter)
                {
                    attrbuild.Append(" and p.ValueList like '%" + attr.ValueID + "%'");
                }
                else if (attr.Type == EnumAttrType.Specification)
                {
                    salebuild.Append(" and AttrValue like '%" + attr.ValueID + "%'");
                }
            }

            DataSet ds = dal.GetFilterProducts(categoryid, attrbuild.ToString(), salebuild.ToString(), doctype, beginprice, endprice, ispublic, keyWords, orderby, isasc ? 1 : 0, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<ProductDetail> list = new List<ProductDetail>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductDetail model = new ProductDetail();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<ProductDetail> GetProductDetails(string wareid, string keywords, string clientid)
        {
            DataSet ds = ProductsDAL.BaseProvider.GetProductDetails(wareid, keywords, clientid);

            List<ProductDetail> list = new List<ProductDetail>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductDetail model = new ProductDetail();
                model.FillData(dr);

                list.Add(model);
            }
            return list;
        }

        public List<ProductUseLogEntity> GetProductUseLogs(string productid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            DataSet ds = ProductsDAL.BaseProvider.GetProductUseLogs(productid, pageSize, pageIndex, ref totalCount, ref pageCount);

            List<ProductUseLogEntity> list = new List<ProductUseLogEntity>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductUseLogEntity model = new ProductUseLogEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public Products GetProductByIDForDetails(string productid, string clientid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductByIDForDetails(productid, clientid);
           
            Products model = new Products();

            if (ds.Tables.Contains("Product") && ds.Tables["Product"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Product"].Rows[0]);

                model.SmallUnit = GetUnitByID(model.UnitID);

                model.AttrLists = new List<ProductAttr>();
                model.SaleAttrs = new List<ProductAttr>();

                model.Providers = new ProvidersEntity();
                if (!string.IsNullOrEmpty(model.ProviderID))
                {
                    model.Providers.FillData(ds.Tables["Providers"].Rows[0]);
                    if (!string.IsNullOrEmpty(model.Providers.CityCode))
                    {
                        var city = CommonBusiness.GetCityByCode(model.Providers.CityCode);
                        model.Providers.Address = city.Description + model.Providers.Address;
                    }
                }

                if (!string.IsNullOrEmpty(model.CategoryID))
                {
                    var category = GetCategoryByID(model.CategoryID);
                    foreach (var attr in category.AttrLists)
                    {
                        ProductAttr attrModel = new ProductAttr();
                        attrModel.AttrName = attr.AttrName;
                        attrModel.AttrValues = new List<AttrValue>();
                        foreach (var value in attr.AttrValues)
                        {
                            if (model.AttrValueList.IndexOf(value.ValueID) >= 0)
                            {
                                attrModel.AttrValues.Add(value);
                                model.AttrLists.Add(attrModel);
                                break;
                            }
                        }
                    }
                    model.SaleAttrs = category.SaleAttrs;
                }

                model.ProductDetails = new List<ProductDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    ProductDetail detail = new ProductDetail();
                    detail.FillData(item);
                    detail.DetailStocks = new List<ProductStock>();
                    foreach (var stocktr in ds.Tables["Stocks"].Select("ProductDetailID='" + detail.ProductDetailID + "'"))
                    {
                        ProductStock stock = new ProductStock();
                        stock.FillData(stocktr);
                        detail.DetailStocks.Add(stock);
                    }
                    model.ProductDetails.Add(detail);
                }
            }

            return model;
        }

        public string AddProduct(string productCode, string productName, string generalName,  string prodiverid, string unitid,
                                 string categoryid, int status, int ispublic, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price, decimal weight, 
                                 int isallow,  decimal discountValue, string productImg, string shapeCode, string description, List<ProductDetail> details, string operateid, string clientid,ref int result)
        {
            lock (SingleLock)
            {
                var dal = new ProductsDAL();
                string pid = dal.AddProduct(productCode, productName, generalName, prodiverid,  unitid, categoryid, status, ispublic, attrlist,
                                        valuelist, attrvaluelist, commonprice, price, weight, isallow, discountValue, productImg, shapeCode, description, operateid, clientid, ref result);
                //产品添加成功添加子产品
                if (!string.IsNullOrEmpty(pid))
                {
                    //日志
                    LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Product, EnumLogType.Create, "", operateid, clientid);

                    foreach (var model in details) 
                    {
                        dal.AddProductDetails(pid, model.DetailsCode, model.ShapeCode, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Price, model.Weight, model.ImgS, model.Description, model.Remark, operateid, clientid);
                    }
                }
                return pid;
            }
        }

        public string AddProductDetails(string productid, string productCode, string shapeCode, string attrlist, string valuelist, string attrvaluelist, decimal price, decimal weight, string productImg, string description, string remark, string operateid, string clientid)
        {
            lock (SingleLock)
            {
                var dal = new ProductsDAL();
                return dal.AddProductDetails(productid, productCode, shapeCode, attrlist, valuelist, attrvaluelist, price, weight, productImg, description, remark, operateid, clientid);
            }
        }


        public bool UpdateProductStatus(string productid, EnumStatus status, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "Status", ((int)status).ToString(), " ProductID='" + productid + "'");
        }


        public bool AuditProductIsPublic(string productid, int ispublic, string operateIP, string operateID)
        {
            bool bl = CommonBusiness.Update("Products", "IsPublic", ispublic, " ProductID='" + productid + "' and IsPublic=1");
            if (bl)
            {
                string msg = ispublic == 2 ? "通过材料公开申请" : "驳回材料公开申请";
                LogBusiness.AddLog(productid, EnumLogObjectType.Product, msg, operateID, operateIP, "", "");
            }
            return bl;
        }

        public bool DeleteProductIsPublic(string productid, string operateIP, string operateID)
        {
            bool bl = CommonBusiness.Update("Products", "IsPublic", 4, " ProductID='" + productid + "' and IsPublic=2");
            if (bl)
            {
                string msg = "撤销材料公开状态";
                LogBusiness.AddLog(productid, EnumLogObjectType.Product, msg, operateID, operateIP, "", "");
            }
            return bl;
        }

        public bool UpdateProduct(string productid, string productCode, string productName, string generalName, string prodiverid, string unitid, 
                         int status, int ispublic, string categoryid, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price, decimal weight, 
                         int isallow, decimal discountValue, string productImg, string shapeCode, string description, string operateid, string clientid,ref int result)
        {

            var dal = new ProductsDAL();
            return dal.UpdateProduct(productid, productCode, productName, generalName, prodiverid, unitid, status, ispublic, categoryid, attrlist,
                                    valuelist, attrvaluelist, commonprice, price, weight, isallow, discountValue, productImg, shapeCode, description, operateid, clientid,ref result);
        }

        public bool UpdateProductDetailsStatus(string productdetailid, EnumStatus status, string operateIP, string operateID)
        {
            return CommonBusiness.Update("ProductDetail", "Status", (int)status, " ProductDetailID='" + productdetailid + "'");
        }

        public bool UpdateProductDetails(string detailid, string productid, string productCode, string shapeCode, string attrlist, string valuelist, string attrvaluelist, decimal price, decimal weight, string description, string remark, string productImg, string operateid, string clientid)
        {
            lock (SingleLock)
            {
                var dal = new ProductsDAL();
                return dal.UpdateProductDetails(detailid, productid, productCode, shapeCode, attrlist, valuelist, attrvaluelist, price, weight, description, remark, productImg);
            }
        }

        public bool DeleteProductByID(string pid, string operateid,out int result)
        {
            var dal = new ProductsDAL();
            return dal.DeleteProductByID(pid, operateid, out result);
        }

        public bool DeleteProductDetailByID(string did, string operateid, out int result)
        {
            var dal = new ProductsDAL();
            return dal.DeleteProductDetailByID(did, operateid, out result);
        }
        #endregion

    }
}
