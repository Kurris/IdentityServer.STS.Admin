using System;
using System.IO;
using IdentityServer.STS.Admin.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;


namespace IdentityServer.STS.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddEnvironmentVariables()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                        .Build();

                    var certificateConfiguration = config.GetSection("CertificateConfiguration").Get<CertificateOption>();

                    string filename = certificateConfiguration.SigningCertificatePfxFilePath;
                    string password = certificateConfiguration.SigningCertificatePfxFilePassword;

                    if (File.Exists(filename))
                    {
                        webBuilder.ConfigureKestrel(options =>
                        {
                            options.ListenAnyIP(5000, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                listenOptions.UseHttps(filename, password);
                            });
                        });
                    }

                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((context, configuration) =>
                {
                    var serilog = configuration.MinimumLevel.Debug()
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                        .MinimumLevel.Override("IdentityServer4", LogEventLevel.Information)
                        .MinimumLevel.Override("AspNet.Security.OAuth", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}"
                            , theme: SystemConsoleTheme.Literate);

                    //生产环境仅显示warning日志
                    serilog.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", context.HostingEnvironment.IsDevelopment() ? LogEventLevel.Information : LogEventLevel.Warning);
                });
    }
}