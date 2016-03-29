using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlibabaSdk
{
    //public enum FentOrderStatus {
    //    [DescriptionAttribute("FENT")]
    //    FENT,
    //    [DescriptionAttribute("PRICING")]
    //    PRICING,
    //    [DescriptionAttribute("SEALED")]
    //    SEALED,
    //    [DescriptionAttribute("CLOSE")]
    //    CLOSE
    //}

    //public enum BulkOrderStatus
    //{
    //    [DescriptionAttribute("PRODUCING")]
    //    PRODUCING,
    //    [DescriptionAttribute("PRODUCED")]
    //    PRODUCED,
    //    [DescriptionAttribute("CLOSE")]
    //    CLOSE
    //}

    public enum EnumOrderStageStatus
    {
        [DescriptionAttribute("FENT")]
        FENT = 1,
        [DescriptionAttribute("PRICING")]
        PRICING = 2,
        [DescriptionAttribute("SEALED")]
        SEALED = 3,
        [DescriptionAttribute("待大货")]
        DDH = 4,
        [DescriptionAttribute("PRODUCING")]
        PRODUCING = 5,
        [DescriptionAttribute("PRODUCED")]
        PRODUCED = 6,
        [DescriptionAttribute("交易结束")]
        JYJS = 7,
        [DescriptionAttribute("回退")]
        TD = 8,
        [DescriptionAttribute("CLOSE")]
        CLOSE = 9
    }

    public enum AliOrderPlanStatus
    {
        [DescriptionAttribute("正常")]
        Normal = 1,
        [DescriptionAttribute("RefreshToken无效")]
        RefreshTokenError = 2,
        [DescriptionAttribute("异常")]
        Exception = 2
    }

    public enum AliOrderDownType
    {
        [DescriptionAttribute("自动")]
        Auto = 1,
        [DescriptionAttribute("手动")]
        Hand = 2
    }

    public enum AliOrderUpdateStatus
    {
        [DescriptionAttribute("正常")]
        Normal = 0,
        [DescriptionAttribute("成功")]
        Success = 1,
        [DescriptionAttribute("失败")]
        Fail = 2
    }
}
