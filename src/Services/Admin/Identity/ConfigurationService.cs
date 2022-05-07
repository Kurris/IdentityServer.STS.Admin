using System.Collections.Generic;
using IdentityServer.STS.Admin.Constants;
using IdentityServer.STS.Admin.Interfaces.Identity;

namespace IdentityServer.STS.Admin.Services.Admin.Identity
{
    public class ConfigurationService : IConfigurationService
    {
        public IEnumerable<string> GetStandardClaims()
        {
            return ClientConstants.StandardClaims;
        }
    }
}