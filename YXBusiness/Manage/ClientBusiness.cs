using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Web;

using IntFactoryEntity.Manage;
using IntFactoryDAL.Manage;
using CloudSalesTool;
using IntFactoryEntity.Manage.Report;
using IntFactoryEnum;


namespace IntFactoryBusiness.Manage
{
    public class ClientBusiness
    {
        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public static string FILEPATH = CloudSalesTool.AppSettings.Settings["UploadFilePath"] + "Logo/" + DateTime.Now.ToString("yyyyMM") + "/";
        public static string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];

        #region Cache
        private static Dictionary<string,Clients> _cacheClients;

        /// <summary>
        /// 缓存客户端信息
        /// </summary>
        public static Dictionary<string, Clients> Clients
        {
            get
            {
                if (_cacheClients == null)
                {
                    _cacheClients = new Dictionary<string, Clients>();
                }
                return _cacheClients;
            }
            set
            {
                _cacheClients = value;
            }
        }
        #endregion

        #region 查询

        public static List<Clients> GetClients(string keyWords, EnumRegisterType type, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string sqlWhere = "Status<>9";
            if (!string.IsNullOrEmpty(keyWords))
            {
                sqlWhere += " and ( MobilePhone = '" + keyWords + "' or  ClientCode = '" + keyWords + "' ";
                if (keyWords.Length > 3) {
                    sqlWhere += " or CompanyName like '%" + keyWords + "%'  ";
                }
                sqlWhere += ")";
            }
            if ((int)type > -1)
            {
                sqlWhere += " and RegisterType=  " + (int)type; 
            }
            string sqlColumn = @" * ";
            bool isAsc=false;
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "AutoID";
            }
            else {
                isAsc = orderBy.IndexOf(" asc") > -1 ? true : false; 
                orderBy = orderBy.Replace(" desc", "").Replace(" asc", "");
            }
            DataTable dt = CommonBusiness.GetPagerData("Clients", sqlColumn, sqlWhere, orderBy, pageSize, pageIndex, out totalCount, out pageCount, isAsc);
            List<Clients> list = new List<Clients>();
            Clients model; 
            foreach (DataRow item in dt.Rows)
            {
                model = new Clients();
                model.FillData(item);

                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
               // model.IndustryEntity =Manage.IndustryBusiness.GetIndustrys().Where(i => i.IndustryID.ToLower() == model.Industry.ToLower()).FirstOrDefault();
                list.Add(model);
            }

