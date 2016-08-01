/**  版本信息模板在安装目录下，可自行修改。
* StorageInDetail.cs
*
* 功 能： N/A
* 类 名： StorageInDetail
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:21   N/A    初版
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
    /// StorageDetail:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class StorageDetail
	{
        public StorageDetail()
		{}

		public int AutoID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string DocID{ set; get; }

        [Property("Lower")]
        public string ProductDetailID { set; get; }
		/// <summary>
		/// 
		/// </summary>
        [Property("Lower")]
		public string ProductID{ set; get; }

        public string UnitID { get; set; }

		/// <summary>
		/// 
		/// </summary>
        public decimal Quantity { set; get; }

        public decimal SurplusQuantity { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public decimal Price{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public decimal TotalMoney{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public decimal TaxMoney{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public decimal TaxRate{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public decimal ReturnPrice{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public decimal ReturnMoney{ set; get; }

        [Property("Lower")] 
        public string WareID { get; set; }

        [Property("Lower")] 
        public string DepotID { get; set; }

        public string DepotCode { get; set; }

        [Property("Lower")] 
		public string ClientID{ set; get; }

        public int Status { get; set; }

        public string Remark { get; set; }

        public string ProductName { get; set; }

        public string UnitName { get; set; }

        public string ProductImage { get; set; }

        public string Imgs { get; set; }

        public string ProductCode { get; set; }

        public string DetailsCode { get; set; }

        public decimal Complete { get; set; }

        [Property("Lower")] 
        public string ProviderID { get; set; }

        public ProvidersEntity Providers { get; set; }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }


	}
}

