using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer.STS.Admin.Configuration;
using IdentityServer.STS.Admin.IdentityServerExtension;
using IdentityServer4;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.STS.Admin.DependencyInjection;

/// <summary>
/// IdentityServer4 extensions
/// </summary>
public static class IdentityServer4Extensions
{
    /// <summary>
    /// 使用IdentityServer4服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <typeparam name="TConfigurationDbContext"></typeparam>
    /// <typeparam name="TPersistedGrantDbContext"></typeparam>
    /// <typeparam name="TUserIdentity"></typeparam>
    public static void AddIdentityServer4<TConfigurationDbContext, TPersistedGrantDbContext, TUserIdentity>(this IServiceCollection services,
        IConfiguration configuration)
        where TPersistedGrantDbContext : DbContext, IPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IConfigurationDbContext
        where TUserIdentity : class
    {
        var frontBaseUrl = configuration.GetSection("FrontendBaseUrl").Value;

        services.AddIdentityServer(options =>
        {
            options.IssuerUri = "identity.isawesome.cn";
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            //自定义交互地址
            options.UserInteraction.LoginUrl = $"{frontBaseUrl}/signIn";
            options.UserInteraction.LoginReturnUrlParameter = "returnUrl";

            options.UserInteraction.ErrorUrl = $"{frontBaseUrl}/error";
            options.UserInteraction.LogoutUrl = $"{frontBaseUrl}/logout";
            options.UserInteraction.ConsentUrl = $"{frontBaseUrl}/consent";
            options.UserInteraction.DeviceVerificationUrl = $"{frontBaseUrl}/device";

            //options.UserInteraction = new UserInteractionOptions
            //{
            //    LoginUrl = "login", //登录地址
            //    LoginReturnUrlParameter = "returnUrl", //设置登录后的"返回地址"的参数名称默认：returnUrl 
            //    //LogoutUrl = null, //注销地址            z
            //    LogoutIdParameter = "logoutId", //注销页面id，默认：logoutId
            //   // ConsentUrl = null, //授权同意页面
            //    ConsentReturnUrlParameter = "returnUrl ", //设置"返回地址"的参数名称，默认：returnUrl 
            //   // ErrorUrl = null, //"错误页面地址"
            //    ErrorIdParameter = "errorId", //错误id，默认：errorId
            //    CustomRedirectReturnUrlParameter = "returnUrl", //设置从授权端点传递给自定义重定向的返回URL参数的名称。默认：returnUrl
            //    CookieMessageThreshold = 5,
            //    DeviceVerificationUrl = "/device",
            //    DeviceVerificationUserCodeParameter = "userCode"
            //};
        })
        .AddConfigurationStore<TConfigurationDbContext>()
        .AddOperationalStore<TPersistedGrantDbContext>()
        .AddAspNetIdentity<TUserIdentity>().AddProfileService<UserProfile>().AddResourceOwnerValidator<BackendPasswordValidator>() //自定义资源拥有者验证 //添加aspnetcore user,用于id4管理用户
        .AddCustomSigningCredential(configuration) //签名
        .AddCustomValidationKey(configuration) //验签
        .AddExtensionGrantValidator<DelegationGrantValidator>()//自定义授权模式
        //.AddConfigurationStoreCache(); 配置缓存
        ;
    }

    /// <summary>
    /// 添加自定义验证key
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static IIdentityServerBuilder AddCustomValidationKey(this IIdentityServerBuilder builder, IConfiguration configuration)
    {
        var certificateConfiguration = configuration.GetSection("CertificateConfiguration").Get<CertificateOption>();

        if (certificateConfiguration.UseValidationCertificatePfxFile)
        {
            if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificatePfxFilePath))
            {
                throw new Exception("验签证书路径没有定义");
            }

            if (File.Exists(certificateConfiguration.ValidationCertificatePfxFilePath))
            {
                try
                {
                    builder.AddValidationKey(new X509Certificate2(certificateConfiguration.ValidationCertificatePfxFilePath, certificateConfiguration.ValidationCertificatePfxFilePassword));
                }
                catch (Exception e)
                {
                    throw new Exception("创建签名密钥时发生错误", e);
                }
            }
            else
            {
                throw new FileNotFoundException($"签名密钥文件: {certificateConfiguration.ValidationCertificatePfxFilePath} 不存在");
            }
        }

        return builder;
    }

    /// <summary>
    /// 添加自定义签名凭证
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder, IConfiguration configuration)
    {
        var certificateConfiguration = configuration.GetSection("CertificateConfiguration").Get<CertificateOption>();

        //开发者签名
        if (certificateConfiguration.UseTemporarySigningKeyForDevelopment)
        {
            builder.AddDeveloperSigningCredential(signingAlgorithm: IdentityServerConstants.RsaSigningAlgorithm.RS256);
        }
        else if (certificateConfiguration.UseSigningCertificatePfxFile)
        {
            if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificatePfxFilePath))
            {
                throw new Exception("验签证书路径没有定义");
            }

            if (File.Exists(certificateConfiguration.SigningCertificatePfxFilePath))
            {
                try
                {
                    builder.AddSigningCredential(new X509Certificate2(certificateConfiguration.SigningCertificatePfxFilePath, certificateConfiguration.SigningCertificatePfxFilePassword));
                }
                catch (Exception e)
                {
                    throw new Exception("创建签名密钥时发生错误", e);
                }
            }
            else
            {
                throw new Exception($"签名密钥文件: {certificateConfiguration.SigningCertificatePfxFilePath} 不存在");
            }
        }
        else
        {
            throw new Exception("签名类型未定义");
        }

        return builder;
    }
}