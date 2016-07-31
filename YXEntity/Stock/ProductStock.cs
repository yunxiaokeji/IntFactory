/**  版本信息模板在安装目录下，可自行修改。
* ProductQuantity.cs
*
* 功 能： N/A
* 类 名： ProductQuantity
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:18   N/A    初版
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
	/// ProductQuantity:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class ProductStock
	{
        public ProductStock()
		{}

		public int AutoID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
        [Property("Lower")] 
        public string ProductDetailID { set; get; }
		/// <summary>
		/// 
		/// </summary>
        [Property("Lower")] 
		public string ProductID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string ProducedDate{ set; get; }
		/// <summary>
		/// 
		/// </summary>
        public decimal StockIn{ set; get; }
		/// <summary>
		/// 
		/// </summary>
        public decimal StockOut{ set; get; }

		/// <summary>
		/// 
		/// </summary>
		public string WareCode{ set; get; }

        [Property("Lower")] 
        public string DepotID { get; set; }

        [Property("Lower")]
        public string WareID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string DepotCode{ set; get; }

        public string SaleAttrValue { get; set; }

        public string WareName { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string DetailsCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ClientID{ set; get; }

        public decimal LogicOut { get; set; }

        public string Description { get; set; }

        public string Remark { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

