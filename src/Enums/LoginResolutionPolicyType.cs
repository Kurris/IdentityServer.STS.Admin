namespace IdentityServer.STS.Admin.Enums
{
    /// <summary>
    /// 登录判断使用的策略
    /// </summary>
    public enum LoginResolutionPolicyType
    {
        /// <summary>
        /// 用户名
        /// </summary>
        Username = 0,

        /// <summary>
        /// 邮件
        /// </summary>
        Email = 1
    }
}