using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity;
using System.Data;
using IntFactoryDAL;
using IntFactoryEnum;

namespace IntFactoryBusiness
{
    /// <summary>
    /// 属性Business
    /// </summary>
    public class AttrBusiness
    {
        private AttrDAL dal = new AttrDAL();

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

        public List<ProductAttr> GetAttrs()
        {
            if (CacheAttrs.Count > 0)
            {
                return CacheAttrs;
            }

            List<ProductAttr> list = new List<ProductAttr>();
            DataTable dt = dal.GetAttrs();
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
            DataSet ds = dal.GetAttrList(categoryid, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount);

            List<ProductAttr> list = new List<ProductAttr>();
            if (ds.Tables.Contains("Attrs"))
            {
                foreach (DataRow dr in ds.Tables["Attrs"].Rows)
                {
                    ProductAttr model = new ProductAttr();
                    model.FillData(dr);
                    model.AttrValues = new List<AttrValue>();
                    list.Add(model);
                }
            }
            return list;
        }

        public List<ProductAttr> GetAttrsByCategoryID(string categoryid)
        {
            DataTable dt = dal.GetAttrsByCategoryID(categoryid);

            List<ProductAttr> list = new List<ProductAttr>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductAttr model = new ProductAttr();
                model.FillData(dr);
                model.AttrValues = new List<AttrValue>();
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
                    DataTable dt = dal.GetAttrValuesByAttrID(attrid);
                    foreach (DataRow item in dt.Rows)
                    {
                        AttrValue attrValue = new AttrValue();
                        attrValue.FillData(item);
                        cache.AttrValues.Add(attrValue);
                    }
                    return cache;
                }
            }
            DataSet ds = dal.GetAttrByID(attrid);

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

        public string AddAttrValue(string valueName, int sort, string attrid, string operateid)
        {
            var valueid = Guid.NewGuid().ToString().ToLower();
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
            return dal.UpdateCategoryAttrStatus(categoryid, attrid, (int)status, type);
        }

        public bool UpdateAttrValueStatus(string valueid, string attrid, EnumStatus status, string operateIP, string operateID)
        {
            bool bl = dal.UpdateAttrValueStatus(valueid, (int)status);
            if (bl)
            {
                var model = GetAttrByID(attrid);
                var value = model.AttrValues.Where(m => m.ValueID == valueid).FirstOrDefault();
                model.AttrValues.Remove(value);
            }
            return bl;
        }

        public static void ClearAttrsCache()
        {
            CacheAttrs = null;
        }
    }
}
