using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEntity.Task;
using IntFactoryEnum;
namespace YXERP.Controllers
{
    public class TaskController : BaseController
    {
        // GET: /Task/

        #region view
        /// <summary>
        /// 任务详情
        /// </summary>
        /// <param name="id"></param>
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Task/MyTask");
            }

            //任务详情
            var task = TaskBusiness.GetTaskDetail(id);
            ViewBag.Model = task;

            //任务对应的订单详情
            var order=OrdersBusiness.BaseBusiness.GetOrderByID(task.OrderID, CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Order = order;

            //任务对应订单的品类属性
            ViewBag.ProductAttr = new ProductsBusiness().GetTaskPlateAttrByCategoryID(order.CategoryID);
            return View();
        }

        /// <summary>
        /// 我的任务
        /// </summary>
        public ActionResult MyTask(string id)
        {
            string nowDate = string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                if(id.Equals("today",StringComparison.OrdinalIgnoreCase))
                    nowDate = DateTime.Now.ToString("yyyy-MM-dd");
            }

            ViewBag.NowDate = nowDate;
            ViewBag.IsMy = 1;

            return View();
        }

        /// <summary>
        /// 所有任务
        /// </summary>
        public ActionResult Tasks()
        {
            ViewBag.IsMy = 0;

            return View("MyTask");
        }
        #endregion

        #region ajax
        public JsonResult GetTasks(bool isMy, string userID, string keyWords, int finishStatus, string beginDate, string endDate, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;
            string ownerID = string.Empty;
            if (isMy){
                ownerID = CurrentUser.UserID;
            }
            else{
                ownerID = userID;
            }

            List<TaskEntity> list = TaskBusiness.GetTasks(keyWords,ownerID,finishStatus, beginDate, endDate, CurrentUser.ClientID, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskDetail(string taskID)
        {
            var item = TaskBusiness.GetTaskDetail(taskID);
            JsonDictionary.Add("Item", item);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderTaskLogs(string taskID, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = LogBusiness.GetLogs(taskID, EnumLogObjectType.OrderTask, 10, pageindex, ref totalCount, ref pageCount, CurrentUser.AgentID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateTaskEndTime(string taskID, string endTime)
        {
            int result = 0;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endTime)) endDate = DateTime.Parse(endTime);

            result = TaskBusiness.UpdateTaskEndTime(taskID, endDate, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.AgentID, CurrentUser.ClientID) ? 1 : 0;

            JsonDictionary.Add("Result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult FinishTask(string taskID, string orderID, int orderType)
        {
            int result = 0;
            bool flag = true;

            if (orderType == 2)
            {
                OrdersBusiness.BaseBusiness.EffectiveOrderProduct(orderID, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
                if (result != 1)
                {
                    flag = false;
                    result = -1;
                }
            }

            if(flag)
                TaskBusiness.FinishTask(taskID, CurrentUser.UserID, ref result, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.AgentID, CurrentUser.ClientID);

            JsonDictionary.Add("Result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [ValidateInput(false)]
        public JsonResult UpdateOrderPlateAttr(string orderID, string taskID, string valueIDs, string platehtml)
        {
            int result = 0;
            valueIDs = valueIDs.Trim('|');

            result = OrdersBusiness.BaseBusiness.UpdateOrderPlateAttr(orderID, taskID, valueIDs, platehtml, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID) ? 1 : 0;

            JsonDictionary.Add("Result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderPlateRemark(string orderID, string plateRemark)
        {
            int result = 0;

            result = OrdersBusiness.BaseBusiness.UpdateOrderPlateRemark(orderID, plateRemark) ? 1 : 0;

            JsonDictionary.Add("Result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion


    }
}
