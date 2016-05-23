using IntFactoryDAL.Manage;
using IntFactoryEntity.Manage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactoryBusiness.Manage
{
    public class ClientOrderAccountBusiness
    {
        /// <summary>
        /// 新增订单账目明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddClientOrderAccount(ClientOrderAccount model )
        {
            return ClientOrderAccountDAL.BaseProvider.InsertClientOrderAccount(model.OrderID,model.PayType,model.RealAmount,model.Type,model.ClientID,model.CreateUserID,model.Remark);
        }

        /// <summary>
        /// 获取客户订单账目列表
        /// </summary>
        public static List<ClientOrderAccount> GetClientOrderAccounts(string keyWords, string orderID, string clientID,int payType,int status,int type, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            string sqlWhere = "a.Status<>9";
            if (!string.IsNullOrEmpty(keyWords))
                sqlWhere += " and ( a.OrderID like '%" + keyWords + "%'  or  a.ClientID  like '%" + keyWords + "%' )";
            if (!string.IsNullOrEmpty(orderID))
                 sqlWhere += " and ( a.OrderID ='"+orderID+"' )";
            if (!string.IsNullOrEmpty(clientID))
                sqlWhere += " and ( a.ClientID ='" + clientID + "' )";
            if (payType>0)
                sqlWhere += " and ( a.PayType ='" + payType + "' )";
            if (type > 0)
                sqlWhere += " and ( a.Type ='" + type + "' )";
            if (status > 0)
                sqlWhere += " and ( a.Status ='" + status + "' )";
            string sqlColumn = @" * ";
            DataTable dt = CommonBusiness.GetPagerData("ClientOrderAccount a", sqlColumn, sqlWhere, "a.AutoID", pageSize, pageIndex, out totalCount, out pageCount);
            List<ClientOrderAccount> list = new List<ClientOrderAccount>();
            ClientOrderAccount model;
            foreach (DataRow item in dt.Rows)
            {
                model = new ClientOrderAccount();
                model.FillData(item);
                if (!string.IsNullOrEmpty(model.CreateUserID))
                {
                    model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.ClientID);
                    if (string.IsNullOrEmpty(model.CreateUser.Name)){
                        M_Users mUser = M_UsersBusiness.GetUserDetail(model.CreateUserID);
                        model.CreateUser.Name = mUser != null ? mUser.Name : "";
                        model.CreateUser.UserID = model.CreateUserID;
                    }
                }
                if(!string.IsNullOrEmpty(model.CheckUserID))
                    model.CheckerUser=M_UsersBusiness.GetUserDetail(model.CheckUserID);
                list.Add(model);
            }

            return list;
        }


        public static ClientOrderAccount GetClientOrderAccountByAutoID(string autoid) {
          DataTable dt=  ClientOrderAccountDAL.BaseProvider.ClientOrderAccountByAutoIDDAL(autoid);
          ClientOrderAccount model = new ClientOrderAccount();
          if (dt.Rows.Count == 1)
          {
              DataRow row = dt.Rows[0];
              model.FillData(row);
              return model;
          }
          else
              return null;
        }


        #region 修改
        public static int UpdateClientOrderAccountStatus(string autoID, int status)
        {
            return ClientOrderAccountDAL.BaseProvider.UpdateClientOrderAccountStatus(autoID, status);             
        } 

        #endregion
    }
}
