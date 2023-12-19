using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IdentityServer.STS.Admin.Models.Admin.Identity;

public enum ClientType
{
    /// <summary>
    /// 空，默认
    /// </summary>
    [Description("空，默认")]
    Empty = 0,

    /// <summary>
    /// Web 应用程序 - Javascript Authorization Code Flow with PKCE
    /// </summary>
    [Description("Web 应用程序 - Javascript Authorization Code Flow with PKCE")]
    Web = 1,

    /// <summary>
    ///单页应用程序 - 服务器端 Authorization Code Flow with PKCE
    /// </summary>
    [Description("单页应用程序 - 服务器端 Authorization Code Flow with PKCE")]
    Spa = 2,

    /// <summary>
    /// 原生应用程序 - 移动/桌面 Authorization Code Flow with PKCE
    /// </summary>
    [Description("原生应用程序 - 移动/桌面 Authorization Code Flow with PKCE")]
    Native = 3,

    /// <summary>
    /// 机械/机器人 资源所有者密码和客户端凭据流
    /// </summary>
    [Description("机械/机器人 资源所有者密码和客户端凭据流")]
    Machine = 4,

    /// <summary>
    /// 电视或者限制输入设备应用程序 设备流程
    /// </summary>
    [Description("电视或者限制输入设备应用程序 设备流程")]
    Device = 5
}


public class EnumEx
{
    public static IEnumerable<SelectItem<int, string>> GetEnumTypes<T>() where T : Enum
    {
        var type = typeof(T);

        var fieldInfos = type.GetFields().Where(x => x.IsStatic);

        var res = new List<SelectItem<int, string>>(fieldInfos.Count());
        foreach (var fieldInfo in fieldInfos)
        {
            if (fieldInfo.IsDefined(typeof(DescriptionAttribute), false))
            {
                var descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
                res.Add(new SelectItem<int, string>((int) fieldInfo.GetValue(type), descriptionAttribute.Description));
            }
            else
            {
                res.Add(new SelectItem<int, string>((int) fieldInfo.GetValue(type), fieldInfo.Name));
            }
        }

        return res;
    }
}