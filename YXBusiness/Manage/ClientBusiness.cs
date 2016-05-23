using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using IntFactoryEntity.Manage;
using IntFactoryDAL.Manage;
using CloudSalesTool;
using System.IO;
using System.Web;
using IntFactoryEntity.Manage.Report;


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
        private static Dictionary<string, Clients> Clients
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

        /// <summary>
        /// 获取客户端列表
        /// </summary>
        public static List<Clients> GetClients(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string sqlWhere = "a.Status<>9";
            if (!string.IsNullOrEmpty(keyWords))
                sqlWhere += " and ( a.CompanyName like '%" + keyWords + "%'  or  a.MobilePhone like '%" + keyWords + "%' or  a.ClientCode like '%" + keyWords + "%')";

            string sqlColumn = @" a.AutoID,a.ClientID,a.ClientCode,a.CompanyName,a.Logo,a.Industry,
a.CityCode,a.Address,a.PostalCode,a.ContactName,a.MobilePhone,a.OfficePhone,
a.Status,b.EndTime,b.UserQuantity,a.TotalIn,a.TotalOut,a.FreezeMoney,
a.Description,a.AuthorizeType,a.IsDefault,a.AgentID,a.CreateTime,a.CreateUserID,a.AliMemberID ";
            DataTable dt = CommonBusiness.GetPagerData("Clients a  join Agents b on a.ClientID=b.ClientID", sqlColumn, sqlWhere, "a.AutoID", pageSize, pageIndex, out totalCount, out pageCount);
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

        /// <summary>
        /// 获取客户端详情
        /// </summary>
        public static Clients GetClientDetail(string clientID)
        {
            if (!Clients.ContainsKey(clientID))
            {
                Clients model = GetClientDetailBase(clientID);
                if (model != null) { 
                    Clients.Add(model.ClientID, model);
                }
                else
                    return null;
            }

            return Clients[clientID];
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
                model.IndustryEntity = Manage.IndustryBusiness.GetIndustrys().Where(i => i.IndustryID.ToLower() == model.Industry.ToLower()).FirstOrDefault();

                return model;
            }
            else
                return null;           
        }
        /// <summary>
        /// 获取工厂注册报表
        /// </summary>
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
        /// <summary>
        /// 获取工厂登陆报表
        /// </summary>
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
        /// <summary>
        /// 获取工厂行为报表
        /// </summary>
        public static List<ClientsBaseEntity> GetClientsAgentActionReport(int type, string begintime, string endtime, string clientId)
        {
            List<ClientsBaseEntity> list = new List<ClientsBaseEntity>();
            DataSet ds = ClientDAL.BaseProvider.GetClientsAgentActionReport(type, begintime, endtime,clientId);
            int k = 0;
            foreach (DataTable dt in ds.Tables)
            {
                List<ClientsItem> item = new List<ClientsItem>();
                foreach (DataRow dr in dt.Rows)
                {
                    ClientsItem model = new ClientsItem();
                    model.Name = dr["ReportDate"].ToString();
                    model.Value = int.Parse(dr["ReportValue"].ToString());
                    item.Add(model);
                }
                if (item.Any())
                {
                    ClientsBaseEntity clientloginEntity = new ClientsBaseEntity
                    {
                        Name = (k == 0 ? "客户" : (k == 1 ? "订单" : (k == 2 ? "材料" : (k == 3 ? "员工" : "采购")))),
                        Items = item
                    };
                    list.Add(clientloginEntity);
                }
                k++;
            }
            return list;
        }

        /// <summary>
        /// 获取工厂活跃度报表
        /// </summary>
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

        /// <summary>
        /// 获取客户端授权日志
        /// </summary>
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
        /// <summary>
        /// 更新客户缓存
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool UpdateClientCache(string clientID,Clients client) {
            if (Clients.ContainsKey(clientID)) {
                Clients[clientID] = client;
            }

            return true;
        }

        /// <summary>
        /// 添加客户端
        /// </summary>
        /// <param name="model">Clients 对象</param>
        /// <param name="loginName">账号</param>
        /// <param name="loginPwd">密码</param>
        /// <param name="userid">操作人</param>
        /// <param name="result">0：失败 1：成功 2：账号已存在 3：模块未选择</param>
        public static string InsertClient(Clients model, string loginname, string bindMobilePhone, string loginPwd, string userid, out int result, string email = "", string mduserid = "", string mdprojectid = "", string aliMemberID="")
        {
            loginPwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginPwd, bindMobilePhone);

            string clientid = ClientDAL.BaseProvider.InsertClient(model.CompanyName, loginname, model.ContactName, model.MobilePhone, model.Industry, model.CityCode,
                                                             model.Address, model.Description, bindMobilePhone, loginPwd, email, mduserid, mdprojectid, userid,aliMemberID, out result);

            return clientid;
        }

        public static bool BindClientAliMember(string clientID,string userID, string memberID) {

            bool flag= ClientDAL.BaseProvider.BindClientAliMember(clientID,userID, memberID);

            if (flag) {
                if (Clients.ContainsKey(clientID))
                    Clients[clientID].AliMemberID = memberID;
            }
            return flag;
        }


        /// <summary>
        /// 添加客户授权日志
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertClientAuthorizeLog(ClientAuthorizeLog model)
        {
            return ClientDAL.BaseProvider.InsertClientAuthorizeLog(model.ClientID,model.AgentID,model.OrderID,
                model.UserQuantity, model.BeginTime, model.EndTime, model.Type);
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
            bool flag= CommonBusiness.Update("Clients", "Status", 9, " ClientID='" + clientID + "'");

            if (flag)
                Clients.Remove(clientID);

            return flag;
        }
        #endregion

        #region  编辑
        /// <summary>
        /// 更新客户信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool UpdateClient(Clients model, string userid)
        {

            if (!string.IsNullOrEmpty(model.Logo) && model.Logo.IndexOf(TempPath) >= 0)
            {
                DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                if (!directory.Exists)
                {
                    directory.Create();
                }

                if (model.Logo.IndexOf("?") > 0)
                {
                    model.Logo = model.Logo.Substring(0, model.Logo.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(model.Logo));
                model.Logo = FILEPATH + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(model.Logo));
                }
            }

            bool flag= ClientDAL.BaseProvider.UpdateClient(model.ClientID, model.CompanyName
                , model.ContactName, model.MobilePhone, model.Industry
                , model.CityCode, model.Address, model.Description,model.Logo==null?"":model.Logo,model.OfficePhone
                , userid);

            if (flag)
            {
                if (Clients.ContainsKey(model.ClientID))
                    Clients[model.ClientID] = GetClientDetailBase(model.ClientID);
                     //Clients[model.ClientID]=model;
            }

            return flag;

        }

        public static void UpdatetClientCache(string clientID){
            if (Clients.ContainsKey(clientID))
            {
                Clients[clientID] = GetClientDetailBase(clientID);
            }                    
        }
        #endregion

    }
}
