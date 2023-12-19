using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.DbContexts;

public class Id4DataProtectionDbContext : DbContext, IDataProtectionKeyContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public Id4DataProtectionDbContext(DbContextOptions<Id4DataProtectionDbContext> options)
        : base(options)
    {
    }
}