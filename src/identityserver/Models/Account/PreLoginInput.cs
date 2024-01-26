using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account;

public class PreLoginInput
{
    /// <summary>
    /// 用户名称
    /// </summary>
    [Required(ErrorMessage = "用户账号不能为空")]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "用户密码不能为空")]
    public string Password { get; set; }
}
