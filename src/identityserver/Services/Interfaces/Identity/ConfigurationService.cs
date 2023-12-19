using System.Collections.Generic;

namespace IdentityServer.STS.Admin.Services.Interfaces.Identity;

public interface IConfigurationService
{
    IEnumerable<string> GetStandardClaims();
}