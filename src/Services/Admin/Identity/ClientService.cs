using System.Threading.Tasks;
using IdentityServer.STS.Admin.DbContexts;
using IdentityServer.STS.Admin.Interfaces;
using IdentityServer.STS.Admin.Interfaces.Identity;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.Services.Admin.Identity
{
    public class ClientService : IClientService
    {
        private readonly IdsConfigurationDbContext _idsConfigurationDbContext;

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
    }
}
