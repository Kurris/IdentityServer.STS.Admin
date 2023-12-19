using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.DbContexts;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer.STS.Admin.Services.Interfaces;
using IdentityServer.STS.Admin.Services.Interfaces.Identity;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.Services.Admin.Identity;

public class ApiResourceService : IApiResourceService
{
    private readonly Id4ConfigurationDbContext _idsConfigurationDbContext;

    public ApiResourceService(Id4ConfigurationDbContext idsConfigurationDbContext)
    {
        _idsConfigurationDbContext = idsConfigurationDbContext;
    }

    public async Task<Pagination<ApiResource>> QueryApiResourcePage(ApiResourcePageIn pageIn)
    {
        return await _idsConfigurationDbContext.ApiResources
            .Include(x => x.Secrets)
            .Include(x => x.Scopes)
            .Include(x => x.Properties)
            .Include(x => x.UserClaims)
            .ToPagination(pageIn);

    }

    public async Task<ApiResource> QueryApiResource(int id)
    {
        return await _idsConfigurationDbContext.ApiResources
            .Where(x => x.Id == id)
            .Include(x => x.Secrets)
            .Include(x => x.Scopes)
            .Include(x => x.Properties)
            .Include(x => x.UserClaims)
            .FirstOrDefaultAsync();
    }

    public async Task SaveApiResource(ApiResource apiResource)
    {
        if (apiResource.Id == 0)
        {
            //add
            await _idsConfigurationDbContext.ApiResources.AddAsync(apiResource);
        }
        else
        {
            //update
            var secrets = await _idsConfigurationDbContext.ApiSecrets.Where(x => x.ApiResourceId == apiResource.Id).ToListAsync();
            _idsConfigurationDbContext.ApiSecrets.RemoveRange(secrets);

            var scopes = await _idsConfigurationDbContext.ApiResourceScopes.Where(x => x.ApiResourceId == apiResource.Id).ToListAsync();
            _idsConfigurationDbContext.ApiResourceScopes.RemoveRange(scopes);


            var properties = await _idsConfigurationDbContext.ApiResourceProperties.Where(x => x.ApiResourceId == apiResource.Id).ToListAsync();
            _idsConfigurationDbContext.ApiResourceProperties.RemoveRange(properties);


            var claims = await _idsConfigurationDbContext.ApiResourceClaims.Where(x => x.ApiResourceId == apiResource.Id).ToListAsync();
            _idsConfigurationDbContext.ApiResourceClaims.RemoveRange(claims);


            _idsConfigurationDbContext.ApiResources.Update(apiResource);
        }

        await _idsConfigurationDbContext.SaveChangesAsync();
    }
}
