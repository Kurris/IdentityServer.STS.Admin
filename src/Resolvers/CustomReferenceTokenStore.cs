using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;

namespace IdentityServer.STS.Admin.Resolvers
{
    public class CustomReferenceTokenStore : DefaultReferenceTokenStore
    {
        public CustomReferenceTokenStore(IPersistedGrantStore store
            , IPersistentGrantSerializer serializer
            , IHandleGenerationService handleGenerationService
            , ILogger<CustomReferenceTokenStore> logger) : base(store, serializer, handleGenerationService, logger)
        {
        }
    }
}