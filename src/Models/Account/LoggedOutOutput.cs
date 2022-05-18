namespace IdentityServer.STS.Admin.Models.Account
{
    /// <summary>
    /// 退出登录返回值
    /// </summary>
    public class LoggedOutOutput
    {
        /// <summary>
        /// 注销后重定向地址
        /// </summary>
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 注销的iframe地址
        /// </summary>
        public string SignOutIframeUrl { get; set; }

        /// <summary>
        /// 注销后是否自动重定向
        /// </summary>
        public bool AutomaticRedirectAfterSignOut { get; set; }

        /// <summary>
        /// 退出登录标识
        /// </summary>
        public string LogoutId { get; set; }

        /// <summary>
        /// 触发外部登录退出
        /// </summary>
        public bool TriggerExternalSignOut => ExternalAuthenticationScheme != null;

        /// <summary>
        /// 外部登录scheme名称
        /// </summary>
        public string ExternalAuthenticationScheme { get; set; }
    }
}