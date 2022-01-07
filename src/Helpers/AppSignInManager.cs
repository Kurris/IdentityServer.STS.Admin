using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer.STS.Admin.Helpers
{
    public class AppSignInManager<TUser> : SignInManager<TUser> where TUser : class
    {
        private readonly ILogger<AppSignInManager<TUser>> _logger;
        private readonly UserManager<TUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserClaimsPrincipalFactory<TUser> _userClaimsPrincipalFactory;
        private readonly IOptions<IdentityOptions> _options;

        public AppSignInManager(UserManager<TUser> userManager
            , IHttpContextAccessor contextAccessor
            , IUserClaimsPrincipalFactory<TUser> claimsFactory
            , IOptions<IdentityOptions> optionsAccessor
            , ILogger<SignInManager<TUser>> logger
            , IAuthenticationSchemeProvider schemes
            , IUserConfirmation<TUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }
    }
}