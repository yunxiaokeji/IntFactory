using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEnum;
using IntFactoryEntity;

namespace YXERP.Controllers
{
    public class StockController : BaseController
    {
        //
        // GET: /Stock/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Stocks()
        {
            return View();
        }

        public ActionResult Detail(string id)
        {
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Stock/StorageDoc");
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult DetailStocks()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View();
        }

        public ActionResult StorageDoc()
        {
            return View();
        }

        public ActionResult ReturnIn()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View();
        }

        public ActionResult ReturnInDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/ReturnIn");
            }
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID) || model.Status > 0)
            {
                return Redirect("/Stock/Detail/" + model.DocID);
            }
            ViewBag.Wares = new SystemBusiness().GetWareHouses(CurrentUser.ClientID);
            ViewBag.Model = model;
            return View();
        }

        public ActionResult Damaged()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            ViewBag.DamagedCount = ShoppingCartBusiness.GetShoppingCartCount(EnumDocType.BS, CurrentUser.UserID);

            return View();
        }

        public ActionResult CreateDamaged(string id)
        {
            var ware = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            ViewBag.guid = CurrentUser.UserID;            
            ViewBag.Ware = ware;
            ViewBag.Items = ShoppingCartBusiness.GetShoppingCart(EnumDocType.BS, CurrentUser.UserID, CurrentUser.UserID);
            return View();
        }

        public ActionResult DamagedDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/Damaged");
            }
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Stock/Damaged");
            }
            if (model.Status >= 2)
            {
                return Redirect("/Stock/Detail/" + model.DocID);
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult Overflow()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            ViewBag.OverFlowCount = ShoppingCartBusiness.GetShoppingCartCount(EnumDocType.BY, CurrentUser.UserID);

            return View();
        }

        public ActionResult CreateOverflow(string id)
        {
            var wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            ViewBag.Ware = wares;
            ViewBag.guid = CurrentUser.UserID;
            ViewBag.Items = ShoppingCartBusiness.GetShoppingCart(EnumDocType.BY, CurrentUser.UserID, CurrentUser.UserID);
            return View();
        }

        public ActionResult OverflowDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/Overflow");
            }
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Stock/Overflow");
            }
            if (model.Status >= 2)
            {
                return Redirect("/Stock/Detail/" + model.DocID);
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult HandOut()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View();
        }

        public ActionResult CreateHandOut(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/HandOut");
            }
            var ware = SystemBusiness.BaseBusiness.GetWareByID(id, CurrentUser.ClientID);
            if (ware == null || string.IsNullOrEmpty(ware.WareID))
            {
                return Redirect("/Stock/HandOut");
            }
            ViewBag.Ware = ware;
            ViewBag.Items = ShoppingCartBusiness.GetShoppingCart(EnumDocType.SGCK, ware.WareID, CurrentUser.UserID);
            return View();
        }

        public ActionResult HandOutDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/HandOut");
            }
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Stock/HandOut");
            }
            if (model.Status >= 2)
            {
                return Redirect("/Stock/Detail/" + model.DocID);
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult ChooseBYProducts()
        {
            ViewBag.Type = (int)EnumDocType.BY;
            ViewBag.GUID = CurrentUser.UserID;
            ViewBag.Title = "选择报溢产品";
            return View("FilterProducts");
        }

        public ActionResult ChooseBSProducts()
        {
            ViewBag.Type = (int)EnumDocType.BS;
            ViewBag.GUID = CurrentUser.UserID;
            ViewBag.Title = "选择报损产品";

            return View("FilterProducts");
        }

        #region Ajax

        public JsonResult GetDetailStocks(string WareID,string Keywords, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = StockBusiness.BaseBusiness.GetDetailStocks(WareID, Keywords, PageSize, PageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductStocks(string Keywords, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = StockBusiness.BaseBusiness.GetProductStocks(Keywords, PageSize, PageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductDetailStocks(string productid)
        {
            var list = StockBusiness.BaseBusiness.GetProductDetailStocks(productid, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetStorageDocs(string keyWords, int pageIndex, int totalCount, int status = -1, int type = -1, string begintime = "", string endtime = "", string wareid = "")
        {
            int pageCount = 0;
            List<StorageDoc> list = StockBusiness.GetStorageDocList(string.Empty, (EnumDocType)type, (EnumDocStatus)status, keyWords, begintime, endtime, wareid, "", PageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateStorageDetailBatch(string docid, string autoid, string batch)
        {
            var bl = new StockBusiness().UpdateStorageDetailBatch(docid, autoid, batch, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditReturnIn(string docid)
        {
            int result = 0;
            string errinfo = "";

            var bl = StockBusiness.BaseBusiness.AuditReturnIn(docid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductsByKeywords(string wareid, string keywords)
        {
            var list = StockBusiness.BaseBusiness.GetProductsByKeywords(wareid, keywords, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SubmitDamagedDoc(string wareid, string remark)
        {
            var bl = StockBusiness.BaseBusiness.SubmitDamagedDoc(wareid, remark, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SubmitOverflowDoc(string wareid, string remark)
        {
            var bl = StockBusiness.BaseBusiness.SubmitOverflowDoc(wareid, remark, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteStorageDoc(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidStorageDoc(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteDamagedDoc(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidDamagedDoc(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditDamagedDoc(string docid)
        {
            int result = 0;
            string errinfo = "";
            var bl = new StockBusiness().AuditDamagedDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOverflowDoc(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidOverflowDoc(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditOverflowDoc(string docid)
        {
            int result = 0;
            string errinfo = "";
            var bl = new StockBusiness().AuditOverflowDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SubmitHandOutDoc(string wareid, string remark)
        {
            var bl = StockBusiness.BaseBusiness.SubmitHandOutDoc(wareid, remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidHandOutDoc(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteHandOutDoc(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditHandOutDoc(string docid)
        {
            int result = 0;
            string errinfo = "";//使用报损逻辑
            var bl = new StockBusiness().AuditDamagedDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion
    }
}
