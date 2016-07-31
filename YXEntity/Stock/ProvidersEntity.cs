/**  版本信息模板在安装目录下，可自行修改。
* Brand.cs
*
* 功 能： N/A
* 类 名： Brand
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:15   N/A    初版
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
	/// Brand:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class ProvidersEntity
	{
        public ProvidersEntity()
		{}

		public int AutoID{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")]
        public string ProviderID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Name{ set; get; }

        public string Contact { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string CityCode{ set; get; }

        public string Address { get; set; }

        public CityEntity City { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int Status{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Remark{ set; get; }

        [Property("Lower")] 
		public string CreateUserID{ set; get; }

        public Users CreateUser { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateTime{ set; get; }

        public string MobileTele { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string OperateIP{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Property("Lower")] 
		public string ClientID{ set; get; }

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

