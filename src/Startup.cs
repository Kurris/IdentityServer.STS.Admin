using System;
using System.Text;
using IdentityServer4.Services;
using IdentityServer.STS.Admin.DbContexts;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using IdentityServer4.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            services.AddCors(setup => { setup.AddDefaultPolicy(policy => { policy.SetIsOriginAllowed(x => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials(); }); });

            //数据访问层,非ids4操作
            services.RegisterDbContexts<IdentityDbContext
                , IdsConfigurationDbContext
                , IdsPersistedGrantDbContext
                , IdsDataProtectionDbContext>(Configuration);

            services.AddAspNetIdentityAuthenticationServices<IdentityDbContext, User, Role>(Configuration);
            services.AddIdentityServer<IdsConfigurationDbContext, IdsPersistedGrantDbContext, User>(Configuration);

            //options.UserInteraction.LoginUrl = "http://localhost:8080/signIn";
            //options.UserInteraction.ErrorUrl = "http://localhost:8080/error";
            //options.UserInteraction.LogoutUrl = "http://localhost:8080/logout";
            //options.UserInteraction.ConsentUrl = "http://localhost:8080/consent";
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = async op =>
                {
                    op.RedirectUri = " http://localhost:8080/signIn";
                    await Task.CompletedTask;
                };
                //options.LoginPath = "http://localhost:8080/signIn";
                //options.LogoutPath = "http://localhost:8080/logout";
                //options.AccessDeniedPath = "http://localhost:8080/error";
            });

            //配置本地登录cookie过期跳转到登录界面
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(15);
                options.Events.OnRedirectToLogin = async context =>
                {
                    var apiResult = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ApiResult<object>()
                    {
                        Code = 302,
                        Data = "http://localhost:8080/signIn"
                    }, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    }));

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    context.Response.ContentLength = apiResult.Length;

                    //MVC
                    //context.Response.RedirectToAbsoluteUrl("绝对路径？ReturnUrl=");

                    await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(apiResult));
                    await Task.CompletedTask;
                };
            });

            services.AddTransient<IReturnUrlParser, ReturnUrlParser>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //chrome 内核 80版本 cookie策略问题
            app.UseCookiePolicy(new CookiePolicyOptions() {MinimumSameSitePolicy = SameSiteMode.Lax});

            app.UseCors();

            app.UseIdentityServer();
            //app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}