namespace IdentityServer.STS.Admin.Models.Manager;


/// <summary>
/// 移除外部关联登录入参
/// </summary>
public class RemoveExternalLoginInput
{
    /// <summary>
    /// 登录提供器
    /// </summary>
    public string LoginProvider { get; set; }

    /// <summary>
    /// 提供器key
    /// </summary>
    public string ProviderKey { get; set; }
}