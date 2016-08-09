﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using IntFactoryEntity;
using IntFactoryEntity.Task;
using IntFactoryEnum;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using YXERP.Models;
namespace YXERP.Controllers
{
    public class TaskController : BaseController
    {
        // GET: /Task/
        //string token = "ef588f83-bf39-418b-8f5c-103f9208a63d";
        string token = "c5e5e1c5-94de-4ae4-9bc3-a259bb1b32fe";
        string refreshToken = "be462dcd-1baf-4665-8444-1646d8350c8c";
        List<string> codes = new List<string>();
        public JsonResult pullFentDataList()
        {
            int successCount = 0;
            int total = 0;
            string error;

            AliOrderBusiness.DownFentOrders(DateTime.Now.AddMonths(-1), DateTime.Now.AddDays(1), token, refreshToken,
                CurrentUser.UserID, CurrentUser.ClientID,
                ref successCount, ref total, out  error, AlibabaSdk.AliOrderDownType.Hand);

            JsonDictionary.Add("result", successCount);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult PullBulkGoodsCodes()
        {
           var  result=AlibabaSdk.OrderBusiness.PullBulkGoodsCodes(DateTime.Now.AddMonths(-6), DateTime.Now.AddDays(1), token);

           JsonDictionary.Add("result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ExecuteDownAliOrdersPlan()
        {
            int successCount = 0, total = 0;
            //获取阿里订单下载计划列表
            var list = AliOrderBusiness.BaseBusiness.GetAliOrderDownloadPlans();

            foreach (var item in list)
            {
                string error;

                //下载阿里打样订单
                var gmtFentEnd = DateTime.Now;
                bool flag = AliOrderBusiness.DownFentOrders(item.FentSuccessEndTime, gmtFentEnd, item.Token, item.RefreshToken,
                    item.UserID, item.ClientID, ref successCount, ref total, out error);

                //新增阿里打样订单下载日志
                AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog(EnumOrderType.ProofOrder, flag, AlibabaSdk.AliOrderDownType.Auto, item.FentSuccessEndTime, gmtFentEnd,
                    successCount, total, item.ClientID, error);

                //添加服务日志
                string state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    ClientID:" + item.ClientID + " 下载打样订单结果:" + (flag ? "成功" : "失败");
                if (!flag)
                    state += "  原因：" + error;
                WriteLog(state);


                //下载阿里大货订单列表
                var gmtBulkEnd = DateTime.Now;
                flag = AliOrderBusiness.DownBulkOrders(item.BulkSuccessEndTime, gmtBulkEnd, item.Token, item.RefreshToken,
                    item.UserID, item.ClientID, ref successCount, ref total, out error);

                //新增阿里大货订单下载日志
                AliOrderBusiness.BaseBusiness.AddAliOrderDownloadLog(EnumOrderType.LargeOrder, flag, AlibabaSdk.AliOrderDownType.Auto, item.BulkSuccessEndTime, gmtBulkEnd,
                    successCount, total, item.ClientID, error);

                //添加服务日志
                state = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "    ClientID:" + item.ClientID + " 下载大货订单结果:" + (flag ? "成功" : "失败");
                if (!flag)
                    state += "  原因：" + error;
                WriteLog(state);
            }
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 添加服务日志
        /// </summary>
        public void WriteLog(string str, int logType = 1)
        {
            string fileName = DateTime.Now.ToString("yyyy-MM-dd");
            string fileExtention = ".txt";
            string directoryName = "downaliorders";
            FileStream fs = null;
            if (logType == 2)
                directoryName = "updatealiorders";

            if (!Directory.Exists(@"c:\log\" + directoryName))
            {
                Directory.CreateDirectory(@"c:\log\" + directoryName);
            }

            fs = new FileStream(@"c:\log\" + directoryName + "\\" + fileName + fileExtention, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("WindowsService: Service Started" + str + "\n");

            sw.Flush();
            sw.Close();
            fs.Close();
        }

        public JsonResult BatchUpdateFentList()
        {
            //List<AlibabaSdk.MutableOrder> list=new List<AlibabaSdk.MutableOrder>();
            //AlibabaSdk.MutableOrder item = new AlibabaSdk.MutableOrder();
            //item.fentGoodsCode = "UUU0080G4G002SS00081";
            //item.status = "b";
            //item.statusDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //item.statusDesc = "bbbbbbbb";
            //list.Add(item);
            //AlibabaSdk.OrderBusiness.BatchUpdateFentList(list, token);


            List<string> fails = new List<string>();
            AliOrderBusiness.UpdateAliFentOrders("2fb14a22-c6a0-4de6-830c-b95624dfdee4", "dd80ba5e-4aa8-4b0f-90f1-f78b95d4ab9f", "be462dcd-1baf-4665-8444-1646d8350c8c", out fails);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #region view

        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Task/MyTask");
            }

            TaskModel taskModel = new Models.TaskModel();
            //任务详情
            var task = TaskBusiness.GetTaskDetail(id);
            taskModel.Task = task;

            //任务对应的订单详情
            var order = OrdersBusiness.BaseBusiness.GetOrderByID(task.OrderID, CurrentUser.ClientID);
            ProcessCategoryEntity item = SystemBusiness.BaseBusiness.GetProcessCategoryByID(order.BigCategoryID);

            ViewBag.plateMarkItems = item.CategoryItems.FindAll(m => m.Type == 4).ToList();
            ViewBag.Modules = item.CategoryItems.FindAll(m => m.Type == 3);
            if (order.Details == null){
                order.Details = new List<IntFactoryEntity.OrderDetail>();
            }
            taskModel.Order = order;

            //判断查看权限
            if(!IsSeeRoot(task,order)){
                Response.Write("<script>alert('您无查看任务权限');location.href='/Task/MyTask';</script>");
                Response.End();
            }

            //任务剩余时间警告
            var isWarn = false;
            if (task.FinishStatus == 1)
            {
                if (task.EndTime > DateTime.Now)
                {
                    var totalHour = (task.EndTime - task.AcceptTime).TotalHours;
                    var residueHour = (task.EndTime - DateTime.Now).TotalHours;

                    var residue = residueHour / totalHour;
                    if (residue < 0.333)
                    {
                        isWarn = true;
                    }
                }
            }
            taskModel.IsWarn = isWarn;

            //当前用户是否为任务负责人
            taskModel.IsTaskOwner = task.OwnerID.Equals(CurrentUser.UserID, StringComparison.OrdinalIgnoreCase) ? true : false;

            //当前用户是否有编辑权限
            var isEditTask = false;
            TaskMember member = task.TaskMembers.Find(a => a.MemberID.ToLower() == CurrentUser.UserID.ToLower());
            if (member != null)
            {
                if (member.PermissionType == 2)
                {
                    isEditTask = true;
                }
            }
            taskModel.IsEditTask = isEditTask;

            //订单的品类属性
            taskModel.ProductAttr = new IntFactoryEntity.ProductAttr();
            //制版任务
            if (task.Mark == 12 || task.Mark==22)
            {
                taskModel.ProductAttr = new ProductsBusiness().GetTaskPlateAttrByCategoryID(order.CategoryID);
            }

            //任务完成周期
            if (task.FinishStatus == 2)
            {
                taskModel.FinishDay = (int)Math.Ceiling((task.CompleteTime - task.AcceptTime).TotalDays);
            }
            //操作权限
            taskModel.IsRoot = (task.Status != 8 && (task.FinishStatus == 1 || task.LockStatus==2) && (taskModel.IsEditTask || taskModel.IsTaskOwner) );
            ViewBag.TaskModel = taskModel;

            return View();
        }

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
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Tasks).ToList();
            ViewBag.UserID = CurrentUser.UserID;
            return View();
        }

        public ActionResult Participate()
        {
            ViewBag.IsMy = 2;
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Tasks).ToList();
            ViewBag.UserID = CurrentUser.UserID;
            return View("MyTask");
        }

        public ActionResult Tasks()
        {
            ViewBag.IsMy = 0;
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Tasks).ToList();
            ViewBag.UserID = CurrentUser.UserID;
            return View("MyTask");
        }
        #endregion

        #region ajax
        //获取任务列表
        public JsonResult GetTasks(string keyWords, bool isMy, int isParticipate, string userID, int taskType, int colorMark, 
            int status, int finishStatus,int invoiceStatus,int preFinishStatus,
            string beginDate, string endDate,string beginEndDate,string endEndDate,
            int orderType, string orderProcessID, string orderStageID, 
            int taskOrderColumn, int isAsc, int pageSize, int pageIndex, string listType)
        {
            int pageCount = 0;
            int totalCount = 0;
            //所有任务
            string ownerID = string.Empty;
            //我的任务
            if (isMy || isParticipate==1)
            {
                ownerID = CurrentUser.UserID;
            }//指定用户的任务
            else{
                if (!string.IsNullOrEmpty(userID))
                {
                    ownerID = userID;
                }
            }
            
            List<TaskEntity> list = TaskBusiness.GetTasks(keyWords.Trim(), ownerID, isParticipate,status, finishStatus,invoiceStatus,preFinishStatus,
                colorMark, taskType, beginDate, endDate, beginEndDate,endEndDate,
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

        //获取任务详情
        public JsonResult GetTaskDetail(string id) {
            var item = TaskBusiness.GetTaskDetail(id);
            JsonDictionary.Add("item", item);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取任务讨论
        public JsonResult GetReplys(string guid, string stageID, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;
            var list = TaskBusiness.GetTaskReplys(guid, stageID, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取下单明细
        public JsonResult GetOrderGoods(string id)
        {
            var list = OrdersBusiness.BaseBusiness.GetOrderGoods(id);
            JsonDictionary.Add("list", list);

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

            var list = LogBusiness.GetLogs(id, EnumLogObjectType.OrderTask, PageSize, pageindex,
                ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取加工成本
        public JsonResult GetOrderCosts(string orderid)
        {
            var list = OrdersBusiness.BaseBusiness.GetOrderCosts(orderid, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取材料列表
        public JsonResult GetOrderDetailsByOrderID(string orderid)
        {
            var list = OrdersBusiness.BaseBusiness.GetOrderDetailsByOrderID(orderid);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取工艺说明
        public JsonResult GetPlateMakings(string orderID)
        {
            var list = TaskBusiness.GetPlateMakings(orderID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //保存任务讨论
        public JsonResult SavaReply(string entity, string taskID, string attchmentEntity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ReplyEntity model = serializer.Deserialize<ReplyEntity>(entity);
            model.Attachments = serializer.Deserialize<List<IntFactoryEntity.Attachment>>(attchmentEntity);
            string replyID = OrdersBusiness.CreateReply(model.GUID, model.StageID, model.Mark, model.Content, CurrentUser.UserID, CurrentUser.ClientID, model.FromReplyID, model.FromReplyUserID, model.FromReplyAgentID);

            if (model.Attachments.Count > 0)
            {
                TaskBusiness.AddTaskReplyAttachments(taskID, replyID, model.Attachments, CurrentUser.UserID, CurrentUser.ClientID);
            }
            List<ReplyEntity> list = new List<ReplyEntity>();
            if (!string.IsNullOrEmpty(replyID))
            {
                model.ReplyID = replyID;
                model.CreateTime = DateTime.Now;
                model.CreateUser = CurrentUser;
                model.CreateUserID = CurrentUser.UserID;
                if (!string.IsNullOrEmpty(model.FromReplyUserID) && !string.IsNullOrEmpty(model.FromReplyAgentID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //设置任务到期时间
        public JsonResult UpdateTaskEndTime(string id, string endTime)
        {
            int result = 0;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endTime)) endDate = DateTime.Parse(endTime);
            TaskBusiness.UpdateTaskEndTime(id, endDate, CurrentUser.UserID, 
                Common.Common.GetRequestIP(), CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //标记任务完成
        public JsonResult FinishTask(string id)
        {
            int result = 0;
            TaskBusiness.FinishTask(id, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.ClientID,out result);
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //添加任务成员
        public JsonResult AddTaskMembers(string id, string memberIDs)
        {
            int result = 0;
            bool flag= TaskBusiness.AddTaskMembers(id,memberIDs, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.ClientID,out result);
            JsonDictionary.Add("result", flag?1:0);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //移除任务成员
        public JsonResult RemoveTaskMember(string id, string memberID)
        {
            bool flag = TaskBusiness.RemoveTaskMember(id, memberID, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.ClientID);
            JsonDictionary.Add("result", flag ? 1 : 0);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //设置任务负责人
        public JsonResult UpdateTaskOwner(string taskid, string userid)
        {
            int result = 0;
            bool bl = TaskBusiness.UpdateTaskOwner(taskid, userid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //设置任务成员权限
        public JsonResult UpdateMemberPermission(string taskID, string memberID, int type)
        {
            bool flag = IntFactoryBusiness.TaskBusiness.UpdateMemberPermission(taskID, memberID, (TaskMemberPermissionType)type, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("result", flag ? 1 : 0);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

       //锁定任务
        public JsonResult LockTask(string taskID)
        {
            int result;

            TaskBusiness.LockTask(taskID, CurrentUser.UserID, OperateIP, CurrentUser.ClientID, out result);

            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        //设置任务颜色标记
        public JsonResult UpdateTaskColorMark(string ids, int mark)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && TaskBusiness.UpdateTaskColorMark(id, mark, CurrentUser.UserID, OperateIP, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //设置制版信息
        [ValidateInput(false)]
        public JsonResult UpdateOrderPlatehtml(string orderID, string taskID, string platehtml)
        {
            int result = 0;
            result = OrdersBusiness.BaseBusiness.UpdateOrderPlateAttr(orderID, taskID, platehtml, 
                CurrentUser.UserID, string.Empty, CurrentUser.ClientID) ? 1 : 0;
            JsonDictionary.Add("result", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //保存工艺说明
        public JsonResult SavePlateMaking(string plate)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            PlateMaking model = serializer.Deserialize<PlateMaking>(plate);
            bool flag = false;
            if (string.IsNullOrEmpty(model.PlateID))
            {
                string plateID = Guid.NewGuid().ToString();
                model.CreateUserID = CurrentUser.UserID;
                flag = IntFactoryBusiness.TaskBusiness.AddPlateMaking(model, CurrentUser.UserID, string.Empty, CurrentUser.ClientID, plateID);
                JsonDictionary.Add("id", plateID);
            }
            else
            {
                flag = IntFactoryBusiness.TaskBusiness.UpdatePlateMaking(model, CurrentUser.UserID, string.Empty,  CurrentUser.ClientID);
            }
            JsonDictionary.Add("result", flag ? 1 : 0);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //删除工艺说明
        public JsonResult DeletePlateMaking(string plateID,string taskID,string title)
        {
            bool flag = IntFactoryBusiness.TaskBusiness.DeletePlateMaking(plateID, taskID, title, CurrentUser.UserID, string.Empty, CurrentUser.ClientID);

            JsonDictionary.Add("result", flag ? 1 : 0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        //删除加工成本
        public JsonResult DeleteOrderCost(string orderid, string autoid)
        {
            var bl = OrdersBusiness.BaseBusiness.DeleteOrderCost(orderid, autoid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //新增加工成本
        public JsonResult CreateOrderCutOutDoc(string orderid, int doctype, int isover, string expressid, string expresscode, string details, string remark, string taskid = "")
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid, taskid, (EnumGoodsDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //新增加工成本
        public JsonResult CreateOrderCost(string orderid, decimal price, string remark)
        {
            var bl = OrdersBusiness.BaseBusiness.CreateOrderCost(orderid, price, remark, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //添加车缝录入、裁剪录入
        public JsonResult CreateOrderSewnDoc(string orderid, string taskid, int doctype, int isover, string expressid, string expresscode, string details, string remark)
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid, taskid, (EnumGoodsDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //车缝退回
        public JsonResult CreateGoodsDocReturn(string orderID, string taskID, int docType, string details, string originalID)
        {
            int result = 0;
            OrdersBusiness.BaseBusiness.CreateGoodsDocReturn(orderID, taskID, (EnumDocType)docType, details, originalID, CurrentUser.ClientID, ref result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        
        //面料登记
        public JsonResult CreateProductUseQuantity(string orderID, string details)
        {
            int result = 0;
            string errInfo = string.Empty;
            OrdersBusiness.BaseBusiness.CreateProductUseQuantity(ref result, ref errInfo, orderID, details, CurrentUser.UserID, Common.Common.GetRequestIP(), CurrentUser.ClientID);
            JsonDictionary.Add("result",result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //添加大货订单发货、打样订单发货
        public JsonResult CreateOrderSendDoc(string orderid, string taskid, int doctype, int isover, string expressid, string expresscode, string details, string remark)
        {
            string id = OrdersBusiness.BaseBusiness.CreateOrderGoodsDoc(orderid, taskid, (EnumGoodsDocType)doctype, isover, expressid, expresscode, details, remark, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("id", id);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        public bool IsSeeRoot(TaskEntity task, IntFactoryEntity.OrderEntity order) {
            if (task.OwnerID.Equals(CurrentUser.UserID, StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            else if(task.TaskMembers.Find(m=>m.MemberID.ToLower()==CurrentUser.UserID.ToLower())!=null)
            {
                return true;
            }
            else if (order.OwnerID.Equals(CurrentUser.UserID, StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            else if (string.IsNullOrEmpty(ExpandClass.IsLimits("109010200")))
            {
                return true;
            }

            return false;
        }

    }
}
