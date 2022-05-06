using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 角色声明
    /// </summary>
    [Table("RoleClaims")]
    public class RoleClaim : IdentityRoleClaim<int>
    {
    }
}