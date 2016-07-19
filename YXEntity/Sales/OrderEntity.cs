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

        public int OrderType { get; set; }

        public string PlanPrice { get; set; }

        public decimal FinalPrice { get; set; }

        [Property("Lower")]
        public string CategoryID { get; set; }

        [Property("Lower")]
        public string BigCategoryID { get; set; }

        public int PlanQuantity { get; set; }

        public string OrderImage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        public string StatusStr { get; set; }

        public int OrderStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ReturnStatus { get; set; }

        [Property("Lower")]
        public string StageID { get; set; }

        [Property("Lower")]
        public string ProcessID { get; set; }

        public OrderProcessEntity OrderProcess { get; set; }


        public decimal Price { get; set; }

        public decimal ProfitPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal TotalMoney { get; set; }


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
        public string OperateIP { get; set; }

        [Property("Lower")] 
        public string ClientID { get; set; }

        public IntFactoryEntity.Manage.Clients Client { get; set; }
        #endregion Model

        /// <summary>
        /// 明细
        /// </summary>
        public List<OrderDetail> Details { get; set; }

        public List<Task.TaskEntity> Tasts { get; set; }

        public List<OrderCostEntity> OrderCoss { get; set; }

        public string Remark { get; set; }

        public string OrderCode { get; set; }

        public string GoodsCode { get; set; }

        public string Title { get; set; }

        [Property("Lower")]
        public string OwnerID { get; set; }

        public Users Owner { get; set; }

        public Users CreateUser { get; set; }

        public DateTime AuditTime { get; set; }

        public DateTime PlanTime { get; set; }

        public DateTime OrderTime { get; set; }

        [Property("Lower")] 
        public string AgentID { get; set; }

        [Property("Lower")] 
        public string CustomerID { get; set; }

        public string CustomerName { get; set; }

        public CustomerEntity Customer { get; set; }

        public int SendStatus { get; set; }

        public int PurchaseStatus { get; set; }

        public string SendStatusStr { get; set; }

        public int ReplyTimes { get; set; }

        public List<OrderStatusEntity> StatusItems { get; set; }

        public string Platemaking { get; set; }

        public string PlateRemark { get; set; }

        public int TaskCount { get; set; }

        public int TaskOver { get; set; }

        [Property("Lower")] 
        public string OriginalID { get; set; }

        public string OriginalCode { get; set; }

        public string OrderImages { get; set; }

        [Property("Lower")] 
        public string EntrustClientID { get; set; }

        public int EntrustStatus { get; set; }

        public DateTime EntrustTime { get; set; }

        public List<OrderGoodsEntity> OrderGoods { get; set; }

        public bool IsSelf { get; set; }

        public int SourceType { get; set; }

        public string SourceTypeStr { get; set; }

        public int CutStatus { get; set; }

        public decimal CostPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal OriginalPrice { get; set; }

        public int Mark { get; set; }

        public decimal PayMoney { get; set; }

        public string IntGoodsCode { get; set; }

        public string GoodsName { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 预警状态 0：正常 1：快到期 2：已超期
        /// </summary>
        public int WarningStatus { get; set; }

        public string WarningTime { get; set; }

        public int WarningDays{get;set;}

        public int UseDays { get; set; }

        public string CategoryName { get; set; }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

    }

    public class OrderStatusEntity
    {
        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

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

