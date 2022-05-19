using System.Collections.Generic;

namespace IdentityServer.STS.Admin.Models.Consent
{
    public class ConsentInput
    {
        public bool Allow { get; set; }

        public IEnumerable<string> ScopesConsented { get; set; }

        public bool RememberConsent { get; set; } = true;

        public string ReturnUrl { get; set; }

        public string Description { get; set; }
    }
}