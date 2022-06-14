using System;
using System.Text;
using IdentityServer4.Services;
using IdentityServer.STS.Admin.DbContexts;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityServer.STS.Admin.DependencyInjection;
using IdentityServer.STS.Admin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using IdentityServer.STS.Admin.Filters;
using IdentityServer.STS.Admin.Interfaces.Identity;
using IdentityServer.STS.Admin.Resolvers;
using IdentityServer.STS.Admin.Services.Admin.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityServer.STS.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(setup =>
            {
                setup.AddPolicy("idCors", policy =>
                {
                    policy.SetIsOriginAllowed(x => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddMvc(options =>
            {
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ModelValidateFilter>();
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            //数据访问层,非ids4操作
            services.RegisterDbContexts<IdentityDbContext
                , IdsConfigurationDbContext
                , IdsPersistedGrantDbContext
                , IdsDataProtectionDbContext>(Configuration);

            services.AddAspIdentity<IdentityDbContext, User, Role>(Configuration);
            services.AddIdentityServer4<IdsConfigurationDbContext, IdsPersistedGrantDbContext, User>(Configuration);

            //必须在AddIdentity之后使用
            //配置identity约束
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = true;
            });

            //配置本地登录cookie相关处理
            services.ConfigureApplicationCookie(options =>
            {
                //本地登录30天cookie
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;

                //local登录,cookie过期触发
                options.Events.OnRedirectToLogin = async context =>
                {
                    var apiResult = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ApiResult<object>
                    {
                        Code = 302,
                        Msg = "登录失败",
                        Data = $"{Configuration.GetSection("FrontendBaseUrl").Value}/signIn"
                    }, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    }));

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    context.Response.ContentLength = apiResult.Length;

                    //MVC重定向
                    //context.Response.RedirectToAbsoluteUrl("绝对路径？ReturnUrl=");

                    await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(apiResult));
                };

                //local登录,访问无权限
                options.Events.OnRedirectToAccessDenied = async context =>
                {
                    var apiResult = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ApiResult<object>()
                    {
                        Code = 403,
                        Msg = "无权访问",
                        Data = $"{Configuration.GetSection("FrontendBaseUrl").Value}/accessDenied"
                    }, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    }));

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    context.Response.ContentLength = apiResult.Length;

                    await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(apiResult));
                };
            });

            //IdentityServer4 Cors
            services.AddSingleton<ICorsPolicyService>(provider =>
            {
                var logger = provider.GetService<ILogger<DefaultCorsPolicyService>>();
                return new DefaultCorsPolicyService(logger)
                {
                    AllowedOrigins = new[] {Configuration.GetSection("FrontendBaseUrl").Value},
                    AllowAll = false
                };
            });

            services.AddDataProtection();

            services.AddTransient(typeof(IApiResult), typeof(ApiResult<object>));

            services.Configure<MailkitOptions>(Configuration.GetSection(nameof(MailkitOptions)));
            services.AddSingleton<EmailGenerateService>();

            //admin service registered
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddSingleton<EmailService>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IIdentityResourceService, IdentityResourceService>();
            services.AddTransient<IReturnUrlParser, CustomReturnUrlParser>();
            services.AddTransient<IApiResourceService, ApiResourceService>();
            services.AddTransient<IApiScopeService, ApiScopeService>();
            services.AddTransient<IClientService, ClientService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }

            //chrome 内核 80版本 cookie策略问题
            app.UseCookiePolicy(new CookiePolicyOptions {MinimumSameSitePolicy = SameSiteMode.Lax});
            app.UseCors("idCors");
            app.UseIdentityServer();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}