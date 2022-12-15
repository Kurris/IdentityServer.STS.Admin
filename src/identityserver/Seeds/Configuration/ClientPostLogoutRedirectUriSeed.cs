using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ClientPostLogoutRedirectUriSeed : IEntityTypeConfiguration<ClientPostLogoutRedirectUri>
{
    public void Configure(EntityTypeBuilder<ClientPostLogoutRedirectUri> builder)
    {
        builder.HasData(new List<ClientPostLogoutRedirectUri>
        {
            new()
            {
                Id = 1,
                PostLogoutRedirectUri = "http://localhost:5005/oauth/logout",
                ClientId = 1,
            }
        });
    }
}