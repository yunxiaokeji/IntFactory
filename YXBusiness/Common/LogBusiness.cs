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

        #region 查询

        public List<UpcomingsEntity> GetClientUpcomings(string userid, string clientid)
        {
            DataTable dt = new LogDAL().GetClientUpcomings(userid, clientid);
            List<UpcomingsEntity> list = new List<UpcomingsEntity>();

            foreach (DataRow dr in dt.Rows)
            {
                UpcomingsEntity entity = new UpcomingsEntity();
                entity.FillData(dr);
                list.Add(entity);
            }

            return list;

        }
        public List<OtherSyncTaskRecord> GetSyncTaskRecord(int type, string status,string orderid, string clientid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string sqlWhere = "a.Status<>9";
            if (!string.IsNullOrEmpty(orderid))
                sqlWhere += " and ( a.OrderID ='" + orderid + "' )";
            if (!string.IsNullOrEmpty(clientid))
                sqlWhere += " and ( a.ClientID ='" + clientid + "' )"; 
            if (type > 0)
                sqlWhere += " and ( a.Type ='" + type + "' )";
            if (!string.IsNullOrEmpty(status))
                sqlWhere += " and ( a.Status in (" + status + ") )";
            string sqlColumn = @" * ";
            DataTable dt = CommonBusiness.GetPagerData("OtherSyncTaskRecord a", sqlColumn, sqlWhere, "a.AutoID", pageSize, pageIndex, out totalCount, out pageCount);
            List<OtherSyncTaskRecord> list = new List<OtherSyncTaskRecord>();

            foreach (DataRow dr in dt.Rows)
            {
                OtherSyncTaskRecord entity = new OtherSyncTaskRecord();
                entity.FillData(dr);
                entity.Content = string.IsNullOrEmpty(entity.Content) ? "" : entity.Content.Replace("“", "\"");
                list.Add(entity);
            }

            return list;

        }
        /// <summary>
        /// 获取日志
        /// </summary>
        /// <returns></returns>
        public static List<LogEntity> GetLogs(string guid, EnumLogObjectType type,EnumLogSubject subject, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
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
                case EnumLogObjectType.OrderTask:
                    tablename = "OrderTaskLog";
                    break;
            }

            var where = "LogGUID='" + guid + "'";
            if (subject != EnumLogSubject.All) 
            {
                where += " and SubjectType=" + (int)subject;
            }

            DataTable dt = CommonBusiness.GetPagerData(tablename, "*", where, "AutoID", pageSize, pageIndex, out totalCount, out pageCount);

            List<LogEntity> list = new List<LogEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                LogEntity model = new LogEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserCacheByUserID(model.CreateUserID, clientid);

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
        public static async void AddLoginLog(string loginname, bool status, EnumSystemType systemtype, string operateip, string userid, string clientid)
        {
            await LogDAL.AddLoginLog(loginname, status ? 1 : 0, (int)systemtype, operateip, userid, clientid);
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
        public static async void AddLog(string logguid, EnumLogObjectType type, string remark, string userid, string operateip, string guid, string clientid, EnumLogSubject subject = EnumLogSubject.Other)
        {
            string tablename = "OperateLog";
            switch (type)
            {
                case EnumLogObjectType.Customer:
                    tablename = "CustomerLog";
                    break;
                case EnumLogObjectType.Orders:
                    tablename = "OrdersLog";
                    break;
                case EnumLogObjectType.OrderTask:
                    tablename = "OrderTaskLog";
                    break;
                default:
                    tablename = "OperateLog";
                    break;
            }
            await LogDAL.AddLog(tablename, logguid, remark, userid, operateip, guid, clientid, (int)subject);
        }

        public static async void AddActionLog(EnumSystemType systemtype, EnumLogObjectType objecttype, EnumLogType actiontype, string operateip, string userid, string clientid)
        {
            await LogDAL.AddActionLog((int)systemtype, (int)objecttype, (int)actiontype, operateip, userid, clientid);
        }
        public static async void AddOtherRecord(int type, string orderid,string othersysid, string content,string remark, string userid, string clientid)
        {
            await LogDAL.AddOtherRecord(type, orderid, othersysid, content, remark, userid, clientid);
        }
        public static bool UpdateOtherRecord(int autoid, int status, string errormsg)
        {
            return LogDAL.updateOtherRecord(autoid, status, errormsg);
        }
        #endregion
    }
}
