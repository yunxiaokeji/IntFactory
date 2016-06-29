using IntFactoryBusiness;
using IntFactoryEntity;
using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXManage.Models;

namespace YXManage.Controllers
{
    [YXManage.Common.UserAuthorize]
    public class ProductsController : BaseController
    {
        //
        // GET: /Product/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 产品单位列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Unit()
        {
            ViewBag.Items = new ProductsBusiness().GetUnits();
            return View();
        }

        /// <summary>
        /// 产品属性 
        /// </summary>
        /// <returns></returns>
        public ActionResult Attr()
        {
            return View();
        }

        public ActionResult AttrDetail(string id)
        {
            ViewBag.AttrID = id;
            return View();
        }

        /// <summary>
        /// 产品分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Category()
        {
            var list = new ProductsBusiness().GetChildCategorysByID("", EnumCategoryType.Product);
            ViewBag.Items = list;
            return View();
        }

        public ActionResult OrderCategory()
        {
            var list = new ProductsBusiness().GetChildCategorysByID("", EnumCategoryType.Order);
            ViewBag.Items = list;
            return View();
        }

        public ActionResult Products()
        {
            ViewBag.Url = CloudSalesTool.AppSettings.Settings["IntFactoryUrl"];
            return View();
        }

        public ActionResult ChooseDetail(string pid)
        {
            if (string.IsNullOrEmpty(pid))
            {
                return Redirect("ProductList");
            }
            var model = new ProductsBusiness().GetProductByIDForDetails(pid, "");
            if (model == null || string.IsNullOrEmpty(model.ProductID))
            {
                return Redirect("ProductList");
            }
            ViewBag.Url = CloudSalesTool.AppSettings.Settings["IntFactoryUrl"];
            ViewBag.Model = model;
            return View();
        }

        #region 单位

        /// <summary>
        /// 保存单位
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public JsonResult SaveUnit(string unit)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProductUnit model = serializer.Deserialize<ProductUnit>(unit);

