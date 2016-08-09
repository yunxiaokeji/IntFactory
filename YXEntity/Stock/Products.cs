using IntFactoryEnum;
/**  版本信息模板在安装目录下，可自行修改。
* Products.cs
*
* 功 能： N/A
* 类 名： Products
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:19   N/A    初版
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
	/// Products:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Products
	{
		public Products()
		{}

		public int AutoID{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
        public string ProductID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ProductCode{ set; get; }
		/// <summary>
		/// 产品名称
		/// </summary>
		public string ProductName{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string GeneralName{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string MnemonicCode{ set; get; }

        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")]
        public string UnitID { get; set; }

        public ProductUnit SmallUnit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string CategoryID{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string CategoryIDList{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string SaleAttr{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string AttrList{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string ValueList{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string AttrValueList{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public decimal? CommonPrice{ set; get; }
		/// <summary>
		/// 
		/// </summary>
        public decimal Price { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public double? TaxRate{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public int? Status{ set; get; }

		/// <summary>
		/// 
		/// </summary>
		public int? IsDiscount{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public decimal? DiscountValue{ set; get; }

        public int IsAllow { get; set; }

		/// <summary>
		/// 
		/// </summary>
        public decimal SaleCount{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public decimal? Weight{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string ProductImage{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public int? EffectiveDays{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string ShapeCode{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")]
        public string ProviderID { get; set; }

        public ProvidersEntity Providers { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Description{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string CreateUserID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateTime{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime UpdateTime{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string OperateIP{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string ClientID{ set; get; }

        public string ProviderName { get; set; }

        public string UnitName { get; set; }

        public Category Category { get; set; }

        public string CategoryName { get; set; }

        public string ApprovalNumber { get; set; }

        [Property("Lower")] 
        public string ProductDetailID { get; set; }

        /// <summary>
        /// 子产品列表
        /// </summary>
        public List<ProductDetail> ProductDetails { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public List<ProductAttr> AttrLists { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public List<ProductAttr> SaleAttrs { get; set; }

        [Property("Lower")] 
        public string SaleAttrValue { get; set; }

        public decimal StockIn { get; set; }

        public decimal LogicOut { get; set; }

        public int IsPublic { get; set; }

        public string IsPublicStr { get; set; }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}

    [Serializable]
    public class FilterAttr
    {
        public string AttrID { get; set; }
        public string ValueID { get; set; }
        public EnumAttrType Type { get; set; }
    }
}

