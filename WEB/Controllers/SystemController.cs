using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using IntFactoryBusiness;
using IntFactoryEntity;
using IntFactoryBusiness.Manage;
using IntFactoryEntity.Manage;
using IntFactoryEnum;
namespace YXERP.Controllers
{
    public class SystemController : BaseController
    {
        //
        // GET: /System/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LabelSet()
        {
            return View();
        }

        public ActionResult Sources()
        {
            return View();
        }

        public ActionResult OrderProcess()
        {
            ViewBag.Categorys = SystemBusiness.BaseBusiness.GetProcessCategorys();
            return View();
        }

        public ActionResult OrderStages(string id)
        {
            var model = new SystemBusiness().GetOrderProcessByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (string.IsNullOrEmpty(model.ProcessID))
            {
                return Redirect("/System/OrderProcess");
            }
            ViewBag.Model = model;
            ViewBag.Items = new SystemBusiness().GetOrderStages(id, CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }

        public ActionResult Teams()
        {
            return View();
        }

        public ActionResult OrderCategory()
        {
            var list = new ProductsBusiness().GetChildCategorysByID("", IntFactoryEnum.EnumCategoryType.Order);
            foreach (var item in list)
            {
                if (item.ChildCategory == null || item.ChildCategory.Count == 0)
                {
                    item.ChildCategory = new ProductsBusiness().GetChildCategorysByID(item.CategoryID, IntFactoryEnum.EnumCategoryType.Order);
                }
            }
            ViewBag.Items = list;

            return View();
        }

        public ActionResult Target()
        {
            return View();
        }

        public ActionResult Warehouse()
        {
            return View();
        }

        public ActionResult DepotSeat(string id = "")
        {
            if (string.IsNullOrEmpty(id)) 
            {
                return Redirect("/System/Warehouse");
            }
            ViewBag.Ware = new SystemBusiness().GetWareByID(id, CurrentUser.ClientID);
            return View();
        }

        public ActionResult Client(string id)
        {
            ViewBag.Industry =IntFactoryBusiness.Manage.IndustryBusiness.GetIndustrys();
            if (string.IsNullOrEmpty(id))
                ViewBag.Option = 0;
            else
                ViewBag.Option = id;

            return View();
        }

        #region Ajax

        #region 客户颜色标记

        /// <summary>
        /// 客户颜色标记
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLableColor(int lableType)
        {
            var list = SystemBusiness.BaseBusiness.GetLableColor(CurrentUser.ClientID, (EnumMarkType)lableType);
            
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetLableColorByColorID(int colorid, int lableType)
        {
            var model = new SystemBusiness().GetLableColorColorID(CurrentUser.ClientID, colorid, (EnumMarkType)lableType);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveLableColor(string lablecolor, int lableType)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();                        
            LableColorEntity model = serializer.Deserialize<LableColorEntity>(lablecolor);
            model.CreateUserID = CurrentUser.UserID;
            model.ClientID = CurrentUser.ClientID;
            model.AgentID = CurrentUser.AgentID;
            model.Status = 0;
            int ColorID = -1;
            if (model.ColorID == 0)
            {
                ColorID = SystemBusiness.BaseBusiness.CreateLableColor(model.ColorName, model.ColorValue,
                    model.AgentID, model.ClientID, model.CreateUserID, model.Status, (EnumMarkType)lableType);
            }
            else
            {
                int bl = SystemBusiness.BaseBusiness.UpdateLableColor(model.AgentID, model.ClientID, model.ColorID,
                    model.ColorName, model.ColorValue, CurrentUser.UserID, (EnumMarkType)lableType);
                ColorID = bl > 0 ? model.ColorID : bl;

            }
            JsonDictionary.Add("ID", ColorID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteColor(int colorid, int lableType)
        {
            int result = SystemBusiness.BaseBusiness.DeleteLableColor(9, colorid, CurrentUser.AgentID, CurrentUser.ClientID, CurrentUser.UserID, (EnumMarkType)lableType);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 订单阶段

        public JsonResult GetOrderProcess(int type = -1)
        {
            var list = new SystemBusiness().GetOrderProcess(CurrentUser.AgentID, CurrentUser.ClientID).ToList();
            if (type > 0)
            {
                JsonDictionary.Add("items", list.Where(m => m.ProcessType == type).ToList());
            }
            else
            {
                JsonDictionary.Add("items", list);
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderProcessByID(string id)
        {
            var model = new SystemBusiness().GetOrderProcessByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveOrderProcess(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderProcessEntity model = serializer.Deserialize<OrderProcessEntity>(entity);

            if (string.IsNullOrEmpty(model.ProcessID))
            {
                model.ProcessID = new SystemBusiness().CreateOrderProcess(model.ProcessName, model.ProcessType, model.CategoryID, model.PlanDays, model.IsDefault, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateOrderProcess(model.ProcessID, model.ProcessName, model.PlanDays, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.ProcessID = "";
                }

            }
            model.Owner = OrganizationBusiness.GetUserByUserID(CurrentUser.UserID, CurrentUser.AgentID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOrderProcess(string id)
        {
            int result = 0;
            bool bl = new SystemBusiness().DeleteOrderProcess(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, ref result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderProcessOwner(string id, string userid)
        {
            bool bl = new SystemBusiness().UpdateOrderProcessOwner(id, userid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderProcessDefault(string id)
        {
            bool bl = new SystemBusiness().UpdateOrderProcessDefault(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveOrderStage(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderStageEntity model = serializer.Deserialize<OrderStageEntity>(entity);

            int result = 0;

            if (string.IsNullOrEmpty(model.StageID))
            {
                model.StageID = new SystemBusiness().CreateOrderStage(model.StageName, model.Sort, model.Mark, model.MaxHours, "", model.ProcessID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, out result);

                model.Owner = OrganizationBusiness.GetUserByUserID(CurrentUser.UserID, CurrentUser.AgentID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateOrderStage(model.StageID, model.StageName, model.Mark, model.MaxHours, model.ProcessID, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (bl)
                {
                    result = 1;
                }
            }
            
            JsonDictionary.Add("status", result);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOrderStage(string id, string processid)
        {
            bool bl = new SystemBusiness().DeleteOrderStage(id, processid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderStageOwner(string id, string processid, string userid)
        {
            bool bl = new SystemBusiness().UpdateOrderStageOwner(id, processid, userid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 销售团队

        public JsonResult GetTeams()
        {

            var list = new SystemBusiness().GetTeams(CurrentUser.AgentID).ToList();
            
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveTeam(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TeamEntity model = serializer.Deserialize<TeamEntity>(entity);

            if (string.IsNullOrEmpty(model.TeamID))
            {
                model.TeamID = new SystemBusiness().CreateTeam(model.TeamName, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateTeam(model.TeamID, model.TeamName, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.TeamID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteTeam(string id)
        {
            bool bl = new SystemBusiness().DeleteTeam(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateUserTeamID(string ids, string teamid)
        {
            bool bl = false;//
            string[] list = ids.Split(',');
            foreach (var userid in list)
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    if (new SystemBusiness().UpdateUserTeamID(userid, teamid, CurrentUser.AgentID, CurrentUser.UserID, OperateIP, CurrentUser.ClientID))
                    {
                        bl = true;
                    }
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        #endregion

        #region 仓库货位

        public JsonResult GetAllWareHouses()
        {
            List<WareHouse> list = new SystemBusiness().GetWareHouses(CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetWareHouses(string keyWords, int pageSize, int pageIndex, int totalCount)
        {
            List<WareHouse> list = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetWareHouseByID(string id)
        {
            WareHouse model = new SystemBusiness().GetWareByID(id, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveWareHouse(string ware)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WareHouse model = serializer.Deserialize<WareHouse>(ware);

            string id = string.Empty;
            if (string.IsNullOrEmpty(model.WareID))
            {
                id = new SystemBusiness().AddWareHouse(model.WareCode, model.Name, model.ShortName, model.CityCode, model.Status, model.DepotCode, model.DepotName, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else if (new SystemBusiness().UpdateWareHouse(model.WareID, model.WareCode, model.Name, model.ShortName, model.CityCode, model.Status, model.Description, CurrentUser.UserID, CurrentUser.ClientID))
            {
                id = model.WareID;
            }


            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateWareHouseStatus(string id, int status)
        {
            bool bl = new SystemBusiness().UpdateWareHouseStatus(id, (IntFactoryEnum.EnumStatus)status, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteWareHouse(string id)
        {
            bool bl = new SystemBusiness().UpdateWareHouseStatus(id, IntFactoryEnum.EnumStatus.Delete, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveDepotSeat(string obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            DepotSeat model = serializer.Deserialize<DepotSeat>(obj);

            string id = string.Empty;
            if (string.IsNullOrEmpty(model.DepotID))
            {
                id = new SystemBusiness().AddDepotSeat(model.DepotCode, model.WareID, model.Name, model.Status, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else if (new SystemBusiness().UpdateDepotSeat(model.DepotID, model.WareID, model.DepotCode, model.Name, model.Status, model.Description, CurrentUser.UserID, CurrentUser.ClientID))
            {
                id = model.WareID;
            }


            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetDepotSeats(string wareid, string keyWords, int pageSize, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<DepotSeat> list = new SystemBusiness().GetDepotSeats(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID, wareid);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetDepotSeatsByWareID(string wareid)
        {
            List<DepotSeat> list = new SystemBusiness().GetDepotSeatsByWareID(wareid, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetDepotByID(string id, string wareid)
        {
            var model = new SystemBusiness().GetDepotByID(id, wareid, CurrentUser.ClientID);
            JsonDictionary.Add("Item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteDepotSeat(string id, string wareid)
        {
            bool bl = new SystemBusiness().DeleteDepotSeat(id, wareid, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateDepotSeatStatus(string id, string wareid, int status)
        {
            bool bl = new SystemBusiness().UpdateDepotSeatStatus(id, wareid, (IntFactoryEnum.EnumStatus)status, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateDepotSeatSort(string depotid, string wareid, int type)
        {
            bool bl = new SystemBusiness().UpdateDepotSeatSort(depotid, wareid, type, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 公司信息
        /// <summary>
        /// 获取客户详情
        /// </summary>
        public JsonResult GetClientDetail()
        {
            var client = ClientBusiness.GetClientDetail(CurrentUser.ClientID);
            var agent = AgentsBusiness.GetAgentDetail(CurrentUser.AgentID);

            JsonDictionary.Add("Client", client);
            JsonDictionary.Add("Agent", agent);
            JsonDictionary.Add("Days", Math.Ceiling((agent.EndTime - DateTime.Now).TotalDays));
            
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取公司订单列表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="type"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult GetClientOrders(int status,int type,string beginDate,string endDate,int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;

            List<ClientOrder> list = ClientOrderBusiness.GetClientOrders(status, type,beginDate,endDate,CurrentUser.AgentID,CurrentUser.ClientID, pageSize, pageIndex, ref totalCount, ref pageCount);
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
        /// 保存公司基本信息
        /// </summary>
        public JsonResult SaveClient(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Clients model = serializer.Deserialize<Clients>(entity);
            model.ClientID = CurrentUser.ClientID;

            bool flag = ClientBusiness.UpdateClient(model, CurrentUser.UserID);
            JsonDictionary.Add("Result", flag ? 1 : 0);

            if (flag) {
                CurrentUser.Client = model;
                Session["ClientManager"] = CurrentUser;
            }
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 关闭公司订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult CloseClientOrder(string id)
        {
            bool flag = ClientOrderBusiness.UpdateClientOrderStatus(id, IntFactoryEnum.EnumClientOrderStatus.Delete);
            JsonDictionary.Add("Result", flag ? 1 : 0);


            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取公司密钥
        /// </summary>
        public JsonResult GetAgentKey()
        {
            var agent = AgentsBusiness.GetAgentDetail(CurrentUser.AgentID);
            string key = string.Empty;

            if (string.IsNullOrEmpty(agent.AgentKey))
            {
                key = DateTime.Now.Ticks.ToString();
                AgentsBusiness.UpdateAgentKey(CurrentUser.AgentID, key);
            }
            else
                key = agent.AgentKey;

            JsonDictionary.Add("Key",key);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region 订单品类

        public JsonResult UpdateOrderCategory(string categoryid, string pid, int status)
        {
            bool bl = new SystemBusiness().UpdateOrderCategory(categoryid, pid, status, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetClientOrderCategorys()
        {
            var list = new ProductsBusiness().GetChildCategorysByID("", IntFactoryEnum.EnumCategoryType.Order).Where(m => m.Status == 1).ToList();
            foreach (var item in list)
            {
                if (item.ChildCategory == null || item.ChildCategory.Count == 0)
                {
                    item.ChildCategory = new ProductsBusiness().GetChildCategorysByID(item.CategoryID, IntFactoryEnum.EnumCategoryType.Order).Where(m => m.Status == 1).ToList();
                }
            }
            JsonDictionary.Add("items", list);
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
