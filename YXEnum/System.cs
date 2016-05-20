using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IntFactoryEnum
{
    /// <summary>
    /// 阶段模块
    /// </summary>
    public enum EnumOrderStageMark
    {
        [DescriptionAttribute("无")]
        Normal = 0,
        [DescriptionAttribute("材料录入")]
        DYProduct=11,
        [DescriptionAttribute("制版工艺")]
        DYEngraving=12,
        [DescriptionAttribute("裁片录入/织片录入")]
        DYCut=13,
        [DescriptionAttribute("车缝/缝盘/套口")]
        DYSewn=14,
        [DescriptionAttribute("发货录入")]
        DYSend=15,
        [DescriptionAttribute("材料录入")]
        DHProduct = 21,
        [DescriptionAttribute("制版工艺")]
        DHEngraving = 22,
        [DescriptionAttribute("裁片录入/织片录入")]
        DHCut = 23,
        [DescriptionAttribute("车缝/缝盘/套口")]
        DHSewn = 24,
        [DescriptionAttribute("发货录入")]
        DHSend = 25,
    }
}
