using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Resolvers
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordMismatch() =>
            new()
            {
                Code = nameof(PasswordMismatch),
                Description = "密码不匹配"
            };
    }
}