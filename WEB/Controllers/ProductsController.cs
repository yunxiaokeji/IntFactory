using IntFactoryBusiness;
using IntFactoryEntity;
using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXERP.Models;

namespace YXERP.Controllers
{
    public class ProductsController : BaseController
    {
        //
        // GET: /Products/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Providers()
        {
            return View();
        }

        /// <summary>
        /// 品牌列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Brand() 
        {
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

        /// <summary>
        /// 添加产品
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductAdd(string id) 
        {
            if (string.IsNullOrEmpty(id))
            {
                var list = new ProductsBusiness().GetChildCategorysByID("", EnumCategoryType.Product);
                ViewBag.Items = list;
                return View("ChooseCategory");
            }
            ViewBag.Model = new ProductsBusiness().GetCategoryDetailByID(id);
            ViewBag.Providers = ProvidersBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            ViewBag.UnitList = new ProductsBusiness().GetUnits();
            return View();
        }

        /// <summary>
        /// 产品详情页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProductDetail(string id)
        {
            var model = new ProductsBusiness().GetProductByID(id);
            ViewBag.Model = model;
            ViewBag.Providers = ProvidersBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            ViewBag.UnitList = new ProductsBusiness().GetUnits();
            return View();
        }

        /// <summary>
        /// 设置子产品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProductDetails(string id)
        {
            var model = new ProductsBusiness().GetProductByID(id);
            ViewBag.Model = model;
            return View();
        }

        /// <summary>
        /// 产品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductList() 
        {
            return View();
        }

        /// <summary>
        /// 加入购物车详情页
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="did"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult ChooseDetail(string pid, string did, int type = 0, string guid = "",string tid="")
        {
            if (string.IsNullOrEmpty(pid))
            {
                return Redirect("ProductList");
            }
            var model = new ProductsBusiness().GetProductByIDForDetails(pid);
            if (model == null || string.IsNullOrEmpty(model.ProductID))
            {
                return Redirect("ProductList");
            }
            ViewBag.Model = model;
            ViewBag.DetailID = did;
            ViewBag.OrderType = type;
            ViewBag.GUID = guid;
            ViewBag.TID = tid;
            return View();
        }

        public ActionResult Purchases()
        {
            ViewBag.Title = "采购管理";
            ViewBag.Type = (int)EnumSearchType.All;
            ViewBag.ProvidersBusiness = ProvidersBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View("Purchases");
        }

        /// <summary>
        /// 采购单确认页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmPurchase(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Products/Purchases");
            }
            var ware = SystemBusiness.BaseBusiness.GetWareByID(id, CurrentUser.ClientID);
            if (ware == null || string.IsNullOrEmpty(ware.WareID))
            {
                return Redirect("/Products/Purchases");
            }
            ViewBag.Ware = ware;
            ViewBag.Items = ShoppingCartBusiness.GetShoppingCart(EnumDocType.RK, ware.WareID, CurrentUser.UserID);
            return View();
        }

