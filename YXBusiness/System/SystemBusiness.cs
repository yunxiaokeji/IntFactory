using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity;
using IntFactoryDAL;
using IntFactoryDAL.Custom;
using IntFactoryEnum;

namespace IntFactoryBusiness
{
    public class SystemBusiness
    {
        public static SystemBusiness BaseBusiness = new SystemBusiness();

        #region Cache

        private static Dictionary<string, List<LableColorEntity>> _customcolor;
        private static Dictionary<string, List<LableColorEntity>> _ordercolor;
        private static Dictionary<string, List<LableColorEntity>> _taskcolor;       

        private static Dictionary<string, List<OrderProcessEntity>> _orderprocess;

        private static Dictionary<string, List<OrderStageEntity>> _orderstages;

        private static Dictionary<string, List<TeamEntity>> _teams;

        private static Dictionary<string, List<WareHouse>> _wares;

        private static List<ProcessCategoryEntity> _processCategory;

        private static Dictionary<string, List<LableColorEntity>> CustomColor
        {
            get
            {
                if (_customcolor == null)
                {
                    _customcolor = new Dictionary<string, List<LableColorEntity>>();
                }
                return _customcolor;
            }
            set
            {
                _customcolor = value;
            }
        }

        private static Dictionary<string, List<LableColorEntity>> OrderColor
        {
            get
            {
                if (_ordercolor == null)
                {
                    _ordercolor = new Dictionary<string, List<LableColorEntity>>();
                }
                return _ordercolor;
            }
            set {_ordercolor = value; }
        }

        private static Dictionary<string, List<LableColorEntity>> TaskColor
        {
            get
            {
                if (_taskcolor == null)
                {
                    _taskcolor = new Dictionary<string, List<LableColorEntity>>();
                }
                return _taskcolor;
            }
            set { _taskcolor = value; }
        }

        private static Dictionary<string, List<OrderProcessEntity>> OrderProcess
        {
            get
            {
                if (_orderprocess == null)
                {
                    _orderprocess = new Dictionary<string, List<OrderProcessEntity>>();
                }
                return _orderprocess;
            }
            set
            {
                _orderprocess = value;
            }
        }

        private static Dictionary<string, List<OrderStageEntity>> OrderStages
        {
            get
            {
                if (_orderstages == null)
                {
                    _orderstages = new Dictionary<string, List<OrderStageEntity>>();
                }
                return _orderstages;
            }
            set
            {
                _orderstages = value;
            }
        }

        private static Dictionary<string, List<TeamEntity>> Teams
        {
            get
            {
                if (_teams == null)
                {
                    _teams = new Dictionary<string, List<TeamEntity>>();
                }
                return _teams;
            }
            set
            {
                _teams = value;
            }
        }

        private static Dictionary<string, List<WareHouse>> WareHouses
        {
            get
            {
                if (_wares == null)
                {
                    _wares = new Dictionary<string, List<WareHouse>>();
                }
                return _wares;
            }
            set
            {
                _wares = value;
            }
        }

        private static List<ProcessCategoryEntity> ProcessCategorys
        {
            get
            {
                if (_processCategory == null)
                {
                    _processCategory = new List<ProcessCategoryEntity>();
                }
                return _processCategory;
            }
            set
            {
                _processCategory = value;
            }
        }

        #endregion

        #region 查询

        public List<LableColorEntity> GetLableColor(string clientid, EnumMarkType lableType)
        {            
            string tableName = "";
            if (lableType == EnumMarkType.Customer)
            {
                tableName = "CustomerColor";
                if (CustomColor.ContainsKey(clientid))
                {
                    return CustomColor[clientid];
                }
            }
            else if (lableType == EnumMarkType.Orders)
            {
                tableName = "OrderColor";
                if (OrderColor.ContainsKey(clientid))
                {
                    return OrderColor[clientid];
                }
            }
            else if (lableType == EnumMarkType.Tasks)
            {
                tableName = "TaskColor";
                if (TaskColor.ContainsKey(clientid))
                {
                    return TaskColor[clientid];
                }
            }

            List<LableColorEntity> list = new List<LableColorEntity>();
            DataTable dt = LableColorDAL.BaseProvider.GetLableColor(tableName, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                LableColorEntity model = new LableColorEntity();
                model.FillData(dr);
                list.Add(model);
            }

            if (lableType == EnumMarkType.Customer)
            {
                CustomColor.Add(clientid, list);
            }
            else if (lableType == EnumMarkType.Orders)
            {
                OrderColor.Add(clientid, list);
            }
            else if (lableType == EnumMarkType.Tasks)
            {
                TaskColor.Add(clientid, list);
            }
            return list;
        }

