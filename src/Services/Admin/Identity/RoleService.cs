using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.DbContexts;
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

        public async Task<IEnumerable<RoleDto>> QueryRolesAsync()
        {
            return await _identityDbContext.Roles.Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }

        public async Task<Pagination<RoleDto>> QueryRolePageAsync(RoleSearchInput input)
        {
            throw new System.NotImplementedException();
        }

        public async Task<RoleDto> QueryRoleByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateRoleAsync(RoleDto dto)
        {
            throw new System.NotImplementedException();
        }

        public async Task AddRoleAsync(RoleDto dto)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> ExistsRoleAsync(RoleDto dto)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> ExistsRoleAsync(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}