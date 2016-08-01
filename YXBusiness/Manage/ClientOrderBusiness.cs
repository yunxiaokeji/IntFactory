using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using IntFactoryDAL.Manage;
using IntFactoryEntity.Manage;
using System.Data;
using IntFactoryEnum;
namespace IntFactoryBusiness.Manage
{
    public class ClientOrderBusiness
    {
        #region 增
        /// <summary>
        /// 新增后台客户订单
        /// </summary>
        public static string AddClientOrder(ClientOrder model)
        {
            string orderID = Guid.NewGuid().ToString();
            SqlConnection conn = new SqlConnection(ClientOrderDAL.ConnectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();

            try
            {
                bool bl = ClientOrderDAL.BaseProvider.AddClientOrder(orderID, model.UserQuantity, model.Years, model.Amount, model.RealAmount,model.Type, model.ClientID, model.CreateUserID,model.PayType,model.SystemType, model.SourceType,tran);
                if (bl)
                {
                    //单据明细
                    foreach (var detail in model.Details)
                    {
                        if (!ClientOrderDAL.BaseProvider.AddClientOrderDetail(orderID, detail.ProductID, detail.Price, detail.Qunatity, detail.CreateUserID, tran))
                        {
                            orderID = string.Empty;
                            tran.Rollback();
                            conn.Dispose();
                        }
                    }

                    tran.Commit();
                    conn.Dispose();
                }
                else
                {
                    orderID = string.Empty;
                    tran.Rollback();
                    conn.Dispose();
                }
            }
            catch
            {
                orderID = string.Empty;
                tran.Rollback();
                conn.Dispose();
            }

            return orderID;
        }

       /// <summary>
        ///  付款订单且授权客户
       /// </summary>
       /// <param name="orderID"></param>
       /// <param name="checkUserID"></param>
       /// <param name="payStatus">-1:不修改状态 0未付款 1全额付款 2部分付款</param>
       /// <returns></returns>      
        public static bool PayOrderAndAuthorizeClient(string orderID, string checkUserID, int payStatus, EnumOrderPayType payType)
        {
            return ClientOrderDAL.BaseProvider.PayOrderAndAuthorizeClient(orderID, checkUserID, payStatus, (int)payType);
        }
        public static bool PayClientOrder (string orderID ,int payStatus)
        {
            return ClientOrderDAL.BaseProvider.UpdateClientOrderPayStatus(orderID, payStatus);
        }
        #endregion

        #region 查
        /// <summary>
        /// 获取订单基本信息
        /// </summary>
        public static ClientOrder GetClientOrderInfo(string orderID)
        {
            DataTable dt = ClientOrderDAL.BaseProvider.GetClientOrderInfo(orderID);
            ClientOrder model = new ClientOrder();
            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                model.FillData(row);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.ClientID);
                if (string.IsNullOrEmpty(model.CreateUser.Name))
                {
                    M_Users mUser = M_UsersBusiness.GetUserDetail(model.CreateUserID);
                    if (mUser != null && !string.IsNullOrEmpty(mUser.Name))
                    {
                        model.CreateUser.Name = mUser.Name;
                    }
                }
                if (!string.IsNullOrEmpty(model.CheckUserID))
                {
                    model.CheckUser = OrganizationBusiness.GetUserByUserID(model.CheckUserID, model.ClientID);
                    if (string.IsNullOrEmpty(model.CheckUser.Name))
                    {
                        M_Users mUser = M_UsersBusiness.GetUserDetail(model.CheckUserID);
                        if (mUser != null && !string.IsNullOrEmpty(mUser.Name))
                        {
                            model.CheckUser.Name = mUser.Name;
                        }
                    }
                }
            }

            return model;
        }

        public static List<ClientOrder> GetClientOrders(int status, int type, string beginDate, string endDate, string clientID, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ClientOrder> list = GetBase("",status, type, beginDate, endDate, clientID,0, pageSize, pageIndex, ref totalCount, ref pageCount);
            return list;
        }
        public static List<ClientOrder> GetBase(string keyWords, int status, int type, string beginDate, string endDate, string clientID, int userType, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            DataTable dt = ClientOrderDAL.BaseProvider.GetClientOrders(keyWords,status, type, beginDate, endDate,clientID, pageSize, pageIndex, ref totalCount, ref pageCount);
            List<ClientOrder> list = new List<ClientOrder>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ClientOrder model = new ClientOrder();
                    model.FillData(row);
                    if (!string.IsNullOrEmpty(model.CheckUserID))
                    {
                        model.CheckUser = OrganizationBusiness.GetUserByUserID(model.CheckUserID, model.ClientID);
                        if (string.IsNullOrEmpty(model.CheckUser.Name))
                        {
                            M_Users mUser = M_UsersBusiness.GetUserDetail(model.CheckUserID);
                            if(mUser!=null && !string.IsNullOrEmpty(mUser.Name)){
                                model.CheckUser.Name = mUser.Name;
                            }
                        }                        
                    }
                    model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.ClientID);
                    if (string.IsNullOrEmpty(model.CreateUser.Name))
                    {
                        M_Users mUser = M_UsersBusiness.GetUserDetail(model.CreateUserID);
                        if (mUser != null && !string.IsNullOrEmpty(mUser.Name))
                        {
                            model.CreateUser.Name = mUser.Name;
                        }
                    }
                    list.Add(model);
                }
            }
            return list;
        } 
        #endregion

        #region 改
        /// <summary>
        /// 更改订单状态
        /// </summary>
        public static bool UpdateClientOrderStatus(string orderID, EnumClientOrderStatus status)
        {
            return ClientOrderDAL.BaseProvider.UpdateClientOrderStatus(orderID,(int)status);
        }

        /// <summary>
        /// 修改客户订单的支付金额
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static bool UpdateOrderAmount(string orderID, decimal amount) {
            return ClientOrderDAL.BaseProvider.UpdateOrderAmount(orderID, amount);
        }
        #endregion
    }
}
