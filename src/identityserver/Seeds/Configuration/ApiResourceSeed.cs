using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ApiResourceSeed : IEntityTypeConfiguration<ApiResource>
{
    public void Configure(EntityTypeBuilder<ApiResource> builder)
    {
        builder.HasData(new List<ApiResource>()
        {
            new()
            {
                Id = 1,
                Enabled = true,
                Name = "Identity.Admin",
                DisplayName = "Identity.Admin",
                Description = null,
                AllowedAccessTokenSigningAlgorithms = null,
                ShowInDiscoveryDocument = false,
                // Secrets = null,
                // Scopes = null,
                // UserClaims = null,
                // Properties = null,
                Created = DateTime.Now,
                Updated = null,
                LastAccessed = null,
                NonEditable = false
            }
        });
    }
}