using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly UserClaimsPrincipalFactory<User, Role> _userClaimsPrincipalFactory;

        public UserProfile(UserClaimsPrincipalFactory<User, Role> userClaimsPrincipalFactory)
        {
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userClaimsPrincipalFactory.UserManager.FindByIdAsync(sub);

            var userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);
            //userClaims附加了user和role的claims,避免重复
            var claims = userClaims.Claims.Distinct(new ClaimTypeComparer()).ToList();

            var issuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type));
            context.IssuedClaims.AddRange(issuedClaims);
        }

        /// <summary>
        /// 设置subject是否在当前client中可获取token
        /// </summary>
        /// <param name="context"></param>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            //可以做黑名单处理

            //var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            context.IsActive = true;
            await Task.CompletedTask;
        }

        private class ClaimTypeComparer : IEqualityComparer<Claim>
        {
            public bool Equals(Claim x, Claim y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Type == y.Type;
            }

            public int GetHashCode(Claim obj)
            {
                return HashCode.Combine(obj.Type);
            }
        }
    }
}