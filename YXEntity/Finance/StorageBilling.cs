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
		public int AutoID{ get; set; }

        [Property("Lower")] 
		public string BillingID{ get; set; }

		public string BillingCode{ get; set; }

        [Property("Lower")] 
		public string DocID{ get; set; }

		public string DocCode{ get; set; }

		public decimal TotalMoney{ get; set; }

		public int Type{ get; set; }

		public int Status{ get; set; }

		public int PayStatus{ get; set; }

        public string PayStatusStr { get; set; }

		public DateTime PayTime{ get; set; }

		public decimal PayMoney{ get; set; }

		public int InvoiceStatus{ get; set; }

        public string InvoiceStatusStr { get; set; }

		public DateTime InvoiceTime{ get; set; }

		public string Remark{ get; set; }

		public DateTime CreateTime{ get; set; }

        [Property("Lower")] 
		public string CreateUserID{ get; set; }

        public Users CreateUser { get; set; }

		public DateTime UpdateTime{ get; set; }

        [Property("Lower")] 
		public string ClientID{ get; set; }

        public List<StorageBillingPay> StorageBillingPays { get; set; }

        public List<StorageBillingInvoice> StorageBillingInvoices { get; set; }

        public decimal InvoiceMoney { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

