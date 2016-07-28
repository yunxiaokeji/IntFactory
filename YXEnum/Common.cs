using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IntFactoryEnum
{
    /// <summary>
    /// 系统类型
    /// </summary>
    public enum EnumSystemType
    {
        [DescriptionAttribute("管理系统")]
        Manage = 1,
        [DescriptionAttribute("云销系统")]
        Client = 2,
        [DescriptionAttribute("代理商系统")]
        Agent = 3
    }

    /// <summary>
    /// 状态枚举
    /// </summary>
    public enum EnumLoginStatus
    {
        [DescriptionAttribute("全部")]
        All = -1,
        [DescriptionAttribute("正常")]
        Normal = 1,
        [DescriptionAttribute("异地登陆")]
        OtherLogin = 2,
        [DescriptionAttribute("删除")]
        Delete = 9
    }

    /// <summary>
    /// 状态枚举
    /// </summary>
    public enum EnumStatus
    {
        [DescriptionAttribute("全部")]
        All = -1,
        [DescriptionAttribute("禁用")]
        Invalid = 0,
        [DescriptionAttribute("正常")]
        Valid = 1,
        [DescriptionAttribute("删除")]
        Delete = 9
    }
    /// <summary>
    /// 执行状态码
    /// </summary>
    public enum EnumResultStatus
    {
        [DescriptionAttribute("失败")]
        Failed = 0,
        [DescriptionAttribute("成功")]
        Success = 1,
        [DescriptionAttribute("无权限")]
        IsLimit = 10000,
        [DescriptionAttribute("系统错误")]
        Error = 10001,
        [DescriptionAttribute("存在数据")]
        Exists = 10002
    }
    /// <summary>
    /// 搜索类型
    /// </summary>
    public enum EnumSearchType
    {
        /// <summary>
        /// 我的
        /// </summary>
        Myself = 1,
        /// <summary>
        /// 下属
        /// </summary>
        Branch = 2,
        /// <summary>
        /// 所有
        /// </summary>
        All = 3,
        /// <summary>
        /// 委托
        /// </summary>
        Entrust = 4
    }
    /// <summary>
    /// 标签类型
    /// </summary>
    public enum EnumMarkType
    {
        Customer = 1,
        Orders = 2,
        Tasks = 3
    }

    /// <summary>
    /// 账号类型
    /// </summary>
    public enum EnumAccountType
    {
        /// <summary>
        /// 账号
        /// </summary>
        UserName = 1,
        /// <summary>
        /// 手机
        /// </summary>
        Mobile = 2,
        /// <summary>
        /// 阿里
        /// </summary>
        Ali = 3,
        /// <summary>
        /// 微信
        /// </summary>
        WeiXin = 4,
    }

    /// <summary>
    /// 注册来源
    /// </summary>
    public enum EnumRegisterType
    {
        /// <summary>
        /// 后台添加
        /// </summary>
        Manage = 1,
        /// <summary>
        /// 自助注册
        /// </summary>
        Self = 2,
        /// <summary>
        /// 阿里
        /// </summary>
        Ali = 3,
        /// <summary>
        /// 微信
        /// </summary>
        WeiXin = 4
    }

}
