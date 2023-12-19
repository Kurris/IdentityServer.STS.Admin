using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account;

/// <summary>
/// 重置密码入参
/// </summary>
public class ResetPasswordInput
{
    /// <summary>
    /// 邮件
    /// </summary>
    [Required(ErrorMessage = "邮件不能为空")]
    [EmailAddress(ErrorMessage = "邮件格式不正确")]
    public string Email { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    public string Password { get; set; }

    /// <summary>
    /// 二次确认密码
    /// </summary>
    [Compare(nameof(Password), ErrorMessage = "确认密码不匹配")]
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// 邮件验证code
    /// </summary>
    public string Code { get; set; }
}