using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer.STS.Admin.Helpers
{
    public class ReferenceTokenTools
    {
        private readonly ITokenService _tokenService;

        public ReferenceTokenTools(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<string> IssueReferenceToken(int lifetime, string issuer, string description, IEnumerable<Claim> claims = null, ICollection<string> audiences = null)
        {
            if (string.IsNullOrWhiteSpace(issuer)) throw new ArgumentNullException(nameof(issuer));
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var tokenModel = new Token
            {
                Audiences = audiences,
                ClientId = "reference",
                CreationTime = DateTime.Now,
                Issuer = issuer,
                Lifetime = lifetime,
                Type = OidcConstants.TokenTypes.AccessToken,
                AccessTokenType = AccessTokenType.Reference,
                Claims = new HashSet<Claim>(claims, new ClaimComparer()),
                Description = description
            };

            var token = await _tokenService.CreateSecurityTokenAsync(tokenModel);

            return token;
        }
    }
}