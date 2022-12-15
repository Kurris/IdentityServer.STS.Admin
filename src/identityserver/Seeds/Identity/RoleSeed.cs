using System;
using System.Collections.Generic;
using IdentityServer.STS.Admin.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Identity;

public class RoleSeed : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(new List<Role>
        {
            new()
            {
                Id = 1,
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        });
    }
}