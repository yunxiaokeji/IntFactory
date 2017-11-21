using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IntFactoryEnum
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum EnumLogType
    {
        Create = 1,
        Update = 2,
        Delete = 3
    }

    //日志对象类型
    public enum EnumLogObjectType
    {
        [DescriptionAttribute("客户")]
        Customer = 1,
        [DescriptionAttribute("订单")]
        Orders = 2,
        [DescriptionAttribute("活动")]
        Activity = 3,
        [DescriptionAttribute("产品")]
        Product = 4,
         [DescriptionAttribute("员工")]
        User = 5,
        [DescriptionAttribute("代理商")]
        Agent = 6,
        [DescriptionAttribute("机会")]
        Opportunity = 7,
        [DescriptionAttribute("采购")]
        StockIn = 8,
        [DescriptionAttribute("出库")]
        StockOut = 9,
        [DescriptionAttribute("任务 ")]
        OrderTask = 10,
        [DescriptionAttribute("阿里订单")]
        PullOrder = 11,
        [DescriptionAttribute("生产订单")]
        OrderDoc = 12,
    }

    /// <summary>
    /// 日志模块
    /// </summary>
    public enum EnumLogModules
    {
        [DescriptionAttribute("库存")]
        Stock = 1
    }
    /// <summary>
    /// 日志对象
    /// </summary>
    public enum EnumLogEntity
    {
        Brand = 1,
        ProductUnit
    }

    public enum EnumLogSubject
    {
        All = -1,

        Other = 0,

        #region 订单

        OrderPlanTime = 101,

        #endregion

        #region 任务

        TaskEndTime = 201

        #endregion
    }
}
