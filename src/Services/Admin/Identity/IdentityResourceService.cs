using System.Collections.Generic;
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
            return await _idsConfigurationDbContext.IdentityResources.AsNoTracking()
                .Include(x => x.Properties).AsNoTracking()
                .Include(x => x.UserClaims).AsNoTracking()
                .FirstAsync(x => x.Id == id);
        }

        public async Task SaveIdentityResource(IdentityResource identityResource)
        {
            IdentityResource dbEntity = null;
            if (identityResource.Id != 0)
            {
                dbEntity = await QueryIdentityResource(identityResource.Id);
            }

            // _idsConfigurationDbContext.Entry(identityResource.UserClaims).State = EntityState.Unchanged;
            // _idsConfigurationDbContext.Entry(identityResource.Properties).State = EntityState.Unchanged;
            _idsConfigurationDbContext.IdentityResources.Update(identityResource);

            await SaveIdentityResourceClaims(identityResource.UserClaims, dbEntity?.UserClaims);
            await SaveIdentityResourceProperties(identityResource.Properties, dbEntity?.Properties);

            await _idsConfigurationDbContext.SaveChangesAsync();
        }


        public async Task SaveIdentityResourceClaims(List<IdentityResourceClaim> identityResourceClaims, List<IdentityResourceClaim> dbEntities)
        {
            //不存在数据库中
            foreach (var dbEntity in dbEntities)
            {
                var item = identityResourceClaims.FirstOrDefault(x => x.Id == dbEntity.Id);
                if (item == null)
                {
                    _idsConfigurationDbContext.IdentityClaims.Remove(dbEntity);
                }
            }

            foreach (var identityResourceClaim in identityResourceClaims)
            {
                if (identityResourceClaim.Id == 0)
                    await _idsConfigurationDbContext.IdentityClaims.AddAsync(identityResourceClaim);
                else
                    _idsConfigurationDbContext.IdentityClaims.Update(identityResourceClaim);
            }
        }

        public async Task SaveIdentityResourceProperties(List<IdentityResourceProperty> identityResourceProperties, List<IdentityResourceProperty> dbEntities)
        {
        }
    }
}