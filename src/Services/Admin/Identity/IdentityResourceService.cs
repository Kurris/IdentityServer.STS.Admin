using System.Linq;
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
    public class IdentityResourceService : IIdentityResourceService
    {
        private readonly IdsConfigurationDbContext _idsConfigurationDbContext;

        public IdentityResourceService(IdsConfigurationDbContext idsConfigurationDbContext)
        {
            _idsConfigurationDbContext = idsConfigurationDbContext;
        }

        public async Task<Pagination<IdentityResource>> QueryIdentityResourcePage(IdentityResourcePageIn pageIn)
        {
            return await _idsConfigurationDbContext.IdentityResources
                .Include(x => x.Properties)
                .Include(x => x.UserClaims)
                .ToPaginationBy(x => x.Created, pageIn);
        }

        public async Task<IdentityResource> QueryIdentityResource(int id)
        {
            return await _idsConfigurationDbContext.IdentityResources
                .Include(x => x.Properties)
                .Include(x => x.UserClaims)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id);
        }

        public async Task SaveIdentityResource(IdentityResource identityResource)
        {
            if (identityResource.Id == 0)
            {
                //add
                await _idsConfigurationDbContext.IdentityResources.AddAsync(identityResource);
            }
            else
            {
                //update
                var claims = await _idsConfigurationDbContext.IdentityClaims.Where(x => x.IdentityResource.Id == identityResource.Id).ToListAsync();
                _idsConfigurationDbContext.IdentityClaims.RemoveRange(claims);

                var properties = await _idsConfigurationDbContext.IdentityResourceProperties.Where(x => x.IdentityResourceId == identityResource.Id).ToListAsync();
                _idsConfigurationDbContext.IdentityResourceProperties.RemoveRange(properties);

                _idsConfigurationDbContext.IdentityResources.Update(identityResource);
            }

            await _idsConfigurationDbContext.SaveChangesAsync();
        }
    }
}