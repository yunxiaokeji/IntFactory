
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using IntFactoryEnum;
using IntFactoryBusiness;
using IntFactoryEntity;

namespace YXERP.Controllers
{
    public class PurchaseController : BaseController
    {
        //
        // GET: /Purchase/

        public ActionResult Index()
        {
            return View("Purchase");
        }
       


        #region Ajax


        /// <summary>
        /// 保存采购单
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public JsonResult SubmitPurchase(string wareid, string remark)
        {

            var bl = StockBusiness.CreateStorageDoc(wareid, remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取我的采购单
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult GetPurchases(string keyWords, int pageIndex, int totalCount, int status = -1, int type = 1, string begintime = "", string endtime = "", string wareid = "", string providerid = "")
        {
            int pageCount = 0;
            List<StorageDoc> list = StockBusiness.GetStorageDocList(type == 3 ? string.Empty : CurrentUser.UserID, EnumDocType.RK, (EnumDocStatus)status, keyWords, begintime, endtime, wareid, providerid, PageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetPurchasesDetails(string docid)
        {
            List<StorageDoc> list = StockBusiness.GetStorageDocDetails(docid);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        ///  删除单据
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
        public JsonResult DeletePurchase(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 作废单据
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
        public JsonResult InvalidPurchase(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 更换入库仓库和货位
        /// </summary>
        /// <param name="autoid"></param>
        /// <param name="wareid"></param>
        /// <param name="depotid"></param>
        /// <returns></returns>
        public JsonResult UpdateStorageDetailWare(string docid,string autoid, string wareid, string depotid)
        {
            var bl = new StockBusiness().UpdateStorageDetailWare(docid, autoid, wareid, depotid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 审核上架
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult AuditPurchase(string docid, int doctype, int isover, string details, string remark)
        {
            int result = 0;
            string errinto = "";
            bool bl = new StockBusiness().AuditStorageIn(docid, doctype, isover, details, remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinto);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取单据日志
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult GetStorageDocLog(string docid, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<StorageDocAction> list = StockBusiness.GetStorageDocAction(docid, 10, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
