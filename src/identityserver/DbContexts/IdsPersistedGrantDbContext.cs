using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.DbContexts;

/// <summary>
/// IdentityServer4's persisted grant dbcontext
/// </summary>
public class Id4PersistedGrantDbContext : PersistedGrantDbContext<Id4PersistedGrantDbContext>
{
    public Id4PersistedGrantDbContext(DbContextOptions<Id4PersistedGrantDbContext> options, OperationalStoreOptions storeOptions)
        : base(options, storeOptions)
    {
    }
}