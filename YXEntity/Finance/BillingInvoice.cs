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

		public int AutoID{ get; set; }

        [Property("Lower")] 
        public string InvoiceID { get; set; }

        [Property("Lower")] 
		public string BillingID{ get; set; }

		public int Status{ get; set; }

        public string StatusStr { get; set; }

		public int Type{ get; set; }

        public int CustomerType { get; set; }

		public string TaxCode{ get; set; }

		public string BankName{ get; set; }

		public string BankAccount{ get; set; }

		public string InvoiceCode{ get; set; }

		public decimal InvoiceMoney{ get; set; }

		public string InvoiceTitle{ get; set; }

		public string ContactName{ get; set; }

		public string ContactPhone{ get; set; }

		public string CityCode{ get; set; }

		public string Address{ get; set; }

        public string PostalCode { get; set; }

		public string Remark{ get; set; }

        [Property("Lower")] 
		public string ExpressID{ get; set; }

		public string ExpressCode{ get; set; }

		public int ExpressStatus{ get; set; }

		public DateTime ExpressTime{ get; set; }

		public DateTime CreateTime{ get; set; }

		public string CreateUserID{ get; set; }

		public DateTime UpdateTime{ get; set; }

        [Property("Lower")] 
		public string ClientID{ get; set; }

        public Users CreateUser { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

