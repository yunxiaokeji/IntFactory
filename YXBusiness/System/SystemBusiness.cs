using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryEntity;
using System.Data;
using IntFactoryDAL;
using IntFactoryEnum;

namespace IntFactoryBusiness
{
    public class SystemBusiness
    {
        public static SystemBusiness BaseBusiness = new SystemBusiness();

        #region Cache

        private static Dictionary<string, List<CustomSourceEntity>> _source;
        private static Dictionary<string, List<CustomStageEntity>> _stages;

        private static Dictionary<string, List<OrderProcessEntity>> _orderprocess;

        private static Dictionary<string, List<OrderStageEntity>> _orderstages;

        private static Dictionary<string, List<OrderTypeEntity>> _ordertypes;
        private static Dictionary<string, List<TeamEntity>> _teams;

        private static Dictionary<string, List<WareHouse>> _wares;

        /// <summary>
        /// 客户来源
        /// </summary>
        private static Dictionary<string, List<CustomSourceEntity>> CustomSources
        {
            get
            {
                if (_source == null)
                {
                    _source = new Dictionary<string, List<CustomSourceEntity>>();
                }
                return _source;
            }
            set 
            {
                _source = value;
            }
        }

        /// <summary>
        /// 客户阶段
        /// </summary>
        private static Dictionary<string, List<CustomStageEntity>> CustomStages
        {
            get
            {
                if (_stages == null)
                {
                    _stages = new Dictionary<string, List<CustomStageEntity>>();
                }
                return _stages;
            }
            set
            {
                _stages = value;
            }
        }

        //订单流程
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

        /// <summary>
        /// 订单阶段
        /// </summary>
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

        /// <summary>
        /// 订单类型
        /// </summary>
        private static Dictionary<string, List<OrderTypeEntity>> OrderTypes
        {
            get
            {
                if (_ordertypes == null)
                {
                    _ordertypes = new Dictionary<string, List<OrderTypeEntity>>();
                }
                return _ordertypes;
            }
            set
            {
                _ordertypes = value;
            }
        }

        /// <summary>
        /// 销售团队
        /// </summary>
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

        /// <summary>
        /// 仓库
        /// </summary>
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

        #endregion

        #region 查询

        public List<CustomSourceEntity> GetCustomSources(string agentid,string clientid)
        {
            if (CustomSources.ContainsKey(clientid)) 
            {
                return CustomSources[clientid];
            }

            List<CustomSourceEntity> list = new List<CustomSourceEntity>();
            DataTable dt = SystemDAL.BaseProvider.GetCustomSources(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                CustomSourceEntity model = new CustomSourceEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }
            CustomSources.Add(clientid, list);

            return list;

        }

        public CustomSourceEntity GetCustomSourcesByID(string sourceid, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(sourceid))
            {
                return null;
            }
            var list = GetCustomSources(agentid, clientid);
            if (list.Where(m => m.SourceID == sourceid).Count() > 0)
            {
                return list.Where(m => m.SourceID == sourceid).FirstOrDefault();
            }

            CustomSourceEntity model = new CustomSourceEntity();
            DataTable dt = SystemDAL.BaseProvider.GetCustomSourceByID(sourceid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                CustomSources[clientid].Add(model);
            }
           
            return model;
        }

        public List<CustomStageEntity> GetCustomStages(string agentid, string clientid)
        {
            if (CustomStages.ContainsKey(clientid))
            {
                return CustomStages[clientid].OrderBy(m => m.Sort).ToList();
            }

            List<CustomStageEntity> list = new List<CustomStageEntity>();
            DataSet ds = SystemDAL.BaseProvider.GetCustomStages(clientid);
            foreach (DataRow dr in ds.Tables["Stages"].Rows)
            {
                CustomStageEntity model = new CustomStageEntity();
                model.FillData(dr);
                model.StageItem = new List<StageItemEntity>();
                foreach (DataRow itemdr in ds.Tables["Items"].Select("StageID='" + model.StageID + "'"))
                {
                    StageItemEntity item = new StageItemEntity();
                    item.FillData(itemdr);
                    model.StageItem.Add(item);
                }

                list.Add(model);
            }
            CustomStages.Add(clientid, list);

            return list;
        }

