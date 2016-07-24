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

        public ActionResult ChooseDetail(int type, string did, string pid)
        {
            if (string.IsNullOrEmpty(pid))
            {
                return Redirect("ProductList");
            }
            var model = new ProductsBusiness().GetProductByIDForDetails(type, did, pid, "");
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
                UnitID = new ProductsBusiness().AddUnit(model.UnitName, "", CurrentUser.UserID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateUnit(model.UnitID, model.UnitName, "", CurrentUser.UserID);
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

        public JsonResult GetAttrList(int index, string keyWorks)
        {
            List<ProductAttr> list = new List<ProductAttr>();

            int totalCount = 0, pageCount = 0;
            list = new ProductsBusiness().GetAttrList(keyWorks, PageSize, index, ref totalCount, ref pageCount);

            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

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

        public JsonResult SaveAttr(string attr)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProductAttr model = serializer.Deserialize<ProductAttr>(attr);

            string attrID = string.Empty;
            if (string.IsNullOrEmpty(model.AttrID))
            {
                attrID = new ProductsBusiness().AddAttr(model.AttrName, model.Description, "", model.Type, CurrentUser.UserID);
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

        public JsonResult SaveAttrValue(string value)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            AttrValue model = serializer.Deserialize<AttrValue>(value);

            string valueID = string.Empty;
            if (!string.IsNullOrEmpty(model.AttrID))
            {
                if (string.IsNullOrEmpty(model.ValueID))
                {
                    valueID = new ProductsBusiness().AddAttrValue(model.ValueName, model.Sort, model.AttrID, CurrentUser.UserID);
                }
                else if (new ProductsBusiness().UpdateAttrValue(model.ValueID, model.AttrID, model.ValueName, model.Sort, OperateIP, CurrentUser.UserID))
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

        public JsonResult DeleteCategoryAttr(string categoryid, string attrid, int type)
        {
            bool bl = new ProductsBusiness().DeleteCategoryAttr(categoryid, attrid, type, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

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

        public JsonResult DeleteProductAttr(string attrid)
        {
            bool bl = ProductsBusiness.BaseBusiness.DeleteProductAttr(attrid, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

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

        #endregion

        #region 分类

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
                caregoryid = new ProductsBusiness().AddCategory(model.CategoryCode, model.CategoryName, model.PID, model.CategoryType, model.Status, attrlist.Split(',').ToList(), saleattr.Split(',').ToList(), model.Description, CurrentUser.UserID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateCategory(model.CategoryID, model.CategoryName, model.Status, attrlist.Split(',').ToList(), saleattr.Split(',').ToList(), model.Description, CurrentUser.UserID);
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

        public JsonResult GetCategoryDetailsByID(string categoryid)
        {
            var model = new ProductsBusiness().GetCategoryByID(categoryid);
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
