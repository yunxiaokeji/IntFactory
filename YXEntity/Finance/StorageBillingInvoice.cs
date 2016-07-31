/**  版本信息模板在安装目录下，可自行修改。
* StorageBillingInvoice.cs
*
* 功 能： N/A
* 类 名： StorageBillingInvoice
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/11/17 22:02:55   N/A    初版
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
	/// StorageBillingInvoice:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class StorageBillingInvoice
	{
		public StorageBillingInvoice()
		{}

		public int AutoID{ get; set; }

        public string InvoiceID { get; set; }

        [Property("Lower")] 
		public string BillingID{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Type{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Status{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string TaxCode{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string BankName{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string BankAccount{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string InvoiceCode{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public decimal InvoiceMoney{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string InvoiceTitle{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ContactName{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ContactPhone{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string CityCode{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Address{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Remark{ get; set; }
        [Property("Lower")] 
		public string ExpressID{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ExpressCode{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int ExpressStatus{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime ExpressTime{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateTime{ get; set; }

        [Property("Lower")] 
		public string CreateUserID{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime UpdateTime{ get; set; }

        public Users CreateUser { get; set; }

        [Property("Lower")] 
		public string ClientID{ get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

