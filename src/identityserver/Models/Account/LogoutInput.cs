namespace IdentityServer.STS.Admin.Models.Account;

/// <summary>
/// 退出登录入参
/// </summary>
public class LogoutInput
{
    /// <summary>
    /// 退出登录标识
    /// </summary>
    public string LogoutId { get; set; }
}