using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IntFactoryDAL;
using System.Threading.Tasks;
using IntFactoryEnum;
using IntFactoryEntity;
using System.Data;

namespace IntFactoryBusiness
{
    public class LogBusiness
    {
        public static LogBusiness BaseBusiness = new LogBusiness();
        #region Cache

        private static Dictionary<string, AgentActionEntity> _agentActions;
        private static Dictionary<string, AgentActionEntity> AgentActions 
        {
            get 
            {
                if (_agentActions == null)
                {
                    _agentActions = new Dictionary<string, AgentActionEntity>();
                }
                return _agentActions;
            }
            set
            {
                _agentActions = value;
            }
        }

        #endregion

        #region 查询

        public AgentActionEntity GetAgentActions(string agentid)
        {
            string datestr = DateTime.Now.ToString("yyyy-MM-dd");
            if (AgentActions.ContainsKey(agentid))
            {
                var obj = AgentActions[agentid];
                if (obj.Date == datestr)
                {
                    return obj;
                }
                DataTable dt = new LogDAL().GetAgentActions(datestr + " 00:00:00", agentid);
                AgentActionEntity model = new AgentActionEntity();
                model.Date = datestr;
                model.Actions = new List<ActionTypeEntity>();
                foreach (DataRow dr in dt.Rows)
                {
                    ActionTypeEntity entity = new ActionTypeEntity();
                    entity.FillData(dr);
                    model.Actions.Add(entity);
                }
                obj = model;
                return obj;
            }
            else
            {
                DataTable dt = new LogDAL().GetAgentActions(datestr + " 00:00:00", agentid);
                AgentActionEntity model = new AgentActionEntity();
                model.Date = datestr;
                model.Actions = new List<ActionTypeEntity>();
                foreach (DataRow dr in dt.Rows)
                {
                    ActionTypeEntity entity = new ActionTypeEntity();
                    entity.FillData(dr);
                    model.Actions.Add(entity);
                }
                AgentActions.Add(agentid, model);
                return model;
            }
        }

        public List<UpcomingsEntity> GetClientUpcomings(string agentid, string clientid)
        {
            DataTable dt = new LogDAL().GetClientUpcomings(agentid, clientid);
            List<UpcomingsEntity> list = new List<UpcomingsEntity>();

            foreach (DataRow dr in dt.Rows)
            {
                UpcomingsEntity entity = new UpcomingsEntity();
                entity.FillData(dr);
                list.Add(entity);
            }

            return list;

        }

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <returns></returns>
        public static List<LogEntity> GetLogs(string guid, EnumLogObjectType type, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid)
        {
            string tablename = "";
            switch (type)
            {
                case EnumLogObjectType.Customer:
                    tablename = "CustomerLog";
                    break;
                case EnumLogObjectType.Orders:
                    tablename = "OrdersLog";
                    break;
            }

            DataTable dt = CommonBusiness.GetPagerData(tablename, "*", "LogGUID='" + guid + "'", "AutoID", pageSize, pageIndex, out totalCount, out pageCount);

            List<LogEntity> list = new List<LogEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                LogEntity model = new LogEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                list.Add(model);
            }
            return list;
        }

        #endregion

        #region 添加

        /// <summary>
        /// 记录登录日志
        /// </summary>
        /// <param name="loginname">用户名</param>
        /// <param name="status">登录结果</param>
        /// <param name="systemtype">系统类型</param>
        /// <param name="operateip">登录IP</param>
        public static async void AddLoginLog(string loginname, bool status, EnumSystemType systemtype, string operateip, string userid, string agentid, string clientid)
        {
            await LogDAL.AddLoginLog(loginname, status ? 1 : 0, (int)systemtype, operateip, userid, agentid, clientid);
        }
        
        /// <summary>
        /// 记录操作日志
        /// </summary>
        public static async void AddOperateLog(string userid, string funcname, EnumLogType type, EnumLogModules modules, EnumLogEntity entity, string guid, string message, string operateip)
        {
            await LogDAL.AddOperateLog(userid, funcname, (int)type, (int)modules, (int)entity, guid, message, operateip);
        }
        
        /// <summary>
        /// 记录错误日志
        /// </summary>
        public static async void AddErrorLog(string userid, string message, EnumSystemType systemtype, string operateip)
        {
            await LogDAL.AddErrorLog(userid, message, (int)systemtype, operateip);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        public static async void AddLog(string logguid, EnumLogObjectType type, string remark, string userid, string operateip, string guid, string agentid, string clientid)
        {
            string tablename = "";
            switch (type)
            {
                case EnumLogObjectType.Customer:
                    tablename = "CustomerLog";
                    break;
                case EnumLogObjectType.Orders:
                    tablename = "OrdersLog";
                    break;
            }
            await LogDAL.AddLog(tablename, logguid, remark, userid, operateip, guid, agentid, clientid);
        }

        public static async void AddActionLog(EnumSystemType systemtype, EnumLogObjectType objecttype, EnumLogType actiontype,  string operateip, string userid, string agentid, string clientid)
        {
            await LogDAL.AddActionLog((int)systemtype, (int)objecttype, (int)actiontype, operateip, userid, agentid, clientid);
        }


        #endregion
    }
}