        public CustomStageEntity GetCustomStageByID(string stageid, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(stageid))
            {
                return null;
            }
            var list = GetCustomStages(agentid, clientid);
            if (list.Where(m => m.StageID == stageid).Count() > 0)
            {
                return list.Where(m => m.StageID == stageid).FirstOrDefault();
            }

            CustomStageEntity model = new CustomStageEntity();
            DataSet ds = SystemDAL.BaseProvider.GetCustomStageByID(stageid);
            if (ds.Tables["Stages"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Stages"].Rows[0]);
                model.StageItem = new List<StageItemEntity>();
                foreach (DataRow itemdr in ds.Tables["Items"].Rows)
                {
                    StageItemEntity item = new StageItemEntity();
                    item.FillData(itemdr);
                    model.StageItem.Add(item);
                }
                CustomStages[clientid].Add(model);
            }
            return model;
        }

        public List<OrderProcessEntity> GetOrderProcess(string agentid, string clientid)
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
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, agentid);
                list.Add(model);
            }
            OrderProcess.Add(clientid, list);

            return list;
        }

        public OrderProcessEntity GetOrderProcessByID(string processid, string agentid, string clientid)
        {
            var list = GetOrderProcess(agentid, clientid);

            if (list.Where(m => m.ProcessID == processid).Count() > 0)
            {
                return list.Where(m => m.ProcessID == processid).FirstOrDefault();
            }
            OrderProcessEntity model = new OrderProcessEntity();
            DataTable dt = SystemDAL.BaseProvider.GetOrderProcessByID(processid);
            if (dt.Rows.Count > 0)
            {
                
                model.FillData(dt.Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, agentid);

                OrderProcess[clientid].Add(model);
            }
            return model;
        }

        public List<OrderStageEntity> GetOrderStages(string processid, string agentid, string clientid)
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
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, agentid);
                model.StageItem = new List<StageItemEntity>();
                foreach (DataRow itemdr in ds.Tables["Items"].Select("StageID='" + model.StageID + "'"))
                {
                    StageItemEntity item = new StageItemEntity();
                    item.FillData(itemdr);
                    model.StageItem.Add(item);
                }
                list.Add(model);
            }
            OrderStages.Add(processid, list);

            return list;
        }

        public OrderStageEntity GetOrderStageByID(string stageid, string processid, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(stageid) || string.IsNullOrEmpty(processid))
            {
                return null;
            }
            var list = GetOrderStages(processid, agentid, clientid);
            if (list.Where(m => m.StageID == stageid).Count() > 0)
            {
                return list.Where(m => m.StageID == stageid).FirstOrDefault();
            }

            OrderStageEntity model = new OrderStageEntity();
            DataSet ds = SystemDAL.BaseProvider.GetOrderStageByID(stageid);
            if (ds.Tables["Stages"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Stages"].Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, agentid);
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

        public List<OrderTypeEntity> GetOrderTypes(string agentid, string clientid)
        {
            if (OrderTypes.ContainsKey(clientid))
            {
                return OrderTypes[clientid].ToList();
            }

            List<OrderTypeEntity> list = new List<OrderTypeEntity>();
            DataSet ds = SystemDAL.BaseProvider.GetOrderTypes(clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderTypeEntity model = new OrderTypeEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }

            OrderTypes.Add(clientid, list);

            return list;
        }

        public OrderTypeEntity GetOrderTypeByID(string typeid, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(typeid))
            {
                return null;
            }
            var list = GetOrderTypes(agentid, clientid);
            if (list.Where(m => m.TypeID == typeid).Count() > 0)
            {
                return list.Where(m => m.TypeID == typeid).FirstOrDefault();
            }

            OrderTypeEntity model = new OrderTypeEntity();
            DataTable dt = SystemDAL.BaseProvider.GetOrderTypeByID(typeid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                OrderTypes[clientid].Add(model);
            }
            
            return model;
        }

        public List<TeamEntity> GetTeams(string agentid)
        {
            if (Teams.ContainsKey(agentid))
            {
                return Teams[agentid];
            }

            List<TeamEntity> list = new List<TeamEntity>();
            DataTable dt = SystemDAL.BaseProvider.GetTeams(agentid);
            foreach (DataRow dr in dt.Rows)
            {
                TeamEntity model = new TeamEntity();
                model.FillData(dr);
                model.Users = OrganizationBusiness.GetUsers(agentid).Where(m => m.TeamID == model.TeamID).ToList();
                list.Add(model);
            }
            Teams.Add(agentid, list);

            return list;

        }

        public TeamEntity GetTeamByID(string teamid, string agentid)
        {

            if (string.IsNullOrEmpty(teamid))
            {
                return null;
            }
            var list = GetTeams(agentid);
            if (list.Where(m => m.TeamID == teamid).Count() > 0)
            {
                return list.Where(m => m.TeamID == teamid).FirstOrDefault();
            }

            TeamEntity model = new TeamEntity();
            DataTable dt = SystemDAL.BaseProvider.GetTeamByID(teamid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.Users = OrganizationBusiness.GetUsers(agentid).Where(m => m.TeamID == model.TeamID).ToList();
                Teams[teamid].Add(model);
            }
            
            return model;
        }

        public List<WareHouse> GetWareHouses(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            DataSet ds = SystemDAL.BaseProvider.GetWareHouses(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<WareHouse> list = new List<WareHouse>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                WareHouse model = new WareHouse();
                model.FillData(dr);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }
            return list;
        }

        public List<WareHouse> GetWareHouses(string clientID)
        {
            if (WareHouses.ContainsKey(clientID))
            {
                return WareHouses[clientID];
            }

            DataTable dt = SystemDAL.BaseProvider.GetWareHouses(clientID);

            List<WareHouse> list = new List<WareHouse>();
            foreach (DataRow dr in dt.Rows)
            {
                WareHouse model = new WareHouse();
                model.FillData(dr);
                list.Add(model);
            }
            WareHouses.Add(clientID, list);
            return list;
        }

        public WareHouse GetWareByID(string wareid, string clientid)
        {
            if (string.IsNullOrEmpty(wareid))
            {
                return null;
            }

            var list = GetWareHouses(clientid);

            if (list.Where(m => m.WareID == wareid).Count() > 0)
            {
                return list.Where(m => m.WareID == wareid).FirstOrDefault();
            }

            DataTable dt = SystemDAL.BaseProvider.GetWareByID(wareid);

            WareHouse model = new WareHouse();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
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
            DataTable dt = SystemDAL.BaseProvider.GetDepotSeatsByWareID(wareid);

            List<DepotSeat> list = new List<DepotSeat>();
            foreach (DataRow dr in dt.Rows)
            {
                DepotSeat model = new DepotSeat();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public DepotSeat GetDepotByID(string depotid)
        {
            DataTable dt = SystemDAL.BaseProvider.GetDepotByID(depotid);

            DepotSeat model = new DepotSeat();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        #endregion

        #region 添加

        public string CreateCustomSource(string sourcecode, string sourcename, int ischoose, string userid, string agentid, string clientid,out int result)
        {
            string sourceid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateCustomSource(sourceid, sourcecode, sourcename, ischoose , userid, clientid, out result);
            if (bl)
            {
                if (!CustomSources.ContainsKey(clientid)) 
                {
                    GetCustomSources(agentid, clientid);
                }

                CustomSources[clientid].Add(new CustomSourceEntity()
                {
                    SourceID = sourceid.ToLower(),
                    SourceName = sourcename,
                    SourceCode = sourcecode,
                    IsChoose = ischoose,
                    IsSystem = 0,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    ClientID = clientid
                });

                return sourceid;
            }
            return "";
        }

        public string CreateCustomStage(string name, int sort, string pid, string userid, string agentid, string clientid, out int result)
        {
            string stageid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateCustomStage(stageid, name, sort, pid, userid, clientid, out result);
            if (bl)
            {
                if (!CustomStages.ContainsKey(clientid))
                {
                    GetCustomStages(agentid, clientid);
                }

                var list = CustomStages[clientid].Where(m => m.Sort >= sort && m.Status == 1).ToList();
                foreach (var model in list)
                {
                    model.Sort += 1;
                }

                CustomStages[clientid].Add(new CustomStageEntity()
                {
                    StageID = stageid.ToLower(),
                    StageName = name,
                    Sort = sort,
                    PID = pid,
                    Mark = 0,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    ClientID = clientid,
                    StageItem = new List<StageItemEntity>()
                });

                return stageid;
            }
            return "";
        }

        public string CreateStageItem(string name, string stageid, string processid, string userid, string agentid, string clientid)
        {
            string itemid = Guid.NewGuid().ToString().ToLower();

            bool bl = SystemDAL.BaseProvider.CreateStageItem(itemid, name, stageid, processid, userid, clientid);
            if (bl)
            {
                var model = GetOrderStageByID(stageid, processid, agentid, clientid);
                if (model.StageItem == null)
                {
                    model.StageItem = new List<StageItemEntity>();
                }
                model.StageItem.Add(new StageItemEntity()
                {
                    ItemID = itemid,
                    ItemName = name,
                    StageID = stageid,
                    ClientID = clientid,
                    CreateTime = DateTime.Now
                });

                return itemid;
            }
            return "";
        }

        public string CreateOrderProcess(string name, int type, int days, int isdefault, string ownerid, string userid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString().ToLower();
            bool bl = SystemDAL.BaseProvider.CreateOrderProcess(id, name, type, days, isdefault, ownerid, userid, clientid);
            if (bl)
            {
                if(!OrderProcess.ContainsKey(clientid))
                {
                    GetOrderProcess(agentid, clientid);
                }
                OrderProcess[clientid].Add(
                    new OrderProcessEntity()
                    {
                        ProcessID = id,
                        ProcessName = name,
                        ProcessType = type,
                        PlanDays = days,
                        CreateTime = DateTime.Now,
                        OwnerID = ownerid,
                        Owner = OrganizationBusiness.GetUserByUserID(ownerid, agentid),
                        IsDefault = 0,
                        Status = 1,
                        ClientID = clientid
                    }
                );
                return id;
            }
            return "";
        }

        public string CreateOrderStage(string name, int sort, string pid, string processid, string userid, string agentid, string clientid, out int result)
        {
            string stageid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateOrderStage(stageid, name, sort, pid, processid, userid, clientid, out result);
            if (bl)
            {
                if (!OrderStages.ContainsKey(processid))
                {
                    GetOrderStages(processid, agentid, clientid);
                }

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
                    Mark = 0,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    ProcessID=processid,
                    OwnerID = userid,
                    Owner = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    ClientID = clientid
                });

                return stageid;
            }
            return "";
        }

        public string CreateOrderType(string typename, string typecode, string userid, string agentid, string clientid)
        {
            string typeid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateOrderType(typeid, typename, typecode, userid, clientid);
            if (bl)
            {
                if (!OrderTypes.ContainsKey(clientid))
                {
                    GetOrderTypes(agentid, clientid);
                }

                OrderTypes[clientid].Add(new OrderTypeEntity()
                {
                    TypeID = typeid.ToLower(),
                    TypeName = typename,
                    TypeCode = typecode,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    ClientID = clientid
                });

                return typeid;
            }
            return "";
        }

        public string CreateTeam(string teamname, string userid, string agentid, string clientid)
        {
            string teamid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateTeam(teamid, teamname, userid, agentid, clientid);
            if (bl)
            {
                if (!Teams.ContainsKey(agentid))
                {
                    GetTeams(agentid);
                }

                Teams[agentid].Add(new TeamEntity()
                {
                    TeamID = teamid.ToLower(),
                    TeamName = teamname,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    ClientID = clientid,
                    Users = new List<Users>()
                });

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
                var model = new WareHouse()
                {
                    WareID = id,
                    WareCode = warecode,
                    Name = name,
                    ShortName = shortname,
                    CityCode = citycode,
                    Status = status,
                    Description = description,
                    CreateUserID = operateid,
                    ClientID = clientid,
                    CreateTime = DateTime.Now
                };
                WareHouses[clientid].Add(model);
                return id.ToString();
            }

            

            return string.Empty;
        }

        public string AddDepotSeat(string depotcode, string wareid, string name, int status, string description, string operateid, string clientid)
        {
            var id = Guid.NewGuid().ToString();
            if (SystemDAL.BaseProvider.AddDepotSeat(id, depotcode, wareid, name, status, description, operateid, clientid))
            {
                return id.ToString();
            }
            return string.Empty;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateCustomSource(string sourceid, string sourcename, int ischoose, string userid,string ip, string agentid, string clientid)
        {
            var model = GetCustomSourcesByID(sourceid, agentid, clientid);
            if (string.IsNullOrEmpty(sourcename))
            {
                sourcename = model.SourceName;
            }
            bool bl = SystemDAL.BaseProvider.UpdateCustomSource(sourceid, sourcename, ischoose, clientid);
            if (bl)
            {
                model.SourceName = sourcename;
                model.IsChoose = ischoose;
            }
            return bl;
        }

        public bool DeleteCustomSource(string sourceid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetCustomSourcesByID(sourceid, agentid, clientid);
            //系统默认来源不能删除
            if (model.IsSystem == 1)
            {
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteCustomSource(sourceid, clientid);
            if (bl)
            {
                CustomSources[clientid].Remove(model);
            }
            return bl;
        }

        public bool UpdateCustomStage(string stageid, string name, string userid, string ip, string agentid, string clientid)
        {
            var model = GetCustomStageByID(stageid, agentid, clientid);

            bool bl = SystemDAL.BaseProvider.UpdateCustomStage(stageid, name, clientid);
            if (bl)
            {
                model.StageName = name;
            }
            return bl;
        }

        public bool DeleteCustomStage(string stageid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetCustomStageByID(stageid, agentid, clientid);
            //新客户和成交客户不能删除
            if (model.Mark != 0)
            {
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteCustomStage(stageid, userid, clientid);
            if (bl)
            {
                CustomStages[clientid].Remove(model);

                var list = CustomStages[clientid].Where(m => m.Sort > model.Sort && m.Status == 1).ToList();
                foreach (var stage in list)
                {
                    stage.Sort -= 1;
                }
            }
            return bl;
        }

        public bool UpdateStageItem(string itemid, string name, string stageid, string processid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderStageByID(stageid, processid, agentid, clientid);

            bool bl = CommonBusiness.Update("StageItem", "ItemName", name, "ItemID='" + itemid + "'");
            if (bl)
            {
                var item = model.StageItem.Where(m => m.ItemID == itemid).FirstOrDefault();
                item.ItemName = name;
            }
            return bl;
        }

        public bool DeleteStageItem(string itemid, string stageid, string processid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderStageByID(stageid, processid, agentid, clientid);

            bool bl = CommonBusiness.Update("StageItem", "Status", "9", "ItemID='" + itemid + "'");
            if (bl)
            {
                var item = model.StageItem.Where(m => m.ItemID == itemid).FirstOrDefault();
                model.StageItem.Remove(item);
            }
            return bl;
        }

        public bool UpdateOrderProcess(string processid, string name, int days, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderProcessByID(processid, agentid, clientid);
            bool bl = SystemDAL.BaseProvider.UpdateOrderProcess(processid, name, days);
            if (bl)
            {
                model.ProcessName = name;
                model.PlanDays = days;
            }
            return bl;
        }

        public bool DeleteOrderProcess(string processid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderProcessByID(processid, agentid, clientid);
            //默认流程不能删除
            if (model.IsDefault == 1)
            {
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteOrderProcess(processid);
            if (bl)
            {
                OrderProcess[clientid].Remove(model);
            }
            return bl;
        }

        public bool UpdateOrderProcessOwner(string processid, string ownerid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderProcessByID(processid, agentid, clientid);

            if (ownerid == model.OwnerID)
            {
                return true;
            }

            bool bl = SystemDAL.BaseProvider.UpdateOrderProcessOwner(processid, ownerid);
            if (bl)
            {
                model.OwnerID = ownerid;
                model.Owner = OrganizationBusiness.GetUserByUserID(ownerid, agentid);
            }
            return bl;
        }

        public bool UpdateOrderProcessDefault(string processid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderProcessByID(processid, agentid, clientid);
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

        public bool UpdateOrderStage(string stageid, string stagename, string processid,  string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderStageByID(stageid, processid, agentid, clientid);

            bool bl = SystemDAL.BaseProvider.UpdateOrderStage(stageid, stagename, clientid);
            if (bl)
            {
                model.StageName = stagename;
            }
            return bl;
        }

        public bool DeleteOrderStage(string stageid, string processid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderStageByID(stageid, processid, agentid, clientid);
            //新订单和内置不能删除
            if (model.Mark != 0)
            {
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteOrderStage(stageid, processid, userid, clientid);
            if (bl)
            {
                OrderStages[processid].Remove(model);

                var list = OrderStages[processid].Where(m => m.Sort > model.Sort && m.Status == 1).ToList();
                foreach (var stage in list)
                {
                    stage.Sort -= 1;
                }
            }
            return bl;
        }

        public bool UpdateOrderStageOwner(string stageid, string processid, string ownerid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderStageByID(stageid, processid, agentid, clientid);

            if (ownerid == model.OwnerID)
            {
                return true;
            }

            bool bl = SystemDAL.BaseProvider.UpdateOrderStageOwner(stageid, ownerid);
            if (bl)
            {
                model.OwnerID = ownerid;
                model.Owner = OrganizationBusiness.GetUserByUserID(ownerid, agentid);
            }
            return bl;
        }

        public bool UpdateOrderType(string typeid, string typename, string typecode, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderTypeByID(typeid, agentid, clientid);

            bool bl = SystemDAL.BaseProvider.UpdateOrderType(typeid, typename, typecode, clientid);
            if (bl)
            {
                model.TypeName = typename;
                model.TypeCode = typecode;
            }
            return bl;
        }

        public bool DeleteOrderType(string typeid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderTypeByID(typeid, agentid, clientid);

            bool bl = CommonBusiness.Update("OrderType", "Status", "9", "TypeID='" + typeid + "'");
            if (bl)
            {
                OrderTypes[clientid].Remove(model);
            }
            return bl;
        }

        public bool UpdateTeam(string teamid, string name, string userid, string ip, string agentid, string clientid)
        {
            var model = GetTeamByID(teamid, agentid);

            bool bl = CommonBusiness.Update("Teams", "TeamName", name, "TeamID='" + teamid + "'");
            if (bl)
            {
                model.TeamName = name;
            }
            return bl;
        }

        public bool DeleteTeam(string teamid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetTeamByID(teamid, agentid);

            bool bl = SystemDAL.BaseProvider.DeleteTeam(teamid, userid, agentid);
            if (bl)
            {
                var list = OrganizationBusiness.GetUsers(agentid).Where(m => m.TeamID == teamid).ToList();
                foreach (var user in list)
                {
                    user.TeamID = "";
                }
                Teams[agentid].Remove(model);
            }
            return bl;
        }

        public bool UpdateUserTeamID(string userid, string teamid, string agentid, string operateid, string ip, string clientid)
        {
            var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
            
            bool bl = SystemDAL.BaseProvider.UpdateUserTeamID(userid, teamid, operateid, agentid);
            if (bl)
            {
                
                if (string.IsNullOrEmpty(teamid))
                {
                    var team = GetTeamByID(model.TeamID, agentid);
                    var user = team.Users.Where(m => m.UserID == userid).FirstOrDefault();
                    team.Users.Remove(user);
                }
                else
                {
                    var team = GetTeamByID(teamid, agentid);
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

        public bool UpdateDepotSeat(string id, string depotcode, string name, int status, string description, string operateid, string clientid)
        {
            return SystemDAL.BaseProvider.UpdateDepotSeat(id, depotcode, name, status, description);
        }

        public bool UpdateDepotSeatStatus(string id, EnumStatus status, string operateid, string clientid)
        {
            return CommonBusiness.Update("DepotSeat", "Status", (int)status, " DepotID='" + id + "'");
        }

        #endregion

    }
}
