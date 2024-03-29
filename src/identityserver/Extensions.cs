using System;
using System.Collections.Specialized;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityServer.STS.Admin;

public static class Extensions
{
    /// <summary>
    /// 授权url后缀
    /// </summary>
    private const string Authorize = "connect/authorize";

    /// <summary>
    /// 授权回调url后缀
    /// </summary>
    private const string AuthorizeCallback = Authorize + "/callback";


    /// <summary>
    /// 检查重定向地址是否为本地客户端
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool IsNativeClient(this AuthorizationRequest context)
    {
        return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
               && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
    }

    /// <summary>
    /// 是否为本地路径
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static bool IsLocal(this string returnUrl, string content = "")
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            return false;
        }

        return returnUrl.Contains(content, StringComparison.OrdinalIgnoreCase)
               || returnUrl.Contains(Authorize, StringComparison.OrdinalIgnoreCase)
               || returnUrl.Contains(AuthorizeCallback, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 时间转换成本地文化格式
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime? ToLocalDateTime(this DateTimeOffset? dateTime)
    {
        return dateTime != null
            ? dateTime.Value.DateTime.Kind == DateTimeKind.Utc
                ? dateTime.Value.DateTime.ToLocalTime()
                : dateTime.Value.DateTime
            : default;
    }

    /// <summary>
    /// 获取url的query参数
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static NameValueCollection ReadQueryStringAsNameValueCollection(this string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return new NameValueCollection();
        }

        var idx = url.IndexOf('?');
        if (idx >= 0)
        {
            url = url[(idx + 1)..];
        }

        var query = QueryHelpers.ParseNullableQuery(url);
        return query.AsNameValueCollection();
    }

    internal static AuthorizationRequest ToAuthorizationRequest(this ValidatedAuthorizeRequest request)
    {
        var authRequest = new AuthorizationRequest
        {
            Client = request.Client,
            DisplayMode = request.DisplayMode,
            RedirectUri = request.RedirectUri,
            UiLocales = request.UiLocales,
            IdP = request.GetIdP(),
            Tenant = request.GetTenant(),
            LoginHint = request.LoginHint,
            PromptModes = request.PromptModes,
            AcrValues = request.GetAcrValues(),
            ValidatedResources = request.ValidatedResources
        };

        authRequest.Parameters.Add(request.Raw);

        return authRequest;
    }
}