using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactoryEntity;
using System.Web.Script.Serialization;

public static class ExpandClass
{
    /// <summary>
    /// 顶层菜单编码
    /// </summary>
    public const string CLIENT_TOP_CODE = "100000000";
    /// <summary>
    /// 默认菜单编码
    /// </summary>
    public const string CLIENT_DEFAULT_CODE = "101000000";

    /// <summary>
    /// 获取下级菜单
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="menuCode"></param>
    /// <returns></returns>
    public static List<Menu> GetChildMenuByCode(HttpContext httpContext, string menuCode)
    {
        if (httpContext.Session["ClientManager"] != null)
        {
            return ((IntFactoryEntity.Users)httpContext.Session["ClientManager"]).Menus.Where(m => m.PCode == menuCode && m.IsMenu == 1).OrderBy(m => m.Sort).ToList();
        }
        else
        {
            return new List<Menu>();
        }
    }

    /// <summary>
    /// 获取菜单
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="menuCode"></param>
    /// <returns></returns>
    public static Menu GetMenuByCode(HttpContext httpContext, string menuCode)
    {
        if (httpContext.Session["ClientManager"] != null)
        {
            return ((IntFactoryEntity.Users)httpContext.Session["ClientManager"]).Menus.Where(m => m.MenuCode == menuCode).FirstOrDefault();
        }
        else
        {
            return new Menu();
        }
    }

    /// <summary>
    /// 返回controllerMenu
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="controller"></param>
    /// <returns></returns>
    public static Menu GetController(HttpContext httpContext, string controller)
    {
        if (httpContext.Session["ClientManager"] != null)
        {
            return IntFactoryBusiness.CommonBusiness.ClientMenus.Where(m => m.Controller.ToUpper() == controller.ToUpper() && m.Layer == 2 && m.IsMenu == 1).FirstOrDefault();
        }
        return new Menu();
    }
    /// <summary>
    /// 是否有权限
    /// </summary>
    public static string IsLimits(HttpContext httpContext, string menucode)
    {
        if (httpContext.Session["ClientManager"] != null)
        {
            IntFactoryEntity.Users model = (IntFactoryEntity.Users)httpContext.Session["ClientManager"];
            if (model.Menus.Where(m => m.MenuCode == menucode).Count() > 0)
            {
                return "";
            }
        }
        return "nolimits";
    }

    /// <summary>
    /// 将对象转换成JSON对象
    /// </summary>
    /// <param name="html"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ToJSONString(this HtmlHelper html, object data)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        if (data != null && !string.IsNullOrEmpty(data.ToString()))
        {
            return serializer.Serialize(data);
        }

        return string.Empty;
    }
}
