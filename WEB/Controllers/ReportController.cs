using IntFactoryBusiness;
using IntFactoryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Controllers
{
    public class ReportController : BaseController
    {
        //
        // GET: /Report/

        public ActionResult MyWorkReport()
        {
            ViewBag.Status = ExpandClass.IsExistMenu("105040000");
            return View();
        }

        public ActionResult UserTaskReport()
        {
            return View();
        }

        public ActionResult UserCutReport()
        {
            return View();
        }

        public ActionResult UserSewnReport()
        {
            return View();
        }

        public ActionResult OrderProductionRPT()
        {
            return View();
        }

        public ActionResult CustomerRateRPT()
        {
            return View();
        }

        public JsonResult GetKanBanReport(int dateType, string beginTime, string endTime)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetKanbanRPT(beginTime, endTime, dateType, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetKanBanItemReport(int dateType, string itemType, string beginTime, string endTime)
        {
            var list = TaskRPTBusiness.GetKanbanItemRPT(dateType, itemType, beginTime, endTime, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #region 任务统计

        /// <summary>
        /// 员工任务数
        /// </summary>
        public JsonResult GetUserTaskQuantity(string beginTime, string endTime, string UserID, string TeamID)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetUserTaskQuantity(beginTime, endTime, UserID, TeamID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserWorkLoad(string beginTime, string endTime, int docType, string UserID, string TeamID)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetUserLoadReport(beginTime, endTime, docType, UserID, TeamID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserSewnProcess(string beginTime, string endTime, string UserID, string TeamID)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetUserSewnProcessReport(beginTime, endTime, UserID, TeamID, CurrentUser.ClientID);
            List<ProcessItemEntity> process = new List<ProcessItemEntity>();
            var processIds = list.Select(m => m.ProcessID).Distinct().ToList();
            if (processIds != null)
            {
                processIds.ForEach(u =>
                {
                    var item = new ProcessItemEntity();
                    item.ProcessID = u;
                    item.ProcessName = list.Where(m => m.ProcessID == u).FirstOrDefault().ProcessName;
                    process.Add(item);
                });
            }

            List<SewnProcessRptEntity> rows = new List<SewnProcessRptEntity>();

            var users = list.Select(m => m.OwnerID).Distinct().ToList();
            if (users != null)
            {
                users.ForEach(u =>
                {
                    var item = new SewnProcessRptEntity();
                    item.UserName = OrganizationBusiness.GetUserCacheByUserID(u, CurrentUser.ClientID).Name;
                    item.ProcessItems = new List<SewnProcessItemEntity>();
                    process.ForEach(p =>
                    {
                        var processitem = list.Where(m => m.ProcessID == p.ProcessID && m.OwnerID == u).FirstOrDefault();
                        if (processitem == null)
                        {
                            processitem = new SewnProcessItemEntity();
                        }
                        item.ProcessItems.Add(processitem);
                    });
                    rows.Add(item);
                });
            }
            
            JsonDictionary.Add("items", rows);
            JsonDictionary.Add("process", process);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 订单统计

        /// <summary>
        /// 订单生产数据统计
        /// </summary>
        public JsonResult GetOrderProductionRPT(string beginTime, string endTime, string keyWords, string UserID, string TeamID, int TimeType)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetOrderProductionRPT(TimeType, beginTime, endTime, keyWords, UserID, TeamID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion 

        #region 客户统计

        /// <summary>
        /// 客户转化率统计
        /// </summary>
        public JsonResult GetCustomerRateRPT(string beginTime, string endTime, string keyWords)
        {
            var list = TaskRPTBusiness.BaseBusiness.GetCustomerRateRPT(beginTime, endTime, keyWords, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion 
    }
}
