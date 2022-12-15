using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ApiScopeSeed : IEntityTypeConfiguration<ApiScope>
{
    public void Configure(EntityTypeBuilder<ApiScope> builder)
    {
        builder.HasData(new List<ApiScope>()
        {
            new()
            {
                Id = 1,
                Enabled = true,
                Name = "identity.admin.api",
                DisplayName = "identity.admin.api",
                Description = null,
                Required = true,
                Emphasize = false,
                ShowInDiscoveryDocument = false,
                // UserClaims = null,
                // Properties = null
            }
        });
    }
}