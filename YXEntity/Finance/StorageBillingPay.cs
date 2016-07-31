﻿/**  版本信息模板在安装目录下，可自行修改。
* StorageBillingPay.cs
*
* 功 能： N/A
* 类 名： StorageBillingPay
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
	/// StorageBillingPay:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class StorageBillingPay
	{
		public StorageBillingPay(){}

		public int AutoID{ get; set; }

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
		public int PayType{ get; set; }

        public string PayTypeStr { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime PayTime{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public decimal PayMoney{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Remark{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateTime{ get; set; }

        [Property("Lower")] 
		public string CreateUserID{ get; set; }

        public Users CreateUser { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime UpdateTime{ get; set; }

        [Property("Lower")] 
		public string ClientID{ get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

