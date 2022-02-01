using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using IdentityServer.STS.Admin.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.DbContexts
{
    public class IdentityDbContext : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            //根据TableAttribute重名identity实体
            var entities = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsDefined(typeof(TableAttribute)));
            foreach (var entity in entities)
            {
                var table = entity.GetCustomAttribute<TableAttribute>();

                builder.Entity(entity).ToTable(table.Name);
            }
        }
    }
}