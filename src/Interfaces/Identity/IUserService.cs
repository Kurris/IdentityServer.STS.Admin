using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;

namespace IdentityServer.STS.Admin.Interfaces.Identity
{
    public interface IUserService
    {
        /// <summary>
        /// 查询用户分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Pagination<UserDto>> QueryUserPageAsync(UserSearchInput input);

        /// <summary>
        /// 根据id查询用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserDto> QueryUserByIdAsync(string id);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task UpdateUserAsync(UserDto dto);

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task AddUserAsync(UserDto dto);

        /// <summary>
        /// 是否存在用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> ExistsUserAsync(UserExistsIn dto);

        /// <summary>
        /// 是否存在用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExistsUserAsync(string id);

        /// <summary>
        /// 用户角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<RoleDto>> QueryUserRoles(string id);

        /// <summary>
        /// 用户外部提供器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Pagination<UserProviderDto>> QueryUserProviderPage(UserProviderSearchInput input);


        /// <summary>
        /// 用户声明
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Pagination<UserClaimsDto>> QueryUserClaimsPage(UserClaimsSearchInput input);
    }
}