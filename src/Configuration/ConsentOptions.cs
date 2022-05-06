namespace IdentityServer.STS.Admin.Configuration
{
    /// <summary>
    /// 同意屏幕配置
    /// </summary>
    public class ConsentOptions
    {
        /// <summary>
        /// 是否开启离线访问(令牌刷新)
        /// </summary>
        public static bool EnableOfflineAccess = true;

        /// <summary>
        /// 离线访问显示名称
        /// </summary>
        public static string OfflineAccessDisplayName = "Offline Access";

        /// <summary>
        /// 离线访问描述
        /// </summary>
        public static string OfflineAccessDescription = "Access to your applications and resources, even when you are offline";

        /// <summary>
        /// 选项缺少错误信息
        /// </summary>
        public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";

        /// <summary>
        /// 非法选择选项错误信息
        /// </summary>
        public static readonly string InvalidSelectionErrorMessage = "Invalid selection";
    }
}
