﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEntity.Task;
using IntFactoryEnum;
using System.Globalization;
namespace YXERP.Controllers
{
    public class TaskController : BaseController
    {
        // GET: /Task/
        string token = "bdc2d699-d2ba-47ea-befe-34b794460130";
        string refreshToken = "be462dcd-1baf-4665-8444-1646d8350c8c";
        List<string> codes = new List<string>();
        public JsonResult pullFentDataList()
        {
            codes = AlibabaSdk.OrderBusiness.PullFentGoodsCodes(DateTime.Now.AddMonths(-3), DateTime.Now.AddDays(1), token).goodsCodeList;

            var result = AlibabaSdk.OrderBusiness.PullFentDataList(codes, token);

            JsonDictionary.Add("result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

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
            var order=OrdersBusiness.BaseBusiness.GetOrderBaseInfoByID(task.OrderID, CurrentUser.AgentID, CurrentUser.ClientID);
            if (order.Details == null){
                order.Details = new List<IntFactoryEntity.OrderDetail>();
            }
            ViewBag.Order = order;

            //订单的品类属性
            ViewBag.ProductAttr = new IntFactoryEntity.ProductAttr();

            //任务状态为关闭
            if (task.Status == 8) {
                return View();
            }

            //打样材料
            if (task.Mark == 1){
                return View("MaterialDetail");
            }
            else if (task.Mark == 2)//打样制版
            {
                //任务对应订单的品类属性
                ViewBag.ProductAttr = new ProductsBusiness().GetTaskPlateAttrByCategoryID(order.CategoryID);
                return View("PlateDetail");
            }//大货材料
            else if (task.Mark == 3){
                return View("CargoMaterialDetail");
            }
            else{
                return View();
            }
            
        }

        /// <summary>
        /// 我的任务 
        /// </summary>
        public ActionResult MyTask(string id)
        {
            string nowDate = string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                if (id.Equals("today", StringComparison.OrdinalIgnoreCase)){
                    nowDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
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

        public ActionResult PlatePrint(string id)
        {
            var order = OrdersBusiness.BaseBusiness.GetOrderBaseInfoByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Order = order;

            return View();
        }
        #endregion

        #region ajax
        public JsonResult GetTasks(string keyWords, bool isMy, string userID, int taskType, int colorMark,int status, int finishStatus, string beginDate, string endDate,int orderType, string orderProcessID, string orderStageID,int taskOrderColumn,int isAsc, int pageSize, int pageIndex){
            int pageCount = 0;
            int totalCount = 0;
            //所有任务
            string ownerID = string.Empty;
            //我的任务
            if (isMy){
                ownerID = CurrentUser.UserID;
            }//指定用户的任务
            else{
                ownerID = userID;
            }

            List<TaskEntity> list = TaskBusiness.GetTasks(keyWords.Trim(),ownerID,status,finishStatus, 
                colorMark,taskType,beginDate,endDate,
                orderType, orderProcessID, orderStageID,
                 (EnumTaskOrderColumn)taskOrderColumn, isAsc, CurrentUser.ClientID, 
                pageSize, pageIndex, ref totalCount, ref pageCount);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public JsonResult GetOrderProcess() {
            var list = SystemBusiness.BaseBusiness.GetOrderProcess(CurrentUser.AgentID, CurrentUser.ClientID);

            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderStages(string id){
            var list = SystemBusiness.BaseBusiness.GetOrderStages(id,CurrentUser.AgentID, CurrentUser.ClientID);

            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskDetail(string id)
        {
            var item = TaskBusiness.GetTaskDetail(id);
            JsonDictionary.Add("item", item);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderTaskLogs(string id, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = LogBusiness.GetLogs(id, EnumLogObjectType.OrderTask, PageSize, pageindex, ref totalCount, ref pageCount, CurrentUser.AgentID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateTaskEndTime(string id, string endTime)
        {
            int result = 0;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endTime)) endDate = DateTime.Parse(endTime);

            TaskBusiness.UpdateTaskEndTime(id, endDate, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult FinishTask(string id)
        {
            int result = 0;
            TaskBusiness.FinishTask(id, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.AgentID, CurrentUser.ClientID,out result);
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateTaskColorMark(string ids, int mark)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && new TaskBusiness().UpdateTaskColorMark(id, mark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }

            JsonDictionary.Add("result", bl);
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
            JsonDictionary.Add("result", result);

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
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateTaskOwner(string taskid, string userid)
        {
            int result = 0;
            bool bl = TaskBusiness.UpdateTaskOwner(taskid, userid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion


    }
}
