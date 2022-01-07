using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    [Table("RoleClaims")]
    public class RoleClaim : IdentityRoleClaim<string>
    {
        
    }
}