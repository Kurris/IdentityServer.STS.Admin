using System.Collections.Generic;
using IdentityServer.STS.Admin.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ClientOwnerSeed : IEntityTypeConfiguration<ClientOwners>
{
    public void Configure(EntityTypeBuilder<ClientOwners> builder)
    {
        builder.HasData(new List<ClientOwners>
        {
            new()
            {
                Id = 1,
                ClientId = 1,
                UserId = 1
            }
        });
    }
}