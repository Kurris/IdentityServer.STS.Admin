using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    [Table("Roles")]
    public class Role : IdentityRole
    {
    }
}