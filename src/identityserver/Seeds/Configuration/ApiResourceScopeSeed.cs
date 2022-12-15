using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ApiResourceScopeSeed : IEntityTypeConfiguration<ApiResourceScope>
{
    public void Configure(EntityTypeBuilder<ApiResourceScope> builder)
    {
        builder.HasData(new List<ApiResourceScope>
        {
            new()
            {
                Id = 1,
                Scope = "identity.admin.api",
                ApiResourceId = 1
            }
        });
    }
}