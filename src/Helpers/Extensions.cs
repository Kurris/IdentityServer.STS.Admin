using System;
using IdentityServer4.Models;

namespace IdentityServer.STS.Admin.Helpers
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
    }
}