﻿using IntFactoryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryBusiness;
using System.Web.Script.Serialization;
using IntFactoryEntity;
using YXERP.Models;
using System.IO;

namespace YXERP.Controllers
{
    public class CustomerController : BaseController
    {
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyCustomer()
        {
            ViewBag.Title = "我的客户";
            ViewBag.Type = (int)EnumSearchType.Myself;
            ViewBag.FirstNames=new char[]{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, 1).ToList();
            //ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View("Customers");
        }

        public ActionResult BranchCustomer()
        {
            ViewBag.Title = "下属客户";
            ViewBag.Type = (int)EnumSearchType.Branch;
            //ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID).ToList();
            return View("Customers");
        }

        public ActionResult Customers()
        {
            ViewBag.Title = "客户列表";
            ViewBag.Type = (int)EnumSearchType.All;
            ViewBag.FirstNames = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, 1).ToList();
            //ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View("Customers");
        }

        public ActionResult Create(string id)
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    ViewBag.Sources = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID).Where(m => m.IsChoose == 1).ToList();
            //}
            //else
            //{
            //    ViewBag.Sources = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID);
            //}
            //ViewBag.ActivityID = id;
            //ViewBag.Industrys = IntFactoryBusiness.Manage.IndustryBusiness.GetIndustrys();
            //ViewBag.Extents = CustomBusiness.GetExtents();
            return View();
        }

        public ActionResult Detail(string id, string nav = "")
        {
            ViewBag.Nav = nav;
            ViewBag.ID = id;            
            return View();
        }

        #region Ajax

        public JsonResult GetCustomerSources()
        {
            var list = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveCustomer(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomerEntity model = serializer.Deserialize<CustomerEntity>(entity);

            if (string.IsNullOrEmpty(model.CustomerID))
            {
                model.CustomerID = new CustomBusiness().CreateCustomer(model.Name, model.Type, model.SourceID, model.ActivityID, model.IndustryID, model.Extent, model.CityCode,
                                                                       model.Address, model.ContactName, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new CustomBusiness().UpdateCustomer(model.CustomerID, model.Name, model.Type, model.IndustryID, model.Extent, model.CityCode, model.Address, model.MobilePhone, model.OfficePhone,
                                                model.Email, model.Jobs, model.Description, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.CustomerID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomers(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterCustomer model = serializer.Deserialize<FilterCustomer>(filter);
            int totalCount = 0;
            int pageCount = 0;
            
            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomers(model.SearchType, model.Type, model.SourceType, 
                model.SourceID, model.StageID, model.Status, model.Mark, model.ActivityID, model.UserID, 
                model.TeamID, model.AgentID, model.BeginTime, model.EndTime,
                model.FirstName, model.Keywords, model.OrderBy, model.PageSize, model.PageIndex, 
                ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomersByKeywords(string keywords, int isAll = 0)
        {

            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomersByKeywords(keywords, isAll == 0 ? CurrentUser.UserID : "", CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomerByID(string customerid)
        {
            var model = CustomBusiness.BaseBusiness.GetCustomerByID(customerid, CurrentUser.AgentID, CurrentUser.ClientID);
            //model.Industrys = IntFactoryBusiness.Manage.IndustryBusiness.GetIndustrys();
            //model.Extents = CustomBusiness.GetExtents();
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetActivityBaseInfoByID(string activityid)
        {
            var model = ActivityBusiness.GetActivityBaseInfoByID(activityid, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateCustomMark(string ids, int mark)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerMark(id, mark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
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

        public JsonResult UpdateCustomOwner(string ids, string userid)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerOwner(id, userid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
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

        public JsonResult UpdateCustomStage(string ids, string stageid)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStage(id, stageid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
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

        public JsonResult GetStageItems(string stageid)
        {
            var list = new SystemBusiness().GetCustomStageByID(stageid, CurrentUser.AgentID, CurrentUser.ClientID).StageItem;
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult LoseCustomer(string ids)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStatus(id, EnumCustomStatus.Loses, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
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

        public JsonResult CloseCustomer(string ids)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStatus(id, EnumCustomStatus.Close, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
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

        public JsonResult RecoveryCustomer(string ids)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStatus(id, EnumCustomStatus.Normal, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
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

        public JsonResult GetCustomerLogs(string customerid, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = LogBusiness.GetLogs(customerid, EnumLogObjectType.Customer, 10, pageindex, ref totalCount, ref pageCount, CurrentUser.AgentID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetClientByKeywords(string keywords)
        {
            int totalCount = 0, pageCount = 0;
            var list = IntFactoryBusiness.Manage.ClientBusiness.GetClients(keywords,-1,"", 20, 1, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #region 联系人

        public JsonResult GetContacts(string customerid)
        {
            var list = CustomBusiness.BaseBusiness.GetContactsByCustomerID(customerid, CurrentUser.AgentID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveContact(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ContactEntity model = serializer.Deserialize<ContactEntity>(entity);

            if (string.IsNullOrEmpty(model.ContactID))
            {
                model.ContactID = new CustomBusiness().CreateContact(model.CustomerID, model.Name, model.CityCode, model.Address, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new CustomBusiness().UpdateContact(model.ContactID, model.CustomerID, model.Name, model.CityCode, model.Address, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.ContactID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetContactByID(string id)
        {
            var model = CustomBusiness.BaseBusiness.GetContactByID(id);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteContact(string id)
        {
            bool bl = CustomBusiness.BaseBusiness.DeleteContact(id, OperateIP, CurrentUser.UserID, CurrentUser.AgentID);
            JsonDictionary.Add("status", bl );
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 讨论

        public JsonResult SavaReply(string entity, string customerID, string attchmentEntity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ReplyEntity model = serializer.Deserialize<ReplyEntity>(entity);
            model.Attachments = serializer.Deserialize<List<Attachment>>(attchmentEntity);
            
            string replyID = "";
            replyID = CustomBusiness.CreateReply(model.GUID, model.Content, CurrentUser.UserID, CurrentUser.AgentID, model.FromReplyID, model.FromReplyUserID, model.FromReplyAgentID);

            //string movePath = CloudSalesTool.AppSettings.Settings["UploadFilePath"] + "Customers/" + DateTime.Now.ToString("yyyyMM") + "/";
            //string uploadTempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];
            //DirectoryInfo directory = new DirectoryInfo(Server.MapPath(movePath));
            //if (!directory.Exists)
            //{
            //    directory.Create();
            //}

            //foreach (var attachments in model.Attachments)
            //{
            //    attachments.ServerUrl = "http://o9h6bx3r4.bkt.clouddn.com/";
            //}


            CustomBusiness.AddCustomerReplyAttachments(customerID, replyID, model.Attachments, CurrentUser.UserID, CurrentUser.ClientID);

            List<ReplyEntity> list = new List<ReplyEntity>();
            if (!string.IsNullOrEmpty(replyID))
            {

                model.ReplyID = replyID;
                model.CreateTime = DateTime.Now;
                model.CreateUser = CurrentUser;
                model.CreateUserID = CurrentUser.UserID;
                model.AgentID = CurrentUser.AgentID;
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

        public JsonResult GetReplys(string guid, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;
            var list = CustomBusiness.GetCustomerReplys(guid, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteReply(string replyid)
        {
            bool bl = CustomBusiness.BaseBusiness.DeleteReply(replyid);
            JsonDictionary.Add("status", bl ? 1 : 0);
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
