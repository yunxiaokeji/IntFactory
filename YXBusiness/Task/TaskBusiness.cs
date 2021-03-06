﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity.Task;
using IntFactoryDAL;
using System.Data;
using IntFactoryEnum;
using IntFactoryEntity;
using System.Data.SqlClient;
namespace IntFactoryBusiness
{
    public class TaskBusiness
    {
        #region 增
        public static bool AddTaskReplyAttachments(string taskid, string replyid, List<Attachment> attachments,string userid,string clientid)
        {
            SqlConnection conn = new SqlConnection(TaskDAL.ConnectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();


            foreach (var attachment in attachments)
            {
                if (!TaskDAL.BaseProvider.AddTaskReplyAttachment(taskid,replyid,attachment.Type,
                    attachment.ServerUrl,attachment.FilePath,attachment.FileName,attachment.OriginalName,attachment.ThumbnailName,attachment.Size,
                    userid,clientid, tran))
                {
                    tran.Rollback();
                    conn.Dispose();

                    return false;
                }
            }

            tran.Commit();
            conn.Dispose();

            return true;
        }
        #endregion

        #region 查
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="ownerID"></param>
        /// <param name="finishStatus">-1：所有；0：进行中；1：已完成</param>
        /// <param name="beginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="clientID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="TotalCount"></param>
        /// <param name="PageCount"></param>
        /// <returns></returns>
        public static List<TaskEntity> GetTasks(string keyWords, string ownerID, int isParticipate, int status, int finishStatus, int invoiceStatus,int preFinishStatus,
            int colorMark, int taskType, string beginDate, string endDate, string beginEndDate, string endEndDate,
            int orderType, string orderProcessID, string orderStageID,
            EnumTaskOrderColumn taskOrderColumn,int isAsc, string clientID,
            int pageSize, int pageIndex, ref int totalCount, ref int pageCount) 
        {
            List<TaskEntity> list = new List<TaskEntity>();
            DataTable dt = TaskDAL.BaseProvider.GetTasks(keyWords, ownerID,isParticipate, status, finishStatus, invoiceStatus,preFinishStatus,
                colorMark, taskType, beginDate, endDate, beginEndDate,endEndDate,
                orderType, orderProcessID, orderStageID,
                (int)taskOrderColumn, isAsc, clientID, 
                pageSize, pageIndex, ref totalCount, ref pageCount);

            foreach (DataRow dr in dt.Rows)
            {
                TaskEntity model = new TaskEntity();
                model.FillData(dr);
                //model.Stage = SystemBusiness.BaseBusiness.GetOrderStageByID(model.StageID, model.ProcessID, model.AgentID, model.ClientID);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                if (model.FinishStatus == 1)
                {
                    if (model.EndTime <= DateTime.Now)
                    {
                        model.WarningStatus = 2;
                        model.WarningTime = "超期：" + (DateTime.Now - model.EndTime).Days.ToString("D2") + "天 " + (DateTime.Now - model.EndTime).Hours.ToString("D2") + "时 " + (DateTime.Now - model.EndTime).Minutes.ToString("D2") + "分";
                        model.WarningDays = (DateTime.Now - model.EndTime).Days;
                        model.UseDays = (model.EndTime - model.AcceptTime).Days;
                    }
                    else if ((model.EndTime - DateTime.Now).TotalHours * 3 < (model.EndTime - model.AcceptTime).TotalHours)
                    {
                        model.WarningStatus = 1;
                        model.WarningTime = "剩余：" + (model.EndTime - DateTime.Now).Days.ToString("D2") + "天 " + (model.EndTime - DateTime.Now).Hours.ToString("D2") + "时 " + (model.EndTime - DateTime.Now).Minutes.ToString("D2") + "分";
                        model.WarningDays = (model.EndTime - DateTime.Now).Days;
                        model.UseDays = (DateTime.Now - model.AcceptTime).Days;
                    }
                    else
                    {
                        model.WarningTime = "剩余：" + (model.EndTime - DateTime.Now).Days.ToString("D2") + "天 " + (model.EndTime - DateTime.Now).Hours.ToString("D2") + "时 " + (model.EndTime - DateTime.Now).Minutes.ToString("D2") + "分";
                        model.WarningDays = (model.EndTime - DateTime.Now).Days;
                        model.UseDays = (DateTime.Now - model.AcceptTime).Days;
                    }
                }
                else
                {
                    model.UseDays = (model.EndTime - model.AcceptTime).Days;
                }
                list.Add(model);
            }

            return list;
        }

        public static List<TaskEntity> GetTasksByEndTime(string startEndTime, string endEndTime, 
            int orderType, int filterType, int finishStatus,int preFinishStatus,int taskType,
            string userID, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<TaskEntity> list = new List<TaskEntity>();
            DataSet ds = TaskDAL.BaseProvider.GetTasksByEndTime(startEndTime, endEndTime, 
                orderType, filterType, finishStatus,preFinishStatus,taskType,
                userID, clientID, pageSize, pageIndex, ref totalCount, ref pageCount);

            DataTable dt = ds.Tables["Tasks"];
            DataTable orders = ds.Tables["Orders"];
            foreach (DataRow dr in dt.Rows)
            {
                TaskEntity model = new TaskEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                if (orders.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in orders.Select(" OrderID='" + model.OrderID + "'"))
                    {
                        OrderEntity order = new OrderEntity();
                        order.FillData(dr2);
                        model.Order = order;
                    }
                }

                if (model.FinishStatus == 1)
                {
                    if (model.EndTime <= DateTime.Now)
                    {
                        model.WarningStatus = 2;
                        model.WarningTime = "超期：" + (DateTime.Now - model.EndTime).Days.ToString("D2") + "天 " + (DateTime.Now - model.EndTime).Hours.ToString("D2") + "时 " + (DateTime.Now - model.EndTime).Minutes.ToString("D2") + "分";
                        model.WarningDays = (DateTime.Now - model.EndTime).Days;
                        model.UseDays = (model.EndTime - model.AcceptTime).Days;
                    }
                    else if ((model.EndTime - DateTime.Now).TotalHours * 3 < (model.EndTime - model.AcceptTime).TotalHours)
                    {
                        model.WarningStatus = 1;
                        model.WarningTime = "剩余：" + (model.EndTime - DateTime.Now).Days.ToString("D2") + "天 " + (model.EndTime - DateTime.Now).Hours.ToString("D2") + "时 " + (model.EndTime - DateTime.Now).Minutes.ToString("D2") + "分";
                        model.WarningDays = (model.EndTime - DateTime.Now).Days;
                        model.UseDays = (DateTime.Now - model.AcceptTime).Days;
                    }
                    else
                    {
                        model.WarningTime = "剩余：" + (model.EndTime - DateTime.Now).Days.ToString("D2") + "天 " + (model.EndTime - DateTime.Now).Hours.ToString("D2") + "时 " + (model.EndTime - DateTime.Now).Minutes.ToString("D2") + "分";
                        model.WarningDays = (model.EndTime - DateTime.Now).Days;
                        model.UseDays = (DateTime.Now - model.AcceptTime).Days;
                    }
                }
                else
                {
                    model.WarningStatus = 3;
                    model.UseDays = (model.EndTime - model.AcceptTime).Days;
                    model.UseDays = (DateTime.Now - model.CompleteTime).Days;
                }
                list.Add(model);
            }

            return list;
        }

