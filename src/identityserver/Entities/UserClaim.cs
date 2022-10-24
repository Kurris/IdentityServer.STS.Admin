using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 用户声明
    /// </summary>
    [Table("UserClaims")]
    public class UserClaim : IdentityUserClaim<int>
    {
        [Key]
        public override int Id { get; set; }
    }
}