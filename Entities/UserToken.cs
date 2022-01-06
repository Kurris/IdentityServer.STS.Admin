using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    [Table("UserTokens")]
    public class UserToken : IdentityUserToken<string>
    {
    }
}