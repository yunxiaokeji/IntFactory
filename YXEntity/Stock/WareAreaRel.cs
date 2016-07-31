/**  版本信息模板在安装目录下，可自行修改。
* WareAreaRel.cs
*
* 功 能： N/A
* 类 名： WareAreaRel
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:25   N/A    初版
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
	/// WareAreaRel:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class WareAreaRel
	{
		public WareAreaRel()
		{}

		public int AutoID{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string WareCode{ set; get; }
		/// <summary>
		/// 
		/// </summary>
		public string AreaCode{ set; get; }
		/// <summary>
		/// 
		/// </summary>
        public string ClientID { set; get; }

	}
}

