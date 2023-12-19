using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.STS.Admin.DependencyInjection;

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
        where TPersistedGrantDbContext : DbContext, IPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IConfigurationDbContext
        where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        var connectionString = configuration.GetConnectionString("Default");
        var migrationsAssembly = "IdentityServer.STS.Admin";

        //aspnet core identity 用户操作
        services.AddDbContext<TIdentityDbContext>(options => { options.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion, builder => { builder.MigrationsAssembly(migrationsAssembly); }); });

        //ids配置操作
        services.AddConfigurationDbContext<TConfigurationDbContext>(options => options.ConfigureDbContext = b => b.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion,
            builder =>
            {
                builder.MigrationsAssembly(migrationsAssembly);
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }));

        //ids持久化授权操作
        services.AddOperationalDbContext<TPersistedGrantDbContext>(options => options.ConfigureDbContext = b => b.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion,
            builder =>
            {
                builder.MigrationsAssembly(migrationsAssembly);
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }));

        //数据保护
        services.AddDbContext<TDataProtectionDbContext>(options => options.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion,
            builder =>
            {
                builder.MigrationsAssembly(migrationsAssembly);
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }));
    }
}