            string UnitID = "";
            if (string.IsNullOrEmpty(model.UnitID))
            {
                UnitID = new ProductsBusiness().AddUnit(model.UnitName, model.Description, CurrentUser.UserID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateUnit(model.UnitID, model.UnitName, model.Description, CurrentUser.UserID);
                if (bl)
                {
                    UnitID = model.UnitID;
                }
            }
            JsonDictionary.Add("ID", UnitID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 删除单位
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteUnit(string unitID)
        {
            bool bl = new ProductsBusiness().DeleteUnit(unitID, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="keyWorks"></param>
        /// <returns></returns>
        public JsonResult GetAttrList(int index, string keyWorks)
        {
            List<ProductAttr> list = new List<ProductAttr>();

            int totalCount = 0, pageCount = 0;
            list = new ProductsBusiness().GetAttrList("", keyWorks, PageSize, index, ref totalCount, ref pageCount);

            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 获取所有属性
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAttrsByCategoryID(string categoryid)
        {
            List<ProductAttr> list = new List<ProductAttr>();
            list = new ProductsBusiness().GetAttrsByCategoryID(categoryid);

            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存属性
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public JsonResult SaveAttr(string attr)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProductAttr model = serializer.Deserialize<ProductAttr>(attr);

            string attrID = string.Empty;
            if (string.IsNullOrEmpty(model.AttrID))
            {
                attrID = new ProductsBusiness().AddAttr(model.AttrName, model.Description, model.CategoryID, model.Type, CurrentUser.UserID);
            }
            else if (new ProductsBusiness().UpdateProductAttr(model.AttrID, model.AttrName, model.Description, OperateIP, CurrentUser.UserID))
            {
                attrID = model.AttrID.ToString();
            }


            JsonDictionary.Add("ID", attrID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取属性详情
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public JsonResult GetAttrByID(string attrID = "")
        {
            if (string.IsNullOrEmpty(attrID))
            {
                JsonDictionary.Add("Item", null);
            }
            else
            {
                var model = new ProductsBusiness().GetAttrByID(attrID);
                JsonDictionary.Add("Item", model);
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存属性值
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public JsonResult SaveAttrValue(string value)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            AttrValue model = serializer.Deserialize<AttrValue>(value);

            string valueID = string.Empty;
            if (!string.IsNullOrEmpty(model.AttrID))
            {
                if (string.IsNullOrEmpty(model.ValueID))
                {
                    valueID = new ProductsBusiness().AddAttrValue(model.ValueName, model.AttrID, CurrentUser.UserID);
                }
                else if (new ProductsBusiness().UpdateAttrValue(model.ValueID, model.AttrID, model.ValueName, OperateIP, CurrentUser.UserID))
                {
                    valueID = model.ValueID.ToString();
                }
            }

            JsonDictionary.Add("ID", valueID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除分类属性
        /// </summary>
        /// <param name="categoryid"></param>
        /// <param name="attrid"></param>
        /// <returns></returns>
        public JsonResult DeleteCategoryAttr(string categoryid, string attrid, int type)
        {
            bool bl = new ProductsBusiness().UpdateCategoryAttrStatus(categoryid, attrid, EnumStatus.Delete, type, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 添加分类通用属性
        /// </summary>
        /// <param name="categoryid"></param>
        /// <param name="attrid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonResult AddCategoryAttr(string categoryid, string attrid, int type)
        {
            bool bl = new ProductsBusiness().AddCategoryAttr(categoryid, attrid, type, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="attrid"></param>
        /// <returns></returns>
        public JsonResult DeleteProductAttr(string attrid)
        {
            bool bl = new ProductsBusiness().UpdateProductAttrStatus(attrid, EnumStatus.Delete, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除属性值
        /// </summary>
        /// <param name="valueid"></param>
        /// <returns></returns>
        public JsonResult DeleteAttrValue(string valueid,string attrid)
        {
            bool bl = new ProductsBusiness().UpdateAttrValueStatus(valueid, attrid, EnumStatus.Delete, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 更新属性值排序
        /// </summary>
        /// <param name="valueid"></param>
        /// <param name="attrid"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public JsonResult UpdateAttrValueSort(string valueid, string attrid,int sort)
        {
            bool bl = new ProductsBusiness().UpdateAttrValueSort(valueid, attrid,sort);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region 分类

        /// <summary>
        /// 保存分类
        /// </summary>
        /// <param name="category"></param>
        /// <param name="attrlist"></param>
        /// <returns></returns>
        public JsonResult SavaCategory(string category, string attrlist, string saleattr)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Category model = serializer.Deserialize<Category>(category);
            //参数
            if (!string.IsNullOrEmpty(attrlist))
            {
                attrlist = attrlist.Substring(0, attrlist.Length - 1);
            }
            //规格
            if (!string.IsNullOrEmpty(saleattr))
            {
                saleattr = saleattr.Substring(0, saleattr.Length - 1);
            }
            string caregoryid = "";
            if (string.IsNullOrEmpty(model.CategoryID))
            {
                caregoryid = new ProductsBusiness().AddCategory(model.CategoryCode, model.CategoryName, model.PID, model.CategoryType, model.Status.Value, attrlist.Split(',').ToList(), saleattr.Split(',').ToList(), model.Description, CurrentUser.UserID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateCategory(model.CategoryID, model.CategoryName, model.Status.Value, attrlist.Split(',').ToList(), saleattr.Split(',').ToList(), model.Description, CurrentUser.UserID);
                if (bl)
                {
                    caregoryid = model.CategoryID;
                }
            }
            JsonDictionary.Add("ID", caregoryid);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取下级分类
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public JsonResult GetChildCategorysByID(string categoryid, int type = 1)
        {
            var list = new ProductsBusiness().GetChildCategorysByID(categoryid, (EnumCategoryType)type);
            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取分类详情
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public JsonResult GetCategoryByID(string categoryid)
        {
            var model = new ProductsBusiness().GetCategoryByID(categoryid);
            JsonDictionary.Add("Model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 获取分类详情(带属性)
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public JsonResult GetCategoryDetailsByID(string categoryid)
        {
            var model = new ProductsBusiness().GetCategoryDetailByID(categoryid);
            JsonDictionary.Add("Model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteCategory(string id)
        {
            int result = 0;
            bool bl = new ProductsBusiness().DeleteCategory(id, CurrentUser.UserID, OperateIP, out result);
            JsonDictionary.Add("status", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        public JsonResult GetProductList(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterProduct model = serializer.Deserialize<FilterProduct>(filter);
            int totalCount = 0;
            int pageCount = 0;

            List<Products> list = new ProductsBusiness().GetProductsAll(model.CategoryID, model.ProviderID, model.BeginPrice, model.EndPrice, model.IsPublic, model.Keywords, model.OrderBy, model.IsAsc, PageSize, model.PageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateProductIsPublic(string productid, int ispublic)
        {
            bool bl = new ProductsBusiness().AuditProductIsPublic(productid, ispublic, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteProductIsPublic(string productid)
        {
            bool bl = new ProductsBusiness().DeleteProductIsPublic(productid, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
