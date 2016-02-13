﻿using IntFactoryBusiness;
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
        /// 产品单位列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Unit() 
        {
            ViewBag.Items = new ProductsBusiness().GetClientUnits();
            return View();
        }

        /// <summary>
        /// 产品分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Category() 
        {
            var list = new ProductsBusiness().GetChildCategorysByID("");
            ViewBag.Items = list;
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
                var list = new ProductsBusiness().GetChildCategorysByID("");
                ViewBag.Items = list;
                return View("ChooseCategory");
            }
            ViewBag.Model = new ProductsBusiness().GetCategoryDetailByID(id);
            ViewBag.Providers = StockBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            ViewBag.UnitList = new ProductsBusiness().GetClientUnits();
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
            ViewBag.Providers = StockBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            ViewBag.UnitList = new ProductsBusiness().GetClientUnits();
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
        public ActionResult ChooseDetail(string pid, string did, int type = 0, string guid = "")
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
            return View();
        }

        public ActionResult Purchases()
        {
            ViewBag.Title = "采购管理";
            ViewBag.Type = (int)EnumSearchType.All;
            ViewBag.Providers = StockBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
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

        #region 品牌

        public JsonResult SavaBrand(string brand)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Brand model = serializer.Deserialize<Brand>(brand);

            string brandID = "";
            if (string.IsNullOrEmpty(model.BrandID))
            {
                brandID = new ProductsBusiness().AddBrand(model.Name, model.AnotherName, model.IcoPath, model.CountryCode, model.CityCode, model.Status.Value, model.Remark, model.BrandStyle, OperateIP, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateBrand(model.BrandID, model.Name, model.AnotherName, model.CountryCode, model.CityCode, model.IcoPath, model.Status.Value, model.Remark, model.BrandStyle, OperateIP, CurrentUser.UserID);
                if (bl)
                {
                    brandID = model.BrandID;
                }
            }
            JsonDictionary.Add("ID", brandID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetBrandList(string keyWords, int pageSize, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<Brand> list = new ProductsBusiness().GetBrandList(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateBrandStatus(string brandID, int status)
        {
            bool bl = new ProductsBusiness().UpdateBrandStatus(brandID, (EnumStatus)status, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteBrand(string brandID)
        {
            bool bl = new ProductsBusiness().UpdateBrandStatus(brandID, EnumStatus.Delete, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetBrandDetail(string id)
        {
            Brand model = new ProductsBusiness().GetBrandByBrandID(id);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

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
            bool bl = new ProductsBusiness().UpdateUnitStatus(unitID, EnumStatus.Delete, OperateIP, CurrentUser.UserID);
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
            list = new ProductsBusiness().GetAttrList(categoryid);

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
                attrID = new ProductsBusiness().AddProductAttr(model.AttrName, model.Description, model.CategoryID, model.Type, CurrentUser.UserID);
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
                var model = new ProductsBusiness().GetProductAttrByID(attrID);
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
        public JsonResult DeleteAttrValue(string valueid, string attrid)
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
                caregoryid = new ProductsBusiness().AddCategory(model.CategoryCode, model.CategoryName, model.PID, model.Status.Value, attrlist.Split(',').ToList(), saleattr.Split(',').ToList(), model.Description, CurrentUser.UserID);
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
        public JsonResult GetChildCategorysByID(string categoryid)
        {
            var list = new ProductsBusiness().GetChildCategorysByID(categoryid);
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
                                                        model.BigSmallMultiple.Value, model.CategoryID, model.Status.Value, model.AttrList, model.ValueList, model.AttrValueList,
                                                        model.CommonPrice.Value, model.Price, model.Weight.Value, model.IsNew.Value == 1, model.IsRecommend.Value == 1, model.IsAllow, model.IsAutoSend, model.EffectiveDays.Value,
                                                        model.DiscountValue.Value, model.ProductImage, model.ShapeCode, model.Description, model.ProductDetails, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateProduct(model.ProductID, model.ProductCode, model.ProductName, model.GeneralName, model.IsCombineProduct.Value == 1, model.ProdiverID, model.BrandID, model.BigUnitID, model.SmallUnitID,
                                                        model.BigSmallMultiple.Value, model.Status.Value, model.CategoryID, model.AttrList, model.ValueList, model.AttrValueList,
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

        #endregion

        #endregion
    }
}
