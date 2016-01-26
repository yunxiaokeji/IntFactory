using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using IntFactoryDAL;
using IntFactoryEntity;
using System.Data;
using IntFactoryEnum;

namespace IntFactoryBusiness
{
    public class ActivityBusiness
    {
        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public static  string FilePath = CloudSalesTool.AppSettings.Settings["UploadFilePath"];
        public static string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];

        #region 查询

        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <param name="userid">负责人</param>
        /// <param name="status">状态</param>
        /// <param name="filterType">过滤类型 0：所有；1：我负责的；2：我参与的</param>
        /// <param name="keyWords">关键词</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageSize">页Size</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总记录</param>
        /// <param name="pageCount">总页数</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public static List<ActivityEntity> GetActivitys(string userid, EnumActivityStage stage,int filterType, string keyWords,string beginTime,string endTime, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            List<ActivityEntity> list = new List<ActivityEntity>();
            DataTable dt = ActivityDAL.BaseProvider.GetActivitys(userid, (int)stage,filterType, keyWords,beginTime,endTime, pageSize, pageIndex, ref totalCount, ref pageCount, agentid, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                ActivityEntity model = new ActivityEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Members = new List<Users>();
                foreach (var id in model.MemberID.Split('|'))
                {
                    model.Members.Add(OrganizationBusiness.GetUserByUserID(id, model.AgentID));
                }
                list.Add(model);
            }
            return list;

        }

        /// <summary>
        /// 根据活动ID获取活动
        /// </summary>
        /// <param name="activityid"></param>
        /// <returns></returns>
        public static ActivityEntity GetActivityByID(string activityid, string agentid, string clientid)
        {
            ActivityEntity model = new ActivityEntity();
            DataTable dt = ActivityDAL.BaseProvider.GetActivityByID(activityid, agentid, clientid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);

                model.Owner= OrganizationBusiness.GetUserByUserID(model.OwnerID,model.AgentID);
                model.Members = new List<Users>();
                foreach (var id in model.MemberID.Split('|')) {
                    model.Members.Add(OrganizationBusiness.GetUserByUserID(id, model.AgentID));
                }

            }
            return model;
        }

        public static ActivityEntity GetActivityBaseInfoByID(string activityid, string agentid, string clientid)
        {
            ActivityEntity model = new ActivityEntity();
            DataTable dt = ActivityDAL.BaseProvider.GetActivityByID(activityid, agentid, clientid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        /// <summary>
        /// 根据活动Code获取活动
        /// </summary>
        /// <param name="activitycode"></param>
        /// <returns></returns>
        public static ActivityEntity GetActivityByCode(string activitycode, string agentid, string clientid)
        {
            ActivityEntity model = new ActivityEntity();
            DataTable dt = ActivityDAL.BaseProvider.GetActivityByCode(activitycode, agentid, clientid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        /// <summary>
        /// 获取活动讨论列表
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageCount"></param>
        public static List<ActivityReplyEntity> GetActivityReplys(string activityID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ActivityReplyEntity> list = new List<ActivityReplyEntity>();
            string whereSql = " status<>9 and activityID='"+activityID+"' ";
            DataTable dt = CommonBusiness.GetPagerData("ActivityReply", "*", whereSql, "AutoID", "CreateTime desc ", pageSize, pageIndex, out totalCount, out pageCount, false);
            
            foreach (DataRow dr in dt.Rows)
            {
                ActivityReplyEntity model = new ActivityReplyEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                if (!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }

            return list;

        }
        #endregion

        #region 添加

        /// <summary>
        /// 新建活动
        /// </summary>
        /// <param name="name">活动名称</param>
        /// <param name="poster">宣传海报</param>
        /// <param name="begintime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <param name="address">地址</param>
        /// <param name="ownerid">负责人（联系人）</param>
        /// <param name="remark">描述</param>
        /// <param name="userid">创建人</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public static string CreateActivity(string name, string poster, string begintime, string endtime, string address, string ownerid, string memberid, string remark, string userid, string agentid, string clientid)
        {
            string activityid = Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(poster))
            {
                if (poster.IndexOf("?") > 0)
                {
                    poster = poster.Substring(0, poster.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(poster));
                poster = FilePath + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(poster));
                }
            }
            bool bl = ActivityDAL.BaseProvider.CreateActivity(activityid, name, poster, begintime, endtime, address, ownerid,memberid, remark, userid, agentid, clientid);
            if (!bl)
            {
                return "";
            }
            else
            {
                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Activity, EnumLogType.Create, "", userid, agentid, clientid);
            }
            return activityid;
        }

        /// <summary>
        /// 添加活动讨论
        /// </summary>
        /// <param name="msg">内容</param>
        /// <param name="fromReplyID">来源讨论ID</param>
        /// <param name="userID">创建者ID</param>
        /// <param name="agentID">创建者代理商ID</param>
        /// <param name="fromReplyUserID">来源讨论创建人ID</param>
        /// <param name="fromReplyAgentID">来源讨论创建人代理商ID</param>
        public static string CreateActivityReply(string activityID, string msg, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID) 
        {
            return ActivityDAL.BaseProvider.CreateActivityReply(activityID, msg, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);
        }
        #endregion

        #region 编辑/删除

        /// <summary>
        /// 编辑活动信息
        /// </summary>
        /// <param name="name">活动名称</param>
        /// <param name="poster">宣传海报</param>
        /// <param name="begintime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <param name="address">地址</param>
        /// <param name="remark">描述</param>
        /// <param name="userid">操作人</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientid">客户端ID</param>
        public static bool UpdateActivity(string activityid, string name, string poster, string begintime, string endtime, string address, string remark, string ownerid, string memberid, string userid, string agentid, string clientid)
        {
            if (!string.IsNullOrEmpty(poster) && poster.IndexOf(TempPath) >= 0)
            {
                if (poster.IndexOf("?") > 0)
                {
                    poster = poster.Substring(0, poster.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(poster));
                poster = FilePath + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(poster));
                }
            }
            bool bl = ActivityDAL.BaseProvider.UpdateActivity(activityid, name, poster, begintime, endtime, address, remark,ownerid,memberid);
            return bl;   
        }

        public bool DeleteActivity(string activityid){
            bool bl = ActivityDAL.BaseProvider.DeleteActivity(activityid);
            return bl;   
        }

        /// <summary>
        /// 删除活动讨论
        /// </summary>
        /// <param name="replyID"></param>
        public static bool DeleteActivityReply(string replyID)
        {
            bool bl = ActivityDAL.BaseProvider.DeleteActivityReply(replyID);

            return bl;
        }

        #endregion
    }
}
