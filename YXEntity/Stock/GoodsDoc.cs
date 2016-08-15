/**  版本信息模板在安装目录下，可自行修改。
* StorageInDoc.cs
*
* 功 能： N/A
* 类 名： StorageInDoc
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/5/3 13:38:22   N/A    初版
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
	/// StorageInDoc:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
    [Serializable]
    public partial class GoodsDoc
    {
        public GoodsDoc()
        { }

        public int AutoID{ set; get; }

        [Property("Lower")] 
        public string DocID{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public int DocType{ set; get; }

        public string DocTypeStr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status{ set; get; }

        public string StatusStr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? ReturnStatus{ set; get; }

        /// <summary>
        /// 
        /// </summary>
        public decimal TotalMoney{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? TaxMoney{ set; get; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? TaxRate{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? ReturnMoney{ set; get; }

        public string OriginalCode{ set; get; }

        [Property("Lower")] 
        public string OriginalID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CountryCode{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string CityCode{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Address{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string PostalCode{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Weight{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Fee{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Freight{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public int? ExpressType{ set; get; }

        [Property("Lower")] 
        public string ExpressID{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string ExpressCode{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string PersonName{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string MobileTele{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string FeedBack{ set; get; }

        [Property("Lower")] 
        public string CreateUserID{ set; get; }

        [Property("Lower")]
        public string OwnerID { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime{ set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string OperateIP{ set; get; }

        [Property("Lower")] 
        public string ClientID{ set; get; }

        /// <summary>
        /// 单据明细
        /// </summary>
        public List<GoodsDocDetail> Details { get; set; }

        public string Remark { get; set; }

        public string DocCode { get; set; }

        public CacheUserEntity CreateUser { get; set; }

        public CacheUserEntity Owner { get; set; }

        [Property("Lower")] 
        public string WareID { get; set; }

        public WareHouse WareHouse { get; set; }

        public string DocImage { get; set; }

        public string DocImages { get; set; }

        public Manage.ExpressCompany Express { get; set; }

        public int Quantity { get; set; }

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

