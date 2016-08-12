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
using System.Data.SqlClient;

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

        public List<CustomerEntity> GetCustomers(EnumSearchType searchtype, int type, int sourcetype, string sourceid, string stageid, int status, int mark, string searchuserid, string searchteamid, 
                                                 string begintime, string endtime, string firstname, string keyWords, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string clientid)
        {
            List<CustomerEntity> list = new List<CustomerEntity>();
            DataSet ds = CustomDAL.BaseProvider.GetCustomers((int)searchtype, type, sourcetype, sourceid, stageid, status, mark, searchuserid, searchteamid, begintime, endtime, firstname, keyWords, orderBy, pageSize, pageIndex, ref totalCount, ref pageCount, userid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CustomerEntity model = new CustomerEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);
                model.City = CommonBusiness.Citys.Where(m => m.CityCode == model.CityCode).FirstOrDefault();

                list.Add(model);
            }
            return list;
        }

        public List<CustomerEntity> GetCustomersByKeywords(string keywords, string userid, string clientid)
        {
            List<CustomerEntity> list = new List<CustomerEntity>();
            DataSet ds = CustomDAL.BaseProvider.GetCustomersByKeywords(keywords, userid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CustomerEntity model = new CustomerEntity();
                model.FillData(dr);
                model.City = CommonBusiness.GetCityByCode(model.CityCode);
                list.Add(model);
            }
            return list;
        }

        public CustomerEntity GetCustomerByID(string customerid,string clientid)
        {
            DataSet ds = CustomDAL.BaseProvider.GetCustomerByID(customerid, clientid);
            CustomerEntity model = new CustomerEntity();
            if (ds.Tables["Customer"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Customer"].Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.ClientID);

                model.City = CommonBusiness.Citys.Where(m => m.CityCode == model.CityCode).FirstOrDefault();

                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.ClientID);

            }
            return model;
        }

        public List<ContactEntity> GetContactsByCustomerID(string customerid, string clientid)
        {
            List<ContactEntity> list = new List<ContactEntity>();

            DataTable dt = CustomDAL.BaseProvider.GetContactsByCustomerID(customerid);
            foreach (DataRow dr in dt.Rows)
            {
                ContactEntity model = new ContactEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.ClientID);
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

        #endregion

        #region 添加

        public string CreateCustomer(string name, int type, string sourceid, string industryid, int extent, string citycode, string address, 
                                     string contactname, string mobile, string officephone, string email, string jobs, string desc, string ownerid, string operateid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            bool bl = CustomDAL.BaseProvider.CreateCustomer(id, name, type, sourceid, industryid, extent, citycode, address, contactname, mobile, officephone, email, jobs, desc, ownerid, operateid, clientid);
            if (!bl)
            {
                id = "";
            }
            else
            {
                //日志
                LogBusiness.AddActionLog(IntFactoryEnum.EnumSystemType.Client, IntFactoryEnum.EnumLogObjectType.Customer, EnumLogType.Create, "", operateid, clientid);
            }
            return id;
        }

        public string CreateContact(string customerid,string name, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string operateid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            bool bl = CustomDAL.BaseProvider.CreateContact(id, customerid, name, citycode, address, mobile, officephone, email, jobs, desc, operateid, clientid);
            if (!bl)
            {
                id = "";
            }
            return id;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateCustomer(string customerid, string name, int type, string industryid, int extent, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string operateid, string ip, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomer(customerid, name, type, industryid, extent, citycode, address, mobile, officephone, email, jobs, desc, operateid, clientid);
            if (bl)
            {
                string msg = "编辑客户信息";
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, "", clientid);
            }
            return bl;
        }

        public bool UpdateCustomerOwner(string customerid, string userid, string operateid, string ip, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomerOwner(customerid, userid, operateid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, clientid);
                string msg = "客户负责人更换为：" + model.Name;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, userid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerStatus(string customerid, EnumCustomStatus status, string operateid, string ip, string clientid)
        {

            bool bl = CommonBusiness.Update("Customer", "Status", (int)status, "CustomerID='" + customerid + "'");
            if (bl)
            {
                var model = CommonBusiness.GetEnumDesc(status);
                string msg = "客户状态更换为：" + model;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, status.ToString(), clientid);
            }
            return bl;
        }

        public bool UpdateCustomerMark(string customerid, int mark, string operateid, string ip, string clientid)
        {
            bool bl = CommonBusiness.Update("Customer", "Mark", mark, "CustomerID='" + customerid + "'");
            if (bl)
            {
                var color = SystemBusiness.BaseBusiness.GetLableColorColorID(clientid, mark, EnumMarkType.Customer);
                string msg = color != null ? "客户更换标签：" + color.ColorName : "标记客户标签";
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, mark.ToString(), clientid);
            }
            return bl;
        }
       
        public bool UpdateContact(string contactid, string customerid, string name, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string operateid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateContact(contactid, customerid, name, citycode, address, mobile, officephone, email, jobs, desc, operateid, clientid);

            return bl;
        }

        public bool SetCustomerYXinfo(string customerID, string name, string mobilePhone, string clientID, string YXAgentID, string YXClientID, string YXClientCode)
        {
            return CustomDAL.BaseProvider.SetCustomerYXinfo(customerID, name, mobilePhone, clientID, YXAgentID, YXClientID, YXClientCode);
        }

        public bool DeleteContact(string contactid, string ip, string userid)
        {
            bool bl = CommonBusiness.Update("Contact", "Status", 9, "ContactID='" + contactid + "'");
            return bl;
        }

        #endregion
    }
}
