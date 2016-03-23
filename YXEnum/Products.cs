using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IntFactoryEnum
{
    public enum EnumProductPublicStatus
    {
        [DescriptionAttribute("全部")]
        All = -1,
        [DescriptionAttribute("未公开")]
        Normal = 0,
        [DescriptionAttribute("待审核")]
        Wait = 1,
        [DescriptionAttribute("已公开")]
        Public = 2,
        [DescriptionAttribute("驳回申请")]
        Overrule = 3,
        [DescriptionAttribute("撤销公开")]
        Cancel = 4
    }

    public enum EnumCategoryType
    {
        All = -1,
        //材料
        Product = 1,
        //订单
        Order = 2
    }
}
