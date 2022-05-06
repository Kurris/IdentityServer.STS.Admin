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
        public static bool AllowLocalLogin = true;

        /// <summary>
        /// 是否允许记住登录
        /// </summary>
        public static bool AllowRememberLogin = true;

        /// <summary>
        /// 记住登录的持续时间
        /// </summary>
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        /// <summary>
        /// 是否显示退出登录提醒
        /// </summary>
        public static bool ShowLogoutPrompt = true;

        /// <summary>
        /// 是否在退出登录后自动重定向
        /// </summary>
        public static bool AutomaticRedirectAfterSignOut = false;

        /// <summary>
        /// 非法的凭证错误信息
        /// </summary>
        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}