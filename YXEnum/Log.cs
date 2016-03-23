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
        Customer = 1,
        Orders = 2,
        Activity = 3,
        Product = 4,
        User = 5,
        Agent = 6,
        Opportunity = 7,
        StockIn = 8,
        StockOut = 9,
        OrderTask=10
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
}
