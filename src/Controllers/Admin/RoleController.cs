using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Interfaces.Identity;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.STS.Admin.Controllers.Admin
{
    [Authorize("Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }


        [HttpGet("page")]
        public async Task<Pagination<Role>> QueryRolePage([FromQuery] RoleSearchInput input)
        {
            return await _roleService.QueryRolePageAsync(input);
        }


        [HttpGet("roles")]
        public async Task<IEnumerable<Role>> QueryRoles()
        {
            return await _roleService.QueryRolesAsync();
        }



        [HttpGet]
        public async Task<Role> QueryRole(string id)
        {
            return await _roleService.QueryRoleByIdAsync(id);
        }


        [HttpPost]
        public async Task SaveRole(Role role)
        {
            await _roleService.SaveRole(role);
        }


        [HttpGet("userPage")]
        public async Task<Pagination<User>> QueryRoleUserPage([FromQuery] RoleUserSearchPageIn pageIn)
        {
            return await _roleService.QueryRoleUserPage(pageIn);
        }
    }
}