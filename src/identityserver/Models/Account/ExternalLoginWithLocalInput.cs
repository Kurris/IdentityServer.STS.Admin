using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account;

public class ExternalLoginWithLocalInput
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; }

    /// <summary>
    /// 跳转链接
    /// </summary>
    public string ReturnUrl { get; set; }
}