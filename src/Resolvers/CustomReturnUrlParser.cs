using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace IdentityServer.STS.Admin.Resolvers
{
    public class CustomReturnUrlParser : IReturnUrlParser
    {
        private const string Authorize = "connect/authorize";
        private const string AuthorizeCallback = Authorize + "/callback";

        private readonly IAuthorizeRequestValidator _validator;
        private readonly IUserSession _userSession;

        private readonly ILogger<CustomReturnUrlParser> _logger;

        public CustomReturnUrlParser(IAuthorizeRequestValidator validator,
            IUserSession userSession,
            ILogger<CustomReturnUrlParser> logger)
        {
            _validator = validator;
            _userSession = userSession;
            _logger = logger;
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
                    _logger.LogTrace("AuthorizationRequest being returned");
                    return result.ValidatedRequest.ToAuthorizationRequest();
                }
            }

            _logger.LogTrace("No AuthorizationRequest being returned");
            return null;
        }

        /// <summary>
        /// 是否为合法的重定向地址
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public bool IsValidReturnUrl(string returnUrl)
        {
            if (returnUrl.IsMvcLocalUrl() || returnUrl.IsLocal())
            {
                _logger.LogTrace("returnUrl is valid");
                return true;
            }

            _logger.LogTrace("returnUrl is not valid");
            return false;
        }
    }
}