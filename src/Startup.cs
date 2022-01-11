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
                setup.AddDefaultPolicy(policy =>
                {
                    policy.SetIsOriginAllowed(x => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });

            //数据访问层,非ids4操作
            services.RegisterDbContexts<IdentityDbContext
                , IdsConfigurationDbContext
                , IdsPersistedGrantDbContext
                , IdsDataProtectionDbContext>(Configuration);

            services.AddAspNetIdentityAuthenticationServices<IdentityDbContext, User, Role>(Configuration);
            services.AddIdentityServer<IdsConfigurationDbContext, IdsPersistedGrantDbContext, User>(Configuration);

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
            app.UseCookiePolicy(new CookiePolicyOptions() { MinimumSameSitePolicy = SameSiteMode.Lax });

            app.UseCors();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}