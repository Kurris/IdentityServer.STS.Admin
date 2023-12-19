using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer.STS.Admin.Resolvers;

public class AppSignInManager<TUser> : SignInManager<TUser>
    where TUser : class
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppSignInManager(UserManager<TUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IUserClaimsPrincipalFactory<TUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<AppSignInManager<TUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<TUser> confirmation) : base(userManager, httpContextAccessor,
        claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
    {
        var claims = additionalClaims.ToList();

        var externalResult = await _httpContextAccessor?.HttpContext?.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (externalResult != null && externalResult.Succeeded)
        {
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                claims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            if (authenticationProperties != null)
            {
                // if the external provider issued an id_token, we'll keep it for sign out
                var idToken = externalResult.Properties?.GetTokenValue("id_token");
                if (idToken != null)
                {
                    authenticationProperties.StoreTokens(new[]
                    {
                        new AuthenticationToken {Name = "id_token", Value = idToken}
                    });
                }
            }

            var authenticationMethod = claims.FirstOrDefault(x => x.Type == ClaimTypes.AuthenticationMethod);
            var idp = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IdentityProvider);

            if (authenticationMethod != null && idp == null)
            {
                claims.Add(new Claim(JwtClaimTypes.IdentityProvider, authenticationMethod.Value));
            }
        }

        await base.SignInWithClaimsAsync(user, authenticationProperties, claims);
    }
}