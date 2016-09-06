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

        public ActionResult Brand() 
        {
            return View();
        }

        public ActionResult Attr() 
        {
            return View();
        }

        public ActionResult ProductAdd(string type = "", string guid = "", string tid = "", string aid = "") 
        {
            if (string.IsNullOrEmpty(type)) 
            {
                type = "0";
            }
            ViewBag.Type = type;
            ViewBag.GUID = guid;
            ViewBag.TID = tid;
            ViewBag.AID = aid;

            ViewBag.UnitList = new ProductsBusiness().GetUnits();
            return View();
        }

        public ActionResult ProductDetail(string id)
        {
            var model = new ProductsBusiness().GetProductByID(id, CurrentUser.ClientID);
            ViewBag.Model = model;
            ViewBag.UnitList = new ProductsBusiness().GetUnits();
            return View();
        }

        public ActionResult ProductList() 
        {
            return View();
        }

        public ActionResult ChooseProducts(string id)
        {
            
            ViewBag.Type = (int)EnumDocType.RK;
            ViewBag.GUID = CurrentUser.UserID;
            ViewBag.TID = "";
            ViewBag.Title = "选择材料";
            return View("FilterProducts");
        }

        public ActionResult ChooseDetail(string pid, string did, int type = 0, string guid = "", string tid = "", string aid = "")
        {
            if (string.IsNullOrEmpty(pid))
            {
                return Redirect("ProductList");
            }
            var model = new ProductsBusiness().GetProductByIDForDetails(pid, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.ProductID))
            {
                return Redirect("ProductList");
            }
            var ware = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID)[0];
            ViewBag.Depots = ware.DepotSeats;
            ViewBag.Model = model;
            ViewBag.DetailID = did;
            ViewBag.OrderType = type;
            ViewBag.GUID = guid;
            ViewBag.TID = tid;
            ViewBag.AID = aid;
            return View();
        }

        public ActionResult Purchases()
        {
            ViewBag.Title = "采购管理";
            ViewBag.Type = (int)EnumSearchType.All;
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            int purchasesCount = ShoppingCartBusiness.GetShoppingCartCount(EnumDocType.RK, CurrentUser.UserID);
            ViewBag.PurchasesCount = purchasesCount;
            return View("Purchases");
        }

        public ActionResult ConfirmPurchase(string id)
        {
            var wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            ViewBag.Ware = wares;
            ViewBag.Items = ShoppingCartBusiness.GetShoppingCart(EnumDocType.RK, CurrentUser.UserID, CurrentUser.UserID);
            ViewBag.guid = CurrentUser.UserID;
            return View();
        }

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

        public ActionResult AuditDetail(string id)
        {
            ViewBag.Model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            return View();
        }

        #region Ajax

        #region 供应商

        public JsonResult GetProviders(string keyWords, int pageIndex, int totalCount = 0, int pageSize = 20)
        {
            int pageCount = 0;
            var list = ProvidersBusiness.BaseBusiness.GetProviders(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
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
                id = ProvidersBusiness.BaseBusiness.AddProviders(model.Name, model.Contact, model.MobileTele, "", model.CityCode, model.Address, model.Remark, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = ProvidersBusiness.BaseBusiness.UpdateProvider(model.ProviderID, model.Name, model.Contact, model.MobileTele, "", model.CityCode, model.Address, model.Remark, CurrentUser.UserID, CurrentUser.ClientID);
                if (bl)
                {
                    id = model.ProviderID;
                }
            }
            if (string.IsNullOrEmpty(id))
            {
                JsonDictionary.Add("status", false);
            }
            else 
            {
                model.ProviderID = id;
                JsonDictionary.Add("status", true);
                JsonDictionary.Add("model", model);
            }
            
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

        #region 分类

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
            var list = new ProductsBusiness().GetChildCategorysByID(categoryid, EnumCategoryType.Order);
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

        public JsonResult GetOrderCategoryDetailsByID(string categoryid, string orderid)
        {
            var model = new ProductsBusiness().GetCategoryByID(categoryid);
            JsonDictionary.Add("Model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 产品

        public JsonResult SavaProduct(string product)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Products model = serializer.Deserialize<Products>(product);
            int result = 0;
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
                id = new ProductsBusiness().AddProduct(model.ProductCode, model.ProductName, model.GeneralName,  model.ProviderID, model.UnitID,
                                                        model.CategoryID, model.Status.Value, model.IsPublic, model.AttrList, model.ValueList, model.AttrValueList,
                                                        model.CommonPrice.Value, model.Price, model.Weight.Value,  model.IsAllow,
                                                        model.DiscountValue.Value, model.ProductImage, model.ShapeCode, model.Description, model.ProductDetails, CurrentUser.UserID, CurrentUser.ClientID, ref result);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateProduct(model.ProductID, model.ProductCode, model.ProductName, model.GeneralName, model.ProviderID, model.UnitID,
                                                        model.Status.Value, model.IsPublic, model.CategoryID, model.AttrList, model.ValueList, model.AttrValueList,
                                                        model.CommonPrice.Value, model.Price, model.Weight.Value,  model.IsAllow, 
                                                        model.DiscountValue.Value, model.ProductImage, model.ShapeCode, model.Description, CurrentUser.UserID, CurrentUser.ClientID,ref result);
                if (bl)
                {
                    id = model.ProductID;
                }
            }
            JsonDictionary.Add("ID", id);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

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

        public JsonResult GetProductByID(string productid) 
        {
            var model = new ProductsBusiness().GetProductByID(productid, CurrentUser.ClientID);
            JsonDictionary.Add("Item", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductByIDForDetails(int type, string did, string productid)
        {
            var model = new ProductsBusiness().GetProductByIDForDetails(productid, CurrentUser.ClientID);
            JsonDictionary.Add("Item", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetWareHouses()
        {
            List<WareHouse> items = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            JsonDictionary.Add("items", items);
            return new JsonResult
            {
                Data = items,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUnits()
        {
            var items = new ProductsBusiness().GetUnits();
            JsonDictionary.Add("items", items);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

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
                                                              model.Price, model.Weight, model.ImgS, model.Description, model.Remark, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateProductDetails(model.ProductDetailID, model.ProductID, model.DetailsCode, model.ShapeCode, model.SaleAttr, model.AttrValue, model.SaleAttrValue,
                                                              model.Price, model.Weight, model.Description, model.Remark, model.ImgS, CurrentUser.UserID, CurrentUser.ClientID); 
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
            var list = new ProductsBusiness().GetProductDetails(wareid, keywords, CurrentUser.ClientID);
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

        public JsonResult DeleteProductByID(string pid) 
        {
            int result;
            ProductsBusiness.BaseBusiness.DeleteProductByID(pid, CurrentUser.UserID, out result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteProductDetailByID(string did)
        {
            int result;
            ProductsBusiness.BaseBusiness.DeleteProductDetailByID(did, CurrentUser.UserID, out result);
            JsonDictionary.Add("result", result);
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
