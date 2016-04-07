using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YXERP.Models;

using IntFactoryBusiness;
using IntFactoryEntity.Task;
using IntFactoryEnum;
using Newtonsoft.Json.Converters;
namespace YXERP.Areas.Api.Controllers
{
    public class TaskController : BaseAPIController
    {
        //
        // GET: /Api/Task/

        public JsonResult GetTasks(string filter)
        {
            var result = new JObject();
            var paras = new FilterTasks();
            if (!string.IsNullOrEmpty(filter)){
                paras = JsonConvert.DeserializeObject<FilterTasks>(filter);
            }

            int pageCount = 0;
            int totalCount = 0;
            string ownerID = string.Empty;
            //我的任务
            ownerID = paras.userID;
            string clientID = "b00e97c6-1f93-4f61-aea1-74845af9cf28";

            List<TaskEntity> list = TaskBusiness.GetTasks(paras.keyWords.Trim(), ownerID, paras.status, paras.finishStatus,
                paras.colorMark, paras.taskType, paras.beginDate, paras.endDate,
                paras.orderType, paras.orderProcessID, paras.orderStageID,
                (EnumTaskOrderColumn)paras.taskOrderColumn, paras.isAsc, clientID,
                paras.pageSize, paras.pageIndex, ref totalCount, ref pageCount);


            List<Dictionary<string, object>> tasks = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                Dictionary<string, object> task = new Dictionary<string, object>();
                task.Add("taskID", item.TaskID);
                task.Add("title", item.Title);
                task.Add("mark", item.Mark);
                task.Add("colorMark", item.ColorMark);
                task.Add("finishStatus", item.FinishStatus);
                task.Add("orderType", item.OrderType);
                task.Add("orderImg", item.OrderImg);
                task.Add("acceptTime", item.AcceptTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.AcceptTime.ToString("yyyy-MM-dd hh:mm:ss"):"");
                task.Add("endTime",item.EndTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.EndTime.ToString("yyyy-MM-dd hh:mm:ss"):"");
                task.Add("completeTime",item.CompleteTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.CompleteTime.ToString("yyyy-MM-dd hh:mm:ss"):"");
                task.Add("createTime", item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                tasks.Add(task);
            }

            JsonDictionary.Add("tasks", tasks);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

           
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderProcess(string agentID, string clientID)
        {

            var list = SystemBusiness.BaseBusiness.GetOrderProcess(agentID, clientID);
            List<Dictionary<string, object>> processss = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                Dictionary<string, object> processs = new Dictionary<string, object>();
                processs.Add("processID", item.ProcessID);
                processs.Add("type", item.ProcessType);
                processs.Add("processName", item.ProcessName);
                processss.Add(processs);
            }

            JsonDictionary.Add("processs", processss);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderStages(string id, string agentID, string clientID)
        {
            var list = SystemBusiness.BaseBusiness.GetOrderStages(id, agentID, clientID);
            List<Dictionary<string, object>> stages = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                Dictionary<string, object> stage = new Dictionary<string, object>();
                stage.Add("stageID", item.StageID);
                stage.Add("stageName", item.StageName);
                stage.Add("mark", item.Mark);
                stages.Add(stage);
            }

            JsonDictionary.Add("stages", stages);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskDetail(string id) {
            if (!string.IsNullOrEmpty(id))
            {
                var item = TaskBusiness.GetTaskDetail(id);
                Dictionary<string, object> task = new Dictionary<string, object>();
                if (item != null)
                {
                    task.Add("taskID", item.TaskID);
                    task.Add("title", item.Title);
                    task.Add("mark", item.Mark);
                    task.Add("colorMark", item.ColorMark);
                    task.Add("finishStatus", item.FinishStatus);
                    task.Add("orderType", item.OrderType);
                    task.Add("orderImg", item.OrderImg);
                    task.Add("acceptTime", item.AcceptTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.AcceptTime.ToString("yyyy-MM-dd hh:mm:ss") : "");
                    task.Add("endTime", item.EndTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.EndTime.ToString("yyyy-MM-dd hh:mm:ss") : "");
                    task.Add("completeTime", item.CompleteTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.CompleteTime.ToString("yyyy-MM-dd hh:mm:ss") : "");
                    task.Add("createTime", item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));

                    JsonDictionary.Add("task", task);
                }
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskReplys(string id, string stageID, int mark, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;

            var list = OrdersBusiness.GetReplys(id, stageID, mark, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("taskReplys", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderInfo(string id,string agentID, string clientID)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var item =OrdersBusiness.BaseBusiness.GetOrderBaseInfoByID(id,agentID,clientID);
                Dictionary<string, object> order = new Dictionary<string, object>();
                if (item != null)
                {
                    order.Add("orderID", item.OrderID);
                    order.Add("remark", item.Remark);
                    JsonDictionary.Add("order", order);
                }
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskLogs(string id, int pageindex=1)
        {
            int totalCount = 0;
            int pageCount = 0;
            string agentID = "9f8af979-8a3b-4e23-b19c-ab8702988466";
            var list = LogBusiness.GetLogs(id, EnumLogObjectType.OrderTask, PageSize, pageindex, ref totalCount, ref pageCount, agentID);

             List<Dictionary<string, object>> logs = new List<Dictionary<string, object>>();
             foreach (var item in list)
             {
                 Dictionary<string, object> log = new Dictionary<string, object>();

                 Dictionary<string, object> createUser = new Dictionary<string, object>();
                 var user=item.CreateUser;
                 createUser.Add("userID", user.UserID);
                 createUser.Add("name", user.Name);
                 createUser.Add("avatar", user.Avatar);

                 log.Add("createUser", createUser);
                 log.Add("remark", item.Remark);
                 log.Add("createTime", item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                 logs.Add(log);
             }

            JsonDictionary.Add("logs", logs);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
