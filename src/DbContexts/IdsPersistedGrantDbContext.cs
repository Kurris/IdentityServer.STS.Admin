using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using IdentityServer.STS.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.DbContexts
{
    /// <summary>
    /// identity server 4 持久数据层
    /// </summary>
    public class IdsPersistedGrantDbContext : PersistedGrantDbContext<IdsPersistedGrantDbContext>, IIdsPersistedGrantDbContext
    {
        public IdsPersistedGrantDbContext(DbContextOptions<IdsPersistedGrantDbContext> options, OperationalStoreOptions storeOptions)
            : base(options, storeOptions)
        {
        }
    }
}