using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityServer.STS.Admin.Models.Manager;

public class AuthenticatorOutput : AuthenticatorInput
{
    /// <summary>
    /// SharedKey
    /// </summary>
    public string SharedKey { get; set; }

    /// <summary>
    /// 验证地址
    /// </summary>
    public string AuthenticatorUri { get; set; }
}