using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.STS.Admin.Resolvers
{
    public class CustomRedirectUriValidator : StrictRedirectUriValidator
    {
        private static bool CustomCheck(IEnumerable<string> uris, string requestedUri)
        {
            return !uris.IsNullOrEmpty() && uris.Any(uri => requestedUri.StartsWith(uri, StringComparison.OrdinalIgnoreCase));
        }

        public override Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(CustomCheck(client.RedirectUris, requestedUri));
        }

        public override Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }
    }
}