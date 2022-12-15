using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Filters;
using IdentityServer.STS.Admin.Seeds.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.DbContexts
{
    /// <summary>
    /// aspnetcore identity framework's dbcontext
    /// </summary>
    public class IdentityDbContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //rename by TableAttribute
            var entities = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsDefined(typeof(TableAttribute)));
            foreach (var entity in entities)
            {
                var table = entity.GetCustomAttribute<TableAttribute>();

                builder.Entity(entity).ToTable(table.Name);
            }

            builder.ApplyConfiguration(new UserSeed());
            builder.ApplyConfiguration(new RoleSeed());
        }
    }
}