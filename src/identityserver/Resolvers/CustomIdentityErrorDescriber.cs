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

        public override IdentityError DuplicateEmail(string email) =>
            new()
            {
                Code = nameof(DuplicateEmail),
                Description = $"邮件地址{email}重复"
            };

        public override IdentityError DuplicateUserName(string userName) =>
            new()
            {
                Code = nameof(DuplicateUserName),
                Description = $"用户名{userName}重复"
            };
    }
}