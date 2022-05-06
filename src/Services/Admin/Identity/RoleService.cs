using System.Collections.Generic;
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
    public class RoleService : IRoleService
    {
        private readonly IdentityDbContext _identityDbContext;

        public RoleService(IdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<IEnumerable<Role>> QueryRolesAsync()
        {
            return await _identityDbContext.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<Pagination<Role>> QueryRolePageAsync(RoleSearchInput input)
        {
            return await _identityDbContext.Roles
                .WhereIf(!string.IsNullOrEmpty(input.Content), x => x.Name.Contains(input.Content))
                .ToPagination(input);
        }

        public async Task<Role> QueryRoleByIdAsync(int id)
        {
            var role = await _identityDbContext.Roles.FindAsync(id);
            return role;
        }

        public async Task SaveRole(Role role)
        {
            if (!await ExistsRoleAsync(role))
            {
                await _identityDbContext.Roles.AddAsync(role);
            }
            else
            {
                _identityDbContext.Roles.Update(role);
            }

            await _identityDbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsRoleAsync(Role role)
        {
            return await _identityDbContext.Roles.AnyAsync(x => x.Id == role.Id);
        }

        public async Task<bool> ExistsRoleAsync(int id)
        {
            return await _identityDbContext.Roles.AnyAsync(x => x.Id == id);
        }

        public Task<Pagination<User>> QueryRoleUserPage(RoleUserSearchPageIn pageIn)
        {
            var users = _identityDbContext.UserRoles.Where(x => x.RoleId == pageIn.RoleId)
                       .GroupJoin(_identityDbContext.Users, ur => ur.UserId, u => u.Id, (ur, u) => new { ur, u })
                       .SelectMany(x => x.u.DefaultIfEmpty(), (ur, u) => u)
                       .ToPagination(pageIn);
            return users;
        }
    }
}