        public static int GetNoAcceptTaskCount(string ownerID,int orderType, string clientID)
        {
            return TaskDAL.BaseProvider.GetNoAcceptTaskCount(ownerID, orderType, clientID);
        }

        public static int GetexceedTaskCount(string ownerID, int orderType, string clientID)
        {
            return TaskDAL.BaseProvider.GetexceedTaskCount(ownerID, orderType, clientID);
        }
        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static TaskEntity GetTaskDetail(string taskID)
        {
            TaskEntity model = null;
            DataSet ds = TaskDAL.BaseProvider.GetTaskDetail(taskID);

            DataTable taskTB= ds.Tables["OrderTask"];
            if (taskTB.Rows.Count == 1)
            {
                model = new TaskEntity();
                model.FillData(taskTB.Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                DataTable memberTB = ds.Tables["TaskMember"];
                model.TaskMembers = new List<IntFactoryEntity.Task.TaskMember>();
                if (memberTB.Rows.Count > 0)
                {
                    foreach (DataRow m in memberTB.Rows)
                    {
                        TaskMember member = new TaskMember();
                        member.FillData(m);
                        member.Member = OrganizationBusiness.GetUserByUserID(member.MemberID, member.AgentID);
                        model.TaskMembers.Add(member);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// 获取订单的任务列表
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static List<TaskEntity> GetTasksByOrderID(string orderID) 
        {
            List<TaskEntity> list = new List<TaskEntity>();
            DataTable dt = TaskDAL.BaseProvider.GetTasksByOrderID(orderID);

            foreach (DataRow dr in dt.Rows)
            {
                TaskEntity model = new TaskEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                list.Add(model);
            }

            return list;
        }

        public static List<ReplyEntity> GetTaskReplys(string guid, string stageid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();

            DataSet ds = TaskDAL.BaseProvider.GetTaskReplys(guid, stageid, pageSize, pageIndex, ref totalCount, ref pageCount);
            DataTable replys = ds.Tables["Replys"];
            DataTable attachments = ds.Tables["Attachments"];
            foreach (DataRow dr in replys.Rows)
            {
                ReplyEntity model = new ReplyEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                if (!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }

                model.Attachments = new List<Attachment>();
                if (attachments.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in attachments.Select(" Guid='" + model.ReplyID + "'"))
                    {
                        Attachment attachment = new Attachment();
                        attachment.FillData(dr2);

                        model.Attachments.Add(attachment);
                    }
                }
                list.Add(model);
            }

            return list;

        }
        #endregion

        #region 改
        /// <summary>
        /// 修改任务负责人
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="OwnerID"></param>
        /// <returns></returns>
        public static bool UpdateTaskOwner(string taskID, string ownerID, string operateid, string ip, string agentid, string clientid,out int result)
        {
            bool flag= TaskDAL.BaseProvider.UpdateTaskOwner(taskID, ownerID,out result);

            if (flag)
            {
                var user = OrganizationBusiness.GetUserByUserID(ownerID, agentid);
                string msg = "将任务负责人更改为:"+(user!=null?user.Name:ownerID);
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
            }

            return flag;
        }

        /// <summary>
        /// 修改任务备注 （制板信息）
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool UpdateTaskRemark(string taskID, string remark)
        {
            bool flag= TaskDAL.BaseProvider.UpdateTaskRemark(taskID, remark);

            
            return flag;
        }

        /// <summary>
        /// 修改任务截至日期
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool UpdateTaskEndTime(string taskID, DateTime? endTime, string operateid, string ip, string agentid, string clientid,out int result)
        {
            bool flag = TaskDAL.BaseProvider.UpdateTaskEndTime(taskID, endTime, operateid,out result);
            if (flag)
            {
                string msg = "将任务截至日期设为：" + (endTime == null ? "未指定日期" : endTime.Value.Date.ToString("yyyy-MM-dd"));
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.OrderTask, EnumLogType.Update, "", operateid, agentid, clientid);
            }

            return flag;
        }

        public bool UpdateTaskColorMark(string taskid, int mark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CommonBusiness.Update("OrderTask", "ColorMark", mark, "TaskID='" + taskid + "'");
            if (bl)
            {
                string msg = "标记任务颜色";
                LogBusiness.AddLog(taskid, EnumLogObjectType.OrderTask, msg, operateid, ip, mark.ToString(), agentid, clientid);
            }
            return bl;
        }

        /// <summary>
        /// 将任务标记完成
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static bool FinishTask(string taskID,string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool flag= TaskDAL.BaseProvider.FinishTask(taskID, operateid, out result);

            if (flag)
            {
                string msg = "将任务标记为完成";
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.OrderTask, EnumLogType.Update, "", operateid, agentid, clientid);
            }

            return flag;
        }

        /// <summary>
        /// 批量添加任务成员
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="memberIDs"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public static bool AddTaskMembers(string taskID, string memberIDs, string operateid, string ip, string agentid, string clientid,out int result)
        {
            memberIDs =memberIDs.Trim(',');
            bool flag = TaskDAL.BaseProvider.AddTaskMembers(taskID, memberIDs, operateid, agentid,out result);

            if (flag)
            {
                var userName=string.Empty;
                foreach(var m in memberIDs.Split(',') )
                {
                    var user = OrganizationBusiness.GetUserByUserID(m, agentid);
                    userName += (user != null ? user.Name : "")+",";
                }
                string msg = "添加任务成员" + userName.Trim(',');
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
            }

            return flag;
        }

        /// <summary>
        /// 移除任务成员
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="memberID"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public static bool RemoveTaskMember(string taskID,string memberID, string operateid, string ip, string agentid, string clientid)
        {
            bool flag = TaskDAL.BaseProvider.RemoveTaskMember(taskID, memberID);

            if (flag)
            {
                var userName = string.Empty;
                var user = OrganizationBusiness.GetUserByUserID(memberID, agentid);
                userName += user != null ? user.Name : "";

                string msg = "删除任务成员" + userName.Trim(',');
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
            }

            return flag;
        }

        /// <summary>
        /// 更新任务成员权限
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="memberID"></param>
        /// <param name="taskMemberPermissionType"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public static bool UpdateMemberPermission(string taskID, string memberID, TaskMemberPermissionType taskMemberPermissionType, string operateid, string ip, string agentid, string clientid)
        {
            bool flag = TaskDAL.BaseProvider.UpdateMemberPermission(taskID, memberID, (int)taskMemberPermissionType);

            if (flag)
            {
                var userName = string.Empty;
                var user = OrganizationBusiness.GetUserByUserID(memberID, agentid);
                userName += user != null ? user.Name : "";

                string msg = "将任务成员" + userName.Trim(',') + "的权限更新为:" + (taskMemberPermissionType == TaskMemberPermissionType.See?"查看":"编辑");
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
            }

            return flag;
        }

        /// <summary>
        /// 将任务标记未完成
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static bool UnFinishTask(string taskID)
        {
            return TaskDAL.BaseProvider.UnFinishTask(taskID);
        }

        /// <summary>
        /// 将任务锁定
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool LockTask(string taskID, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool flag = TaskDAL.BaseProvider.LockTask(taskID, operateid, out result);

            if (flag)
            {
                string msg = "将任务锁定";
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.OrderTask, EnumLogType.Update, "", operateid, agentid, clientid);
            }

            return flag;
        }

        /// <summary>
        /// 将任务解锁
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="operateid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool UnLockTask(string taskID, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool flag = TaskDAL.BaseProvider.UnLockTask(taskID, operateid, out result);
            if (flag)
            {
                string msg = "将任务解锁";
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.OrderTask, EnumLogType.Update, "", operateid, agentid, clientid);
            }

