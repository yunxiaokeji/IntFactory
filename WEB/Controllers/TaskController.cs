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
            return View();
        }

        public ActionResult Tasks()
        {
            return View("MyTask");
        }

        #region ajax
        public JsonResult GetTasks(bool isMy, string beginDate, string endDate, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;
            string ownerID = string.Empty;
            if (isMy)
            {
                ownerID = CurrentUser.UserID;
            }

            List<TaskEntity> list = TaskBusiness.GetTasks(ownerID, beginDate, endDate, CurrentUser.ClientID, pageSize, pageIndex, ref totalCount, ref pageCount);
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
