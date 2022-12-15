using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ClientCorsOriginSeed : IEntityTypeConfiguration<ClientCorsOrigin>
{
    public void Configure(EntityTypeBuilder<ClientCorsOrigin> builder)
    {
        builder.HasData(new List<ClientCorsOrigin>
        {
            new()
            {
                Id = 1,
                Origin = "http://localhost:5005",
                ClientId = 1
            }
        });
    }
}