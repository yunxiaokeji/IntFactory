/**  版本信息模板在安装目录下，可自行修改。
* ProductDetail.cs
*
* 功 能： N/A
* 类 名： ProductDetail
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
using System.Collections.Generic;
namespace IntFactoryEntity
{
	/// <summary>
	/// ProductDetail:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class ProductDetail
	{
		public ProductDetail()
		{}

		public int AutoID{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
        public string ProductDetailID { set; get; }

        public string ProductCode { get; set; }

        public string DetailsCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string ProductID{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string SaleAttr{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string AttrValue{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string SaleAttrValue{ set; get; }

        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
        public string UnitID { get; set; }

        public ProductUnit Unit { get; set; }

		/// <summary>
		/// 
		/// </summary>
        public decimal StockOut { get; set; }

        public decimal StockIn { get; set; }

        public decimal LogicOut { get; set; }

        public int IsDefault { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ImgS{ set; get; }

		/// <summary>
		/// 
		/// </summary>
        public decimal WarnCount{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public int? Status{ set; get; }
		

        public string Description { set; get; }

        public string Remark { get; set; }

        public string ShapeCode { get; set; }

        public decimal Price { get; set; }

        public decimal Weight { get; set; }

        public string ProductName { get; set; }

        public string UnitName { get; set; }

        public decimal Quantity { get; set; }

        public string DepotCode { get; set; }

        public string ProductImage { get; set; }

        public string ProviderName { get; set; }

        public List<ProductStock> DetailStocks { get; set; }

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

