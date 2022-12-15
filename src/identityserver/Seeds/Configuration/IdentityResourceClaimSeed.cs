using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class IdentityResourceClaimSeed : IEntityTypeConfiguration<IdentityResourceClaim>
{
    public void Configure(EntityTypeBuilder<IdentityResourceClaim> builder)
    {
        var identityResourceClaims = new List<IdentityResourceClaim>
        {
            new()
            {
                Id = 1,
                Type = "sub",
                IdentityResourceId = 1
            }
        };

        identityResourceClaims.Add(new()
        {
            Id = 2,
            Type = "email",
            IdentityResourceId = 2
        });

        identityResourceClaims.Add(new()
        {
            Id = 3,
            Type = "address",
            IdentityResourceId = 3
        });

        identityResourceClaims.Add(new()
        {
            Id = 4,
            Type = "role",
            IdentityResourceId = 4
        });

        var profile = new
        {
            IdentityResourceId = 5,
            Claims = new[]
            {
                "name",
                "family_name",
                "given_name",
                "middle_name",
                "nickname",
                "preferred_username",
                "profile",
                "picture",
                "website",
                "gender",
                "birthdate",
                "zoneinfo",
                "locale",
                "updated_at"
            }
        };
        for (var i = 0; i < profile.Claims.Length; i++)
        {
            identityResourceClaims.Add(new IdentityResourceClaim
            {
                Id = i + 5,
                Type = profile.Claims[i],
                IdentityResourceId = profile.IdentityResourceId
            });
        }

        builder.HasData(identityResourceClaims);
    }
}