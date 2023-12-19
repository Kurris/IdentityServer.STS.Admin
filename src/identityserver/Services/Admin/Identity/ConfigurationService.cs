using System.Collections.Generic;
using IdentityServer.STS.Admin.Configuration;
using IdentityServer.STS.Admin.Services.Interfaces.Identity;

namespace IdentityServer.STS.Admin.Services.Admin.Identity;

public class ConfigurationService : IConfigurationService
{
    public IEnumerable<string> GetStandardClaims()
    {
        return ClientConstants.StandardClaims;
    }
}