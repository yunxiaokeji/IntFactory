/**  版本信息模板在安装目录下，可自行修改。
* Role.cs
*
* 功 能： N/A
* 类 名： Role
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/4/8 19:58:55   N/A    初版
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
	/// Role:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Role
	{
		public Role()
		{}

		public int AutoID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
        [Property("Lower")] 
		public string RoleID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Name{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string ParentID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public int? Status{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string Description{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateTime{ set; get; }
		/// <summary>
		/// 
		/// </summary>
        [Property("Lower")] 
		public string CreateUserID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
        [Property("Lower")] 
		public string ClientID{ set; get; }

        public int IsDefault { get; set; }

        public Users CreateUser { get; set; }

        public List<Menu> Menus { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
	}
}

