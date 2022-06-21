using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;

namespace IdentityServer.STS.Admin.Resolvers
{
    public class CustomReturnUrlParser : IReturnUrlParser
    {
        private readonly IAuthorizeRequestValidator _validator;
        private readonly IUserSession _userSession;


        public CustomReturnUrlParser(IAuthorizeRequestValidator validator,
            IUserSession userSession)
        {
            _validator = validator;
            _userSession = userSession;
        }

        public async Task<AuthorizationRequest> ParseAsync(string returnUrl)
        {
            if (IsValidReturnUrl(returnUrl))
            {
                var parameters = returnUrl.ReadQueryStringAsNameValueCollection();
                var user = await _userSession.GetUserAsync();
                var result = await _validator.ValidateAsync(parameters, user);

                if (!result.IsError)
                {
                    return result.ValidatedRequest.ToAuthorizationRequest();
                }
            }

            return null;
        }

        public bool IsValidReturnUrl(string returnUrl) => returnUrl.IsMvcLocalUrl() || returnUrl.IsLocal();
    }
}