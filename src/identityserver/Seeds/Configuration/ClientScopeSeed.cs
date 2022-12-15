using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ClientScopeSeed : IEntityTypeConfiguration<ClientScope>
{
    public void Configure(EntityTypeBuilder<ClientScope> builder)
    {
        builder.HasData(new List<ClientScope>
        {
            new()
            {
                Id = 1,
                Scope = "openid",
                ClientId = 1,
            },
            new()
            {
                Id = 2,
                Scope = "profile",
                ClientId = 1,
            }
        });
    }
}