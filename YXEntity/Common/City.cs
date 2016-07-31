/**  版本信息模板在安装目录下，可自行修改。
* City.cs
*
* 功 能： N/A
* 类 名： City
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
	/// City:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class CityEntity
	{
        public CityEntity()
		{}

        public int AutoID { get; set; }

        public string CityCode { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public string PCode { get; set; }

        public string CountryCode { get; set; }

        public string Country { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Counties { get; set; }

        public string PostalCode { get; set; }

        public string Description { get; set; }

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

