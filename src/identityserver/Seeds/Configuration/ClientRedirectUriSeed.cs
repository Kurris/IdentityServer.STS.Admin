using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ClientRedirectUriSeed: IEntityTypeConfiguration<ClientRedirectUri>
{
    public void Configure(EntityTypeBuilder<ClientRedirectUri> builder)
    {
        builder.HasData(new List<ClientRedirectUri>
        {
            new()
            {
                Id = 1,
                RedirectUri = "http://localhost:5005/oauth/callback",
                ClientId = 1,
            }
        });
    }
}