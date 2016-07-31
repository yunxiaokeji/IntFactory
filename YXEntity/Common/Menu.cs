/**  版本信息模板在安装目录下，可自行修改。
* Menu.cs
*
* 功 能： N/A
* 类 名： Menu
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
using System.Collections.Generic;
using System.Data;
namespace IntFactoryEntity
{
	/// <summary>
	/// Menu:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Menu
	{
		public Menu()
		{}

        public int AutoID { get; set; }

        public string MenuCode { get; set; }

        public string Name { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string View { get; set; }

        public string IcoPath { get; set; }

        public string IcoHover { get; set; }

        public int Type { get; set; }

        public int IsHide { get; set; }

        public string PCode { get; set; }

        public int Sort { get; set; }

        public int Layer { get; set; }

        public List<Menu> ChildMenus { get; set; }

        public int IsMenu { get; set; }

        public int IsLimit { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

