using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity.Task;
using IntFactoryDAL;
using System.Data;
using IntFactoryEnum;
namespace IntFactoryBusiness
{
    public class TaskBusiness
    {
        #region 增
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static bool CreateTask(string orderID) 
        {
            return TaskDAL.BaseProvider.CreateTask(orderID);
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
        public static List<TaskEntity> GetTasks(string keyWords, string ownerID, int finishStatus, string beginDate, string endDate, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount) 
        {
            List<TaskEntity> list = new List<TaskEntity>();
            DataTable dt = TaskDAL.BaseProvider.GetTasks(keyWords,ownerID,finishStatus, beginDate, endDate, clientID, pageSize, pageIndex, ref totalCount, ref pageCount);

            foreach (DataRow dr in dt.Rows)
            {
                TaskEntity model = new TaskEntity();
                model.FillData(dr);
                model.Stage = SystemBusiness.BaseBusiness.GetOrderStageByID(model.StageID, model.ProcessID, model.AgentID, model.ClientID);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                list.Add(model);
            }

            return list;
        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static TaskEntity GetTaskDetail(string taskID)
        {
            TaskEntity model = null;
            DataTable dt = TaskDAL.BaseProvider.GetTaskDetail(taskID);

            if(dt.Rows.Count==1)
            {
                model = new TaskEntity();
                model.FillData(dt.Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
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
        #endregion

        #region 改
        /// <summary>
        /// 修改任务负责人
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="OwnerID"></param>
        /// <returns></returns>
        public static bool UpdateTaskOwner(string taskID,string ownerID)
        {
            return TaskDAL.BaseProvider.UpdateTaskOwner(taskID, ownerID);
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
        public static bool UpdateTaskEndTime(string taskID, DateTime? endTime, string operateid, string ip, string agentid, string clientid)
        {
            bool flag= TaskDAL.BaseProvider.UpdateTaskEndTime(taskID, endTime);
            if (flag)
            {
                string msg = "将任务截至日期设为：" + (endTime == null ? "未指定日期" : endTime.Value.Date.ToString("yyyy-MM-dd"));
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
            }

            return flag;
        }

        /// <summary>
        /// 将任务标记完成
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static void FinishTask(string taskID, string userID, ref int result, string operateid, string ip, string agentid, string clientid)
        {
            TaskDAL.BaseProvider.FinishTask(taskID,userID,ref result);

            if (result==1)
            {
                string msg = "将任务标记为完成";
                LogBusiness.AddLog(taskID, EnumLogObjectType.OrderTask, msg, operateid, ip, "", agentid, clientid);
            }
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
        #endregion
    }
}
