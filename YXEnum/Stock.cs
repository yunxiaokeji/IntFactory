using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IntFactoryEnum
{
    /// <summary>
    /// 产品属性类型
    /// </summary>
    public enum EnumAttrType
    {
        /// <summary>
        /// 产品属性
        /// </summary>
        [DescriptionAttribute("产品属性")]
        Parameter = 1,
        /// <summary>
        /// 产品规格
        /// </summary>
        [DescriptionAttribute("产品规格")]
        Specification = 2

    }

    /// <summary>
    /// 单据类型
    /// </summary>
    public enum EnumDocType
    {
        /// <summary>
        /// 全部（仅供查询）
        /// </summary>
        All = -1,
        [DescriptionAttribute("采购单")]
        RK = 1,
        [DescriptionAttribute("出库单")]
        CK = 2,
        [DescriptionAttribute("报损单")]
        BS = 3,
        [DescriptionAttribute("报溢单")]
        BY = 4,
        [DescriptionAttribute("调拨单")]
        DB = 5,
        [DescriptionAttribute("退货单")]
        TH = 6,
        [DescriptionAttribute("手工出库单")]
        SGCK = 7,
        [DescriptionAttribute("销售订单")]
        Order = 11,
        [DescriptionAttribute("代理商订单")]
        AgentsOrder = 21,
        [DescriptionAttribute("统计用-账单")]
        Billing = 111
    }

    /// <summary>
    /// 成品单据类型
    /// </summary>
    public enum EnumGoodsDocType
    {
        /// <summary>
        /// 全部（仅供查询）
        /// </summary>
        All = -1,
        [DescriptionAttribute("裁剪单")]
        RK = 1,
        [DescriptionAttribute("发货单")]
        CK = 2,
        [DescriptionAttribute("报损单")]
        BS = 3,
        [DescriptionAttribute("报溢单")]
        BY = 4,
        [DescriptionAttribute("调拨单")]
        DB = 5,
        [DescriptionAttribute("退货单")]
        TH = 6,
        [DescriptionAttribute("手工出库单")]
        SGCK = 7,
        [DescriptionAttribute("销售订单")]
        Order = 11,
        [DescriptionAttribute("代理商订单")]
        AgentsOrder = 21,
        [DescriptionAttribute("统计用-账单")]
        Billing = 111
    }
    /// <summary>
    /// 单据状态
    /// </summary>
    public enum EnumDocStatus
    {
        /// <summary>
        /// 全部（仅供查询）
        /// </summary>
        All = -1,
        [DescriptionAttribute("未处理")]
        Normal = 0,
        [DescriptionAttribute("部分审核")]
        AuditPart = 1,
        [DescriptionAttribute("已审核")]
        AuditAll = 2,
        [DescriptionAttribute("已作废")]
        Invalid = 4,
        [DescriptionAttribute("已删除")]
        Delete = 9
    }

}
