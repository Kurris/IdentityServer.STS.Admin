using IdentityServer4.Models;

namespace IdentityServer.STS.Admin.Models.Consent
{
    /// <summary>
    /// 处理同意屏幕的结果
    /// </summary>
    public class ProcessConsentResult
    {
        public bool IsRedirect => !string.IsNullOrEmpty(RedirectUri);

        public string RedirectUri { get; set; }

        public Client Client { get; set; }

        public bool HasValidationError => !string.IsNullOrEmpty(ValidationError);

        public string ValidationError { get; set; }
    }
}