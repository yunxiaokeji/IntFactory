using IntFactoryBusiness;
using IntFactoryEntity;
using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace YXERP.Controllers
{

    public class ActivityController : BaseController
    {
        #region view
        public ActionResult Index()
        {
            return Redirect("/Activity/MyActivity");
        }

        public ActionResult MyActivity()
        {
            ViewBag.Option = 0;
            return View();
        }

        public ActionResult Activitys()
        {
            ViewBag.Option = 1;
            return View("MyActivity");
        }

        public ActionResult Create()
        {
            ViewBag.ActivityID = "";
            return View("Operate");
        }

        public ActionResult Operate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Activity/MyActivity");
            }
            ViewBag.ActivityID = id;
            return View();
        }

        public ActionResult Detail(string id)
        {
            ViewBag.ActivityID = id ?? string.Empty;
            ViewBag.MDUserID = CurrentUser.MDToken;
            return View();
        }
        #endregion

        #region ajax

        #region 活动
        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetActivityList(string keyWords, int pageSize, int pageIndex, int isAll, string beginTime, string endTime, int stage, int filterType, string userID)
        {
            int pageCount = 0;
            int totalCount = 0;
            string ownerID=CurrentUser.UserID;
            if (isAll == 1)
            {
                if (!string.IsNullOrEmpty(userID))
                    ownerID = userID;
                else
                    ownerID = string.Empty;

            }

            List<ActivityEntity> list = ActivityBusiness.GetActivitys(ownerID, (EnumActivityStage)stage, filterType, keyWords, beginTime, endTime, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取活动详细信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetActivityDetail(string activityID)
        {
            ActivityEntity model = ActivityBusiness.GetActivityByID(activityID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("Item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        public JsonResult GetUserDetail(string id)
        {
            Users model = null;
            if(!string.IsNullOrEmpty(id))
             model = OrganizationBusiness.GetUserByUserID(id, CurrentUser.AgentID);
            else
                model = OrganizationBusiness.GetUserByUserID(CurrentUser.UserID, CurrentUser.AgentID);
            JsonDictionary.Add("Item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取活动对应的客户列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomersByActivityID(string activityID,int pageSize,int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;

            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomersByActivityID(activityID, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存活动
        /// </summary>
        public JsonResult SavaActivity(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivityEntity model = serializer.Deserialize<ActivityEntity>(entity);

            string activityID = "";
            model.OwnerID = model.OwnerID.Trim('|');
            model.MemberID = model.MemberID.Trim('|');
            //新增
            if (string.IsNullOrEmpty(model.ActivityID))
            {
                activityID =ActivityBusiness.CreateActivity(model.Name, model.Poster, model.BeginTime.ToString(), model.EndTime.ToString(), model.Address, model.OwnerID,model.MemberID, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }//编辑
            else
            {
                bool bl =ActivityBusiness.UpdateActivity(model.ActivityID, model.Name,model.Poster, model.BeginTime.ToString(), model.EndTime.ToString(), model.Address, model.Remark,model.OwnerID,model.MemberID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);

                if (bl)
                {
                    activityID = model.ActivityID;
                }
            }

            JsonDictionary.Add("ID", activityID);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        
        /// <summary>
        /// 删除活动
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteActivity(string activityID)
        {
            bool bl = new ActivityBusiness().DeleteActivity(activityID);
            JsonDictionary.Add("Result", bl?1:0);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region 活动讨论
        /// <summary>
        /// 保存活动讨论
        /// </summary>
        public JsonResult SavaActivityReply(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivityReplyEntity model = serializer.Deserialize<ActivityReplyEntity>(entity);

            string replyID =string.Empty;
            replyID = ActivityBusiness.CreateActivityReply(model.ActivityID, model.Msg, CurrentUser.UserID, CurrentUser.AgentID, model.FromReplyID, model.FromReplyUserID, model.FromReplyAgentID);

            List<ActivityReplyEntity> list = new List<ActivityReplyEntity>();
            if (!string.IsNullOrEmpty(replyID))
            {
                model.ReplyID = replyID;
                model.CreateTime = DateTime.Now;
                model.CreateUser = CurrentUser;
                model.CreateUserID = CurrentUser.UserID;
                model.AgentID = CurrentUser.AgentID;
                if(!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }
            JsonDictionary.Add("Items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取活动讨论列表
        /// </summary>
        public JsonResult GetActivityReplys(string activityID, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;

            List<ActivityReplyEntity> list = ActivityBusiness.GetActivityReplys(activityID, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除活动讨论
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteActivityReply(string activityID)
        {
            bool bl =ActivityBusiness.DeleteActivityReply(activityID);

            JsonDictionary.Add("Result", bl ? 1 : 0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        #endregion

        #endregion

    }
}
