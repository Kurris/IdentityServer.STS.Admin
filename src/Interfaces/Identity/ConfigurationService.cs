using System.Collections.Generic;

namespace IdentityServer.STS.Admin.Interfaces.Identity
{
    public interface IConfigurationService
    {
        IEnumerable<string> GetStandardClaims();
    }
}