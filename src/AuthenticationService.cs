using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.STS.Admin
{
    public class AuthenticationService :  IAuthenticationService
    {
        public async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
        {
            throw new System.NotImplementedException();
        }

        public async Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }

        public async Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }

        public async Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }

        public async Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties)
        {
            throw new System.NotImplementedException();
        }
    }
}