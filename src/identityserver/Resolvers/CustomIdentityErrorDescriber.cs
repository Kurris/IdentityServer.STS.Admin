using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Resolvers;

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

    public override IdentityError ConcurrencyFailure()
    {
        return new IdentityError
        {
            Code = nameof(ConcurrencyFailure),
            Description = "并发更新,执行失败"
        };
    }

    public override IdentityError UserNotInRole(string role)
    {
        return new IdentityError
        {
            Code = nameof(UserNotInRole),
            Description = $"当前用户不存在角色[{role}]"
        };
    }

    public override IdentityError DuplicateRoleName(string role)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateRoleName),
            Description = $"[{role}]重复"
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return base.PasswordTooShort(length);
    }

    public override IdentityError InvalidToken()
    {
        return new IdentityError
        {
            Code = nameof(InvalidToken),
            Description = "非法的token"
        };
    }

    public override IdentityError UserLockoutNotEnabled()
    {
        return new IdentityError
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = "用户验证失败锁定功能尚未启用"
        };
    }

    public override IdentityError UserAlreadyHasPassword()
    {
        return new IdentityError
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = "用户已经存在密码"
        };
    }

    public override IdentityError InvalidUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidUserName),
            Description = "用户名不合法"
        };
    }

    public override IdentityError UserAlreadyInRole(string role)
    {
        return new IdentityError
        {
            Code = nameof(UserAlreadyInRole),
            Description = $"用户已经存在角色[{role}]"
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "密码中需要有一个 0-9 之间的数字"
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "密码中需要有小写字符"
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "密码中需要有大写字符"
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "密码中需要有非字母数字字符"
        };
    }

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = "密码中需要有非重复字符数"
        };
    }
}