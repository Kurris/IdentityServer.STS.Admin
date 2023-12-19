using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account;

/// <summary>
/// 注册入参
/// </summary>
public class RegisterInput
{
    [Required(ErrorMessage = "用户名不能为空")]
    [RegularExpression(@"^[a-zA-Z0-9_@\-\.\+]+$", ErrorMessage = "用户名只支持数字,字母和@-.+的组合")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "邮件不能为空")]
    [EmailAddress(ErrorMessage = "邮件格式不正确")]
    public string Email { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; }

    [Compare(nameof(Password), ErrorMessage = "确认密码不匹配")]
    public string ConfirmPassword { get; set; }

    public string ReturnUrl { get; set; }
}