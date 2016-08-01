using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntFactoryDAL;
using IntFactoryEntity;
using System.Data;
using IntFactoryEnum;

namespace IntFactoryBusiness
{
    public class FinanceBusiness
    {
        public static FinanceBusiness BaseBusiness = new FinanceBusiness();
        #region 查询

        public List<StorageBilling> GetPayableBills(int paystatus, int invoicestatus, string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string clientid)
        {
            List<StorageBilling> list = new List<StorageBilling>();
            DataSet ds = FinanceDAL.BaseProvider.GetPayableBills(paystatus, invoicestatus, begintime, endtime, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, userid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StorageBilling model = new StorageBilling();
                model.FillData(dr);

                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, clientid);

                model.PayStatusStr = model.PayStatus == 0 ? "未付款"
                                : model.PayStatus == 1 ? "部分付款"
                                : model.PayStatus == 2 ? "已付款"
                                : model.PayStatus == 9 ? "已删除"
                                : "";

                model.InvoiceStatusStr = model.InvoiceStatus == 0 ? "未开票"
                                : model.InvoiceStatus == 1 ? "部分开票"
                                : model.InvoiceStatus == 2 ? "已开票"
                                : model.InvoiceStatus == 9 ? "已删除"
                                : "";

                list.Add(model);
            }
            return list;
        }

        public StorageBilling GetPayableBillByID(string billingid, string clientid)
        {
            StorageBilling model = new StorageBilling();
            DataSet ds = FinanceDAL.BaseProvider.GetPayableBillByID(billingid, clientid);
            if (ds.Tables["Billing"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Billing"].Rows[0]);

                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, clientid);

                model.PayStatusStr = model.PayStatus == 0 ? "未付款"
                                : model.PayStatus == 1 ? "部分付款"
                                : model.PayStatus == 2 ? "已付款"
                                : model.PayStatus == 9 ? "已删除"
                                : "";

                model.InvoiceStatusStr = model.InvoiceStatus == 0 ? "未开票"
                                : model.InvoiceStatus == 1 ? "部分开票"
                                : model.InvoiceStatus == 2 ? "已开票"
                                : model.InvoiceStatus == 9 ? "已删除"
                                 : "";

                model.StorageBillingPays = new List<StorageBillingPay>();
                foreach (DataRow dr in ds.Tables["Pays"].Rows)
                {
                    StorageBillingPay pay = new StorageBillingPay();
                    pay.FillData(dr);
                    switch (pay.PayType)
                    {
                        case 1:
                            pay.PayTypeStr = "现金支付";
                            break;
                        case 2:
                            pay.PayTypeStr = "在线支付";
                            break;
                        case 3:
                            pay.PayTypeStr = "支付宝";
                            break;
                        case 4:
                            pay.PayTypeStr = "微信";
                            break;
                        case 5:
                            pay.PayTypeStr = "线下汇款";
                            break;

                    }
                    pay.CreateUser = OrganizationBusiness.GetUserByUserID(pay.CreateUserID, clientid);
                    model.StorageBillingPays.Add(pay);
                }

                model.StorageBillingInvoices = new List<StorageBillingInvoice>();
                foreach (DataRow dr in ds.Tables["Invoices"].Rows)
                {
                    StorageBillingInvoice invoice = new StorageBillingInvoice();
                    invoice.FillData(dr);
                    invoice.CreateUser = OrganizationBusiness.GetUserByUserID(invoice.CreateUserID, clientid);
                    model.StorageBillingInvoices.Add(invoice);
                }
            }
            return model;
        }

        public List<ClientAccountsEntity> GetAccountBills(int mark, string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid,  string clientid)
        {
            List<ClientAccountsEntity> list = new List<ClientAccountsEntity>();
            DataSet ds = FinanceDAL.BaseProvider.GetAccountBills(mark, begintime, endtime, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, userid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ClientAccountsEntity model = new ClientAccountsEntity();
                model.FillData(dr);

                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.ClientID);

                list.Add(model);
            }
            return list;
        }

        public List<BillingPay> GetOrderPays(string orderid)
        {
            List<BillingPay> list = new List<BillingPay>();
            DataTable dt = FinanceDAL.BaseProvider.GetOrderPays(orderid);
            foreach (DataRow dr in dt.Rows)
            {
                BillingPay model = new BillingPay();
                model.FillData(dr);
                model.PayTypeStr = CommonBusiness.GetEnumDesc<EnumOrderPayType>((EnumOrderPayType)model.PayType);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.ClientID);
                list.Add(model);
            }

            return list;
        }
        
        #endregion

        #region 添加

        public bool CreateStorageBillingPay(string billingid, int type, int paytype, decimal paymoney, DateTime paytime, string remark, string userid, string clientid)
        {
            bool bl = FinanceDAL.BaseProvider.CreateStorageBillingPay(billingid, type, paytype, paymoney, paytime, remark, userid, clientid);
            return bl;
        }

        public string CreateStorageBillingInvoice(string billingid, int type, decimal invoicemoney, string invoicecode, string remark, string userid, string clientid)
        {
            string id = Guid.NewGuid().ToString().ToLower();
            bool bl = FinanceDAL.BaseProvider.CreateStorageBillingInvoice(id, billingid, type, invoicemoney, invoicecode, remark, userid, clientid);
            if (bl)
            {
                return id;
            }
            return "";
        }

        public bool CreateBillingPay(string orderid, int type, int paytype, decimal paymoney, DateTime paytime, string remark, string userid, string clientid)
        {
            bool bl = FinanceDAL.BaseProvider.CreateOrderPay(orderid, type, paytype, paymoney, paytime, remark, userid, clientid);
            return bl;
        }

        public string CreateBillingInvoice(string billingid, int type, int customertype, decimal invoicemoney, string title, string citycode, string address, string postalcode, string name, string mobile, string remark, string userid, string clientid)
        {
            string id = Guid.NewGuid().ToString().ToLower();
            bool bl = FinanceDAL.BaseProvider.CreateBillingInvoice(id, billingid, type, customertype, invoicemoney, title, citycode, address, postalcode, name, mobile, remark, userid, clientid);
            if (bl)
            {
                return id;
            }
            return "";
        }


        #endregion

        #region 编辑/删除

        public bool DeleteStorageBillingInvoice(string invoiceid, string billingid, string userid, string clientid)
        {
            return FinanceDAL.BaseProvider.DeleteStorageBillingInvoice(invoiceid, billingid, userid, clientid);
        }

        public bool DeleteBillingInvoice(string invoiceid, string billingid, string userid, string clientid)
        {
            return FinanceDAL.BaseProvider.DeleteBillingInvoice(invoiceid, billingid, userid, clientid);
        }

        public bool AuditBillingInvoice(string invoiceid, string billingid, decimal invoicemoney, string invoicecode, string expressid, string expresscode, string userid, string clientid)
        {
            bool bl = FinanceDAL.BaseProvider.AuditBillingInvoice(invoiceid, billingid, invoicemoney, invoicecode, expressid, expresscode, userid, clientid);
            return bl;
        }


        #endregion
    }
}
