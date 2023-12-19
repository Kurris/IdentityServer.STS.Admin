using IdentityServer.STS.Admin.Models.Consent;

namespace IdentityServer.STS.Admin.Models.Device;

/// <summary>
/// 设备授权返回值
/// </summary>
public class DeviceAuthorizationOutput : ConsentOutput
{
    /// <summary>
    /// 用户验证码
    /// </summary>
    public string UserCode { get; set; }

    /// <summary>
    /// 确认用户验证码
    /// </summary>
    public bool ConfirmUserCode { get; set; }
}