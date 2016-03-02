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
            string sqlWhere = "Status<>9";
            if (!string.IsNullOrEmpty(keyWords))
                sqlWhere += " and ( CompanyName like '%" + keyWords + "%'  or  MobilePhone like '%" + keyWords + "%')";

            DataTable dt = CommonBusiness.GetPagerData("Clients", "*", sqlWhere, "AutoID", pageSize, pageIndex, out totalCount, out pageCount);
            List<Clients> list = new List<Clients>();
            Clients model; 
            foreach (DataRow item in dt.Rows)
            {
                model = new Clients();
                model.FillData(item);

                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                model.IndustryEntity =Manage.IndustryBusiness.GetIndustrys().Where(i => i.IndustryID.ToLower() == model.Industry.ToLower()).FirstOrDefault();
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
                DataTable dt = ClientDAL.BaseProvider.GetClientDetail(clientID);
                Clients model = new Clients();
                if (dt.Rows.Count == 1)
                {
                    DataRow row = dt.Rows[0];
                    model.FillData(row);

                    model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                    model.IndustryEntity = Manage.IndustryBusiness.GetIndustrys().Where(i => i.IndustryID.ToLower() == model.Industry.ToLower()).FirstOrDefault();

                    Clients.Add(model.ClientID, model);
                }
                else
                    return null;
            }

            return Clients[clientID];
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
        public static string InsertClient(Clients model, string loginname, string bindMobilePhone, string loginPwd, string userid, out int result, string email = "", string mduserid = "", string mdprojectid = "")
        {
            loginPwd = CloudSalesTool.Encrypt.GetEncryptPwd(loginPwd, bindMobilePhone);

            string clientid = ClientDAL.BaseProvider.InsertClient(model.CompanyName, loginname, model.ContactName, model.MobilePhone, model.Industry, model.CityCode,
                                                             model.Address, model.Description, bindMobilePhone, loginPwd, email, mduserid, mdprojectid, userid, out result);

            return clientid;
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
                , model.CityCode, model.Address, model.Description,model.Logo,model.OfficePhone
                , userid);

            if (flag)
            {
                 if(Clients.ContainsKey(model.ClientID))
                     Clients[model.ClientID]=model;
            }

            return flag;

        }

        #endregion

    }
}
