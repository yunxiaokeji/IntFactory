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

        #region 品牌

        public List<Brand> GetBrandList(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetBrandList(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<Brand> list = new List<Brand>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Brand model = new Brand();
                model.FillData(dr);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }
            return list;
        }

        public List<Brand> GetBrandList(string clientID)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetBrandList(clientID);

            List<Brand> list = new List<Brand>();
            foreach (DataRow dr in dt.Rows)
            {
                Brand model = new Brand();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public Brand GetBrandByBrandID(string brandID)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetBrandByBrandID(brandID);

            Brand model = new Brand();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
            }
            return model;
        }

        public string AddBrand(string name, string anotherName, string icoPath, string countryCode, string cityCode, int status, string remark, string brandStyle, string operateIP, string operateID, string clientID)
        {
            lock (SingleLock)
            {
                //if (!string.IsNullOrEmpty(icoPath))
                //{
                //    if (icoPath.IndexOf("?") > 0)
                //    {
                //        icoPath = icoPath.Substring(0, icoPath.IndexOf("?"));
                //    }
                //    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(icoPath));
                //    icoPath = FILEPATH + file.Name;
                //    if (file.Exists)
                //    {
                //        file.MoveTo(HttpContext.Current.Server.MapPath(icoPath));
                //    }
                //}

                return new ProductsDAL().AddBrand(name, anotherName, icoPath, countryCode, cityCode, status, remark, brandStyle, operateIP, operateID, clientID);
            }
        }

        public bool UpdateBrandStatus(string brandID, EnumStatus status, string operateIP, string operateID)
        {
            bool bl = CommonBusiness.Update("Brand", "Status", ((int)status).ToString(), " BrandID='" + brandID + "'");

            return bl;
        }

        public bool UpdateBrand(string brandID, string name, string anotherName, string countryCode, string cityCode, string icopath, int status, string remark, string brandStyle, string operateIP, string operateID)
        {
            if (!string.IsNullOrEmpty(icopath) && icopath.IndexOf(TempPath) >= 0)
            {
                if (icopath.IndexOf("?") > 0)
                {
                    icopath = icopath.Substring(0, icopath.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(icopath));
                icopath = FILEPATH + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(icopath));
                }
            }
            var dal = new ProductsDAL();
            return dal.UpdateBrand(brandID, name, anotherName, countryCode, cityCode, status, icopath, remark, brandStyle, operateIP, operateID);
        }

        #endregion

        #region 单位

        public List<ProductUnit> GetUnits()
        {
            if (CacheUnits.Count() > 0)
            {
                return CacheUnits;
            }

            var dal = new ProductsDAL();
            DataTable dt = dal.GetUnits();

            List<ProductUnit> list = new List<ProductUnit>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductUnit model = new ProductUnit();
                model.FillData(dr);
                list.Add(model);
                CacheUnits.Add(model);
            }
            return list;
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
            var dal = new ProductsDAL();
            bool bl = dal.UpdateUnitStatus(unitid, (int)EnumStatus.Delete);
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

            List<ProductAttr> list = new List<ProductAttr>();
            DataTable dt = new ProductsDAL().GetAttrs();
            foreach (DataRow dr in dt.Rows)
            {
                ProductAttr model = new ProductAttr();
                model.FillData(dr);
                model.AttrValues = new List<AttrValue>();
                CacheAttrs.Add(model);
            }
            return list;
        }

        public List<ProductAttr> GetAttrList(string categoryid, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetAttrList(categoryid, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount);

            List<ProductAttr> list = new List<ProductAttr>();
            if (ds.Tables.Contains("Attrs"))
            {
                foreach (DataRow dr in ds.Tables["Attrs"].Rows)
                {
                    ProductAttr model = new ProductAttr();
                    model.FillData(dr);

                    //List<AttrValue> valueList = new List<AttrValue>();
                    //foreach (DataRow drValue in ds.Tables["Values"].Select("AttrID='" + model.AttrID + "'"))
                    //{
                    //    AttrValue valueModel = new AttrValue();
                    //    valueModel.FillData(drValue);
                    //    valueList.Add(valueModel);
                    //}
                    //model.AttrValues = valueList;

                    list.Add(model);
                }
            }
            return list;
        }

        public List<ProductAttr> GetAttrsByCategoryID(string categoryid)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetAttrsByCategoryID(categoryid);

            List<ProductAttr> list = new List<ProductAttr>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductAttr model = new ProductAttr();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public ProductAttr GetAttrByID(string attrid)
        {
            var list = GetAttrs();

            if (list.Where(m => m.AttrID.ToLower() == attrid.ToLower()).Count() > 0)
            {
                var cache = list.Where(m => m.AttrID.ToLower() == attrid.ToLower()).FirstOrDefault();
                if (cache.AttrValues.Count > 0)
                {
                    return cache;
                }
                else
                {
                    DataTable dt = new ProductsDAL().GetAttrValuesByAttrID(attrid);
                    foreach (DataRow item in dt.Rows)
                    {
                        AttrValue attrValue = new AttrValue();
                        attrValue.FillData(item);
                        cache.AttrValues.Add(attrValue);
                    }
                    return cache;
                }
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
                CacheAttrs.Add(model);
            }

            return model;
        }

        public ProductAttr GetTaskPlateAttrByCategoryID(string categoryid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetTaskPlateAttrByCategoryID(categoryid);

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
            }

            return model;
        }

        public string AddAttr(string attrname, string description, string categoryID, int type, string operateid)
        {
            var attrid = Guid.NewGuid().ToString().ToLower();
            var dal = new ProductsDAL();
            if (dal.AddAttr(attrid, attrname, description, categoryID, type, operateid))
            {
                CacheAttrs.Add(new ProductAttr()
                {
                    AttrID = attrid,
                    AttrName = attrname,
                    Description = description,
                    CategoryID = categoryID,
                    ClientID = "",
                    CreateTime = DateTime.Now,
                    CreateUserID = operateid,
                    Status = 1,
                    AttrValues = new List<AttrValue>()
                });
                return attrid;
            }
            return string.Empty;
        }

        public string AddAttrValue(string valueName, string attrid, string operateid)
        {
            var valueid = Guid.NewGuid().ToString().ToLower();
            var dal = new ProductsDAL();
            if (dal.AddAttrValue(valueid, valueName, attrid, operateid))
            {
                var model = GetAttrByID(attrid);
                model.AttrValues.Add(new AttrValue()
                {
                    ValueID = valueid,
                    ValueName = valueName,
                    Status = 1,
                    AttrID = attrid,
                    CreateTime = DateTime.Now
                });

                return valueid;
            }
            return string.Empty;
        }

        public bool UpdateProductAttr(string attrID, string attrName, string description, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            var bl = dal.UpdateProductAttr(attrID, attrName, description);
            if (bl)
            {
                var model = GetAttrByID(attrID);
                model.AttrName = attrName;
                model.Description = description;
            }
            return bl;
        }

        public bool UpdateAttrValue(string valueID, string attrid, string valueName, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            var bl = dal.UpdateAttrValue(valueID, valueName);
            if (bl)
            {
                var model = GetAttrByID(attrid);
                var value = model.AttrValues.Where(m => m.ValueID == valueID).FirstOrDefault();
                value.ValueName = valueName;
            }
            return bl;
        }

        public bool UpdateProductAttrStatus(string attrid, EnumStatus status, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            var bl = dal.UpdateProductAttrStatus(attrid, (int)status);
            if (bl)
            {
                var model = GetAttrByID(attrid);
                CacheAttrs.Remove(model);
            }
            return bl;
        }

        public bool UpdateCategoryAttrStatus(string categoryid, string attrid, EnumStatus status, int type, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            return dal.UpdateCategoryAttrStatus(categoryid, attrid, (int)status, type);
        }

        public bool UpdateAttrValueStatus(string valueid, string attrid, EnumStatus status, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            bool bl = dal.UpdateAttrValueStatus(valueid, (int)status);
            if (bl)
            {
                var model = GetAttrByID(attrid);
                var value = model.AttrValues.Where(m => m.ValueID == valueid).FirstOrDefault();
                model.AttrValues.Remove(value);
            }
            return bl;
        }

        public bool UpdateAttrValueSort(string valueid, string attrid, int sort) {
            var dal = new ProductsDAL();
            bool bl = dal.UpdateAttrValueStatus(valueid, attrid,sort);

            if (bl)
            {
                var model = GetAttrByID(attrid);
                model.AttrValues=new List<AttrValue>();

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

            DataTable dt = ProductsDAL.BaseProvider.GetCategorys();

            List<Category> list = new List<Category>();

            foreach (DataRow dr in dt.Rows)
            {
                Category model = new Category();
                model.FillData(dr);
                model.ChildCategory = new List<Category>();
                list.Add(model);

                CacheCategory.Add(model);
            }
            return list;

        }

        public List<Category> GetChildCategorysByID(string categoryid, EnumCategoryType type)
        {
            var cacheList = GetCategorys();
            if (cacheList.Where(m => m.PID.ToLower() == categoryid.ToLower() && m.CategoryType == (int)type).Count() > 0)
            {
                return cacheList.Where(m => m.PID.ToLower() == categoryid.ToLower() && m.CategoryType == (int)type).ToList();
            }

            DataTable dt = ProductsDAL.BaseProvider.GetChildCategorysByID(categoryid, (int)type);

            List<Category> list = new List<Category>();

            foreach (DataRow dr in dt.Rows)
            {
                Category model = new Category();
                model.FillData(dr);
                model.ChildCategory = new List<Category>();
                list.Add(model);

                CacheCategory.Add(model);
                
            }
            return list;
        }

        public List<Category> GetClientCategorysByPID(string categoryid, EnumCategoryType type, string clientid)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetClientCategorysByPID(categoryid, (int)type, clientid);

            List<Category> list = new List<Category>();

            foreach (DataRow dr in dt.Rows)
            {
                Category model = new Category();
                model.FillData(dr);
                list.Add(model);

            }
            return list;
        }

        public Category GetCategoryByID(string categoryid)
        {
            var cacheList = GetCategorys();
            if (cacheList.Where(m => m.CategoryID.ToLower() == categoryid.ToLower()).Count() > 0)
            {
                return cacheList.Where(m => m.CategoryID.ToLower() == categoryid.ToLower()).FirstOrDefault();
            }

            var dal = new ProductsDAL();
            DataTable dt = dal.GetCategoryByID(categoryid);

            Category model = new Category();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.ChildCategory = new List<Category>();
                CacheCategory.Add(model);
            }

            return model;
        }

        public Category GetCategoryDetailByID(string categoryid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetCategoryDetailByID(categoryid);

            Category model = new Category();
            if (ds.Tables.Contains("Category") && ds.Tables["Category"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Category"].Rows[0]);
                List<ProductAttr> salelist = new List<ProductAttr>();
                List<ProductAttr> attrlist = new List<ProductAttr>();

                foreach (DataRow attr in ds.Tables["Attrs"].Rows)
                {

                    ProductAttr modelattr = new ProductAttr();
                    modelattr.FillData(attr);
                    if (modelattr.Type == 1)
                    {
                        attrlist.Add(modelattr);
                    }
                    else if (modelattr.Type == 2)
                    {
                        salelist.Add(modelattr);
                    }
                    modelattr.AttrValues = new List<AttrValue>();
                    foreach (DataRow value in ds.Tables["Values"].Select("AttrID='" + modelattr.AttrID + "'"))
                    {
                        AttrValue valuemodel = new AttrValue();
                        valuemodel.FillData(value);
                        modelattr.AttrValues.Add(valuemodel);
                    }
                }

                model.SaleAttrs = salelist;
                model.AttrLists = attrlist;
            }

            return model;
        }

        public Category GetOrderCategoryDetailsByID(string categoryid, string orderid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetOrderCategoryDetailsByID(categoryid, orderid);

            Category model = new Category();
            if (ds.Tables.Contains("Category") && ds.Tables["Category"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Category"].Rows[0]);
                List<ProductAttr> salelist = new List<ProductAttr>();
                List<ProductAttr> attrlist = new List<ProductAttr>();

                foreach (DataRow attr in ds.Tables["Attrs"].Rows)
                {

                    ProductAttr modelattr = new ProductAttr();
                    modelattr.FillData(attr);
                    if (modelattr.Type == 1)
                    {
                        attrlist.Add(modelattr);
                    }
                    else if (modelattr.Type == 2)
                    {
                        salelist.Add(modelattr);
                    }
                    modelattr.AttrValues = new List<AttrValue>();
                    foreach (DataRow value in ds.Tables["Values"].Select("AttrID='" + modelattr.AttrID + "'"))
                    {
                        AttrValue valuemodel = new AttrValue();
                        valuemodel.FillData(value);
                        modelattr.AttrValues.Add(valuemodel);
                    }
                }

                model.SaleAttrs = salelist;
                model.AttrLists = attrlist;
            }

            return model;
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
                        ChildCategory = new List<Category>()
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
                        Description = description
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
            var dal = new ProductsDAL();
            return dal.AddCategoryAttr(categoryid, attrid, type, operateID);
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
                if (!string.IsNullOrEmpty(model.SmallUnitID))
                {
                    model.UnitName = GetUnitByID(model.SmallUnitID).UnitName;
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
                if (!string.IsNullOrEmpty(model.SmallUnitID))
                {
                    model.UnitName = GetUnitByID(model.SmallUnitID).UnitName;
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
                model.Category = GetCategoryDetailByID(model.CategoryID);
                model.SmallUnit = GetUnitByID(model.SmallUnitID);

                if (!string.IsNullOrEmpty(model.ProdiverID))
                {
                    model.Providers = ProvidersBusiness.BaseBusiness.GetProviderByID(model.ProdiverID);
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

        public List<ProductDetail> GetProductDetails(string wareid, string keywords, string agentid, string clientid)
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

                model.SmallUnit = GetUnitByID(model.SmallUnitID);

                model.AttrLists = new List<ProductAttr>();
                model.SaleAttrs = new List<ProductAttr>();

                model.Providers = new ProvidersEntity();
                if (!string.IsNullOrEmpty(model.ProdiverID))
                {
                    model.Providers.FillData(ds.Tables["Providers"].Rows[0]);
                    if (!string.IsNullOrEmpty(model.Providers.CityCode))
                    {
                        var city = CommonBusiness.GetCityByCode(model.Providers.CityCode);
                        model.Providers.Address = city.Description + model.Providers.Address;
                    }
                }

                foreach (DataRow attrtr in ds.Tables["Attrs"].Rows)
                {
                    ProductAttr attrModel = new ProductAttr();
                    attrModel.FillData(attrtr);
                    attrModel.AttrValues = new List<AttrValue>();

                    //参数
                    if (attrModel.Type == (int)EnumAttrType.Parameter)
                    {
                        foreach (DataRow valuetr in ds.Tables["Values"].Select("AttrID='" + attrModel.AttrID + "'"))
                        {
                            AttrValue valueModel = new AttrValue();
                            valueModel.FillData(valuetr);
                            if (model.AttrValueList.IndexOf(valueModel.ValueID) >= 0)
                            {
                                attrModel.AttrValues.Add(valueModel);
                                model.AttrLists.Add(attrModel);
                                break;
                            }
                        }
                    }
                    else
                    {
                        model.SaleAttrs.Add(attrModel);
                    }
                }

                model.ProductDetails = new List<ProductDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {

                    ProductDetail detail = new ProductDetail();
                    detail.FillData(item);

                    //填充存在的规格
                    foreach (var attrModel in model.SaleAttrs)
                    {
                        foreach (DataRow valuetr in ds.Tables["Values"].Select("AttrID='" + attrModel.AttrID + "'"))
                        {
                            AttrValue valueModel = new AttrValue();
                            valueModel.FillData(valuetr);
                            if (detail.AttrValue.IndexOf(valueModel.ValueID) >= 0)
                            {
                                if (attrModel.AttrValues.Where(v => v.ValueID == valueModel.ValueID).Count() == 0)
                                {
                                    attrModel.AttrValues.Add(valueModel);
                                }
                                break;
                            }
                        }
                    }
                    model.ProductDetails.Add(detail);
                }
            }

            return model;
        }

        public string AddProduct(string productCode, string productName, string generalName, bool iscombineproduct, string prodiverid, string brandid, string bigunitid, string smallunitid, int bigSmallMultiple,
                                 string categoryid, int status, int ispublic, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price, decimal weight, bool isnew,
                                 bool isRecommend, int isallow, int isautosend, int effectiveDays, decimal discountValue, string productImg, string shapeCode, string description, List<ProductDetail> details, string operateid, string agentid, string clientid)
        {
            lock (SingleLock)
            {
                var dal = new ProductsDAL();
                string pid = dal.AddProduct(productCode, productName, generalName, iscombineproduct, prodiverid, brandid, bigunitid, smallunitid, bigSmallMultiple, categoryid, status, ispublic, attrlist,
                                        valuelist, attrvaluelist, commonprice, price, weight, isnew, isRecommend, isallow, isautosend, effectiveDays, discountValue, productImg, shapeCode, description, operateid, clientid);
                //产品添加成功添加子产品
                if (!string.IsNullOrEmpty(pid))
                {
                    //日志
                    LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Product, EnumLogType.Create, "", operateid, agentid, clientid);

                    foreach (var model in details) 
                    {
                        dal.AddProductDetails(pid, model.DetailsCode, model.ShapeCode, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Price, model.Weight, model.BigPrice, model.ImgS, model.Description, model.Remark, operateid, clientid);
                    }
                }
                return pid;
            }
        }

        public string AddProductDetails(string productid, string productCode, string shapeCode, string attrlist, string valuelist, string attrvaluelist, decimal price, decimal weight, decimal bigprice, string productImg, string description, string remark, string operateid, string clientid)
        {
            lock (SingleLock)
            {
                var dal = new ProductsDAL();
                return dal.AddProductDetails(productid, productCode, shapeCode, attrlist, valuelist, attrvaluelist, price, weight, bigprice, productImg, description, remark, operateid, clientid);
            }
        }


        public bool UpdateProductStatus(string productid, EnumStatus status, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "Status", ((int)status).ToString(), " ProductID='" + productid + "'");
        }

        public bool UpdateProductIsNew(string productid, bool isNew, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "IsNew", isNew ? "1" : "0", " ProductID='" + productid + "'");
        }

        public bool AuditProductIsPublic(string productid, int ispublic, string operateIP, string operateID)
        {
            bool bl = CommonBusiness.Update("Products", "IsPublic", ispublic, " ProductID='" + productid + "' and IsPublic=1");
            if (bl)
            {
                string msg = ispublic == 2 ? "通过材料公开申请" : "驳回材料公开申请";
                LogBusiness.AddLog(productid, EnumLogObjectType.Product, msg, operateID, operateIP, "", "", "");
            }
            return bl;
        }

        public bool DeleteProductIsPublic(string productid, string operateIP, string operateID)
        {
            bool bl = CommonBusiness.Update("Products", "IsPublic", 4, " ProductID='" + productid + "' and IsPublic=2");
            if (bl)
            {
                string msg = "撤销材料公开状态";
                LogBusiness.AddLog(productid, EnumLogObjectType.Product, msg, operateID, operateIP, "", "", "");
            }
            return bl;
        }

        public bool UpdateProductIsRecommend(string productid, bool isRecommend, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "IsRecommend", isRecommend ? "1" : "0", " ProductID='" + productid + "'");
        }

        public bool UpdateProduct(string productid, string productCode, string productName, string generalName, bool iscombineproduct, string prodiverid, string brandid, string bigunitid, string smallunitid, int bigSmallMultiple,
                         int status, int ispublic, string categoryid, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price, decimal weight, bool isnew,
                         bool isRecommend, int isallow, int isautosend, int effectiveDays, decimal discountValue, string productImg, string shapeCode, string description, string operateid, string clientid)
        {

            var dal = new ProductsDAL();
            return dal.UpdateProduct(productid, productCode, productName, generalName, iscombineproduct, prodiverid, brandid, bigunitid, smallunitid, bigSmallMultiple, status, ispublic, categoryid, attrlist,
                                    valuelist, attrvaluelist, commonprice, price, weight, isnew, isRecommend, isallow, isautosend, effectiveDays, discountValue, productImg, shapeCode, description, operateid, clientid);
        }

        public bool UpdateProductDetailsStatus(string productdetailid, EnumStatus status, string operateIP, string operateID)
        {
            return CommonBusiness.Update("ProductDetail", "Status", (int)status, " ProductDetailID='" + productdetailid + "'");
        }

        public bool UpdateProductDetails(string detailid, string productid, string productCode, string shapeCode, decimal bigPrice, string attrlist, string valuelist, string attrvaluelist, decimal price, decimal weight, string description, string remark, string productImg, string operateid, string clientid)
        {
            lock (SingleLock)
            {
                var dal = new ProductsDAL();
                return dal.UpdateProductDetails(detailid, productid, productCode, shapeCode, bigPrice, attrlist, valuelist, attrvaluelist, price, weight, description, remark, productImg);
            }
        }

        #endregion

    }
}