            return list;
        }

        public static Clients GetClientDetail(string clientid)
        {
            if (!Clients.ContainsKey(clientid))
            {
                Clients model = GetClientDetailBase(clientid);
                if (model != null)
                {
                    Clients.Add(model.ClientID, model);
                }
                else
                {
                    return null;
                }
            }

            return Clients[clientid];
        }

        public static Clients GetClientDetailBase(string clientID)
        {            
            DataTable dt = ClientDAL.BaseProvider.GetClientDetail(clientID);
            Clients model = new Clients();
            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                model.FillData(row);

                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();

                return model;
            }
            else
            {
                return null;
            }
        }

        public static List<ClientsDateEntity> GetClientsGrow(int type, string begintime, string endtime)
        {
            List<ClientsDateEntity> list = new List<ClientsDateEntity>();

            DataTable dt = ClientDAL.BaseProvider.GetClientsGrow(type, begintime, endtime);
            foreach (DataRow dr in dt.Rows)
            {
                ClientsDateEntity model = new ClientsDateEntity();
                model.Name = dr["CreateTime"].ToString();
                model.Value = int.Parse(dr["TotalNum"].ToString());               
                list.Add(model);
            }
            return list;
        }

        public static List<ClientsBaseEntity> GetClientsLoginReport(int type, string begintime, string endtime)
        {
            List<ClientsBaseEntity> list = new List<ClientsBaseEntity>();
            DataSet ds = ClientDAL.BaseProvider.GetClientsLoginReport(type, begintime, endtime);
            int k = 0;
            foreach (DataTable dt in ds.Tables)
            {
                List<ClientsItem> item = new List<ClientsItem>();
                foreach (DataRow dr in dt.Rows)
                {
                    ClientsItem model = new ClientsItem();
                    model.Name = dr["ReportDate"].ToString();
                    model.Value = int.Parse(dr["Num"].ToString());
                    item.Add(model);
                }                
                if (item.Any()) {
                    ClientsBaseEntity clientloginEntity = new ClientsBaseEntity
                    {
                        Name = (k == 0 ? "登录次数" : (k == 1 ? "登陆人数" : "登陆工厂数")),
                        Items = item
                    };
                    list.Add(clientloginEntity);
                }
                k++;
            }
            return list;
        }

        public static List<ClientsBaseEntity> GetClientsAgentActionReport(int type, string begintime, string endtime, string clientId)
        {
            List<ClientsBaseEntity> list = new List<ClientsBaseEntity>();
            DataSet ds = ClientDAL.BaseProvider.GetClientsAgentActionReport(type, begintime, endtime,clientId);
 
            if ( ds.Tables.Count>0)
            {               
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    if (dc.ColumnName != "ReportDate")
                    {
                        List<ClientsItem> item = new List<ClientsItem>();

                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            ClientsItem model = new ClientsItem();
                            model.Name = dr["ReportDate"].ToString();
                            model.Value = int.Parse(dr[dc.ColumnName].ToString());
                            item.Add(model);
                        }
                        ClientsBaseEntity clientloginEntity = new ClientsBaseEntity
                       {
                           Name =GetCloumnName(dc.ColumnName),
                           Items = item
                       };
                        list.Add(clientloginEntity);
                    }
                } 
            }
            return list;
        }

        private static string GetCloumnName(string cloumnName)
        {
            switch (cloumnName) {
                case "CustomerCount":
                    return "客户";
                case "OrdersCount":
                    return "订单";
                case "ActivityCount":
                    return "活动";
                case "ProductCount":
                    return "产品";
                case "UsersCount":
                    return "员工";
                case "AgentCount":
                    return "代理商";
                case "OpportunityCount":
                    return "机会";
                case "PurchaseCount":
                    return "采购";
                case "WarehousingCount":
                    return "出库";
                case "TaskCount":
                    return "任务";
                case "DownOrderCount":
                    return "拉取阿里订单";
                case "ProductOrderCount":
                    return "生产订单";
                default:
                    return "";
            }
        }

        public static List<ClientVitalityEntity> GetClientsVitalityReport(int type, string begintime, string endtime, string clientId)
        {
            List<ClientVitalityEntity> list = new List<ClientVitalityEntity>();
            DataSet ds = ClientDAL.BaseProvider.GetClientsVitalityReport(type, begintime, endtime,clientId);
            string clientName = "当前";
            if (!string.IsNullOrEmpty(clientId))
            { 
                 Clients client=   GetClientDetail(clientId);
                 if (client != null && !string.IsNullOrEmpty(client.CompanyName))
                 {
                     clientName = client.CompanyName;
                 }
            }
            List<ClientVitalityItem> sysReport = new List<ClientVitalityItem>();
            List<ClientVitalityItem> clientReport = new List<ClientVitalityItem>();
            foreach (DataRow dr in ds.Tables["SystemReport"].Rows)
            {
                sysReport.Add(new ClientVitalityItem() { Name = dr["ReportDate"].ToString(), Value = Convert.ToDecimal(dr["Vitality"]) });
                if (ds.Tables["ClientReport"].Rows.Count > 0)
                {
                    DataRow[] drs = ds.Tables["ClientReport"].Select("ReportDate='" + dr["ReportDate"].ToString() + "'");
                    decimal clientValue = drs.Count() > 0 ? Convert.ToDecimal(drs.FirstOrDefault()["Vitality"]) : (decimal)0.0000;
                    clientReport.Add(new ClientVitalityItem() { Name = dr["ReportDate"].ToString(), Value = clientValue });
                }
                else { clientReport.Add(new ClientVitalityItem() { Name = dr["ReportDate"].ToString(), Value = (decimal)0.0000 }); }

            }
            list.Add(new ClientVitalityEntity() { Name = "系统均值", Items = sysReport });
            list.Add(new ClientVitalityEntity() { Name = clientName, Items = clientReport });

            return list;
        }

        public static List<ClientAuthorizeLog> GetClientAuthorizeLogs(string clientID,string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string sqlWhere =" Status<>9 and ClientID='" + clientID+"' ";
            DataTable dt = CommonBusiness.GetPagerData("ClientAuthorizeLog", "*", sqlWhere, "AutoID", pageSize, pageIndex, out totalCount, out pageCount);

            List<ClientAuthorizeLog> list = new List<ClientAuthorizeLog>();
            ClientAuthorizeLog model;
            foreach (DataRow item in dt.Rows)
            {
                model = new ClientAuthorizeLog();
                model.FillData(item);
                list.Add(model);
            }

            return list;
        }
        #endregion

        #region 添加
        public static bool UpdateClientCache(string clientid,Clients client) {
            if (Clients.ContainsKey(clientid)) 
            {
                Clients[clientid] = client;
            }

            return true;
        }

        public static string InsertClient(EnumRegisterType registerType, EnumAccountType accountType, string account, string loginPwd, string clientName, string contactName, string mobile, string email, string industry, string citycode, string address, string remark,
                                          string companyid, string operateid, out int result, out string userid)
        {
            loginPwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginPwd, account);

            string clientid = ClientDAL.BaseProvider.InsertClient((int)registerType, (int)accountType, account, loginPwd, clientName, contactName, mobile, email, industry, citycode, address, remark, companyid, operateid, out result, out userid);

            return clientid;
        }

        public static bool AddClientUserQuantity(string clientid, int quantity)
        {
            return ClientDAL.BaseProvider.AddClientUserQuantity(clientid, quantity);
        }

        public static bool SetClientEndTime(string clientid, DateTime endTime)
        {
            return ClientDAL.BaseProvider.SetClientEndTime(clientid, endTime);
        }

        public static bool InsertClientAuthorizeLog(ClientAuthorizeLog model)
        {
            return ClientDAL.BaseProvider.InsertClientAuthorizeLog(model.ClientID, model.OrderID, model.UserQuantity, model.BeginTime, model.EndTime, model.Type);
        }

        #endregion

        #region 删
        /// <summary>
        /// 删除客户端
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static bool DeleteClient(string clientID)
        {
            bool flag = ClientDAL.BaseProvider.DeleteClient(clientID);

            if (flag)
                Clients.Remove(clientID);

            return flag;
        }
        #endregion

        #region  编辑

        public static bool UpdateClient(Clients model, string userid)
        {
            bool flag = ClientDAL.BaseProvider.UpdateClient(model.ClientID, model.CompanyName
                , model.ContactName, model.MobilePhone, model.Industry
                , model.CityCode, model.Address, model.Description, model.Logo == null ? "" : model.Logo, model.OfficePhone
                , userid);

            if (flag)
            {
                if (Clients.ContainsKey(model.ClientID))
                {
                    Clients[model.ClientID] = GetClientDetailBase(model.ClientID);
                }
            }

            return flag;

        }

        public static void UpdateClientCache(string clientID)
        {
            if (Clients.ContainsKey(clientID))
            {
                Clients[clientID] = GetClientDetailBase(clientID);
            }
        }

        public static int SetClientProcess(string ids, string userid, string clientid)
        {
            var model = GetClientDetail(clientid);
            if (model.GuideStep != 1)
            {
                return model.GuideStep;
            }
            bool bl = ClientDAL.BaseProvider.SetClientProcess(ids, userid, clientid);

            Clients[model.ClientID] = GetClientDetailBase(model.ClientID);

            return Clients[model.ClientID].GuideStep;
        }

        public static int SetClientCategory(string ids, string userid, string clientid)
        {
            var model = GetClientDetail(clientid);
            if (model.GuideStep != 2)
            {
                return model.GuideStep;
            }
            bool bl = ClientDAL.BaseProvider.SetClientCategory(ids, userid, clientid);

            Clients[model.ClientID] = GetClientDetailBase(model.ClientID);

            return Clients[model.ClientID].GuideStep;

        }

        public static bool FinishInitSetting(string clientid)
        {
            var model = GetClientDetail(clientid);
            if (model.GuideStep != 3)
            {
                return false;
            }
            bool bl = ClientDAL.BaseProvider.FinishInitSetting(clientid);

            model.GuideStep = 0;

            return bl;

        }
        #endregion

    }
}
