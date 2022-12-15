using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Seeds.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.DbContexts
{
    /// <summary>
    /// IdentityServer4 配置dbContext
    /// </summary>
    public class IdsConfigurationDbContext : ConfigurationDbContext<IdsConfigurationDbContext>
    {
        public IdsConfigurationDbContext(DbContextOptions<IdsConfigurationDbContext> options, ConfigurationStoreOptions storeOptions)
            : base(options, storeOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new ClientCorsOriginSeed());
            modelBuilder.ApplyConfiguration(new ClientGrantTypeSeed());
            modelBuilder.ApplyConfiguration(new ClientOwnerSeed());
            modelBuilder.ApplyConfiguration(new ClientPostLogoutRedirectUriSeed());
            modelBuilder.ApplyConfiguration(new ClientRedirectUriSeed());
            modelBuilder.ApplyConfiguration(new ClientScopeSeed());
            modelBuilder.ApplyConfiguration(new ClientSeed());
            
            modelBuilder.ApplyConfiguration(new IdentityResourceSeed());
            modelBuilder.ApplyConfiguration(new IdentityResourceClaimSeed());
            
            modelBuilder.ApplyConfiguration(new ApiScopeSeed());
            modelBuilder.ApplyConfiguration(new ApiScopeClaimSeed());
            modelBuilder.ApplyConfiguration(new ApiResourceSeed());
            modelBuilder.ApplyConfiguration(new ApiResourceScopeSeed());
        }

        public DbSet<ClientOwners> ClientOwners { get; set; }

        public DbSet<ApiResourceProperty> ApiResourceProperties { get; set; }

        public DbSet<IdentityResourceProperty> IdentityResourceProperties { get; set; }

        public DbSet<ApiResourceSecret> ApiSecrets { get; set; }

        public DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        public DbSet<IdentityResourceClaim> IdentityClaims { get; set; }

        public DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }

        public DbSet<ClientGrantType> ClientGrantTypes { get; set; }

        public DbSet<ClientScope> ClientScopes { get; set; }

        public DbSet<ClientSecret> ClientSecrets { get; set; }

        public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }

        public DbSet<ClientIdPRestriction> ClientIdPRestrictions { get; set; }

        public DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }

        public DbSet<ClientClaim> ClientClaims { get; set; }

        public DbSet<ClientProperty> ClientProperties { get; set; }

        public DbSet<ApiScopeProperty> ApiScopeProperties { get; set; }

        public DbSet<ApiResourceScope> ApiResourceScopes { get; set; }
    }
}