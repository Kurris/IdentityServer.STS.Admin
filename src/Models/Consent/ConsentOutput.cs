using System.Collections.Generic;

namespace IdentityServer.STS.Admin.Models.Consent
{
    public class ConsentOutput : ConsentInputModel
    {
        public string ClientName { get; set; }

        public string ClientUrl { get; set; }

        public string ClientLogoUrl { get; set; }

        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeOutput> IdentityScopes { get; set; }

        public IEnumerable<ScopeOutput> ApiScopes { get; set; }
    }
}
