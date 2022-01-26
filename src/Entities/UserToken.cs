using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 用户令牌
    /// </summary>
    [Table("UserTokens")]
    public class UserToken : IdentityUserToken<string>
    {
    }
}