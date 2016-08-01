/**  版本信息模板在安装目录下，可自行修改。
* Country.cs
*
* 功 能： N/A
* 类 名： Country
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/3/6 19:52:51   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
using System.Data;
namespace IntFactoryEntity
{
	/// <summary>
	/// Country:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Country
	{
		public Country()
		{}

        public int AutoID { get; set; }

        public string CountryCode { get; set; }

        public string Name { get; set; }

        public string EnglishName { get; set; }

        public string PhoneCode { get; set; }

		public string Language{ get; set; }

        public int Currency { get; set; }

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

