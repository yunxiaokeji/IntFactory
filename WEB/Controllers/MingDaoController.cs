using IntFactoryBusiness;
using IntFactoryEntity;
using CloudSalesTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace YXERP.Controllers
{
    public class MingDaoController :BaseController
    {
        #region view
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region ajax
        /// <summary>
        /// 获取当前明道用户所属群组列表
        /// </summary>
        public JsonResult GetUserGroups() 
        {
            //AlibabaSdk.GrouptList list = AlibabaSdk.GroupBusiness.GetMyJoined(CurrentUser.MDToken);
            
            //JsonDictionary.Add("Items", list.groups);
            //JsonDictionary.Add("Result", list.error_code==0?1:0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 分享到明道动态
        /// </summary>
        public JsonResult SharePost(FormCollection paras)
        {
            string msg=paras["msg"];
            string gIDs=paras["gIDs"];
            
            int postShareType=3;
            int error_code=0;

            if (gIDs.Contains("everyone"))
            {
                gIDs = gIDs.Replace("everyone,", "");
                gIDs = gIDs.Trim(',');

                if (gIDs.Split(',').Length > 1)
                    postShareType = 2;
                else
                    postShareType = 0;
            }
            else
            {
                if (!string.IsNullOrEmpty(gIDs))
                {
                    gIDs = gIDs.Trim(',');
                    postShareType = 1;
                }
            }
            

            //AlibabaSdk.PostBusiness.Update(CurrentUser.MDToken, msg, gIDs, (AlibabaSdk.PostShareType)postShareType, AlibabaSdk.PostType.Normal, out error_code);

            JsonDictionary.Add("Result", error_code == 0 ? 1 : 0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 分享到明道任务
        /// </summary>
        public JsonResult ShareTask(FormCollection paras)
        {
            string name=paras["name"];

            string ownerUserID = paras["ownerUserID"];
            ownerUserID = ownerUserID.Trim('|');
            string ownerID = "";//OrganizationBusiness.GetUserByUserID(ownerUserID, CurrentUser.AgentID).MDUserID;

            string memberIDs = paras["memberIDs"];
            memberIDs = memberIDs.Trim('|');
            List<string> members = new List<string>();
            foreach (var m in memberIDs.Split('|'))
            {
                //members.Add(OrganizationBusiness.GetUserByUserID(m, CurrentUser.AgentID).MDUserID);
            }

            string des = paras["des"];
            string url = paras["url"];
            if (!string.IsNullOrEmpty(url))
            {
                des += Request.Url.Authority + url;
            }
            string endDate = paras["endDate"];
            int error_code;
            //string des = "点击前往：<a href='" + Request.Url.Authority + "/Activity/Detail/" + model.ActivityID + "' target='_blank'>活动详情</a>";
            //string id = AlibabaSdk.TaskBusiness.AddTask(CurrentUser.MDToken, name, ownerID, members, endDate, string.Empty, des, out error_code);

            //JsonDictionary.Add("Result", !string.IsNullOrEmpty(id) ? 1 : 0);
            //JsonDictionary.Add("Url", "https://www.mingdao.com/Apps/task/task_" + id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 分享到明道日程
        /// </summary>
        public JsonResult ShareCalendar(FormCollection paras)
        {
            string name = paras["name"];

            string memberIDs = paras["memberIDs"];
            memberIDs = memberIDs.Trim('|');
            List<string> members = new List<string>();
            foreach (var m in memberIDs.Split('|'))
            {
                //members.Add(OrganizationBusiness.GetUserByUserID(m, CurrentUser.AgentID).MDUserID);
            }

            string des = paras["des"];
            string address = paras["address"];
            string startDate = paras["startDate"];
            string endDate = paras["endDate"];
            string url = paras["url"];
            int error_code;

            if (!string.IsNullOrEmpty(url))
            {
                des += Request.Url.Authority + url;
            }

            //string des = "分享来源地址：" + Request.Url.Authority + "/Activity/Detail/" + model.ActivityID;
            //string id = AlibabaSdk.CalendarBusiness.AddCalendar(CurrentUser.MDToken, name, members, des, address, startDate, endDate, out error_code);

            //JsonDictionary.Add("Result", !string.IsNullOrEmpty(id) ? 1 : 0);
            //JsonDictionary.Add("Url", "https://www.mingdao.com/apps/calendar/detail_" + id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
