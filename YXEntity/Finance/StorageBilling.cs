/**  版本信息模板在安装目录下，可自行修改。
* StorageBilling.cs
*
* 功 能： N/A
* 类 名： StorageBilling
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/11/17 22:02:54   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
namespace IntFactoryEntity
{
	/// <summary>
	/// StorageBilling:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class StorageBilling
	{
		public StorageBilling()
		{}
		#region Model
		private int _autoid;
		private string _billingid;
		private string _billingcode;
		private string _docid;
		private string _doccode;
		private decimal _totalmoney;
		private int _type;
		private int _status=1;
		private int _paystatus=0;
		private DateTime _paytime;
		private decimal _paymoney=0M;
		private int _invoicestatus=0;
		private DateTime _invoicetime;
		private string _remark="";
		private DateTime _createtime= DateTime.Now;
		private string _createuserid;
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
		public string BillingID
		{
			set{ _billingid=value;}
			get{return _billingid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string BillingCode
		{
			set{ _billingcode=value;}
			get{return _billingcode;}
		}
        [Property("Lower")] 
		public string DocID
		{
			set{ _docid=value;}
			get{return _docid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string DocCode
		{
			set{ _doccode=value;}
			get{return _doccode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal TotalMoney
		{
			set{ _totalmoney=value;}
			get{return _totalmoney;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int Type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int Status
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int PayStatus
		{
			set{ _paystatus=value;}
			get{return _paystatus;}
		}
        public string PayStatusStr { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime PayTime
		{
			set{ _paytime=value;}
			get{return _paytime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal PayMoney
		{
			set{ _paymoney=value;}
			get{return _paymoney;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int InvoiceStatus
		{
			set{ _invoicestatus=value;}
			get{return _invoicestatus;}
		}
        public string InvoiceStatusStr { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime InvoiceTime
		{
			set{ _invoicetime=value;}
			get{return _invoicetime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
        [Property("Lower")] 
		public string CreateUserID
		{
			set{ _createuserid=value;}
			get{return _createuserid;}
		}

        public Users CreateUser { get; set; }
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

        public List<StorageBillingPay> StorageBillingPays { get; set; }

        public List<StorageBillingInvoice> StorageBillingInvoices { get; set; }

        public decimal InvoiceMoney { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

