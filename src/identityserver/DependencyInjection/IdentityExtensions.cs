using System;
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

namespace IdentityServer.STS.Admin.DependencyInjection;

public static class IdentityExtensions
{
    /// <summary>
    /// 添加identity验证服务，包括identity模型，外部登录提供器
    /// </summary>
    /// <typeparam name="TIdentityDbContext">DbContext for Identity</typeparam>
    /// <typeparam name="TUser">User Identity class</typeparam>
    /// <typeparam name="TRole">User Identity Role class</typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static IServiceCollection AddIdentity<TIdentityDbContext, TUser, TRole>(this IServiceCollection services, IConfiguration configuration)
        where TIdentityDbContext : DbContext
        where TUser : class
        where TRole : class
    {
        services.AddIdentity<TUser, TRole>()//用户,角色
            .AddEntityFrameworkStores<TIdentityDbContext>() //aspnetcore user 操作
            .AddDefaultTokenProviders().AddErrorDescriber<CustomIdentityErrorDescriber>()//token生成，验证提供器和错误描述器
            .AddSignInManager<AppSignInManager<TUser>>(); //替换登陆器

        //配置cookie，处理chrome类浏览器cookie策略问题
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            options.Secure = CookieSecurePolicy.SameAsRequest;
            options.OnAppendCookie = cookieContext => AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext => AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });

        //必须在AddIdentity之后使用
        //https://learn.microsoft.com/zh-cn/aspnet/core/security/authentication/identity-configuration
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true; //数字
            options.Password.RequireLowercase = true; //小写字母
            options.Password.RequireUppercase = true; //大写
            options.Password.RequireNonAlphanumeric = true; //非数字字母
            options.Password.RequiredUniqueChars = 1;//默认

            options.SignIn.RequireConfirmedEmail = true;//需要验证邮件
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            options.Lockout.MaxFailedAccessAttempts = 5;//登陆失败次数
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);//冻结时间

            options.User.RequireUniqueEmail = true; //邮件是否唯一
        });

        services.AddAuthentication().AddOAuth2ExternalProviderSetting(configuration);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
            {
                policy.RequireRole("Admin")
                      .RequireAuthenticatedUser();
            });
        });


        return services;
    }

    /// <summary>
    /// 添加oauth2.0登录
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    private static AuthenticationBuilder AddOAuth2ExternalProviderSetting(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        if (AbleIntegrate(configuration, "GitHub"))
        {
            builder.AddGitHub(options => options.SetDefaultSetting(configuration, "GitHub"));
        }

        if (AbleIntegrate(configuration, "Alipay"))
        {
            builder.AddAlipay(options => options.SetDefaultSetting(configuration, "Alipay"));
        }

        if (AbleIntegrate(configuration, "Discord"))
        {
            builder.AddDiscord(options => options.SetDefaultSetting(configuration, "Discord"));
        }

        if (AbleIntegrate(configuration, "Weibo"))
        {
            builder.AddWeibo(options =>
            {
                options.SetDefaultSetting(configuration, "Weibo");
                options.Scope.Remove("email");
            });
        }


        if (AbleIntegrate(configuration, "Jinko"))
        {
            builder.AddJinko(options =>
            {
                options.SetDefaultSetting(configuration, "Jinko");
            });
        }

        return builder;
    }

    private static bool AbleIntegrate(IConfiguration configuration, string scheme)
    {
        var clientId = configuration.GetSection($"OAuth:{scheme}:ClientId").Value;
        var clientSecret = configuration.GetSection($"OAuth:{scheme}:ClientSecret").Value;

        return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret);
    }

    /// <summary>
    /// 默认处理
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configuration"></param>
    /// <param name="scheme"></param>
    // ReSharper disable once UnusedMethodReturnValue.Local
    private static OAuthOptions SetDefaultSetting(this OAuthOptions options, IConfiguration configuration, string scheme)
    {
        var frontendBaseUrl = configuration.GetSection("FrontendBaseUrl").Value;

        options.ClientId = configuration.GetSection($"OAuth:{scheme}:ClientId").Value;
        options.ClientSecret = configuration.GetSection($"OAuth:{scheme}:ClientSecret").Value;

        options.SaveTokens = true;
        options.ReturnUrlParameter = "returnUrl";

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

        options.Events.OnTicketReceived = context =>
        {
            //provider的token
            return Task.CompletedTask;
        };

        return options;
    }
}