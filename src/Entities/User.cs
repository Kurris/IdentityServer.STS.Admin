using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    [Table("Users")]
    public class User : IdentityUser
    {
    }
}