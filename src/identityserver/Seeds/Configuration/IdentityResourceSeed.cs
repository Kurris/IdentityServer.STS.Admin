using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class IdentityResourceSeed : IEntityTypeConfiguration<IdentityResource>
{
    public void Configure(EntityTypeBuilder<IdentityResource> builder)
    {
        builder.HasData(new List<IdentityResource>
        {
            new()
            {
                Id = 1,
                Enabled = true,
                Name = "openid",
                DisplayName = "Your user identifier",
                Description = null,
                Required = true,
                Emphasize = true,
                ShowInDiscoveryDocument = true,
                 UserClaims = null,
                // Properties = null,
                Created = DateTime.Now,
                Updated = null,
                NonEditable = false
            },
            new()
            {
                Id = 2,
                Enabled = true,
                Name = "email",
                DisplayName = "Email",
                Description = null,
                Required = false,
                Emphasize = false,
                ShowInDiscoveryDocument = true,
                // UserClaims = null,
                // Properties = null,
                Created = DateTime.Now,
                Updated = null,
                NonEditable = false
            },
            new()
            {
                Id = 3,
                Enabled = true,
                Name = "address",
                DisplayName = "Your address",
                Description =null,
                Required = false,
                Emphasize = false,
                ShowInDiscoveryDocument = true,
                // UserClaims = null,
                // Properties = null,
                Created = DateTime.Now,
                Updated = null,
                NonEditable = false
            },
            new()
            {
                Id = 4,
                Enabled = true,
                Name = "roles",
                DisplayName = "Roles",
                Description = null,
                Required = true,
                Emphasize = true,
                ShowInDiscoveryDocument = true,
                // UserClaims = null,
                // Properties = null,
                Created = DateTime.Now,
                Updated = null,
                NonEditable = false
            },
            new()
            {
                Id = 5,
                Enabled = true,
                Name = "profile",
                DisplayName = "profile",
                Description = "profile",
                Required = true,
                Emphasize = true,
                ShowInDiscoveryDocument = true,
                // UserClaims = null,
                // Properties = null,
                Created = DateTime.Now,
                Updated = null,
                NonEditable = false
            },
        });
    }
}