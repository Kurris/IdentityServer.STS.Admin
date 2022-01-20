using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Resolvers
{
    /// <summary>
    /// 用户处理器
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class UserResolver<TUser> where TUser : class
    {
        private readonly UserManager<TUser> _userManager;

        public UserResolver(UserManager<TUser> userManager)
        {
            _userManager = userManager;
            
        }

        public async Task<TUser> GetUserAsync(string userNameOrEmail)
        {
            return await _userManager.FindByNameAsync(userNameOrEmail);

            // switch (_loginResolutionPolicy)
            // {
            //     case LoginResolutionPolicy.Username:
            //         return await _userManager.FindByNameAsync(userNameOrEmail);
            //     case LoginResolutionPolicy.Email:
            //         return await _userManager.FindByEmailAsync(userNameOrEmail);
            //     default:
            //         return null;
            // }
        }
    }
}