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
using IdentityServer.STS.Admin.DependencyInjection;
using IdentityServer.STS.Admin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using IdentityServer.STS.Admin.Filters;
using IdentityServer.STS.Admin.Interfaces.Identity;
using IdentityServer.STS.Admin.Resolvers;
using IdentityServer.STS.Admin.Services;
using IdentityServer.STS.Admin.Services.Admin.Identity;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using QrCodeServer;
using StackExchange.Redis;
using Role = IdentityServer.STS.Admin.Entities.Role;

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
            var frontendBaseUrl = Configuration.GetSection("FrontendBaseUrl").Value;

            services.AddDataProtection();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //跨域处理
            services.AddCors(setup =>
            {
                setup.AddPolicy("idCors", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            //mvc filters
            services.AddMvc(options =>
            {
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ModelValidateFilter>();
            });

            //序列化/格式化/循环引用
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            //数据访问层
            services.RegisterDbContexts<IdentityDbContext
                , IdsConfigurationDbContext
                , IdsPersistedGrantDbContext
                , IdsDataProtectionDbContext>(Configuration);

            services.AddAspIdentity<IdentityDbContext, User, Role>(Configuration);
            services.AddIdentityServer4<IdsConfigurationDbContext, IdsPersistedGrantDbContext, User>(Configuration);

            //配置本地登录cookie相关处理
            //还没有这个配置会在401/403跳转到框架默认的路由,但这是mvc的处理,api模式写入结果json到response,最后由前端拦截处理,交互
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
                        Data = $"{frontendBaseUrl}/signIn"
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
                        Data = $"{frontendBaseUrl}/accessDenied"
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

            //IdentityServer4 Cors,在框架cors之后处理
            services.AddSingleton<ICorsPolicyService>(provider =>
            {
                var logger = provider.GetService<ILogger<DefaultCorsPolicyService>>();
                return new DefaultCorsPolicyService(logger)
                {
                    AllowedOrigins = new[] {Configuration.GetSection("FrontendBaseUrl").Value},
                    AllowAll = true
                };
            });
            
            services.AddSingleton<RedisClient>();
            services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect("isawesome.cn:6379,password=zxc111"));

            services.AddScoped<IReturnUrlParser, CustomReturnUrlParser>();
            services.AddScoped<IRedirectUriValidator, CustomRedirectUriValidator>();

            services.AddSingleton(typeof(IApiResult), typeof(ApiResult<object>));
            services.Configure<MailkitOptions>(Configuration.GetSection(nameof(MailkitOptions)));
            services.AddSingleton<EmailGenerateService>();
            services.AddScoped<ReferenceTokenToolService>();
            services.AddSingleton<EmailService>();
            RedirectHelper.Initialize(frontendBaseUrl, "/successed", "/error");

            //admin service registered
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IIdentityResourceService, IdentityResourceService>();

            services.AddScoped<IApiResourceService, ApiResourceService>();
            services.AddScoped<IApiScopeService, ApiScopeService>();
            services.AddScoped<IClientService, ClientService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("idCors");
            app.UseMiddleware<GlobalExceptionMiddleware>();

            //chrome 内核 80版本 cookie策略问题
            app.UseCookiePolicy(new CookiePolicyOptions {MinimumSameSitePolicy = SameSiteMode.Lax});

            app.UseRouting();
            app.UseIdentityServer();

            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}