        public bool ExistLableColor(string clientid, int lableType = 1, int colorid = 0)
        {
            string tableName = "";
            if (lableType == 1)
            {
                tableName = "Customer";
            }
            else if (lableType == 2)
            {
                tableName = "Orders";
            }
            else if (lableType == 3)
            {
                tableName = "OrderTask";
            }

            return LableColorDAL.BaseProvider.ExistLableColor(tableName, clientid, colorid);
        }

        public LableColorEntity GetLableColorColorID(string clientid, int colorid, EnumMarkType markType)
        {
            var list = GetLableColor(clientid, markType);
            return list.Where(x =>x.Status!=9 && x.ColorID == colorid).FirstOrDefault();
        }

        public List<OrderProcessEntity> GetOrderProcess(string clientid)
        {
            if (OrderProcess.ContainsKey(clientid))
            {
                return OrderProcess[clientid].ToList();
            }

            List<OrderProcessEntity> list = new List<OrderProcessEntity>();
            DataSet ds = SystemDAL.BaseProvider.GetOrderProcess(clientid);
            foreach (DataRow dr in ds.Tables["Stages"].Rows)
            {
                OrderProcessEntity model = new OrderProcessEntity();
                model.FillData(dr);
                model.OwnerName = OrganizationBusiness.GetUserByUserID(model.OwnerID, clientid).Name;
                model.CategoryName = GetProcessCategoryByID(model.CategoryID).Name;
                list.Add(model);
            }
            OrderProcess.Add(clientid, list);

            return list;
        }

        public OrderProcessEntity GetOrderProcessByID(string processid,string clientid)
        {
            var list = GetOrderProcess(clientid);

            if (list.Where(m => m.ProcessID == processid).Count() > 0)
            {
                return list.Where(m => m.ProcessID == processid).FirstOrDefault();
            }
            OrderProcessEntity model = new OrderProcessEntity();
            DataTable dt = SystemDAL.BaseProvider.GetOrderProcessByID(processid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.OwnerName = OrganizationBusiness.GetUserByUserID(model.OwnerID, clientid).Name;
                model.CategoryName = GetProcessCategoryByID(model.CategoryID).Name;

                list.Add(model);
            }
            return model;
        }

        public List<OrderStageEntity> GetOrderStages(string processid,string clientid)
        {
            if (OrderStages.ContainsKey(processid))
            {
                return OrderStages[processid].OrderBy(m => m.Sort).ToList();
            }

            List<OrderStageEntity> list = new List<OrderStageEntity>();
            DataSet ds = SystemDAL.BaseProvider.GetOrderStages(processid);
            foreach (DataRow dr in ds.Tables["Stages"].Rows)
            {
                OrderStageEntity model = new OrderStageEntity();
                model.FillData(dr);

                model.MarkStr = CommonBusiness.GetEnumDesc<EnumOrderStageMark>((EnumOrderStageMark)model.Mark);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, clientid);
                //model.StageItem = new List<StageItemEntity>();
                //foreach (DataRow itemdr in ds.Tables["Items"].Select("StageID='" + model.StageID + "'"))
                //{
                //    StageItemEntity item = new StageItemEntity();
                //    item.FillData(itemdr);
                //    model.StageItem.Add(item);
                //}
                list.Add(model);
            }
            OrderStages.Add(processid, list);

            return list;
        }

