namespace IdentityServer.STS.Admin.Models.Manager;

/// <summary>
/// 双重验证返回值
/// </summary>
public class TwoFactorAuthenticationOutput
{
    /// <summary>
    /// 是否存在双重验证器
    /// </summary>
    public bool HasAuthenticator { get; set; }

    /// <summary>
    /// 剩余的恢复码个数
    /// </summary>
    public int RecoveryCodesLeft { get; set; }

    public bool Is2FaEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }
}