/**  版本信息模板在安装目录下，可自行修改。
* Clients.cs
*
* 功 能： N/A
* 类 名： Clients
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/3/6 19:52:52   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
namespace IntFactoryEntity.Manage
{
	/// <summary>
	/// Clients:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Clients
	{
		public Clients()
		{}

		public int AutoID{ get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string ClientID{ get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string CompanyName{ get; set; }

        public string Logo{ get; set; }

		/// <summary>
		/// 
		/// </summary>
        public string Industry{ get; set; }
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
		public string PostalCode{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ContactName{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string MobilePhone{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string OfficePhone{ get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Status{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AuthorizeType{ get; set; }

        public string Description { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateTime{ get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string CreateUserID{ get; set; }

        public CityEntity City { get; set; }

        public Industry IndustryEntity { get; set; }

        public int UserQuantity { get; set; }

        public int Years { get; set; }

        public DateTime EndTime { get; set; }

        public string AliMemberID { get; set; }

        public string ClientCode { get; set; }

        public int GuideStep { get; set; }

        public int GoodsNum { get; set; }

        public int OrderNum { get; set; }

        public DateTime LastTime { get; set; }

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

