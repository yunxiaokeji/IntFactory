using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IntFactoryEnum
{
    /// <summary>
    /// 活动阶段
    /// </summary>
    public enum EnumActivityStage
    {
        All = -1,
        /// <summary>
        /// 已结束
        /// </summary>
        End = 1,
        /// <summary>
        /// 进行中
        /// </summary>
        Runing = 2,
        /// <summary>
        /// 未开始
        /// </summary>
        NoBegin = 3
    }

    /// <summary>
    /// 客户阶段标记
    /// </summary>
    public enum EnumCustomStageMark
    {
        [DescriptionAttribute("普通阶段")]
        Normal = 0,
        [DescriptionAttribute("新客户阶段")]
        New = 1,
        [DescriptionAttribute("成交阶段")]
        Success = 2
    }
    /// <summary>
    /// 客户状态
    /// </summary>
    public enum EnumCustomStatus
    {
        All = -1,
        [DescriptionAttribute("正常")]
        Normal = 1,
        [DescriptionAttribute("关闭")]
        Close = 2,
        [DescriptionAttribute("丢失")]
        Loses = 3,
        [DescriptionAttribute("删除")]
        Delete = 9
    }
}
