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
    [YXERP.Common.ApiAuthorize]
    public class TaskController : BaseAPIController
    {
        //
        // GET: /Api/Task/
        /// <summary>
        #region global
        IntFactoryEntity.Users CurrentUser;
        
        #endregion

        #region get

        public JsonResult GetTasks(string filter,string userID,string agentID)
        {
            
            var paras = new FilterTasks();
            if (!string.IsNullOrEmpty(filter)){
                paras = JsonConvert.DeserializeObject<FilterTasks>(filter);
            }

            int pageCount = 0;
            int totalCount = 0;
            string ownerID = string.Empty;
            //我的任务
            ownerID = paras.userID;
            if (paras.isMy) {
                ownerID = userID;
            }
            var currentUser = OrganizationBusiness.GetUserByUserID(userID,agentID);

            List<TaskEntity> list = TaskBusiness.GetTasks(paras.keyWords.Trim(), ownerID,paras.isParticipate?1:0, paras.status, paras.finishStatus,
                paras.colorMark, paras.taskType, paras.beginDate, paras.endDate,
                paras.orderType, paras.orderProcessID, paras.orderStageID,
                (EnumTaskOrderColumn)paras.taskOrderColumn, paras.isAsc, currentUser.ClientID,
                paras.pageSize, paras.pageIndex, ref totalCount, ref pageCount);


            List<Dictionary<string, object>> tasks = new List<Dictionary<string, object>>();
            string domainUrl = Request.Url.Scheme + "://" + Request.Url.Host;
            foreach (var item in list)
            {
                Dictionary<string, object> task = new Dictionary<string, object>();
                task.Add("taskID", item.TaskID);
                task.Add("title", item.Title);
                task.Add("mark", item.Mark);
                task.Add("colorMark", item.ColorMark);
                task.Add("finishStatus", item.FinishStatus);
                task.Add("orderType", item.OrderType);
                var orderImg =string.Empty;
                if (!string.IsNullOrEmpty(item.OrderImg)) { 
                    orderImg=domainUrl+ item.OrderImg;
                }
                task.Add("orderImg", orderImg);
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

        public JsonResult GetOrderProcess(string userID, string agentID)
        {
            var currentUser = OrganizationBusiness.GetUserByUserID(userID, agentID);
            var list = SystemBusiness.BaseBusiness.GetOrderProcess(agentID, currentUser.ClientID);
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

        public JsonResult GetOrderStages(string processID, string userID, string agentID)
        {
            if (!string.IsNullOrEmpty(processID))
            {
                var currentUser = OrganizationBusiness.GetUserByUserID(userID, agentID);
                var list = SystemBusiness.BaseBusiness.GetOrderStages(processID, agentID, currentUser.ClientID);
                List<Dictionary<string, object>> stages = new List<Dictionary<string, object>>();

                foreach (var item in list)
                {
                    Dictionary<string, object> stage = new Dictionary<string, object>();
                    stage.Add("stageID", item.StageID);
                    stage.Add("processID", item.ProcessID);
                    stage.Add("stageName", item.StageName);
                    stage.Add("mark", item.Mark);
                    stages.Add(stage);
                }

                JsonDictionary.Add("processStages", stages);
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskDetail(string taskID, string userID, string agentID)
        {
            if (!string.IsNullOrEmpty(taskID))
            {
                var item = TaskBusiness.GetTaskDetail(taskID);
                Dictionary<string, object> task = new Dictionary<string, object>();
                List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();
                string domainUrl = Request.Url.Scheme +"://"+ Request.Url.Host;
                if (item != null)
                {
                    task.Add("taskID", item.TaskID);
                    task.Add("taskCode", item.TaskCode);
                    task.Add("orderID", item.OrderID);
                    task.Add("processID", item.ProcessID);
                    task.Add("stageID", item.StageID);
                    task.Add("title", item.Title);
                    task.Add("mark", item.Mark);
                    task.Add("colorMark", item.ColorMark);
                    task.Add("finishStatus", item.FinishStatus);
                    task.Add("orderType", item.OrderType);
                    string orderImg = string.Empty;
                    if (!string.IsNullOrEmpty(item.OrderImg))
                    {
                        orderImg = domainUrl + item.OrderImg;
                    }
                    task.Add("orderImg", orderImg);
                    task.Add("acceptTime", item.AcceptTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.AcceptTime.ToString("yyyy-MM-dd hh:mm:ss") : "");
                    task.Add("endTime", item.EndTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.EndTime.ToString("yyyy-MM-dd hh:mm:ss") : "");
                    task.Add("completeTime", item.CompleteTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.CompleteTime.ToString("yyyy-MM-dd hh:mm:ss") : "");
                    task.Add("createTime", item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                    task.Add("ownerUser", GetUserBaseObj(item.Owner));

                    var currentUser = OrganizationBusiness.GetUserByUserID(userID, agentID);
                    var orderDetail = OrdersBusiness.BaseBusiness.GetOrderBaseInfoByID(item.OrderID, agentID, currentUser.ClientID);
                    Dictionary<string, object> order = new Dictionary<string, object>();
                    if (orderDetail != null)
                    {
                        order.Add("orderID", orderDetail.OrderID);
                        order.Add("orderCode", orderDetail.OrderCode);
                        string orderdetailImg = string.Empty;
                        if (!string.IsNullOrEmpty(orderDetail.OrderImage))
                        {
                            orderdetailImg = domainUrl + orderDetail.OrderImage;
                        }
                        order.Add("orderImage", orderdetailImg);
                        order.Add("orderImages", orderDetail.OrderImages);
                        order.Add("platemaking", orderDetail.Platemaking);
                        order.Add("plateRemark", orderDetail.PlateRemark);
                        order.Add("remark", orderDetail.Remark);
                        task.Add("order", order);
                        
                        foreach (var d in orderDetail.Details)
                        {
                            Dictionary<string, object> detail = new Dictionary<string, object>();
                            detail.Add("code", d.DetailsCode);
                            detail.Add("productImage", d.ProductImage);
                            detail.Add("productName", d.ProductName);
                            detail.Add("remark", d.Remark);
                            detail.Add("unitName", d.UnitName);
                            detail.Add("price", d.Price);
                            detail.Add("quantity", d.Quantity);
                            detail.Add("loss", d.Loss);
                            detail.Add("totalMoney", d.TotalMoney);

                            details.Add(detail);
                        }
                        
                    }

                    JsonDictionary.Add("task", task);
                    JsonDictionary.Add("domainUrl", domainUrl);
                    JsonDictionary.Add("materialList", details);
                }
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskReplys(string orderID, string stageID, string userID, string agentID, int pageSize = 10, int pageIndex = 1)
        {
            if (!string.IsNullOrEmpty(orderID) && !string.IsNullOrEmpty(stageID))
            {
                int pageCount = 0;
                int totalCount = 0;
                var list = OrdersBusiness.GetReplys(orderID, stageID, pageSize, pageIndex, ref totalCount, ref pageCount);
                List<Dictionary<string, object>> replys = new List<Dictionary<string, object>>();

                foreach (var item in list)
                {
                    Dictionary<string, object> reply = new Dictionary<string, object>();
                    reply.Add("replyID", item.ReplyID);
                    reply.Add("orderID", item.GUID);
                    reply.Add("stageID", item.StageID);
                    reply.Add("mark", item.Mark);
                    reply.Add("content", item.Content);
                    reply.Add("createUser", GetUserBaseObj(item.CreateUser));
                    reply.Add("fromReplyUser", GetUserBaseObj(item.FromReplyUser));
                    reply.Add("createTime", item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));

                    replys.Add(reply);
                }

                JsonDictionary.Add("taskReplys", replys);
                JsonDictionary.Add("totalCount", totalCount);
                JsonDictionary.Add("pageCount", pageCount);
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskLogs(string taskID, string userID, string agentID, int pageindex = 1)
        {
            if (!string.IsNullOrEmpty(taskID))
            {
                int totalCount = 0;
                int pageCount = 0;
                var list = LogBusiness.GetLogs(taskID, EnumLogObjectType.OrderTask, PageSize, pageindex, ref totalCount, ref pageCount, agentID);

                List<Dictionary<string, object>> logs = new List<Dictionary<string, object>>();
                foreach (var item in list)
                {
                    Dictionary<string, object> log = new Dictionary<string, object>();
                    log.Add("createUser", GetUserBaseObj(item.CreateUser));
                    log.Add("remark", item.Remark);
                    log.Add("createTime", item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                    logs.Add(log);
                }

                JsonDictionary.Add("taskLogs", logs);
                JsonDictionary.Add("totalCount", totalCount);
                JsonDictionary.Add("pageCount", pageCount);
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderInfo(string orderID, string userID, string agentID)
        {
            if (!string.IsNullOrEmpty(orderID))
            {
                var currentUser = OrganizationBusiness.GetUserByUserID(userID, agentID);
                var orderDetail = OrdersBusiness.BaseBusiness.GetOrderBaseInfoByID(orderID, agentID, currentUser.ClientID);
                Dictionary<string, object> order = new Dictionary<string, object>();
                List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();

                if (orderDetail != null)
                {
                    order.Add("orderID", orderDetail.OrderID);
                    order.Add("orderCode", orderDetail.OrderCode);
                    order.Add("orderImage", orderDetail.OrderImage);
                    order.Add("orderImages", orderDetail.OrderImages);
                    order.Add("platemaking", orderDetail.Platemaking);
                    order.Add("plateRemark", orderDetail.PlateRemark);
                    order.Add("remark", orderDetail.Remark);
                    JsonDictionary.Add("order", order);

                    foreach (var item in orderDetail.Details)
                    {
                        Dictionary<string, object> detail = new Dictionary<string, object>();
                        detail.Add("code",string.IsNullOrEmpty( item.DetailsCode)?item.ProductCode:item.DetailsCode);
                        detail.Add("productImage", item.ProductImage);
                        detail.Add("productName", item.ProductName);
                        detail.Add("remark", item.Remark);
                        detail.Add("unitName", item.UnitName);
                        detail.Add("price", item.Price);
                        detail.Add("quantity", item.Quantity);
                        detail.Add("loss", item.Loss);
                        detail.Add("totalMoney", item.TotalMoney);

                        details.Add(detail);
                    }
                    JsonDictionary.Add("materialList", details);
                }
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region update
        public JsonResult UpdateTaskEndTime(string taskID, string endTime, string userID, string agentID)
        {
            int result = 0;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endTime)) endDate = DateTime.Parse(endTime);
            CurrentUser = OrganizationBusiness.GetUserByUserID(userID, agentID);

            TaskBusiness.UpdateTaskEndTime(taskID, endDate, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult FinishTask(string taskID, string userID, string agentID)
        {
            int result = 0;
            CurrentUser = OrganizationBusiness.GetUserByUserID(userID, agentID);

            TaskBusiness.FinishTask(taskID, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SavaTaskReply(string reply, string userID, string agentID)
        {
            var model = JsonConvert.DeserializeObject<IntFactoryEntity.ReplyJson>(reply);

            string replyID = OrdersBusiness.CreateReply(model.orderID, model.stageID, model.mark,
                model.content, userID, agentID, 
                model.fromReplyID, model.fromReplyUserID, model.fromReplyAgentID);

            if (!string.IsNullOrEmpty(replyID))
            {
                List<Dictionary<string, object>> replys = new List<Dictionary<string, object>>();
                Dictionary<string, object> replyObj = new Dictionary<string, object>();
                replyObj.Add("replyID", replyID);
                replyObj.Add("orderID", model.orderID);
                replyObj.Add("stageID", model.stageID);
                replyObj.Add("mark", model.mark);
                replyObj.Add("content", model.content);
                replyObj.Add("createUser", GetUserBaseObj(OrganizationBusiness.GetUserByUserID(userID, agentID)));
                if (!string.IsNullOrEmpty(model.fromReplyUserID) && !string.IsNullOrEmpty(model.fromReplyAgentID))
                {
                    replyObj.Add("fromReplyUser", GetUserBaseObj(OrganizationBusiness.GetUserByUserID(model.fromReplyUserID, model.fromReplyAgentID)));
                }
                replyObj.Add("createTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                replys.Add(replyObj);

                JsonDictionary.Add("taskReplys", replys);
            }
            
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region common
        public Dictionary<string, object> GetUserBaseObj(IntFactoryEntity.Users user) 
        {
            Dictionary<string, object> userObj = new Dictionary<string, object>();
            if (user != null)
            {
                userObj.Add("userID", user.UserID);
                userObj.Add("agentID", user.AgentID);
                userObj.Add("name", user.Name);
                userObj.Add("avatar", user.Avatar);
            }

            return userObj;
        }
        #endregion
    }
}
