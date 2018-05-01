using IntFactoryBusiness.Manage;
using IntFactoryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryBusiness
{
    public class OrderHelper
    {
        //单据查询工厂处理委托显示数据
        public static void HandleEntrustOrder(OrderEntity model, string userId, string clientId)
        {
            //未委托
            if (string.IsNullOrEmpty(model.EntrustClientID))
            {
                model.IsEntrustClient = false;
                model.Owner = OrganizationBusiness.GetUserCacheByUserID(model.OwnerID, model.ClientID);
            }
            //受委托工厂查看订单
            else if (model.EntrustClientID.ToLower() == clientId.ToLower())
            {
                model.IsEntrustClient = true;
                model.Owner = OrganizationBusiness.GetUserCacheByUserID(model.OwnerID,  model.EntrustClientID);
                var client = ClientBusiness.GetClientDetailBase(model.ClientID);
                model.CustomerID = client.ClientID;
                model.CustomerName = client.CompanyName + "（委托）";
                model.MobileTele = client.MobilePhone;
                model.PersonName = client.ContactName;
                model.City = client.City;
                model.Address = client.Address;
            }
            else //查看已委托给其他工厂的订单
            {
                var entrustClient = ClientBusiness.GetClientDetailBase(model.EntrustClientID);
                model.Owner = new CacheUserEntity() { UserID = "", Name = entrustClient.CompanyName + "（供应商）" };
            }
        }
    }
}