        /// <summary>
        /// 我的采购详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DocDetail(string id)
        {
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Products/Purchases");
            }
            ViewBag.Model = model;
            return View();
        }

        /// <summary>
        /// 采购审核页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AuditDetail(string id)
        {
            ViewBag.Model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            return View();
        }

        #region Ajax

        #region 供应商

        public JsonResult GetProviders(string keyWords, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            var list = ProvidersBusiness.BaseBusiness.GetProviders(keyWords, PageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetAllProviders()
        {
            var list = ProvidersBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProviderDetail(string id)
        {
            var model = ProvidersBusiness.BaseBusiness.GetProviderByID(id);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SavaProviders(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProvidersEntity model = serializer.Deserialize<ProvidersEntity>(entity);

            string id = "";
            if (string.IsNullOrEmpty(model.ProviderID))
            {
                id = ProvidersBusiness.BaseBusiness.AddProviders(model.Name, model.Contact, model.MobileTele, "", model.CityCode, model.Address, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = ProvidersBusiness.BaseBusiness.UpdateProvider(model.ProviderID, model.Name, model.Contact, model.MobileTele, "", model.CityCode, model.Address, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
                if (bl)
                {
                    id = model.ProviderID;
                }
            }
            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteProvider(string id)
        {
            bool bl = ProvidersBusiness.BaseBusiness.UpdateProviderStatus(id, EnumStatus.Delete, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 品牌

        //public JsonResult SavaBrand(string brand)
        //{
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    Brand model = serializer.Deserialize<Brand>(brand);

        //    string brandID = "";
        //    if (string.IsNullOrEmpty(model.BrandID))
        //    {
        //        brandID = new ProductsBusiness().AddBrand(model.Name, model.AnotherName, model.IcoPath, model.CountryCode, model.CityCode, model.Status.Value, model.Remark, model.BrandStyle, OperateIP, CurrentUser.UserID, CurrentUser.ClientID);
        //    }
        //    else
        //    {
        //        bool bl = new ProductsBusiness().UpdateBrand(model.BrandID, model.Name, model.AnotherName, model.CountryCode, model.CityCode, model.IcoPath, model.Status.Value, model.Remark, model.BrandStyle, OperateIP, CurrentUser.UserID);
        //        if (bl)
        //        {
        //            brandID = model.BrandID;
        //        }
        //    }
        //    JsonDictionary.Add("ID", brandID);
        //    return new JsonResult
        //    {
        //        Data = JsonDictionary,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}

        //public JsonResult GetBrandList(string keyWords, int pageSize, int pageIndex, int totalCount)
        //{
        //    int pageCount = 0;
        //    List<Brand> list = new ProductsBusiness().GetBrandList(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
        //    JsonDictionary.Add("Items", list);
        //    JsonDictionary.Add("TotalCount", totalCount);
        //    JsonDictionary.Add("PageCount", pageCount);
        //    return new JsonResult
        //    {
        //        Data = JsonDictionary,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}

        //public JsonResult UpdateBrandStatus(string brandID, int status)
        //{
        //    bool bl = new ProductsBusiness().UpdateBrandStatus(brandID, (EnumStatus)status, OperateIP, CurrentUser.UserID);
        //    JsonDictionary.Add("Status", bl);
        //    return new JsonResult
        //    {
        //        Data = JsonDictionary,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}

        //public JsonResult DeleteBrand(string brandID)
        //{
        //    bool bl = new ProductsBusiness().UpdateBrandStatus(brandID, EnumStatus.Delete, OperateIP, CurrentUser.UserID);
        //    JsonDictionary.Add("Status", bl);
        //    return new JsonResult
        //    {
        //        Data = JsonDictionary,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}

        //public JsonResult GetBrandDetail(string id)
        //{
        //    Brand model = new ProductsBusiness().GetBrandByBrandID(id);
        //    JsonDictionary.Add("model", model);
        //    return new JsonResult
        //    {
        //        Data = JsonDictionary,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}

        #endregion

        #region 分类

        /// <summary>
        /// 获取下级分类
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public JsonResult GetChildCategorysByID(string categoryid)
        {
            var list = new ProductsBusiness().GetChildCategorysByID(categoryid, EnumCategoryType.Product);
            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetChildOrderCategorysByID(string categoryid)
        {
            var list = new ProductsBusiness().GetClientCategorysByPID(categoryid, EnumCategoryType.Order, CurrentUser.ClientID);
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

        public JsonResult GetOrderCategoryDetailsByID(string categoryid, string orderid)
        {
            var model = new ProductsBusiness().GetOrderCategoryDetailsByID(categoryid, orderid);
            JsonDictionary.Add("Model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 产品

        /// <summary>
        /// 保存产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public JsonResult SavaProduct(string product)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Products model = serializer.Deserialize<Products>(product);

            if (!string.IsNullOrEmpty(model.AttrList))
            {
                model.AttrList = model.AttrList.Substring(0, model.AttrList.Length - 1);
            }
            if (!string.IsNullOrEmpty(model.ValueList))
            {
                model.ValueList = model.ValueList.Substring(0, model.ValueList.Length - 1);
            }
            if (!string.IsNullOrEmpty(model.AttrValueList))
            {
                model.AttrValueList = model.AttrValueList.Substring(0, model.AttrValueList.Length - 1);
            }

            string id = "";
            if (string.IsNullOrEmpty(model.ProductID))
            {
                id = new ProductsBusiness().AddProduct(model.ProductCode, model.ProductName, model.GeneralName, model.IsCombineProduct.Value == 1, model.ProdiverID, model.BrandID, model.BigUnitID, model.SmallUnitID,
                                                        model.BigSmallMultiple.Value, model.CategoryID, model.Status.Value, model.IsPublic, model.AttrList, model.ValueList, model.AttrValueList,
                                                        model.CommonPrice.Value, model.Price, model.Weight.Value, model.IsNew.Value == 1, model.IsRecommend.Value == 1, model.IsAllow, model.IsAutoSend, model.EffectiveDays.Value,
                                                        model.DiscountValue.Value, model.ProductImage, model.ShapeCode, model.Description, model.ProductDetails, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateProduct(model.ProductID, model.ProductCode, model.ProductName, model.GeneralName, model.IsCombineProduct.Value == 1, model.ProdiverID, model.BrandID, model.BigUnitID, model.SmallUnitID,
                                                        model.BigSmallMultiple.Value, model.Status.Value, model.IsPublic, model.CategoryID, model.AttrList, model.ValueList, model.AttrValueList,
                                                        model.CommonPrice.Value, model.Price, model.Weight.Value, model.IsNew.Value == 1, model.IsRecommend.Value == 1, model.IsAllow, model.IsAutoSend, model.EffectiveDays.Value,
                                                        model.DiscountValue.Value, model.ProductImage, model.ShapeCode, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
                if (bl)
                {
                    id = model.ProductID;
                }
            }
            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProductList(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterProduct model = serializer.Deserialize<FilterProduct>(filter);
            int totalCount = 0;
            int pageCount = 0;

            List<Products> list = new ProductsBusiness().GetProductList(model.CategoryID, model.ProviderID, model.BeginPrice, model.EndPrice, model.Keywords, model.OrderBy, model.IsAsc, PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
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
        /// 获取产品信息
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public JsonResult GetProductByID(string productid) 
        {
            var model = new ProductsBusiness().GetProductByID(productid);
            JsonDictionary.Add("Item", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取产品信息（加入购物车）
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public JsonResult GetProductByIDForDetails(string productid)
        {
            var model = new ProductsBusiness().GetProductByIDForDetails(productid);
            JsonDictionary.Add("Item", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 编辑产品状态
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult UpdateProductStatus(string productid, int status)
        {
            bool bl = new ProductsBusiness().UpdateProductStatus(productid, (EnumStatus)status, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 编辑产品是否新品
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="isnew"></param>
        /// <returns></returns>
        public JsonResult UpdateProductIsNew(string productid, bool isnew)
        {
            bool bl = new ProductsBusiness().UpdateProductIsNew(productid, isnew, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 编辑产品是否推荐
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="isRecommend"></param>
        /// <returns></returns>
        public JsonResult UpdateProductIsRecommend(string productid, bool isRecommend)
        {
            bool bl = new ProductsBusiness().UpdateProductIsRecommend(productid, isRecommend, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 产品编码是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public JsonResult IsExistsProductCode(string code)
        {
            bool bl = new ProductsBusiness().IsExistProductCode(code, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存子产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public JsonResult SavaProductDetail(string product)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProductDetail model = serializer.Deserialize<ProductDetail>(product);

            if (!string.IsNullOrEmpty(model.SaleAttr))
            {
                model.SaleAttr = model.SaleAttr.Substring(0, model.SaleAttr.Length - 1);
            }
            if (!string.IsNullOrEmpty(model.AttrValue))
            {
                model.AttrValue = model.AttrValue.Substring(0, model.AttrValue.Length - 1);
            }
            if (!string.IsNullOrEmpty(model.SaleAttrValue))
            {
                model.SaleAttrValue = model.SaleAttrValue.Substring(0, model.SaleAttrValue.Length - 1);
            }

            string id = "";
            if (string.IsNullOrEmpty(model.ProductDetailID))
            {
                id = new ProductsBusiness().AddProductDetails(model.ProductID, model.DetailsCode, model.ShapeCode, model.SaleAttr, model.AttrValue, model.SaleAttrValue,
                                                              model.Price, model.Weight, model.BigPrice, model.ImgS, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateProductDetails(model.ProductDetailID, model.ProductID, model.DetailsCode, model.ShapeCode, model.BigPrice, model.SaleAttr, model.AttrValue, model.SaleAttrValue,
                                                              model.Price, model.Weight, model.Description, model.ImgS, CurrentUser.UserID, CurrentUser.ClientID); 
                if (bl)
                {
                    id = model.ProductDetailID;
                }
            }
            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 编辑子产品状态
        /// </summary>
        /// <param name="productdetailid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult UpdateProductDetailsStatus(string productdetailid, int status)
        {
            bool bl = new ProductsBusiness().UpdateProductDetailsStatus(productdetailid, (EnumStatus)status, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductDetails(string wareid, string keywords)
        {
            var list = new ProductsBusiness().GetProductDetails(wareid, keywords, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public JsonResult GetProductUseLogs(string productid,int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = new ProductsBusiness().GetProductUseLogs(productid, 20, pageindex, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #endregion
    }
}
