using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 用户角色
    /// </summary>
    [Table("UserRoles")]
    public class UserRole : IdentityUserRole<int>
    {
    }
}