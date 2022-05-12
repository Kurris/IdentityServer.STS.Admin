using System;

namespace IdentityServer.STS.Admin.Configuration
{
    /// <summary>
    /// 账号选项
    /// </summary>
    public class AccountOptions
    {
        /// <summary>
        /// 是否允许本地登录
        /// </summary>
        public const bool AllowLocalLogin = true;

        /// <summary>
        /// 是否允许记住登录
        /// </summary>
        public const bool AllowRememberLogin = true;

        /// <summary>
        /// 是否显示退出登录提醒
        /// </summary>
        public const bool ShowLogoutPrompt = true;

        /// <summary>
        /// 是否在退出登录后自动重定向
        /// </summary>
        public const bool AutomaticRedirectAfterSignOut = true;
    }
}