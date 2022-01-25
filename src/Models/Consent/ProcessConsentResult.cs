using IdentityServer4.Models;

namespace IdentityServer.STS.Admin.Models.Consent
{
    public class ProcessConsentResult
    {
        public bool IsRedirect => RedirectUri != null;

        public string RedirectUri { get; set; }

        public Client Client { get; set; }

        public bool ShowView => ConsentModel != null;

        public ConsentOutputModel ConsentModel { get; set; }

        public bool HasValidationError => ValidationError != null;

        public string ValidationError { get; set; }
    }
}
