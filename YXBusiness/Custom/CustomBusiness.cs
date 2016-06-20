using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using IntFactoryDAL;
using IntFactoryEntity;
using System.Data;
using IntFactoryEnum;

namespace IntFactoryBusiness
{
    public class CustomBusiness
    {
        public static CustomBusiness BaseBusiness = new CustomBusiness();

        #region 查询

        /// <summary>
        /// 公司规模
        /// </summary>
        /// <returns></returns>
        public static List<ExtentEntity> GetExtents()
        {
            List<ExtentEntity> list = new List<ExtentEntity>();
            list.Add(new ExtentEntity() { ExtentID = "1", ExtentName = "0-49人" });
            list.Add(new ExtentEntity() { ExtentID = "2", ExtentName = "50-99人" });
            list.Add(new ExtentEntity() { ExtentID = "3", ExtentName = "100-199人" });
            list.Add(new ExtentEntity() { ExtentID = "4", ExtentName = "200-499人" });
            list.Add(new ExtentEntity() { ExtentID = "5", ExtentName = "500-999人" });
            list.Add(new ExtentEntity() { ExtentID = "6", ExtentName = "1000人以上" });
            return list;
        }


        public List<CustomerEntity> GetCustomers(EnumSearchType searchtype, int type, int sourcetype, string sourceid, string stageid, int status, int mark, string activityid, string searchuserid, string searchteamid, string searchagentid,
                                                 string begintime, string endtime, string firstname, string keyWords, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<CustomerEntity> list = new List<CustomerEntity>();
            DataSet ds = CustomDAL.BaseProvider.GetCustomers((int)searchtype, type, sourcetype, sourceid, stageid, status, mark, activityid, searchuserid, searchteamid, searchagentid, begintime, endtime, firstname, keyWords, orderBy, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CustomerEntity model = new CustomerEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.City = CommonBusiness.Citys.Where(m => m.CityCode == model.CityCode).FirstOrDefault();

                list.Add(model);
            }
            return list;
        }

        public List<CustomerEntity> GetCustomersByActivityID(string activityid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<CustomerEntity> list = new List<CustomerEntity>();
            string sqlWhere = " ActivityID='" + activityid + "' and status<>9";

            DataTable dt = CommonBusiness.GetPagerData("Customer", "*", sqlWhere, "CustomerID", pageSize, pageIndex, out totalCount, out pageCount);
            foreach (DataRow dr in dt.Rows)
            {
                CustomerEntity model = new CustomerEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Source = SystemBusiness.BaseBusiness.GetCustomSourcesByID(model.SourceID, model.AgentID, model.ClientID);
                model.Stage = SystemBusiness.BaseBusiness.GetCustomStageByID(model.StageID, model.AgentID, model.ClientID);
                list.Add(model);
            }
            return list;
        }

        public List<CustomerEntity> GetCustomersByKeywords(string keywords, string userid, string agentid, string clientid)
        {
            List<CustomerEntity> list = new List<CustomerEntity>();
            DataSet ds = CustomDAL.BaseProvider.GetCustomersByKeywords(keywords, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CustomerEntity model = new CustomerEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Source = SystemBusiness.BaseBusiness.GetCustomSourcesByID(model.SourceID, model.AgentID, model.ClientID);
                //model.Stage = SystemBusiness.BaseBusiness.GetCustomStageByID(model.StageID, model.AgentID, model.ClientID);
                list.Add(model);
            }
            return list;
        }

