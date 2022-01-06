using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    [Table("UserLogins")]
    public class UserLogin : IdentityUserLogin<string>
    {
        
    }
}