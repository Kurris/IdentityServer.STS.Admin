using System.ComponentModel.DataAnnotations;
using IdentityServer.STS.Admin.Enums;

namespace IdentityServer.STS.Admin.Models.Account;

/// <summary>
/// 忘记密码入参
/// </summary>
public class ForgotPasswordInput
{
    [Required(ErrorMessage = "用户名或邮箱不能为空")]
    public string Content { get; set; }
}