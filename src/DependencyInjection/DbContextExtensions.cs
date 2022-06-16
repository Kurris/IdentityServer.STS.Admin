using IdentityServer.STS.Admin.Configuration;
using IdentityServer.STS.Admin.Constants;
using IdentityServer.STS.Admin.Interfaces;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer.STS.Admin.DependencyInjection
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// 注册相关DbContext上下文
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TDataProtectionDbContext"></typeparam>
        public static void RegisterDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TDataProtectionDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IIdsPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IIdsConfigurationDbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            var identityConnectionString = configuration.GetConnectionString(ConfigurationConstants.IdentityDbConnectionStringKey);
            var configurationConnectionString = configuration.GetConnectionString(ConfigurationConstants.ConfigurationDbConnectionStringKey);
            var persistedGrantsConnectionString = configuration.GetConnectionString(ConfigurationConstants.PersistedGrantDbConnectionStringKey);
            var dataProtectionConnectionString = configuration.GetConnectionString(ConfigurationConstants.DataProtectionDbConnectionStringKey);

            var migrationsAssembly = "IdentityServer.STS.Admin";

            //aspnet core identity 用户操作
            services.AddDbContext<TIdentityDbContext>(options =>
            {
                options.UseMySql(identityConnectionString, MySqlServerVersion.LatestSupportedServerVersion, builder =>
                {
                    builder.MigrationsAssembly(migrationsAssembly);
                    builder.EnableRetryOnFailure();
                });

                // options.EnableSensitiveDataLogging();
                // var loggerFactory = provider.GetService<ILoggerFactory>();
                // options.UseLoggerFactory(loggerFactory);
            });

            //ids 配置操作
            services.AddConfigurationDbContext<TConfigurationDbContext>(options => options.ConfigureDbContext = b => b.UseMySql(configurationConnectionString, MySqlServerVersion.LatestSupportedServerVersion,
                builder => builder.MigrationsAssembly(migrationsAssembly)));

            //ids 持久化授权操作
            services.AddOperationalDbContext<TPersistedGrantDbContext>(options => options.ConfigureDbContext = b => b.UseMySql(persistedGrantsConnectionString, MySqlServerVersion.LatestSupportedServerVersion,
                builder => builder.MigrationsAssembly(migrationsAssembly)));

            //数据保护
            services.AddDbContext<TDataProtectionDbContext>(options => options.UseMySql(dataProtectionConnectionString, MySqlServerVersion.LatestSupportedServerVersion,
                builder => builder.MigrationsAssembly(migrationsAssembly)));
        }
    }
}