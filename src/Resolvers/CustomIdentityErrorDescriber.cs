using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Resolvers
{
    /// <summary>
    /// 自定义处理identity framework的错误提醒
    /// </summary>
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