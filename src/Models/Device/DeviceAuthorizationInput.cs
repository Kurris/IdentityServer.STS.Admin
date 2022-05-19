using IdentityServer.STS.Admin.Models.Consent;

namespace IdentityServer.STS.Admin.Models.Device
{
    /// <summary>
    /// 设备授权入参
    /// </summary>
    public class DeviceAuthorizationInput : ConsentInput
    {
        /// <summary>
        /// 用户验证码
        /// </summary>
        public string UserCode { get; set; }
    }
}