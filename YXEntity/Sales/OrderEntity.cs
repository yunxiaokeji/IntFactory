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
    /// OrderEntity:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
    [Serializable]
    public partial class OrderEntity
    {
        public OrderEntity()
        { }
        #region Model
        /// <summary>
        /// 
        /// </summary>
        public int AutoID { get; set; }
        [Property("Lower")] 
        public string OrderID { get; set; }

        [Property("Lower")] 
        public string TypeID{ get; set; }

        public OrderTypeEntity OrderType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        public string StatusStr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ReturnStatus { get; set; }

        [Property("Lower")]
        public string StageID { get; set; }

        public OpportunityStageEntity Stage { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal TaxMoney { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal TaxRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ReturnMoney { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CountryCode { get; set; }

        public CityEntity City { get; set; }

        public string CityCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Freight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ExpressType { get; set; }

        public string ExpressTypeStr { get; set; }

        [Property("Lower")] 
        public string ExpressID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExpressCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MobileTele { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FeedBack { get; set; }

        [Property("Lower")] 
        public string CreateUserID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OperateIP { get; set; }

        [Property("Lower")] 
        public string ClientID { get; set; }
        #endregion Model

        /// <summary>
        /// 明细
        /// </summary>
        public List<OrderDetail> Details { get; set; }

        public string Remark { get; set; }

        public string OrderCode { get; set; }

        [Property("Lower")]
        public string OwnerID { get; set; }

        public Users Owner { get; set; }

        public Users CreateUser { get; set; }

        public DateTime AuditTime { get; set; }

        public DateTime OrderTime { get; set; }

        [Property("Lower")] 
        public string AgentID { get; set; }

        [Property("Lower")] 
        public string CustomerID { get; set; }

        public string CustomerName { get; set; }

        public CustomerEntity Customer { get; set; }

        public int SendStatus { get; set; }

        public string SendStatusStr { get; set; }

        public int ReplyTimes { get; set; }

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

