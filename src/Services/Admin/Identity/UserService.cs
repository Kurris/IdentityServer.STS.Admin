using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.DbContexts;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Interfaces;
using IdentityServer.STS.Admin.Interfaces.Identity;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.Services.Admin.Identity
{
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
                Username = user.UserName,
                Email = user.Email,
                IsConfirmEmail = user.EmailConfirmed,
                Phone = user.PhoneNumber,
                IsConfirmPhone = user.PhoneNumberConfirmed,
                IsEnableLockout = user.LockoutEnabled,
                LockoutExpire = user.LockoutEnd,
                IsEnableTwoFactor = user.TwoFactorEnabled,
                LoginFailCount = user.AccessFailedCount
            }).ToPaginationBy(x => x.Username, input, false);

            return pagination;
        }

        public async Task<UserDto> QueryUserByIdAsync(string id)
        {
            var user = await _identityDbContext.Users.FindAsync(id);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
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
            if (string.IsNullOrEmpty(dto.Id))
                throw new Exception("用户标识不能为空");

            var user = await _identityDbContext.Users.FindAsync(dto.Id);
            if (user == null)
                throw new Exception("用户不存在");

            user.UserName = dto.Username;
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
            if (!string.IsNullOrEmpty(dto.Id))
                throw new Exception("用户标识有误");

            if (await ExistsUserAsync(new UserExistsIn
                {
                    Username = dto.Username,
                    Email = dto.Email
                }))
                throw new Exception("已经存在相同的用户名称或者邮件地址");

            var user = new User
            {
                UserName = dto.Username,
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
            var username = dto.Username;
            var email = dto.Email;

            var exist = await _identityDbContext.Users.AnyAsync(x => x.UserName == username || x.Email == email);
            return exist;
        }

        public async Task<bool> ExistsUserAsync(string id)
        {
            return await _identityDbContext.Users.AnyAsync(x => x.Id == id);
        }
    }
}