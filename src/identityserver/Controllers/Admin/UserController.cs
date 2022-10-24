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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<UserDto> QueryUserById(int id)
        {
            return await _userService.QueryUserByIdAsync(id);
        }

        [HttpGet("page")]
        public async Task<Pagination<UserDto>> QueryUserPage([FromQuery] UserSearchInput input)
        {
            return await _userService.QueryUserPageAsync(input);
        }

        [HttpPost]
        public async Task AddUser(UserDto dto)
        {
            await _userService.AddUserAsync(dto);
        }

        [HttpGet("roles")]
        public async Task<IEnumerable<Role>> QueryUserRoles(int id)
        {
            return await _userService.QueryUserRoles(id);
        }

        [HttpPut]
        public async Task UpdateUser(UserDto dto)
        {
            await _userService.UpdateUserAsync(dto);
        }

        [HttpGet("userOrEmail/existence")]
        public async Task<bool> ExistsUser(UserExistsIn dto)
        {
            return await _userService.ExistsUserAsync(dto);
        }

        [HttpGet("externalProvider/page")]
        public async Task<Pagination<UserProviderDto>> QueryUserProviderPage([FromQuery] UserProviderSearchInput input)
        {
            return await _userService.QueryUserProviderPage(input);
        }

        [HttpGet("claims/page")]
        public async Task<Pagination<UserClaimsDto>> QueryUserClaimsPage([FromQuery] UserClaimsSearchInput input)
        {
            return await _userService.QueryUserClaimsPage(input);
        }
    }
}