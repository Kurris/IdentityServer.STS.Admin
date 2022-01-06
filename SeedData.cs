// using System;
// using IdentityServer4.EntityFramework.DbContexts;
// using IdentityServer4.EntityFramework.Mappers;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Internal;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace IdentityServer.STS.Admin
// {
//     public class SeedData
//     {
//         public static void EnsureSeedData(IServiceProvider serviceProvider)
//         {
//             Console.WriteLine("Seeding database...");
//
//             using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
//             {
//                 scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();
//
//                 var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
//                 context.Database.Migrate();
//                 EnsureSeedData(context);
//             }
//
//             Console.WriteLine("Done seeding database.");
//             Console.WriteLine();
//         }
//
//         private static void EnsureSeedData(ConfigurationDbContext context)
//         {
//             if (!context.Clients.Any())
//             {
//                 Console.WriteLine("Clients 正在初始化");
//                 foreach (var client in Config.Clients)
//                 {
//                     context.Clients.Add(client.ToEntity());
//                 }
//
//                 context.SaveChanges();
//             }
//
//             if (!context.IdentityResources.Any())
//             {
//                 Console.WriteLine("IdentityResources 正在初始化");
//                 foreach (var resource in Config.IdentityResources)
//                 {
//                     context.IdentityResources.Add(resource.ToEntity());
//                 }
//
//                 context.SaveChanges();
//             }
//
//             if (!context.ApiResources.Any())
//             {
//                 Console.WriteLine("ApiResources 正在初始化");
//                 foreach (var resource in Config.ApiResources)
//                 {
//                     context.ApiResources.Add(resource.ToEntity());
//                 }
//
//                 context.SaveChanges();
//             }
//
//             if (!context.ApiScopes.Any())
//             {
//                 Console.WriteLine("ApiScopes 正在初始化");
//                 foreach (var resource in Config.ApiScopes)
//                 {
//                     context.ApiScopes.Add(resource.ToEntity());
//                 }
//
//                 context.SaveChanges();
//             }
//         }
//     }
// }