        public OrderStageEntity GetOrderStageByID(string stageid, string processid, string clientid)
        {
            if (string.IsNullOrEmpty(stageid) || string.IsNullOrEmpty(processid))
            {
                return null;
            }
            var list = GetOrderStages(processid,clientid);
            if (list.Where(m => m.StageID == stageid).Count() > 0)
            {
                return list.Where(m => m.StageID == stageid).FirstOrDefault();
            }

            OrderStageEntity model = new OrderStageEntity();
            DataSet ds = SystemDAL.BaseProvider.GetOrderStageByID(stageid);
            if (ds.Tables["Stages"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Stages"].Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, clientid);
                model.StageItem = new List<StageItemEntity>();
                foreach (DataRow itemdr in ds.Tables["Items"].Rows)
                {
                    StageItemEntity item = new StageItemEntity();
                    item.FillData(itemdr);
                    model.StageItem.Add(item);
                }
                OrderStages[clientid].Add(model);
            }

            return model;
        }

        public List<TeamEntity> GetTeams(string clientid)
        {
            if (Teams.ContainsKey(clientid))
            {
                return Teams[clientid];
            }

            List<TeamEntity> list = new List<TeamEntity>();
            DataTable dt = SystemDAL.BaseProvider.GetTeams(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                TeamEntity model = new TeamEntity();
                model.FillData(dr);
                model.Users = OrganizationBusiness.GetUsers(clientid).Where(m => m.TeamID == model.TeamID).ToList();
                list.Add(model);
            }
            Teams.Add(clientid, list);

            return list;

        }

        public TeamEntity GetTeamByID(string teamid, string clientid)
        {

            if (string.IsNullOrEmpty(teamid))
            {
                return null;
            }
            var list = GetTeams(clientid);
            if (list.Where(m => m.TeamID == teamid).Count() > 0)
            {
                return list.Where(m => m.TeamID == teamid).FirstOrDefault();
            }

            TeamEntity model = new TeamEntity();
            DataTable dt = SystemDAL.BaseProvider.GetTeamByID(teamid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.Users = OrganizationBusiness.GetUsers(clientid).Where(m => m.TeamID == model.TeamID).ToList();
                Teams[teamid].Add(model);
            }
            
            return model;
        }

        public List<WareHouse> GetWareHouses(string clientid)
        {
            if (WareHouses.ContainsKey(clientid))
            {
                return WareHouses[clientid];
            }

            DataSet ds = SystemDAL.BaseProvider.GetWareHouses(clientid);

            List<WareHouse> list = new List<WareHouse>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                WareHouse model = new WareHouse();
                model.FillData(dr);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                model.DepotSeats = new List<DepotSeat>();
                foreach (var item in ds.Tables[1].Select("WareID='" + model.WareID + "'"))
                {
                    DepotSeat depot = new DepotSeat();
                    depot.FillData(item);
                    model.DepotSeats.Add(depot);
                }
                list.Add(model);
            }
            WareHouses.Add(clientid, list);
            return list;
        }

        public WareHouse GetWareByID(string wareid, string clientid)
        {
            if (string.IsNullOrEmpty(wareid))
            {
                return null;
            }

            var list = GetWareHouses(clientid);

            if (list.Where(m => m.WareID.ToLower() == wareid.ToLower()).Count() > 0)
            {
                return list.Where(m => m.WareID == wareid).FirstOrDefault();
            }

            DataSet ds = SystemDAL.BaseProvider.GetWareByID(wareid);

            WareHouse model = new WareHouse();
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.FillData(ds.Tables[0].Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                model.DepotSeats = new List<DepotSeat>();
                foreach (DataRow item in ds.Tables[1].Rows)
                {
                    DepotSeat depot = new DepotSeat();
                    depot.FillData(item);
                    model.DepotSeats.Add(depot);
                }
                WareHouses[clientid].Add(model);
            }

            return model;
        }

