﻿/**  版本信息模板在安装目录下，可自行修改。
* WareHouse.cs
*
* 功 能： N/A
* 类 名： WareHouse
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:26   N/A    初版
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
	/// WareHouse:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class WareHouse
	{
		public WareHouse()
		{}

        [Property("Lower")] 
        public string WareID { get; set; }

        public string WareCode { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string CityCode { get; set; }

        public int Status { get; set; }

        public string Description { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public string DepotCode { get; set; }

        public string DepotName { get; set; }

        public CityEntity City { get; set; }

        public List<DepotSeat> DepotSeats { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

