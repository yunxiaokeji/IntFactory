/**  版本信息模板在安装目录下，可自行修改。
* BillingInvoice.cs
*
* 功 能： N/A
* 类 名： BillingInvoice
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/11/17 22:02:51   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
namespace IntFactoryEntity
{
	/// <summary>
	/// BillingInvoice:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class BillingInvoice
	{
		public BillingInvoice()
		{}
		#region Model
		private int _autoid;
		private string _billingid;
		private int _status=0;
		private int _type=1;
		private string _taxcode="";
		private string _bankname="";
		private string _bankaccount="";
		private string _invoicecode="";
		private decimal _invoicemoney=0M;
		private string _invoicetitle="";
		private string _contactname="";
		private string _contactphone="";
		private string _citycode="";
		private string _address="";
		private string _remark="";
		private string _expressid="";
		private string _expresscode="";
		private int _expressstatus=0;
		private DateTime _expresstime;
		private DateTime _createtime= DateTime.Now;
		private string _createuserid="";
		private DateTime _updatetime= DateTime.Now;
		private string _clientid;
		/// <summary>
		/// 
		/// </summary>
		public int AutoID
		{
			set{ _autoid=value;}
			get{return _autoid;}
		}
        [Property("Lower")] 
        public string InvoiceID { get; set; }

        [Property("Lower")] 
		public string BillingID
		{
			set{ _billingid=value;}
			get{return _billingid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int Status
		{
			set{ _status=value;}
			get{return _status;}
		}

        public string StatusStr { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Type
		{
			set{ _type=value;}
			get{return _type;}
		}

        public int CustomerType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string TaxCode
		{
			set{ _taxcode=value;}
			get{return _taxcode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string BankName
		{
			set{ _bankname=value;}
			get{return _bankname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string BankAccount
		{
			set{ _bankaccount=value;}
			get{return _bankaccount;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string InvoiceCode
		{
			set{ _invoicecode=value;}
			get{return _invoicecode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal InvoiceMoney
		{
			set{ _invoicemoney=value;}
			get{return _invoicemoney;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string InvoiceTitle
		{
			set{ _invoicetitle=value;}
			get{return _invoicetitle;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ContactName
		{
			set{ _contactname=value;}
			get{return _contactname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ContactPhone
		{
			set{ _contactphone=value;}
			get{return _contactphone;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CityCode
		{
			set{ _citycode=value;}
			get{return _citycode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Address
		{
			set{ _address=value;}
			get{return _address;}
		}
        public string PostalCode { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
        [Property("Lower")] 
		public string ExpressID
		{
			set{ _expressid=value;}
			get{return _expressid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ExpressCode
		{
			set{ _expresscode=value;}
			get{return _expresscode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int ExpressStatus
		{
			set{ _expressstatus=value;}
			get{return _expressstatus;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime ExpressTime
		{
			set{ _expresstime=value;}
			get{return _expresstime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CreateUserID
		{
			set{ _createuserid=value;}
			get{return _createuserid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime UpdateTime
		{
			set{ _updatetime=value;}
			get{return _updatetime;}
		}

        [Property("Lower")]
        public string AgentID { get; set; }

        [Property("Lower")] 
		public string ClientID
		{
			set{ _clientid=value;}
			get{return _clientid;}
		}
		#endregion Model

        public Users CreateUser { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

