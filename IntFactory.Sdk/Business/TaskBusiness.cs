using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace IntFactory.Sdk
{
    public class TaskBusiness
    {
        #region get

        public static TaskListResult GetTasks(string userID = "BC6802E9-285C-471C-8172-3867C87803E2", string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466")
        { 
            var paras = new Dictionary<string, object>();
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer < TaskListResult>(ApiOption.GetTasks, paras);
        }

        public ProcessListResult GetOrderProcess(string userID = "BC6802E9-285C-471C-8172-3867C87803E2", string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<ProcessListResult>(ApiOption.GetOrderProcess, paras);
        }

        public ProcessStageListResult GetOrderStages(string processID, string userID = "BC6802E9-285C-471C-8172-3867C87803E2", string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("processID", processID);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<ProcessStageListResult>(ApiOption.GetOrderStages, paras);
        }

        public TaskDetailResult GetTaskDetail(string taskID, string userID = "BC6802E9-285C-471C-8172-3867C87803E2", string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("taskID", taskID);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<TaskDetailResult>(ApiOption.GetTaskDetail, paras);
        }

        public TaskReplyListResult GetTaskReplys(string orderID, string stageID, int pageIndex = 1, string userID = "BC6802E9-285C-471C-8172-3867C87803E2", string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("orderID", orderID);
            paras.Add("stageID", stageID);
            paras.Add("pageIndex", pageIndex);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<TaskReplyListResult>(ApiOption.GetTaskReplys, paras);
        }

        public TaskLogListResult GetTaskLogs(string taskID, int pageindex = 1, string userID = "BC6802E9-285C-471C-8172-3867C87803E2", string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("taskID", taskID);
            paras.Add("pageIndex", pageindex);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<TaskLogListResult>(ApiOption.GetTaskLogs, paras);
        }

        public OrderBaseResult GetOrderInfo(string orderID, string userID = "BC6802E9-285C-471C-8172-3867C87803E2", string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("orderID", orderID);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<OrderBaseResult>(ApiOption.GetOrderInfo, paras);
        }
        #endregion

        #region update
        public UpdateResult UpdateTaskEndTime(string taskID, string endTime, string userID = "BC6802E9-285C-471C-8172-3867C87803E2", string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("taskID", taskID);
            paras.Add("endTime", endTime);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<UpdateResult>(ApiOption.UpdateTaskEndTime, paras);
        }

        public UpdateResult FinishTask(string taskID, string userID = "BC6802E9-285C-471C-8172-3867C87803E2", string agentID = "9F8AF979-8A3B-4E23-B19C-AB8702988466")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("taskID", taskID);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<UpdateResult>(ApiOption.FinishTask, paras);
        }
        #endregion

    }
}
