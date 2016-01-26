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

        private static Dictionary<string, List<OpportunityStageEntity>> _opportunitystages;

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


        /// <summary>
        /// 机会阶段
        /// </summary>
        private static Dictionary<string, List<OpportunityStageEntity>> OpportunityStages
        {
            get
            {
                if (_opportunitystages == null)
                {
                    _opportunitystages = new Dictionary<string, List<OpportunityStageEntity>>();
                }
                return _opportunitystages;
            }
            set
            {
                _opportunitystages = value;
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

        public List<OpportunityStageEntity> GetOpportunityStages(string agentid, string clientid)
        {
            if (OpportunityStages.ContainsKey(clientid))
            {
                return OpportunityStages[clientid].OrderBy(m => m.Probability).ToList();
            }

            List<OpportunityStageEntity> list = new List<OpportunityStageEntity>();
            DataSet ds = SystemDAL.BaseProvider.GetOpportunityStages(clientid);
            foreach (DataRow dr in ds.Tables["Stages"].Rows)
            {
                OpportunityStageEntity model = new OpportunityStageEntity();
                model.FillData(dr);

                list.Add(model);
            }
            OpportunityStages.Add(clientid, list);

            return list;
        }

        public OpportunityStageEntity GetOpportunityStageByID(string stageid, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(stageid))
            {
                return null;
            }
            var list = GetOpportunityStages(agentid, clientid);
            if (list.Where(m => m.StageID == stageid).Count() > 0)
            {
                return list.Where(m => m.StageID == stageid).FirstOrDefault();
            }

            OpportunityStageEntity model = new OpportunityStageEntity();
            DataTable dt = SystemDAL.BaseProvider.GetOpportunityStageByID(stageid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                OpportunityStages[clientid].Add(model);
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

        /// <summary>
        /// 获取货位列表
        /// </summary>
        /// <param name="keyWords">关键词</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总记录数</param>
        /// <param name="pageCount">总页数</param>
        /// <param name="clientID">客户端ID</param>
        /// <returns></returns>
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

        public string CreateStageItem(string name, string stageid, string userid, string agentid, string clientid)
        {
            string itemid = Guid.NewGuid().ToString().ToLower();

            bool bl = SystemDAL.BaseProvider.CreateStageItem(itemid, name, stageid, userid, clientid);
            if (bl)
            {
                var model = GetCustomStageByID(stageid, agentid, clientid);
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

        public string CreateOpportunityStage(string stagename, decimal probability, string userid, string agentid, string clientid)
        {
            string guid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateOpportunityStage(guid, stagename, probability, userid, clientid);
            if (bl)
            {
                if (!OpportunityStages.ContainsKey(clientid))
                {
                    GetOpportunityStages(agentid, clientid);
                }

                OpportunityStages[clientid].Add(new OpportunityStageEntity()
                {
                    StageID = guid.ToLower(),
                    StageName = stagename,
                    Probability = probability,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    ClientID = clientid
                });

                return guid;
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

        public bool UpdateStageItem(string itemid, string name, string stageid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetCustomStageByID(stageid, agentid, clientid);

            bool bl = CommonBusiness.Update("StageItem", "ItemName", name, "ItemID='" + itemid + "'");
            if (bl)
            {
                var item = model.StageItem.Where(m => m.ItemID == itemid).FirstOrDefault();
                item.ItemName = name;
            }
            return bl;
        }

        public bool DeleteStageItem(string itemid, string stageid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetCustomStageByID(stageid, agentid, clientid);

            bool bl = CommonBusiness.Update("StageItem", "Status", "9", "ItemID='" + itemid + "'");
            if (bl)
            {
                var item = model.StageItem.Where(m => m.ItemID == itemid).FirstOrDefault();
                model.StageItem.Remove(item);
            }
            return bl;
        }

        public bool UpdateOpportunityStage(string stageid, string stagename, decimal probability, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOpportunityStageByID(stageid, agentid, clientid);

            bool bl = SystemDAL.BaseProvider.UpdateOpportunityStage(stageid, stagename, probability, clientid);
            if (bl)
            {
                model.StageName = stagename;
                model.Probability = probability;
            }
            return bl;
        }

        public bool DeleteOpportunityStage(string stageid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOpportunityStageByID(stageid, agentid, clientid);
            //新客户和成交客户不能删除
            if (model.Mark != 0)
            {
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteOpportunityStage(stageid, userid, clientid);
            if (bl)
            {
                OpportunityStages[clientid].Remove(model);
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
