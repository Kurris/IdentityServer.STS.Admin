using System;
using System.Collections.Specialized;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityServer.STS.Admin
{
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

        public static bool IsLocal(this string returnUrl, string currentIp = "")
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return false;
            }

            return returnUrl.Contains(currentIp, StringComparison.OrdinalIgnoreCase)
                   || returnUrl.Contains("localhost", StringComparison.OrdinalIgnoreCase)
                   || returnUrl.Contains(Authorize, StringComparison.OrdinalIgnoreCase)
                   || returnUrl.Contains(AuthorizeCallback, StringComparison.OrdinalIgnoreCase);
        }

        public static DateTime? ToLocalDateTime(this DateTime? dateTime)
        {
            return dateTime != null
                ? dateTime.Value.Kind == DateTimeKind.Utc
                    ? dateTime.Value.ToLocalTime()
                    : dateTime.Value
                : default;
        }

        public static DateTime? ToLocalDateTime(this DateTimeOffset? dateTime)
        {
            return dateTime != null
                ? dateTime.Value.DateTime.Kind == DateTimeKind.Utc
                    ? dateTime.Value.DateTime.ToLocalTime()
                    : dateTime.Value.DateTime
                : default;
        }

        public static NameValueCollection ReadQueryStringAsNameValueCollection(this string url)
        {
            if (url != null)
            {
                var idx = url.IndexOf('?');
                if (idx >= 0)
                {
                    url = url.Substring(idx + 1);
                }

                var query = QueryHelpers.ParseNullableQuery(url);
                if (query != null)
                {
                    return query.AsNameValueCollection();
                }
            }

            return new NameValueCollection();
        }

        /// <summary>
        /// 是否为本地地址， 例如： "/" 和"～/"  
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsLocalUrl(this string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            // Allows "/" or "/foo" but not "//" or "/\".
            if (url[0] == '/')
            {
                // url is exactly "/"
                if (url.Length == 1)
                {
                    return true;
                }

                // url doesn't start with "//" or "/\"
                if (url[1] != '/' && url[1] != '\\')
                {
                    return true;
                }

                return false;
            }

            // Allows "~/" or "~/foo" but not "~//" or "~/\".
            if (url[0] == '~' && url.Length > 1 && url[1] == '/')
            {
                // url is exactly "~/"
                if (url.Length == 2)
                {
                    return true;
                }

                // url doesn't start with "~//" or "~/\"
                if (url[2] != '/' && url[2] != '\\')
                {
                    return true;
                }

                return false;
            }

            return false;
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
}