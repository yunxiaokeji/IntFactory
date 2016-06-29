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
using IntFactoryEntity.Custom;
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

        public ActionResult MarkColor()
        {
            return View();
        }

        public ActionResult Sources()
        {
            return View();
        }

        public ActionResult Stages()
        {
            ViewBag.Items = new SystemBusiness().GetCustomStages(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }
        public ActionResult OrderProcess()
        {
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

        public ActionResult OrderType()
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

        #region 客户来源

        /// <summary>
        /// 获取客户来源列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomSources()
        {

            var list = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID).Where(m => m.Status == 1).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取客户来源实体
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomSourceByID(string id)
        {

            var model = new SystemBusiness().GetCustomSourcesByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存客户来源
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveCustomSource(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomSourceEntity model = serializer.Deserialize<CustomSourceEntity>(entity);

            int result = 0;

            if (string.IsNullOrEmpty(model.SourceID))
            {
                model.SourceID = new SystemBusiness().CreateCustomSource(model.SourceCode, model.SourceName, model.IsChoose, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateCustomSource(model.SourceID, model.SourceName, model.IsChoose, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
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

        /// <summary>
        /// 删除客户来源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteCustomSource(string id)
        {
            bool bl = new SystemBusiness().DeleteCustomSource(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 客户颜色标记
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomColor(string tableName)
        {
            var list = SystemBusiness.BaseBusiness.GetCustomerColors(tableName,CurrentUser.ClientID).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomerColorByColorID(string tableName,int colorid)
        {
            var model = new SystemBusiness().GetCustomerColorsColorID(tableName,CurrentUser.ClientID, colorid);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveCustomerColor(string tableName, string customercolor)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomerColorEntity model = serializer.Deserialize<CustomerColorEntity>(customercolor);
            model.CreateUserID = CurrentUser.UserID;
            model.ClientID = CurrentUser.ClientID;
            model.AgentID = CurrentUser.AgentID;
            model.Status = 0;
            int ColorID = -1;
            if (model.ColorID == 0)
            {
                ColorID = SystemBusiness.BaseBusiness.CreateCustomerColor(tableName,model.ColorName, model.ColorValue,
                    model.AgentID, model.ClientID, model.CreateUserID, model.Status);
            }
            else
            {
                int bl = SystemBusiness.BaseBusiness.UpdateCustomerColor(tableName,model.AgentID, model.ClientID, model.ColorID,
                    model.ColorName, model.ColorValue, CurrentUser.UserID);
                ColorID = bl > 0 ? model.ColorID : bl;

            }
            JsonDictionary.Add("ID", ColorID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteColor(string tableName,int colorid)
        {
            int result = SystemBusiness.BaseBusiness.DeleteCutomerColor(tableName,9, colorid, CurrentUser.AgentID, CurrentUser.ClientID,
                CurrentUser.UserID);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 客户阶段配置

        public JsonResult SaveCustomStage(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomStageEntity model = serializer.Deserialize<CustomStageEntity>(entity);

            int result = 0;

            if (string.IsNullOrEmpty(model.StageID))
            {
                model.StageID = new SystemBusiness().CreateCustomStage(model.StageName, model.Sort, "", CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateCustomStage(model.StageID, model.StageName, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
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

        public JsonResult SaveStageItem(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StageItemEntity model = serializer.Deserialize<StageItemEntity>(entity);

            if (string.IsNullOrEmpty(model.ItemID))
            {
                model.ItemID = new SystemBusiness().CreateStageItem(model.ItemName, model.StageID, model.ProcessID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateStageItem(model.ItemID, model.ItemName, model.StageID, model.ProcessID, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.ItemID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteCustomStage(string id)
        {
            bool bl = new SystemBusiness().DeleteCustomStage(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteStageItem(string id, string stageid,string processid)
        {
            bool bl = new SystemBusiness().DeleteStageItem(id, stageid, processid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
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
                model.ProcessID = new SystemBusiness().CreateOrderProcess(model.ProcessName, model.ProcessType, model.CategoryType, model.PlanDays, model.IsDefault, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
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

        #region 订单类型

        public JsonResult GetOrderTypes()
        {

            var list = new SystemBusiness().GetOrderTypes(CurrentUser.AgentID, CurrentUser.ClientID).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderTypeByID(string id)
        {

            var model = new SystemBusiness().GetOrderTypeByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveOrderType(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderTypeEntity model = serializer.Deserialize<OrderTypeEntity>(entity);

            if (string.IsNullOrEmpty(model.TypeID))
            {
                model.TypeID = new SystemBusiness().CreateOrderType(model.TypeName, model.TypeCode, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateOrderType(model.TypeID, model.TypeName, model.TypeCode, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.TypeID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOrderType(string id)
        {
            bool bl = new SystemBusiness().DeleteOrderType(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

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

        public JsonResult GetOrderCategorys()
        {

            var list = new SystemBusiness().GetOrderCategorys(CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetClientOrderCategorys()
        {
            var list = new ProductsBusiness().GetClientCategorysByPID("", IntFactoryEnum.EnumCategoryType.Order, CurrentUser.ClientID).Where(m => m.Status == 1).ToList();
            foreach (var item in list)
            {
                if (item.ChildCategory == null || item.ChildCategory.Count == 0)
                {
                    item.ChildCategory = new ProductsBusiness().GetClientCategorysByPID(item.CategoryID, IntFactoryEnum.EnumCategoryType.Order, CurrentUser.ClientID).Where(m => m.Status == 1).ToList();
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

        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWareHouses(string keyWords, int pageSize, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<WareHouse> list = new SystemBusiness().GetWareHouses(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
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
        /// 根据ID获取仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 保存仓库
        /// </summary>
        /// <param name="ware"></param>
        /// <returns></returns>
        public JsonResult SaveWareHouse(string ware)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WareHouse model = serializer.Deserialize<WareHouse>(ware);

            string id = string.Empty;
            if (string.IsNullOrEmpty(model.WareID))
            {
                id = new SystemBusiness().AddWareHouse(model.WareCode, model.Name, model.ShortName, model.CityCode, model.Status.Value, model.DepotCode, model.DepotName, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else if (new SystemBusiness().UpdateWareHouse(model.WareID, model.WareCode, model.Name, model.ShortName, model.CityCode, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID))
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

        /// <summary>
        /// 编辑仓库状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 保存货位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonResult SaveDepotSeat(string obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            DepotSeat model = serializer.Deserialize<DepotSeat>(obj);

            string id = string.Empty;
            if (string.IsNullOrEmpty(model.DepotID))
            {
                id = new SystemBusiness().AddDepotSeat(model.DepotCode, model.WareID, model.Name, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else if (new SystemBusiness().UpdateDepotSeat(model.DepotID, model.DepotCode, model.Name, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID))
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

        /// <summary>
        /// 获取货位列表
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据仓库获取货位
        /// </summary>
        /// <param name="wareid"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取货位详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetDepotByID(string id = "")
        {
            var model = new SystemBusiness().GetDepotByID(id);
            JsonDictionary.Add("Item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除货位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteDepotSeat(string id)
        {
            bool bl = new SystemBusiness().DeleteDepotSeat(id, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 编辑货位状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult UpdateDepotSeatStatus(string id, int status)
        {
            bool bl = new SystemBusiness().UpdateDepotSeatStatus(id, (IntFactoryEnum.EnumStatus)status, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateDepotSeatSort(string depotid, string wareid, int type)
        {
            bool bl = new SystemBusiness().UpdateDepotSeatSort(depotid, wareid, type);
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

        #endregion

    }
}
