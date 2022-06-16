using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer.STS.Admin.Configuration;
using IdentityServer.STS.Admin.IdentityServerExtension;
using IdentityServer.STS.Admin.Interfaces;
using IdentityServer4;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.STS.Admin.DependencyInjection
{
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
            where TPersistedGrantDbContext : DbContext, IIdsPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IIdsConfigurationDbContext
            where TUserIdentity : class
        {
            var frontBaseUrl = configuration.GetSection("FrontendBaseUrl").Value;

            services.AddIdentityServer(options =>
                {
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
                .AddAspNetIdentity<TUserIdentity>() //添加aspnetcore user
                .AddCustomSigningCredential(configuration) //证书
                .AddCustomValidationKey(configuration) //密钥
                .AddProfileService<UserProfile>()
                .AddExtensionGrantValidator<DelegationGrantValidator>(); //自定义授权模式
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

            if (certificateConfiguration.UseValidationCertificateThumbprint)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificateThumbprint))
                {
                    throw new Exception("指纹证书没有定义");
                }

                var certStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadOnly);

                var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateConfiguration.ValidationCertificateThumbprint, false);
                if (certCollection.Count == 0)
                {
                    throw new Exception("找不到指纹证书");
                }

                var certificate = certCollection[0];

                builder.AddValidationKey(certificate);
            }
            else if (certificateConfiguration.UseValidationCertificatePfxFile)
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

            //指纹
            if (certificateConfiguration.UseSigningCertificateThumbprint)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificateThumbprint))
                {
                    //  throw new Exception(SigningCertificateThumbprintNotFound);
                }

                StoreLocation storeLocation;
                var validOnly = certificateConfiguration.CertificateValidOnly;

                // Parse the Certificate StoreLocation
                var certStoreLocationLower = certificateConfiguration.CertificateStoreLocation.ToLower();

                if (certStoreLocationLower == StoreLocation.CurrentUser.ToString().ToLower()
                    || certificateConfiguration.CertificateStoreLocation == ((int) StoreLocation.CurrentUser).ToString())
                {
                    storeLocation = StoreLocation.CurrentUser;
                }
                else if (certStoreLocationLower == StoreLocation.LocalMachine.ToString().ToLower()
                         || certStoreLocationLower == ((int) StoreLocation.LocalMachine).ToString())
                {
                    storeLocation = StoreLocation.LocalMachine;
                }
                else
                {
                    storeLocation = StoreLocation.LocalMachine;
                    validOnly = true;
                }

                // Open Certificate
                var certStore = new X509Store(StoreName.My, storeLocation);
                certStore.Open(OpenFlags.ReadOnly);

                var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateConfiguration.SigningCertificateThumbprint, validOnly);
                if (certCollection.Count == 0)
                {
                    // throw new Exception(CertificateNotFound);
                }

                var certificate = certCollection[0];

                builder.AddSigningCredential(certificate);
            }
            //pfx
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
            //开发者签名
            else if (certificateConfiguration.UseTemporarySigningKeyForDevelopment)
            {
                builder.AddDeveloperSigningCredential(signingAlgorithm: IdentityServerConstants.RsaSigningAlgorithm.RS256);
            }
            else
            {
                throw new Exception("签名类型未定义");
            }

            return builder;
        }
    }
}