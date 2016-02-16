using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity.Task;
using IntFactoryDAL;
using System.Data;
namespace IntFactoryBusiness
{
    public class TaskBusiness
    {
        #region 增
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="stageID"></param>
        /// <param name="createUserID"></param>
        /// <returns></returns>
        public static bool CreateTask(string orderID, string stageID, string createUserID) 
        {
            return TaskDAL.BaseProvider.CreateTask(orderID, stageID);
        }
        #endregion

        #region 查
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="ownerID"></param>
        /// <param name="beginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="clientID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="TotalCount"></param>
        /// <param name="PageCount"></param>
        /// <returns></returns>
        public static List<TaskEntity> GetTasks(string ownerID, string beginDate, string endDate, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount) 
        {
            List<TaskEntity> list = new List<TaskEntity>();
            DataTable dt = TaskDAL.BaseProvider.GetTasks(ownerID, beginDate, endDate, clientID, pageSize, pageIndex, ref totalCount, ref pageCount);

            foreach (DataRow dr in dt.Rows)
            {
                TaskEntity model = new TaskEntity();
                model.FillData(dr);

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
        /// 修改任务名称
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool UpdateTaskTitle(string taskID,string title)
        {
            return TaskDAL.BaseProvider.UpdateTaskTitle(taskID, title);
        }

        /// <summary>
        /// 修改任务截至日期
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool UpdateTaskEndTime(string taskID, DateTime endTime)
        {
            return TaskDAL.BaseProvider.UpdateTaskEndTime(taskID, endTime);
        }

        /// <summary>
        /// 将任务标记完成
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static bool FinishTask(string taskID)
        {
            return TaskDAL.BaseProvider.FinishTask(taskID);
        }

        /// <summary>
        /// 将任务标记未完成
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static bool UnFinishTask(string taskID)
        {
            return TaskDAL.BaseProvider.FinishTask(taskID);
        }
        #endregion
    }
}
