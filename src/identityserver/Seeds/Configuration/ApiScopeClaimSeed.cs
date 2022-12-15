using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ApiScopeClaimSeed : IEntityTypeConfiguration<ApiScopeClaim>
{
    public void Configure(EntityTypeBuilder<ApiScopeClaim> builder)
    {
        builder.HasData(new List<ApiScopeClaim>()
        {
            new()
            {
                Id = 1,
                Type = "name",
                ScopeId = 1
            },
            new()
            {
                Id = 2,
                Type = "role",
                ScopeId = 1
            },
        });
    }
}