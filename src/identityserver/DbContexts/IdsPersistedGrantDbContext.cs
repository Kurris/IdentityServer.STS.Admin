using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using IdentityServer.STS.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.DbContexts
{
    /// <summary>
    /// IdentityServer4's persisted grant dbcontext
    /// </summary>
    public class IdsPersistedGrantDbContext : PersistedGrantDbContext<IdsPersistedGrantDbContext>
    {
        public IdsPersistedGrantDbContext(DbContextOptions<IdsPersistedGrantDbContext> options, OperationalStoreOptions storeOptions)
            : base(options, storeOptions)
        {
        }
    }
}