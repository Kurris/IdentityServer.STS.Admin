using System.Threading.Tasks;
using IdentityServer.STS.Admin.Entities;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.IdentityServerExtension
{
    public class UserProfile : IProfileService
    {
        private readonly UserManager<User> _userManager;

        public UserProfile(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            var claims = await _userManager.GetClaimsAsync(user);
            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            context.IsActive = user != null;
        }
    }
}