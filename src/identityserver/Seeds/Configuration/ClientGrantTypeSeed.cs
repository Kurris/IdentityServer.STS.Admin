using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ClientGrantTypeSeed : IEntityTypeConfiguration<ClientGrantType>
{
    public void Configure(EntityTypeBuilder<ClientGrantType> builder)
    {
        builder.HasData(new List<ClientGrantType>
        {
            new()
            {
                Id = 1,
                GrantType = GrantTypes.Code.First(),
                ClientId = 1,
            }
        });
    }
}