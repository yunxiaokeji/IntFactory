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
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Customer).ToList();
            //ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View("Customers");
        }

        public ActionResult BranchCustomer()
        {
            ViewBag.Title = "下属客户";
            ViewBag.Type = (int)EnumSearchType.Branch;
            //ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Customer).ToList();
            return View("Customers");
        }

        public ActionResult Customers()
        {
            ViewBag.Title = "所有客户";
            ViewBag.Type = (int)EnumSearchType.All;
            ViewBag.FirstNames = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            ViewBag.list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, EnumMarkType.Customer).ToList();
            //ViewBag.Stages = SystemBusiness.BaseBusiness.GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View("Customers");
        }

        public ActionResult Create(string id)
        {
            return View();
        }

        public ActionResult Detail(string id, string nav = "")
        {
            ViewBag.Nav = nav;
            ViewBag.ID = id;            
            return View();
        }

        #region Ajax

        public JsonResult SaveCustomer(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomerEntity model = serializer.Deserialize<CustomerEntity>(entity);

            if (string.IsNullOrEmpty(model.CustomerID))
            {
                model.CustomerID = new CustomBusiness().CreateCustomer(model.Name, model.Type, model.SourceID, model.IndustryID, model.Extent, model.CityCode,
                                                                       model.Address, model.ContactName, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new CustomBusiness().UpdateCustomer(model.CustomerID, model.Name, model.Type, model.IndustryID, model.Extent, model.CityCode, model.Address, model.MobilePhone, model.OfficePhone,
                                                model.Email, model.Jobs, model.Description, CurrentUser.UserID, OperateIP,CurrentUser.ClientID);
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
                model.SourceID, model.StageID, model.Status, model.Mark, model.UserID, 
                model.TeamID, model.BeginTime, model.EndTime,
                model.FirstName, model.Keywords, model.OrderBy, model.PageSize, model.PageIndex, 
                ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.ClientID);

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

            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomersByKeywords(keywords, isAll == 0 ? CurrentUser.UserID : "",  CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomerByID(string customerid)
        {
            var model = CustomBusiness.BaseBusiness.GetCustomerByID(customerid, CurrentUser.ClientID);
            //model.Industrys = IntFactoryBusiness.Manage.IndustryBusiness.GetIndustrys();
            //model.Extents = CustomBusiness.GetExtents();
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
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerMark(id, mark, CurrentUser.UserID, OperateIP, CurrentUser.ClientID))
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
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerOwner(id, userid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID))
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

        public JsonResult LoseCustomer(string ids)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStatus(id, EnumCustomStatus.Loses, CurrentUser.UserID, OperateIP, CurrentUser.ClientID))
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
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStatus(id, EnumCustomStatus.Close, CurrentUser.UserID, OperateIP, CurrentUser.ClientID))
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
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStatus(id, EnumCustomStatus.Normal, CurrentUser.UserID, OperateIP, CurrentUser.ClientID))
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

        public JsonResult GetClientByKeywords(string keywords)
        {
            int totalCount = 0, pageCount = 0;
            var list = IntFactoryBusiness.Manage.ClientBusiness.GetClients(keywords, EnumRegisterType.All, "", 20, 1, ref totalCount, ref pageCount);
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
            var list = CustomBusiness.BaseBusiness.GetContactsByCustomerID(customerid, CurrentUser.ClientID);
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
                model.ContactID = new CustomBusiness().CreateContact(model.CustomerID, model.Name, model.CityCode, model.Address, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new CustomBusiness().UpdateContact(model.ContactID, model.CustomerID, model.Name, model.CityCode, model.Address, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
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
            bool bl = CustomBusiness.BaseBusiness.DeleteContact(id, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("status", bl );
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
