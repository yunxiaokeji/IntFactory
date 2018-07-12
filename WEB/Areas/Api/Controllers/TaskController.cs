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
using IntFactoryEntity;
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
        public JsonResult GetTasks(string filter, string userID, string clientID)
        {
            var paras = new FilterTasks();
            if (!string.IsNullOrEmpty(filter)){
                paras = JsonConvert.DeserializeObject<FilterTasks>(filter);
            }
            int pageCount = 0;
            int totalCount = 0;
            string ownerID = string.Empty;
            if (paras.filtertype == -1)
            {
                if (!string.IsNullOrEmpty(paras.userID))
                {
                    ownerID = paras.userID;
                }
            }
            else
            {
                ownerID = userID;
            }
            List<TaskEntity> list = TaskBusiness.GetTasks(paras.keyWords.Trim(), ownerID,paras.filtertype, paras.status, paras.finishStatus,paras.invoiceStatus,-1,
                paras.colorMark, paras.taskType, paras.beginDate, paras.endDate,string.Empty,string.Empty,
                paras.orderType, paras.orderProcessID, paras.orderStageID,
                (EnumTaskOrderColumn)paras.taskOrderColumn, paras.isAsc, clientID,
                paras.pageSize, paras.pageIndex, ref totalCount, ref pageCount);
            var lables = SystemBusiness.BaseBusiness.GetLableColor(clientID, EnumMarkType.Tasks).ToList();

            List<Dictionary<string, object>> tasks = new List<Dictionary<string, object>>();
            string domainUrl = Request.Url.Scheme + "://" + Request.Url.Host;
            foreach (var item in list)
            {
                Dictionary<string, object> task = new Dictionary<string, object>();
                task.Add("taskID", item.TaskID);
                task.Add("title", item.Title);
                task.Add("mark", item.Mark);
                task.Add("colorMark", item.ColorMark);
                string colorValue = string.Empty;
                var lable=lables.Find(m=>m.ColorID==item.ColorMark);
                if (lable != null) { 
                    colorValue=lable.ColorValue;
                }
                task.Add("colorValue", colorValue);
                task.Add("finishStatus", item.FinishStatus);
                task.Add("preTitle", item.PreTitle);
                task.Add("preFinishStatus", item.PreFinishStatus);
                task.Add("pEndTime", item.PEndTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.PEndTime.ToString("yyyy-MM-dd") : "");
                task.Add("orderType", item.OrderType);
                var orderImg = item.OrderImg;
                if (!string.IsNullOrEmpty(item.OrderImg) && !item.OrderImg.Contains("bkt.clouddn.com")){
                    orderImg = domainUrl + item.OrderImg;
                }
                task.Add("orderImg", orderImg);
                task.Add("acceptTime", item.AcceptTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.AcceptTime.ToString("yyyy-MM-dd"):"");
                task.Add("endTime",item.EndTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.EndTime.ToString("yyyy-MM-dd"):"");
                task.Add("completeTime",item.CompleteTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.CompleteTime.ToString("yyyy-MM-dd"):"");
                task.Add("createTime", item.CreateTime.ToString("yyyy-MM-dd"));

                Dictionary<string, object> order = new Dictionary<string, object>();
                var orderItem = item.Order;
                order.Add("goodsName", orderItem.GoodsName);
                order.Add("goodsCode", orderItem.IntGoodsCode);
                order.Add("planTime", orderItem.PlanTime.ToString("yyyy-MM-dd"));
                task.Add("order", order);
                tasks.Add(task);
            }
            JsonDictionary.Add("items", tasks);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskTotalCount(int filterType, string userID, string clientID)
        {
            int searchType = 0;
            if (filterType == 2) {
                searchType = 1;
            }
            var list = TaskRPTBusiness.BaseBusiness.GetTaskTabCount(userID, searchType, clientID);
            int normal = 0, complete = 0, nobegin = 0;
            if (list.Count(m => m.OrderStatus == 0) > 0)
            {
                nobegin = list.Where(m => m.OrderStatus == 0).FirstOrDefault().OrderQuantity;
            }
            if (list.Count(m => m.OrderStatus == 1) > 0)
            {
                normal = list.Where(m => m.OrderStatus == 1).FirstOrDefault().OrderQuantity;
            }
            if (list.Count(m => m.OrderStatus == 2) > 0)
            {
                complete = list.Where(m => m.OrderStatus == 2).FirstOrDefault().OrderQuantity;
            }
            JsonDictionary.Add("nobegin", nobegin);
            JsonDictionary.Add("normal", normal);
            JsonDictionary.Add("complete", complete);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskDetail(string taskID, string userID,string clientID)
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
                    task.Add("taskMembers", item.TaskMembers);
                    task.Add("lockStatus", item.LockStatus);
                    string orderImg = string.Empty;
                    if (!string.IsNullOrEmpty(item.OrderImg) && !item.OrderImg.Contains("bkt.clouddn.com"))
                    {
                        orderImg = domainUrl + item.OrderImg;
                    }
                    else
                    {
                        orderImg = item.OrderImg;
                    }
                    task.Add("orderImg", orderImg);
                    task.Add("acceptTime", item.AcceptTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.AcceptTime.ToString("yyyy-MM-dd") : "");
                    task.Add("endTime", item.EndTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.EndTime.ToString("yyyy-MM-dd") : "");
                    task.Add("completeTime", item.CompleteTime.ToString("yyyy-MM-dd") != "0001-01-01" ? item.CompleteTime.ToString("yyyy-MM-dd") : "");
                    task.Add("createTime", item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                    task.Add("ownerUser", GetUserBaseObj(item.Owner));

                    var orderDetail = OrdersBusiness.BaseBusiness.GetOrderByID(item.OrderID);
                    Dictionary<string, object> order = new Dictionary<string, object>();
                    if (orderDetail != null)
                    {
                        order.Add("orderID", orderDetail.OrderID);
                        order.Add("originalID", orderDetail.OriginalID);
                        order.Add("orderType", orderDetail.OrderType);
                        order.Add("goodsName", orderDetail.GoodsName);
                        order.Add("goodsCode", orderDetail.IntGoodsCode);
                        order.Add("planTime", orderDetail.PlanTime.ToString("yyyy-MM-dd") != "0001-01-01" ? orderDetail.PlanTime.ToString("yyyy-MM-dd") : "");
                        order.Add("orderCode", orderDetail.OrderCode);
                        string orderdetailImg = string.Empty;
                        if (!string.IsNullOrEmpty(orderDetail.OrderImage) && !item.OrderImg.Contains("bkt.clouddn.com"))
                        {
                            orderdetailImg = domainUrl + orderDetail.OrderImage;
                        }
                        else {
                            orderdetailImg = orderDetail.OrderImage;
                        }
                        order.Add("orderImage", orderdetailImg);
                        order.Add("orderImages", orderDetail.OrderImages);
                        order.Add("platemaking", orderDetail.Platemaking);
                        order.Add("remark", orderDetail.Remark);
                        task.Add("order", order);
                    }
                    var moduleName = string.Empty;
                    if (item.Mark > 0)
                    {
                        ProcessCategoryEntity processCategory = SystemBusiness.BaseBusiness.GetProcessCategoryByID(orderDetail.BigCategoryID);
                        var categoryItems = processCategory.CategoryItems.FindAll(m => m.Type == 3);
                        var categoryItem = categoryItems.Find(m => m.Mark == item.Mark);
                        moduleName = categoryItem != null ? categoryItem.Name : string.Empty;
                    }
                    JsonDictionary.Add("moduleName", moduleName);
                    JsonDictionary.Add("task", task);
                    JsonDictionary.Add("domainUrl", domainUrl);
                }
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetTaskReplys(string taskID, string userID, string clientID, int pageSize = 10, int pageIndex = 1)
        {
            if (!string.IsNullOrEmpty(taskID))
            {
                int pageCount = 0;
                int totalCount = 0;
                var list = ReplyBusiness.GetTaskReplys(taskID,  pageSize, pageIndex, ref totalCount, ref pageCount);
                List<Dictionary<string, object>> replys = new List<Dictionary<string, object>>();

                foreach (var item in list)
                {
                    Dictionary<string, object> reply = new Dictionary<string, object>();
                    reply.Add("replyID", item.ReplyID);
                    reply.Add("taskID", item.GUID);
                    reply.Add("content", item.Content);
                    reply.Add("createUser", GetUserBaseObj(item.CreateUser));
                    reply.Add("fromReplyUser", GetUserBaseObj(item.FromReplyUser));
                    reply.Add("createTime", item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                    reply.Add("attachments", item.Attachments);
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

        public JsonResult GetTaskLogs(string taskID, string userID, string clientID, int pageindex = 1)
        {
            if (!string.IsNullOrEmpty(taskID))
            {
                int totalCount = 0;
                int pageCount = 0;
                var list = LogBusiness.GetLogs(taskID, EnumLogObjectType.OrderTask, EnumLogSubject.All, PageSize, pageindex, ref totalCount, ref pageCount, clientID);

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

        public JsonResult GetOrderInfo(string orderID, string userID, string clientID)
        {
            if (!string.IsNullOrEmpty(orderID))
            {
                var orderDetail = OrdersBusiness.BaseBusiness.GetOrderByID(orderID, clientID);
                Dictionary<string, object> order = new Dictionary<string, object>();
                List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();

                if (orderDetail != null)
                {
                    order.Add("orderID", orderDetail.OrderID);
                    order.Add("orderCode", orderDetail.OrderCode);
                    order.Add("orderImage", orderDetail.OrderImage);
                    order.Add("orderImages", orderDetail.OrderImages);
                    order.Add("platemaking", orderDetail.Platemaking);
                    order.Add("remark", orderDetail.Remark);
                    JsonDictionary.Add("order", order);

                    foreach (var item in orderDetail.Details)
                    {
                        Dictionary<string, object> detail = new Dictionary<string, object>();
                        detail.Add("code",string.IsNullOrEmpty(item.DetailsCode)?item.ProductCode:item.DetailsCode);
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

        public JsonResult GetTaskLableColors(string clientID)
        {
            var lables = SystemBusiness.BaseBusiness.GetLableColor(clientID, EnumMarkType.Tasks).ToList();
            JsonDictionary.Add("items", lables);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region update
        public JsonResult UpdateTaskEndTime(string taskID, string endTime, string userID, string clientID)
        {
            int result = 0;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endTime)) endDate = DateTime.Parse(endTime);
            CurrentUser = OrganizationBusiness.GetUserByUserID(userID, clientID);

            TaskBusiness.UpdateTaskEndTime(taskID, endDate, "", false, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult FinishTask(string taskID, string userID, string clientID)
        {
            int result = 0;
            CurrentUser = OrganizationBusiness.GetUserByUserID(userID, clientID);
            TaskBusiness.FinishTask(taskID, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        //锁定任务
        public JsonResult LockTask(string taskID, string operateID, string clientID)
        {
            int result;
            TaskBusiness.LockTask(taskID, operateID, "", clientID, out result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [ValidateInput(false)]
        public JsonResult SavaTaskReply(string reply, string userID, string clientID,string taskID)
        {
            var model = JsonConvert.DeserializeObject<IntFactoryEntity.ReplyJson>(reply);
            string replyID = ReplyBusiness.CreateTaskReply(model.taskID, model.content, userID, clientID, 
                model.fromReplyID, model.fromReplyUserID, model.fromReplyAgentID);
            ReplyBusiness.AddTaskReplyAttachments(taskID, replyID, model.attachments, userID, clientID);

            if (!string.IsNullOrEmpty(replyID))
            {
                List<Dictionary<string, object>> replys = new List<Dictionary<string, object>>();
                Dictionary<string, object> replyObj = new Dictionary<string, object>();
                replyObj.Add("replyID", replyID);
                replyObj.Add("taskID", model.taskID);
                replyObj.Add("content", model.content);
                replyObj.Add("createUser", GetUserBaseObj(OrganizationBusiness.GetUserCacheByUserID(userID, clientID)));
                if (!string.IsNullOrEmpty(model.fromReplyUserID) && !string.IsNullOrEmpty(model.fromReplyAgentID))
                {
                    replyObj.Add("fromReplyUser", GetUserBaseObj(OrganizationBusiness.GetUserCacheByUserID(model.fromReplyUserID, model.fromReplyAgentID)));
                }
                replyObj.Add("attachments", model.attachments);
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

        //车缝、裁剪录入
        public JsonResult CreateOrderGoodsDoc(string orderID, string taskID, int docType, int isOver, string details, string remark, string ownerID, string operateID, string clientID, string expressID, string expressCode, string processid = "")
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderID, taskID, (EnumGoodsDocType)docType, isOver, expressID, expressCode, details, remark, ownerID, operateID, clientID, processid: processid);
            JsonDictionary.Add("id", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #region common
        public Dictionary<string, object> GetUserBaseObj(IntFactoryEntity.CacheUserEntity user) 
        {
            Dictionary<string, object> userObj = new Dictionary<string, object>();
            if (user != null)
            {
                userObj.Add("userID", user.UserID);
                userObj.Add("clientID", user.ClientID);
                userObj.Add("name", user.Name);
                userObj.Add("avatar", user.Avatar);
            }

            return userObj;
        }
        #endregion
    }
}
