using System.Collections.Generic;

namespace IdentityServer.STS.Admin.Models.Consent
{
    public class ConsentOutputModel : ConsentInputModel
    {
        public string ClientName { get; set; }

        public string ClientUrl { get; set; }

        public string ClientLogoUrl { get; set; }

        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeOutputModel> IdentityScopes { get; set; }

        public IEnumerable<ScopeOutputModel> ApiScopes { get; set; }
    }
}
