using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    [Table("UserClaims")]
    public class UserClaim : IdentityUserClaim<string>
    {
    }
}