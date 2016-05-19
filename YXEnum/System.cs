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
        [DescriptionAttribute("材料")]
        DYProduct=11,
        [DescriptionAttribute("制版")]
        DYEngraving=12,
        [DescriptionAttribute("裁片/织片")]
        DYCut=13,
        [DescriptionAttribute("车缝/套口")]
        DYSewn=14,
        [DescriptionAttribute("发货")]
        DYSend=15,
        [DescriptionAttribute("材料")]
        DHProduct = 21,
        [DescriptionAttribute("制版")]
        DHEngraving = 22,
        [DescriptionAttribute("裁片/织片")]
        DHCut = 23,
        [DescriptionAttribute("车缝/套口")]
        DHSewn = 24,
        [DescriptionAttribute("发货")]
        DHSend = 25,
    }
}