            return flag;
        }

        public static bool DeleteTaskReplyAttachment(string attachmentid, string operateid,out int result)
        {
            return TaskDAL.BaseProvider.DeleteTaskReplyAttachment(attachmentid, operateid, out result);
        }
        #endregion

        #region 制版工艺
        /// <summary>
        /// 获取制版工艺信息
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static List<PlateMaking> GetPlateMakings(string orderID) {
            List<PlateMaking> list = new List<PlateMaking>();
            DataTable dt = TaskDAL.BaseProvider.GetPlateMakings(orderID);

            foreach (DataRow dr in dt.Rows)
            {
                PlateMaking item = new PlateMaking();
                item.FillData(dr);
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 获取制版工艺信息
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static List<PlateMaking> GetPlateMakings(string orderID, string taskID)
        {
            List<PlateMaking> list = new List<PlateMaking>();
            DataTable dt = TaskDAL.BaseProvider.GetPlateMakings(orderID, taskID);

            foreach (DataRow dr in dt.Rows)
            {
                PlateMaking item = new PlateMaking();
                item.FillData(dr);
                list.Add(item);
            } 
            return list;
        }

        /// <summary>
        /// 获取制版工艺详情
        /// </summary>
        /// <param name="plateID"></param>
        /// <returns></returns>
        public static PlateMaking GetPlateMakingDetail(string plateID)
        { 
            PlateMaking item=new PlateMaking();
            DataTable dt = TaskDAL.BaseProvider.GetPlateMakingDetail(plateID);

            if (dt.Rows.Count == 1) {
                item.FillData(dt.Rows[0]);
            }
            return item;
        }

        /// <summary>
        /// 添加制版工艺
        /// </summary>
        /// <param name="plate"></param>
        /// <returns></returns>
        public static bool AddPlateMaking(PlateMaking plate, string operateid, string ip, string agentid, string clientid)
        {
            bool flag= TaskDAL.BaseProvider.AddPlateMaking(plate.Title, plate.Remark, plate.Icon,
                plate.TaskID,plate.Type,plate.OrderID,plate.CreateUserID,plate.AgentID);

            if (flag)
            {
                string msg = "新增工艺说明："+plate.Title;
                LogBusiness.AddLog(plate.TaskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
            }

            return flag;
        }

        /// <summary>
        /// 修改制版工艺
        /// </summary>
        /// <param name="plate"></param>
        /// <returns></returns>
        public static bool UpdatePlateMaking(PlateMaking plate, string operateid, string ip, string agentid, string clientid)
        {
            bool flag= TaskDAL.BaseProvider.UpdatePlateMaking(plate.PlateID,plate.Title,plate.Remark,plate.Icon,plate.Type);

            if (flag)
            {
                string msg = "编辑工艺说明为：" + plate.Title;
                LogBusiness.AddLog(plate.TaskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
            }

            return flag;
        }

        /// <summary>
        /// 删除制版工艺
        /// </summary>
        /// <param name="plateID"></param>
        /// <returns></returns>
        public static bool DeletePlateMaking(string plateID,string taskid,string title, string operateid, string ip, string agentid, string clientid)
        {
            bool flag= TaskDAL.BaseProvider.DeletePlateMaking(plateID);

            if (flag)
            {
                string msg = "删除工艺说明：" + title;
                LogBusiness.AddLog(taskid, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
            }

            return flag;
        }
        #endregion
    }
}
