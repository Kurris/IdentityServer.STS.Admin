using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.DbContexts;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer.STS.Admin.Services.Interfaces;
using IdentityServer.STS.Admin.Services.Interfaces.Identity;
using Microsoft.EntityFrameworkCore;


namespace IdentityServer.STS.Admin.Services.Admin.Identity;

public class UserService : IUserService
{
    private readonly IdentityDbContext _identityDbContext;

    public UserService(IdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<Pagination<UserDto>> QueryUserPageAsync(UserSearchInput input)
    {
        var query = _identityDbContext.Users
            .WhereIf(!string.IsNullOrEmpty(input.Content), x => x.Email.Contains(input.Content)
                                                                || x.UserName.Contains(input.Content));

        var pagination = await query.Select(user => new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            IsConfirmEmail = user.EmailConfirmed,
            Phone = user.PhoneNumber,
            IsConfirmPhone = user.PhoneNumberConfirmed,
            IsEnableLockout = user.LockoutEnabled,
            LockoutExpire = user.LockoutEnd,
            IsEnableTwoFactor = user.TwoFactorEnabled,
            LoginFailCount = user.AccessFailedCount
        }).ToPaginationBy(x => x.UserName, input, false);

        return pagination;
    }

    public async Task<UserDto> QueryUserByIdAsync(int id)
    {
        var user = await _identityDbContext.Users.FindAsync(id);
        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            IsConfirmEmail = user.EmailConfirmed,
            Phone = user.PhoneNumber,
            IsConfirmPhone = user.PhoneNumberConfirmed,
            IsEnableLockout = user.LockoutEnabled,
            LockoutExpire = user.LockoutEnd,
            IsEnableTwoFactor = user.TwoFactorEnabled,
            LoginFailCount = user.AccessFailedCount
        };
    }

    public async Task UpdateUserAsync(UserDto dto)
    {
        if (dto.Id <= 0)
            throw new Exception("用户标识不能为空");

        var user = await _identityDbContext.Users.FindAsync(dto.Id);
        if (user == null)
            throw new Exception("用户不存在");

        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.EmailConfirmed = dto.IsConfirmEmail;
        user.PhoneNumber = dto.Phone;
        user.PhoneNumberConfirmed = dto.IsConfirmPhone;
        user.LockoutEnabled = dto.IsEnableLockout;
        user.LockoutEnd = dto.LockoutExpire;
        user.TwoFactorEnabled = dto.IsEnableTwoFactor;
        user.AccessFailedCount = dto.LoginFailCount;

        await _identityDbContext.SaveChangesAsync();
    }

    public async Task AddUserAsync(UserDto dto)
    {
        if (dto.Id <= 0)
            throw new Exception("用户标识有误");

        if (await ExistsUserAsync(new UserExistsIn
            {
                UserName = dto.UserName,
                Email = dto.Email
            }))
            throw new Exception("已经存在相同的用户名称或者邮件地址");

        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            EmailConfirmed = dto.IsConfirmEmail,
            PhoneNumber = dto.Phone,
            PhoneNumberConfirmed = dto.IsConfirmPhone,
            LockoutEnabled = dto.IsEnableLockout,
            LockoutEnd = dto.LockoutExpire,
            TwoFactorEnabled = dto.IsEnableTwoFactor,
            AccessFailedCount = dto.LoginFailCount
        };

        _identityDbContext.Users.Add(user);
        await _identityDbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsUserAsync(UserExistsIn dto)
    {
        var userName = dto.UserName;
        var email = dto.Email;

        var exist = await _identityDbContext.Users.AnyAsync(x => x.UserName == userName || x.Email == email);
        return exist;
    }

    public async Task<bool> ExistsUserAsync(int id)
    {
        return await _identityDbContext.Users.AnyAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Role>> QueryUserRoles(int id)
    {
        var user = await QueryUserByIdAsync(id);
        if (user == null)
            return Enumerable.Empty<Role>();

        var rows = await _identityDbContext.UserRoles.Where(x => x.UserId == id)
            .GroupJoin(_identityDbContext.Roles,
                ur => ur.RoleId, r => r.Id, (userRole, roles) => new {userRole, roles})
            .SelectMany(x => x.roles.DefaultIfEmpty(), (userRoles, role) => role)
            .OrderBy(x => x.Name).ToListAsync();

        var query = from userRoles in _identityDbContext.UserRoles
                    where userRoles.UserId == id
                    join role in _identityDbContext.Roles on userRoles.RoleId equals role.Id into g
                    from n in g.DefaultIfEmpty()
                    select n;

        return await query.OrderBy(x => x.Name).ToListAsync();
    }


    public async Task<Pagination<UserProviderDto>> QueryUserProviderPage(UserProviderSearchInput input)
    {
        var pages = await _identityDbContext.UserLogins.Where(x => x.UserId == input.Id)
            .GroupJoin(_identityDbContext.Users,
                ul => ul.UserId, u => u.Id, (login, users) => new {login, users})
            .SelectMany(x => x.users.DefaultIfEmpty(), (x, y) => new UserProviderDto
            {
                UserId = y.Id,
                UserName = y.UserName,
                ProviderKey = x.login.ProviderKey,
                LoginProvider = x.login.LoginProvider,
                ProviderDisplayName = x.login.ProviderDisplayName
            })
            .OrderBy(x => x.LoginProvider)
            .ToPagination(input);

        return pages;
    }

    public async Task<Pagination<UserClaimsDto>> QueryUserClaimsPage(UserClaimsSearchInput input)
    {
        return await _identityDbContext.UserClaims.Where(x => x.UserId == input.UserId)
            .Select(x => new UserClaimsDto
            {
                ClaimId = x.Id,
                UserId = x.UserId,
                ClaimType = x.ClaimType,
                ClaimValue = x.ClaimValue
            }).ToPaginationBy(x => x.ClaimType, input, false);
    }
}