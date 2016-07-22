/**  版本信息模板在安装目录下，可自行修改。
* DepotSeat.cs
*
* 功 能： N/A
* 类 名： DepotSeat
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:16   N/A    初版
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
	/// DepotSeat:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class DepotSeat
	{
		public DepotSeat()
		{}

        [Property("Lower")] 
        public string DepotID { set; get; }

        public string DepotCode { get; set; }

        [Property("Lower")] 
        public string WareID { set; get; }

        public string WareName { set; get; }

        public string Name { get; set; }

        public int Status { get; set; }

        public string Description { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

	}
}

