using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account;

public class LoginInput
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
    [MinLength(6)]
    public string Password { get; set; }

    /// <summary>
    /// 是否记住密码
    /// </summary>
    public bool RememberLogin { get; set; }

    /// <summary>
    /// 跳转链接
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    /// 传入登陆租户
    /// </summary>
    public string Tenant { get; set; }
}