        public CustomerEntity GetCustomerByID(string customerid, string agentid, string clientid)
        {
            DataSet ds = CustomDAL.BaseProvider.GetCustomerByID(customerid, agentid, clientid);
            CustomerEntity model = new CustomerEntity();
            if (ds.Tables["Customer"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Customer"].Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.City = CommonBusiness.Citys.Where(m => m.CityCode == model.CityCode).FirstOrDefault();

                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

            }
            return model;
        }

        public List<ContactEntity> GetContactsByCustomerID(string customerid, string agentid)
        {
            List<ContactEntity> list = new List<ContactEntity>();

            DataTable dt = CustomDAL.BaseProvider.GetContactsByCustomerID(customerid);
            foreach (DataRow dr in dt.Rows)
            {
                ContactEntity model = new ContactEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                model.City = CommonBusiness.Citys.Where(m => m.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }

            return list;
        }

        public ContactEntity GetContactByID(string contactid)
        {
            ContactEntity model = new ContactEntity();
            DataTable dt = CustomDAL.BaseProvider.GetContactByID(contactid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        public static List<ReplyEntity> GetReplys(string guid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();
            string whereSql = " Status<>9 and GUID='" + guid + "' ";
            DataTable dt = CommonBusiness.GetPagerData("CustomerReply", "*", whereSql, "AutoID", "CreateTime desc ", pageSize, pageIndex, out totalCount, out pageCount, false);

            foreach (DataRow dr in dt.Rows)
            {
                ReplyEntity model = new ReplyEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                if (!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }

            return list;

        }

        #endregion

        #region 添加

        public string CreateCustomer(string name, int type, string sourceid, string activityid, string industryid, int extent, string citycode, string address, 
                                     string contactname, string mobile, string officephone, string email, string jobs, string desc, string ownerid, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            bool bl = CustomDAL.BaseProvider.CreateCustomer(id, name, type, sourceid, activityid, industryid, extent, citycode, address, contactname, mobile, officephone, email, jobs, desc, ownerid, operateid, agentid, clientid);
            if (!bl)
            {
                id = "";
            }
            else
            {
                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Customer, EnumLogType.Create, "", operateid, agentid, clientid);
            }
            return id;
        }

        public static string CreateReply(string guid, string content, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            return CustomDAL.BaseProvider.CreateReply(guid, content, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);
        }

        public string CreateContact(string customerid,string name, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            bool bl = CustomDAL.BaseProvider.CreateContact(id, customerid, name, citycode, address, mobile, officephone, email, jobs, desc, operateid, agentid, clientid);
            if (!bl)
            {
                id = "";
            }
            return id;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateCustomer(string customerid, string name, int type, string industryid, int extent, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomer(customerid, name, type, industryid, extent, citycode, address, mobile, officephone, email, jobs, desc, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "编辑客户信息";
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerStage(string customerid, string stageid, string operateid, string ip,string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomerStage(customerid, stageid, operateid, agentid, clientid);
            if (bl)
            {
                var model = SystemBusiness.BaseBusiness.GetCustomStageByID(stageid, agentid, clientid);
                string msg = "客户阶段更换为：" + model.StageName;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, stageid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerOwner(string customerid, string userid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomerOwner(customerid, userid, operateid, agentid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
                string msg = "客户负责人更换为：" + model.Name;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, userid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerAgent(string customerid, string newagentid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomerAgent(customerid, newagentid, operateid, agentid, clientid);
            return bl;
        }

        public bool UpdateCustomerStatus(string customerid, EnumCustomStatus status, string operateid, string ip, string agentid, string clientid)
        {

            bool bl = CommonBusiness.Update("Customer", "Status", (int)status, "CustomerID='" + customerid + "'");
            if (bl)
            {
                var model = CommonBusiness.GetEnumDesc(status);
                string msg = "客户状态更换为：" + model;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, status.ToString(), agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerMark(string customerid, int mark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CommonBusiness.Update("Customer", "Mark", mark, "CustomerID='" + customerid + "'");
            if (bl)
            {
                string msg = "标记客户颜色";
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, mark.ToString(), agentid, clientid);
            }
            return bl;
        }
       
        public bool UpdateContact(string contactid, string customerid, string name, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string operateid, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateContact(contactid, customerid, name, citycode, address, mobile, officephone, email, jobs, desc, operateid, agentid, clientid);

            return bl;
        }

        public bool DeleteContact(string contactid, string ip, string userid, string agentid)
        {
            bool bl = CommonBusiness.Update("Contact", "Status", 9, "ContactID='" + contactid + "'");
            return bl;
        }

        public bool DeleteReply(string replyid)
        {
            bool bl = CommonBusiness.Update("CustomerReply", "Status", 9, "ReplyID='" + replyid + "'");
            return bl;
        }

        #endregion
    }
}
