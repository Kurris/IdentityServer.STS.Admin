namespace IdentityServer.STS.Admin.Models.Account
{
    /// <summary>
    /// 外部登录入参
    /// </summary>
    public class ExternalLoginInput
    {
        /// <summary>
        /// 重定向地址
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 登录提供器
        /// </summary>
        public string Provider { get; set; }
    }
}