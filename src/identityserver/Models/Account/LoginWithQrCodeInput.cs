namespace IdentityServer.STS.Admin.Models.Account;

public class LoginWithQrCodeInput
{
    /// <summary>
    /// 跳转链接
    /// </summary>
    public string ReturnUrl { get; set; }

    public string Key { get; set; }
}