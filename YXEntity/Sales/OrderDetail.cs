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
    /// OrderDetail:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class OrderDetail
	{
        public OrderDetail()
		{}

		public int AutoID{ set; get; }

        [Property("Lower")]
        public string OrderID { get; set; }

        [Property("Lower")]
        public string ProductDetailID { set; get; }
		/// <summary>
		/// 
		/// </summary>
        [Property("Lower")]
		public string ProductID{ set; get; }

        [Property("Lower")]
        public string UnitID { get; set; }

        public string UnitName { get; set; }

        public decimal Quantity { set; get; }

        public decimal Price { get; set; }

        public decimal Loss { get; set; }

        public decimal PurchaseQuantity { get; set; }

        public decimal InQuantity { get; set; }

        public decimal UseQuantity { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public decimal TotalMoney{ set; get; }

        public string WareID { get; set; }

        public string DepotID { get; set; }

        public string Remark { get; set; }

        public string ProductName { get; set; }

        public string ImgS { get; set; }

        public string ProductImage { get; set; }

        public string DetailsCode { get; set; }

        public string ProductCode { get; set; }

        public string ProviderID { get; set; }

        public string ProviderName { get; set; }

        public string ProviderMobileTele { get; set; }

        public string ProviderAddress { get; set; }

        public string ProviderCityCode { get; set; }

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

