using System.Collections.Generic;
using IdentityServer.STS.Admin.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.STS.Admin.Seeds.Identity;

public class UserSeed : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(new List<User>
        {
            new()
            {
                Id = 1,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "Ligy.97@foxmail.com",
                NormalizedEmail = "LIGY.97@FOXMAIL.COM",
                EmailConfirmed = false,
                PasswordHash = "AQAAAAEAACcQAAAAELpqgKMsKnyQhcGwsW0Tj5O0FQlrrpzJM7i3jvO+JNa+qMXothnApZsMrpm9hBX4+A==", //@p123456
                SecurityStamp = "6INQFJT2SDQEWLJWAJ4ARINXIY7MZ4SW",
                ConcurrencyStamp = "b8890833-d206-4d86-ba1b-3180d3b43e93",
                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0
            }
        });
    }
}