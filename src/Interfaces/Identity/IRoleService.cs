using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;

namespace IdentityServer.STS.Admin.Interfaces.Identity
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> QueryRolesAsync();
        Task<Pagination<Role>> QueryRolePageAsync(RoleSearchInput input);
        Task<Role> QueryRoleByIdAsync(string id);
        Task SaveRole(Role role);

        Task<Pagination<User>> QueryRoleUserPage(RoleUserSearchPageIn pageIn);

         Task<bool> ExistsRoleAsync(Role dto);
        Task<bool> ExistsRoleAsync(string id);
    }
}