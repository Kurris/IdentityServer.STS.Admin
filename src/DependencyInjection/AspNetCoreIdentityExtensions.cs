using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using IdentityServer.STS.Admin.Helpers;
using IdentityServer.STS.Admin.Resolvers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
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
                .AddScoped<AppSignInManager<TUser>>() //用户登录管理器
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

            var frontendBaseUrl = configuration.GetSection("FrontendBaseUrl").Value;

            services.AddAuthentication()
                .AddGitHub(options =>
                {
                    options.ClientId = "6aced974f4ac1536ff1d";
                    options.ClientSecret = "a9cca44681973f866de814371ee81c70959f651a";

                    options.Scope.Add("user:email");
                    options.Scope.Add("read:user");

                    options.SetOnRemoteFailure(frontendBaseUrl).WithDefaultSetting();
                })
                .AddWeibo(options =>
                {
                    options.ClientId = "3217031503";
                    options.ClientSecret = "4b03e98edacf79eaeb75ec131699f52a";

                    options.SaveTokens = true;
                    options.ReturnUrlParameter = "returnUrl";

                    options.SetOnRemoteFailure(frontendBaseUrl).WithDefaultSetting();
                })
                .AddDiscord(options =>
                {
                    options.ClientId = "986458746777657364";
                    options.ClientSecret = "BwPpzFswPvYqrM1OSHjVgD_PX0VliCMI";

                    options.SetOnRemoteFailure(frontendBaseUrl).WithDefaultSetting();
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

        /// <summary>
        /// 远程处理异常
        /// </summary>
        /// <param name="options"></param>
        /// <param name="frontendBaseUrl"></param>
        private static OAuthOptions SetOnRemoteFailure(this OAuthOptions options, string frontendBaseUrl)
        {
            options.Events.OnRemoteFailure = async context =>
            {
                if (context.Properties != null)
                {
                    var p = context.Properties.RedirectUri.IndexOf('?');
                    var l = context.Properties.RedirectUri.Length;
                    var kv = HttpUtility.ParseQueryString(context.Properties.RedirectUri.Substring(p + 1, l - p - 1));

                    var query = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["remoteError"] = context.Failure.Message,
                        ["returnUrl"] = kv.Get("returnUrl"),
                    });

                    context.Response.Redirect($"{frontendBaseUrl}/error?{await query.ReadAsStringAsync()}");
                }
                else
                {
                    context.Response.Redirect($"{frontendBaseUrl}/error?remoteError={context.Failure.Message}");
                }
            };

            return options;
        }


        private static OAuthOptions WithDefaultSetting(this OAuthOptions options)
        {
            options.SaveTokens = true;
            options.ReturnUrlParameter = "returnUrl";
            return options;
        }
    }
}