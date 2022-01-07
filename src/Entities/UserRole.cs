using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    [Table("UserRoles")]
    public class UserRole : IdentityUserRole<string>
    {
        
    }
}