        public List<DepotSeat> GetDepotSeats(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID, string wareid = "")
        {
            DataSet ds = SystemDAL.BaseProvider.GetDepotSeats(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID, wareid);

            List<DepotSeat> list = new List<DepotSeat>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DepotSeat model = new DepotSeat();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<DepotSeat> GetDepotSeatsByWareID(string wareid, string clientid)
        {
            var model = GetWareByID(wareid, clientid);
            return model.DepotSeats;
        }

        public DepotSeat GetDepotByID(string depotid, string wareid, string clientid)
        {
            var ware = GetWareByID(wareid, clientid);
            return ware.DepotSeats.Where(m => m.DepotID.ToLower() == depotid.ToLower()).FirstOrDefault();
        }

        #endregion

        #region 添加

        public string CreateStageItem(string name, string stageid, string processid, string userid, string clientid)
        {
            string itemid = Guid.NewGuid().ToString().ToLower();

            bool bl = SystemDAL.BaseProvider.CreateStageItem(itemid, name, stageid, processid, userid, clientid);
            if (bl)
            {
                var model = GetOrderStageByID(stageid, processid, clientid);
                if (model.StageItem == null)
                {
                    model.StageItem = new List<StageItemEntity>();
                }
                else if (model.StageItem.Where(m => m.ItemID.ToLower() == itemid).Count() == 0)
                {
                    model.StageItem.Add(new StageItemEntity()
                    {
                        ItemID = itemid,
                        ItemName = name,
                        StageID = stageid,
                        ClientID = clientid,
                        CreateTime = DateTime.Now
                    });
                }

                return itemid;
            }
            return "";
        }

        public int CreateLableColor(string colorName, string colorValue, string clientid, string userid,
            int status = 0, EnumMarkType lableType = EnumMarkType.Customer)
        {
            string procName = "P_InsertCustomerColor";
            if (lableType == EnumMarkType.Orders)
            {
                procName = "P_InsertOrderColor";
            }
            else if (lableType == EnumMarkType.Tasks)
            {
                procName = "P_InsertTaskColor";
            }
            int result = LableColorDAL.BaseProvider.InsertLableColor(procName, colorName, colorValue,clientid, userid, status);
            if (result > 0)
            {
                var list = GetLableColor(clientid, lableType);
                if (list.Where(m => m.ColorID == result).Count() == 0)
                {
                    list.Add(new LableColorEntity()
                    {
                        ColorID = result,
                        ColorValue = colorValue,
                        ColorName = colorName,
                        ClientID = clientid,
                        CreateUserID = userid,
                        CreateTime = DateTime.Now,
                        Status = 0
                    });
                }
            }
            return result;
        }

        public string CreateOrderProcess(string name, int type, string categoryid, int days, int isdefault, string ownerid, string userid, string clientid)
        {
            string id = Guid.NewGuid().ToString().ToLower();
            string otherProcessID = "";
            bool bl = SystemDAL.BaseProvider.CreateOrderProcess(id, name, type, categoryid, days, isdefault, ownerid, userid, clientid, out otherProcessID);
            if (bl)
            {
                if (!OrderProcess.ContainsKey(clientid))
                {
                    GetOrderProcess(clientid);
                }
                var model = GetOrderProcessByID(id,clientid);
                if (OrderProcess[clientid].Where(m => m.ProcessID.ToLower() == id).Count() == 0)
                {
                    OrderProcess[clientid].Add(model);
                }
                
                if (!string.IsNullOrEmpty(otherProcessID))
                {
                    var oModel = GetOrderProcessByID(otherProcessID, clientid);

                    if (OrderProcess[clientid].Where(m => m.ProcessID.ToLower() == otherProcessID.ToLower()).Count() == 0)
                    {
                        OrderProcess[clientid].Add(oModel);
                    }
                }
                
                return id;
            }
            return "";
        }

        public string CreateOrderStage(string name, int sort, int mark, int hours, string pid, string processid, string userid,  string clientid, out int result)
        {
            string stageid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateOrderStage(stageid, name, sort, mark, hours, pid, processid, userid, clientid, out result);
            if (bl)
            {
                if (!OrderStages.ContainsKey(processid))
                {
                    GetOrderStages(processid, clientid);
                }
                else
                {
                    var list = OrderStages[processid].Where(m => m.Sort >= sort && m.Status == 1).ToList();

                    foreach (var model in list)
                    {
                        model.Sort += 1;
                    }

                    OrderStages[processid].Add(new OrderStageEntity()
                    {
                        StageID = stageid.ToLower(),
                        StageName = name,
                        Sort = sort,
                        PID = pid,
                        Mark = mark,
                        MaxHours = hours,
                        MarkStr = CommonBusiness.GetEnumDesc<EnumOrderStageMark>((EnumOrderStageMark)mark),
                        Status = 1,
                        CreateTime = DateTime.Now,
                        ProcessID = processid,
                        OwnerID = userid,
                        StageItem = new List<StageItemEntity>(),
                        Owner = OrganizationBusiness.GetUserByUserID(userid, clientid),
                        ClientID = clientid
                    });
                }

                return stageid;
            }
            return "";
        }

        public string CreateTeam(string teamname, string userid, string clientid)
        {
            string teamid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateTeam(teamid, teamname, userid, clientid);
            if (bl)
            {
                if (!Teams.ContainsKey(clientid))
                {
                    GetTeams(clientid);
                }
                else
                {
                    Teams[clientid].Add(new TeamEntity()
                    {
                        TeamID = teamid.ToLower(),
                        TeamName = teamname,
                        Status = 1,
                        CreateTime = DateTime.Now,
                        CreateUserID = userid,
                        ClientID = clientid,
                        Users = new List<Users>()
                    });
                }

                return teamid;
            }
            return "";
        }

        public string AddWareHouse(string warecode, string name, string shortname, string citycode, int status, string depotcode, string depotname, string description, string operateid, string clientid)
        {
            var id = Guid.NewGuid().ToString();
            if (SystemDAL.BaseProvider.AddWareHouse(id, warecode, name, shortname, citycode, status, depotcode, depotname, description, operateid, clientid))
            {
                if (!WareHouses.ContainsKey(clientid))
                {
                    GetWareHouses(clientid);
                }
                else
                {
                    var model = new WareHouse()
                    {
                        WareID = id,
                        WareCode = warecode,
                        Name = name,
                        ShortName = shortname,
                        CityCode = citycode,
                        Status = status,
                        Description = description,
                        ClientID = clientid,
                        DepotSeats = new List<DepotSeat>()
                    };
                    WareHouses[clientid].Add(model);
                }
                return id.ToString();
            }

            return string.Empty;
        }

        public string AddDepotSeat(string depotcode, string wareid, string name, int status, string description, string operateid, string clientid)
        {
            var id = Guid.NewGuid().ToString().ToLower();
            var bl = SystemDAL.BaseProvider.AddDepotSeat(id, depotcode, wareid, name, status, description, operateid, clientid);
            if (bl)
            {
                var model = GetWareByID(wareid, clientid);
                if (model.DepotSeats.Where(m => m.DepotID.ToLower() == id).Count() == 0)
                {
                    model.DepotSeats.Add(new DepotSeat() { DepotID = id, DepotCode = depotcode, Name = name, ClientID = clientid, Status = 1, WareID = wareid, Description = description });
                }
                return id;
            }
            return string.Empty;
        }

        #endregion

        #region 编辑/删除

        public int UpdateLableColor(string clientid, int colorid, string colorName, string colorValue, string updateuserid, EnumMarkType lableType)
        {
            string tableName = "CustomerColor";
            if (lableType == EnumMarkType.Orders)
            {
                tableName = "OrderColor";
            }
            else if (lableType == EnumMarkType.Tasks)
            {
                tableName = "TaskColor";
            }
            bool result = LableColorDAL.BaseProvider.UpdateLableColor(tableName, clientid, colorid, colorName, colorValue, updateuserid);
            if (result)
            {
                var model = GetLableColorColorID(clientid, colorid, lableType);
                model.ColorValue = colorValue;
                model.ColorName = colorName;
            }
            return result ? 1 : 0;
        }

        public int DeleteLableColor(int status, int colorid, string clientid, string updateuserid, EnumMarkType lableType)
        {
            if (ExistLableColor(clientid, (int)lableType, colorid))
            {
                return 2;
            }

            var model = GetLableColorColorID(clientid, colorid, lableType);
            if (model == null)
            {
                return -200;
            }
            if (CustomColor[clientid].Count == 1)
            {
                return -100;
            }

            string tableName = "CustomerColor";
            if (lableType == EnumMarkType.Orders)
            {
                tableName = "OrderColor";
            }
            else if (lableType == EnumMarkType.Tasks)
            {
                tableName = "TaskColor";
            }
            bool result = LableColorDAL.BaseProvider.DeleteLableColor(tableName, status, colorid,clientid, updateuserid);
            if (result)
            {
                var list = GetLableColor(clientid, lableType);
                list.Remove(model);
            }
            return result ? 1 : 0;
        }

        public bool UpdateOrderProcess(string processid, string name, int days, string userid, string ip, string clientid)
        {
            bool bl = SystemDAL.BaseProvider.UpdateOrderProcess(processid, name, days);
            if (bl)
            {
                var model = GetOrderProcessByID(processid, clientid);
                model.ProcessName = name;
                model.PlanDays = days;
            }
            return bl;
        }

        public bool DeleteOrderProcess(string processid, string userid, string ip, string clientid,ref int result)
        {
            var model = GetOrderProcessByID(processid, clientid);
            //默认流程不能删除
            if (model.IsDefault == 1)
            {
                result = 3;
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteOrderProcess(processid, clientid, ref result);
            if (bl)
            {
                OrderProcess[clientid].Remove(model);
            }
            return bl;
        }

        public bool UpdateOrderProcessOwner(string processid, string ownerid, string userid, string ip,string clientid)
        {
            var model = GetOrderProcessByID(processid, clientid);

            if (ownerid == model.OwnerID)
            {
                return true;
            }

            bool bl = SystemDAL.BaseProvider.UpdateOrderProcessOwner(processid, ownerid);
            if (bl)
            {
                model.OwnerID = ownerid;
                model.Owner = OrganizationBusiness.GetUserByUserID(ownerid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderProcessDefault(string processid, string userid, string ip, string clientid)
        {
            var model = GetOrderProcessByID(processid,clientid);
            //默认流程不能删除
            if (model.IsDefault == 1)
            {
                return true;
            }
            bool bl = SystemDAL.BaseProvider.UpdateOrderProcessDefault(processid, model.ProcessType, model.ClientID);
            if (bl)
            {
                foreach (var item in OrderProcess[clientid])
                {
                    if (item.IsDefault == 1 && item.ProcessType == model.ProcessType)
                    {
                        item.IsDefault = 0;
                    }
                }
                model.IsDefault = 1;
            }
            return bl;
        }

        public bool UpdateOrderStage(string stageid, string stagename, int mark, int hours,string processid, string userid, string ip, string clientid)
        {
            bool bl = SystemDAL.BaseProvider.UpdateOrderStage(stageid, stagename, mark, hours,clientid);
            if (bl)
            {
                var model = GetOrderStageByID(stageid, processid, clientid);
                model.StageName = stagename;
                model.Mark = mark;
                model.MaxHours = hours;
                model.MarkStr = CommonBusiness.GetEnumDesc<EnumOrderStageMark>((EnumOrderStageMark)model.Mark);
            }
            return bl;
        }

        public bool DeleteOrderStage(string stageid, string processid, string userid, string ip, string clientid)
        {
            bool bl = SystemDAL.BaseProvider.DeleteOrderStage(stageid, processid, userid, clientid);
            if (bl)
            {
                var model = GetOrderStageByID(stageid, processid,clientid);

                OrderStages[processid].Remove(model);

                var list = OrderStages[processid].Where(m => m.Sort > model.Sort && m.Status == 1).ToList();
                foreach (var stage in list)
                {
                    stage.Sort -= 1;
                }
            }
            return bl;
        }

        public bool UpdateOrderStageOwner(string stageid, string processid, string ownerid, string userid, string ip, string clientid)
        {
            var model = GetOrderStageByID(stageid, processid,  clientid);

            if (ownerid == model.OwnerID)
            {
                return true;
            }

            bool bl = SystemDAL.BaseProvider.UpdateOrderStageOwner(stageid, ownerid);
            if (bl)
            {
                model.OwnerID = ownerid;
                model.Owner = OrganizationBusiness.GetUserByUserID(ownerid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderCategory(string categoryid, string pid, int status, string clientid)
        {
            bool bl = SystemDAL.BaseProvider.UpdateOrderCategory(categoryid, pid, status, clientid);
            return bl;
        }

        public bool UpdateTeam(string teamid, string name, string userid, string ip,string clientid)
        {

            bool bl = CommonBusiness.Update("Teams", "TeamName", name, "TeamID='" + teamid + "'");
            if (bl)
            {
                var model = GetTeamByID(teamid, clientid);
                model.TeamName = name;
            }
            return bl;
        }

        public bool DeleteTeam(string teamid, string userid, string ip, string clientid)
        {
            bool bl = SystemDAL.BaseProvider.DeleteTeam(teamid, userid);
            if (bl)
            {
                var model = GetTeamByID(teamid, clientid);
                var list = OrganizationBusiness.GetUsers(clientid).Where(m => m.TeamID == teamid).ToList();
                foreach (var user in list)
                {
                    user.TeamID = "";
                }
                Teams[clientid].Remove(model);
            }
            return bl;
        }

        public bool UpdateUserTeamID(string userid, string teamid,string operateid, string ip, string clientid)
        {
            bool bl = SystemDAL.BaseProvider.UpdateUserTeamID(userid, teamid, operateid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, clientid);
                if (string.IsNullOrEmpty(teamid))
                {
                    var team = GetTeamByID(model.TeamID, clientid);
                    var user = team.Users.Where(m => m.UserID == userid).FirstOrDefault();
                    team.Users.Remove(user);
                }
                else
                {
                    var team = GetTeamByID(teamid, clientid);
                    team.Users.Add(model);
                }

                model.TeamID = teamid;
            }
            return bl;
        }

        public bool UpdateWareHouse(string id, string code, string name, string shortname, string citycode, int status, string description, string operateid, string clientid)
        {
            var bl = SystemDAL.BaseProvider.UpdateWareHouse(id, code, name, shortname, citycode, status, description);
            if (bl)
            {
                var model = GetWareByID(id, clientid);
                model.WareCode = code;
                model.Name = name;
                model.ShortName = shortname;
                model.CityCode = citycode;
                model.Status = status;
                model.Description = description;
            }
            return bl;
        }

        public bool UpdateWareHouseStatus(string id, EnumStatus status, string operateid, string clientid)
        {
            bool bl= CommonBusiness.Update("WareHouse", "Status", (int)status, " WareID='" + id + "'");
            if (bl)
            {
                var model = GetWareByID(id, clientid);
                if (status == EnumStatus.Delete)
                {
                    WareHouses[clientid].Remove(model);
                }
                else
                {
                    model.Status = (int)status;
                }
            }
            return bl;
        }

        public bool UpdateDepotSeat(string id, string wareid, string depotcode, string name, int status, string description, string operateid, string clientid)
        {
            bool bl = SystemDAL.BaseProvider.UpdateDepotSeat(id, depotcode, name, status, description);
            if (bl)
            {
                var model = GetDepotByID(id, wareid, clientid);
                model.DepotCode = depotcode;
                model.Name = name;
                model.Status = status;
                model.Description = description;
            }
            return bl;
        }

        public bool UpdateDepotSeatStatus(string id, string wareid, EnumStatus status, string operateid, string clientid)
        {
            
            bool bl = CommonBusiness.Update("DepotSeat", "Status", (int)status, " DepotID='" + id + "'");
            if (bl)
            {
                var model = GetDepotByID(id, wareid, clientid);
                model.Status = (int)status;
            }
            return bl;
        }

        public bool DeleteDepotSeat(string depotid, string wareid, string operateid, string clientid)
        {
            bool bl = SystemDAL.BaseProvider.DeleteDepotSeat(depotid, clientid);
            if (bl)
            {
                var ware = GetWareByID(wareid, clientid);
                var depot = GetDepotByID(depotid, wareid, clientid);
                ware.DepotSeats.Remove(depot);
            }
            return bl;
        }

        public bool UpdateDepotSeatSort(string depotid, string wareid, int type, string clientid)
        {
            bool bl = SystemDAL.BaseProvider.UpdateDepotSeatSort(depotid, wareid, type);

            return bl;
        }

        #endregion

        #region 订单品类

        public List<ProcessCategoryEntity> GetProcessCategorys()
        {
            if (ProcessCategorys.Count > 0)
            {
                return ProcessCategorys;
            }
            List<ProcessCategoryEntity> list = new List<ProcessCategoryEntity>();

            DataSet ds = SystemDAL.BaseProvider.GetProcessCategory();
            foreach (DataRow tr in ds.Tables[0].Rows)
            {
                ProcessCategoryEntity model = new ProcessCategoryEntity();
                model.FillData(tr);
                model.CategoryItems = new List<CategoryItemsEntity>();
                foreach (DataRow itemtr in ds.Tables[1].Select("CategoryID='" + model.CategoryID + "'"))
                {
                    CategoryItemsEntity item = new CategoryItemsEntity();
                    item.FillData(itemtr);
                    switch (item.Mark % 10) 
                    {
                        case 1:
                            item.Remark = "材料";
                            break;
                        case 2:
                            item.Remark = "制版";
                            break;
                        case 3:
                            item.Remark = "裁片";
                            break;
                        case 4:
                            item.Remark = "车缝";
                            break;
                        case 5:
                            item.Remark = "发货";
                            break;
                        case 6:
                            item.Remark = "加工成本";
                            break;
                        default:
                            item.Remark = "";
                            break;
                    }
                    model.CategoryItems.Add(item);
                }
                list.Add(model);
            }

            ProcessCategorys = list;

            return ProcessCategorys;
        }

        public ProcessCategoryEntity GetProcessCategoryByID(string categoryid)
        {
            var list = GetProcessCategorys();
            if (list.Where(m => m.CategoryID.ToLower() == categoryid.ToLower()).Count() > 0)
            {
                return list.Where(m => m.CategoryID.ToLower() == categoryid.ToLower()).FirstOrDefault();
            }
            ProcessCategoryEntity model = new ProcessCategoryEntity();

            DataSet ds = SystemDAL.BaseProvider.GetProcessCategoryByID(categoryid);
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.FillData(ds.Tables[0].Rows[0]);
                model.CategoryItems = new List<CategoryItemsEntity>();
                foreach (DataRow itemtr in ds.Tables[1].Rows)
                {
                    CategoryItemsEntity item = new CategoryItemsEntity();
                    item.FillData(itemtr);
                    switch (item.Mark % 10)
                    {
                        case 1:
                            item.Remark = "材料";
                            break;
                        case 2:
                            item.Remark = "制版";
                            break;
                        case 3:
                            item.Remark = "裁片";
                            break;
                        case 4:
                            item.Remark = "车缝";
                            break;
                        case 5:
                            item.Remark = "发货";
                            break;
                        case 6:
                            item.Remark = "加工成本";
                            break;
                        default:
                            item.Remark = "";
                            break;
                    }
                    model.CategoryItems.Add(item);
                }
                list.Add(model);
            }
            return model;
        }

        public string CreateProcessCategory(string name, string remark, string userid)
        {
            string id = Guid.NewGuid().ToString().ToLower();
            bool bl = SystemDAL.BaseProvider.CreateProcessCategory(id, name, remark, userid);
            if (bl)
            {
                //添加缓存
                GetProcessCategoryByID(id);

                return id;
            }
            return "";
        }

        public bool UpdateProcessCategory(string categoryid,string name, string remark)
        {
            bool bl = SystemDAL.BaseProvider.UpdateProcessCategory(categoryid, name, remark);
            if (bl)
            {
                var model =GetProcessCategoryByID(categoryid);
                model.Name = name;
                model.Remark = remark;
            }
            return bl;
        }

        public bool DeleteProcessCategory(string categoryid, string userid)
        {
            bool bl = SystemDAL.BaseProvider.DeleteProcessCategory(categoryid, userid);
            if (bl)
            {
                var list = GetProcessCategorys();
                var model = GetProcessCategoryByID(categoryid);
                list.Remove(model);
            }
            return bl;
        }

        public bool UpdateCategoryItem(string categoryid, string itemid, string name, int sort)
        {
            bool bl = SystemDAL.BaseProvider.UpdateCategoryItems(itemid, name, sort);
            if (bl)
            {
                var model = GetProcessCategoryByID(categoryid);
                var item = model.CategoryItems.Where(m => m.ItemID.ToLower() == itemid.ToLower()).FirstOrDefault();
                item.Name = name;
                item.Sort = sort;
            }
            return bl;
        }

        #endregion

    }
}
