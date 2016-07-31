/**  版本信息模板在安装目录下，可自行修改。
* ModulesMenu.cs
*
* 功 能： N/A
* 类 名： ModulesMenu
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/3/6 19:52:53   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
namespace IntFactoryEntity.Manage
{
	/// <summary>
	/// ModulesMenu:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class SystemMenu
	{
		public SystemMenu()
		{}
		/// <summary>
		/// 
		/// </summary>
        public int AutoID { set; get; }
		/// <summary>
		/// 
		/// </summary>
        public string MenuCode { set; get; }

        public string Name { set; get; }

        public string Area { set; get; }

        public string Controller { set; get; }

        public string View { set; get; }

        public string IcoPath { set; get; }

        public string IcoHover { set; get; }

        public int Type { set; get; }

        public int IsHide { set; get; }

        public string PCode { set; get; }

        public string PCodeName { set; get; }

        public int Sort { set; get; }

        public int IsMenu { set; get; }

        public int Layer { set; get; }

        public int IsLimit { set; get; }

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

