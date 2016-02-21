using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEntity.Task;
namespace YXERP.Controllers
{
    public class TaskController : BaseController
    {
        //
        // GET: /Task/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyTask()
        {
            ViewBag.IsMy = 1;
            return View();
        }

        public ActionResult Tasks()
        {
            ViewBag.IsMy = 0;
            return View("MyTask");
        }

        #region ajax
        public JsonResult GetTasks(bool isMy, string keyWords,int finishStatus, string beginDate, string endDate, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;
            string ownerID = string.Empty;
            if (isMy)
            {
                ownerID = CurrentUser.UserID;
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

        public JsonResult UpdateTaskEndTime(string taskID, string endTime)
        {
            int result = 0;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endTime)) endDate = DateTime.Parse(endTime);

            result=TaskBusiness.UpdateTaskEndTime(taskID, endDate)?1:0;

            JsonDictionary.Add("Result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult FinishTask(string taskID) {
            int result = 0;

            TaskBusiness.FinishTask(taskID,CurrentUser.UserID,ref result);

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
