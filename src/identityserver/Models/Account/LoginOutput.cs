using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer.STS.Admin.Enums;

namespace IdentityServer.STS.Admin.Models.Account
{
    public class LoginOutput : LoginInput
    {
        public bool EnableLocalLogin { get; set; } = true;
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));
        public bool IsExternalLoginOnly => !EnableLocalLogin && ExternalProviders?.Count() == 1;
        public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

        public bool ShowQrCodeOption { get; set; }
    }
}