using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.STS.Admin.Resolvers;

/// <summary>
/// 自定义重定向uri验证
/// </summary>
public class CustomRedirectUriValidator : StrictRedirectUriValidator
{
    private static bool CustomCheck(IEnumerable<string> uris, string requestedUri)
    {
        return !uris.IsNullOrEmpty() && uris.Any(uri => requestedUri.StartsWith(uri, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 重写验证重定向地址,兼容重定向地址包含参数(避免空格问题)
    /// eg:swagger oauth2
    /// </summary>
    /// <param name="requestedUri"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public override async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
    {
        var result = CustomCheck(client.RedirectUris, requestedUri.Trim());
        return await Task.FromResult(result);
    }

    /// <summary>
    /// 重写验证退出后重定向地址,避免空格问题
    /// </summary>
    /// <param name="requestedUri"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public override async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
    {
        var result = await base.IsPostLogoutRedirectUriValidAsync(requestedUri.Trim(), client);
        return result;
    }
}