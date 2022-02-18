using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.DbContexts;
using IdentityServer.STS.Admin.Interfaces;
using IdentityServer.STS.Admin.Interfaces.Identity;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer.STS.Admin.Services.Admin.Identity
{
    public class ClientService : IClientService
    {
        private readonly IdsConfigurationDbContext _idsConfigurationDbContext;

        private const string SharedSecret = "SharedSecret";

        public ClientService(IdsConfigurationDbContext idsConfigurationDbContext)
        {
            _idsConfigurationDbContext = idsConfigurationDbContext;
        }

        public async Task<Pagination<Client>> QueryClientPage(ClientSearchPageIn pageIn)
        {
            return await _idsConfigurationDbContext.Clients
                .Include(x => x.ClientSecrets)
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.Claims)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Properties)
                .ToPagination(pageIn);
        }

        public async Task SaveClient(ClientInput client)
        {
            if (client.Id == 0)
            {
                await _idsConfigurationDbContext.Clients.AddAsync(client);
            }
            else
            {
                var secrets = await _idsConfigurationDbContext.ClientSecrets.Where(x => x.ClientId == client.Id).ToListAsync();
                _idsConfigurationDbContext.ClientSecrets.RemoveRange(secrets);

                var grantTypes = await _idsConfigurationDbContext.ClientGrantTypes.Where(x => x.ClientId == client.Id).ToListAsync();
                _idsConfigurationDbContext.ClientGrantTypes.RemoveRange(grantTypes);

                var redirectUris = await _idsConfigurationDbContext.ClientRedirectUris.Where(x => x.ClientId == client.Id).ToListAsync();
                _idsConfigurationDbContext.ClientRedirectUris.RemoveRange(redirectUris);

                var postLogoutRedirectUris = await _idsConfigurationDbContext.ClientPostLogoutRedirectUris.Where(x => x.ClientId == client.Id).ToListAsync();
                _idsConfigurationDbContext.ClientPostLogoutRedirectUris.RemoveRange(postLogoutRedirectUris);

                var scopes = await _idsConfigurationDbContext.ClientScopes.Where(x => x.ClientId == client.Id).ToListAsync();
                _idsConfigurationDbContext.ClientScopes.RemoveRange(scopes);

                var idPRestrictions = await _idsConfigurationDbContext.ClientIdPRestrictions.Where(x => x.ClientId == client.Id).ToListAsync();
                _idsConfigurationDbContext.ClientIdPRestrictions.RemoveRange(idPRestrictions);

                var corsOrigins = await _idsConfigurationDbContext.ClientCorsOrigins.Where(x => x.ClientId == client.Id).ToListAsync();
                _idsConfigurationDbContext.ClientCorsOrigins.RemoveRange(corsOrigins);

                var properties = await _idsConfigurationDbContext.ClientProperties.Where(x => x.ClientId == client.Id).ToListAsync();
                _idsConfigurationDbContext.ClientProperties.RemoveRange(properties);

                _idsConfigurationDbContext.Clients.Update(client);
            }

            await _idsConfigurationDbContext.SaveChangesAsync();
        }


        private void PrepareClientTypeForNewClient(ClientInput client)
        {
            switch (client.ClientType)
            {
                case ClientType.Empty:
                    break;
                case ClientType.Web:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Code.Select(x => new ClientGrantType
                    {
                        GrantType = x,
                    }));
                    client.RequirePkce = true;
                    client.RequireClientSecret = true;
                    break;
                case ClientType.Spa:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Code);
                    client.RequirePkce = true;
                    client.RequireClientSecret = false;
                    break;
                case ClientType.Native:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Code);
                    client.RequirePkce = true;
                    client.RequireClientSecret = false;
                    break;
                case ClientType.Machine:
                    client.AllowedGrantTypes.AddRange(GrantTypes.ClientCredentials);
                    break;
                case ClientType.Device:
                    client.AllowedGrantTypes.AddRange(GrantTypes.DeviceFlow);
                    client.RequireClientSecret = false;
                    client.AllowOfflineAccess = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<Client> QueryClientById(int id)
        {
            return await _idsConfigurationDbContext.Clients.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}