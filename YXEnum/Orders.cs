using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IntFactoryEnum
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum EnumOrderStatus
    {
        [DescriptionAttribute("全部")]
        All = -1,
        [DescriptionAttribute("待分配")]
        Normal = 0,
        [DescriptionAttribute("打样中")]
        DY = 1,
        [DescriptionAttribute("合价中")]
        HJ = 2,
        [DescriptionAttribute("封样封价")]
        FYFJ = 3,
        [DescriptionAttribute("生产中")]
        SC = 4,
        [DescriptionAttribute("生产完成")]
        SCWC = 5,
        [DescriptionAttribute("交易结束")]
        JYJS = 6,
        [DescriptionAttribute("退单")]
        TD = 7,
        [DescriptionAttribute("作废")]
        Delete = 8
    }
    /// <summary>
    /// 出库状态
    /// </summary>
    public enum EnumSendStatus
    {
        [DescriptionAttribute("全部")]
        All = -1,
        [DescriptionAttribute("待出库")]
        NoOut = 0,
        [DescriptionAttribute("待发货")]
        NoSend = 1,
        [DescriptionAttribute("已发货")]
        Send = 2,
        [DescriptionAttribute("已签收")]
        Sign = 3,
        /// <summary>
        /// 查询用所有已出库
        /// </summary>
        AllOut = 11,
        /// <summary>
        /// 查询用所有已发货
        /// </summary>
        AllSend = 12,
    }
    /// <summary>
    /// 快递方式
    /// </summary>
    public enum EnumExpressType
    {
        [DescriptionAttribute("全部")]
        All = -1,
        [DescriptionAttribute("邮寄")]
        Post = 0,
        [DescriptionAttribute("海运")]
        Sea = 1,
        [DescriptionAttribute("空运")]
        Air = 2,
        [DescriptionAttribute("自提")]
        Self = 3
    }

    /// <summary>
    /// 后台订单状态
    /// </summary>
    public enum EnumClientOrderStatus
    {
        [DescriptionAttribute("全部")]
        All = -1,
        [DescriptionAttribute("未支付")]
        NoPay = 0,
        [DescriptionAttribute("已支付")]
        Pay = 1,
        [DescriptionAttribute("删除")]
        Delete = 9
    }
    public enum EnumReturnStatus
    {
        [DescriptionAttribute("全部")]
        All = -1,
        [DescriptionAttribute("未退货")]
        Normal = 0,
        [DescriptionAttribute("已申请")]
        Apply = 1,
        [DescriptionAttribute("部分退货")]
        PartReturn = 2,
        [DescriptionAttribute("已退单")]
        Return = 3,
        /// <summary>
        /// 全部退单用于查询
        /// </summary>
        AllReturn=11
    }
}
