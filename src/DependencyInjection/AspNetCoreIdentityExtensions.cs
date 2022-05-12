using IdentityServer.STS.Admin.Helpers;
using IdentityServer.STS.Admin.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.STS.Admin.DependencyInjection
{
    public static class AspNetCoreIdentity
    {
        /// <summary>
        /// 添加identity验证服务，包括identity模型，外部登录提供起
        /// </summary>
        /// <typeparam name="TIdentityDbContext">DbContext for Identity</typeparam>
        /// <typeparam name="TUser">User Identity class</typeparam>
        /// <typeparam name="TRole">User Identity Role class</typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddAspIdentity<TIdentityDbContext, TUser, TRole>(this IServiceCollection services, IConfiguration configuration)
            where TIdentityDbContext : DbContext
            where TUser : class
            where TRole : class
        {
            services.AddSingleton<IdentityOptions>() //默认配置
                .AddScoped<ApplicationSignInManager<TUser>>() //用户登录管理器
                .AddIdentity<TUser, TRole>() //用户,角色
                .AddEntityFrameworkStores<TIdentityDbContext>() //aspnetcore user 操作
                .AddDefaultTokenProviders(); //token生成，验证提供器

            //配置cookie，处理chrome类浏览器cookie策略问题
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.Secure = CookieSecurePolicy.SameAsRequest;
                options.OnAppendCookie = cookieContext =>
                    AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            services.AddAuthentication()
                .AddGitHub(options =>
                {
                    options.ClientId = "6aced974f4ac1536ff1d";
                    options.ClientSecret = "a9cca44681973f866de814371ee81c70959f651a";
                    options.AccessDeniedPath = "/api/authenticate/externalLoginCallback";

                    options.Scope.Add("user:email");
                    options.Scope.Add("user");
                    options.SaveTokens = true;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin")
                        .RequireAuthenticatedUser();
                });
            });
        }
    }
}