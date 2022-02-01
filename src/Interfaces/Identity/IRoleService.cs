using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;

namespace IdentityServer.STS.Admin.Interfaces.Identity
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> QueryRolesAsync();
        Task<Pagination<RoleDto>> QueryRolePageAsync(RoleSearchInput input);
        Task<RoleDto> QueryRoleByIdAsync(string id);
        Task UpdateRoleAsync(RoleDto dto);
        Task AddRoleAsync(RoleDto dto);
        Task<bool> ExistsRoleAsync(RoleDto dto);
        Task<bool> ExistsRoleAsync(string id);
    }
}