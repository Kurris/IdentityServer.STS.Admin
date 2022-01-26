using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 用户外部登录
    /// </summary>
    [Table("UserLogins")]
    public class UserLogin : IdentityUserLogin<string>
    {
    }
}