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

public class ApiScopeService : IApiScopeService
{
    private readonly Id4ConfigurationDbContext _idsConfigurationDbContext;

    public ApiScopeService(Id4ConfigurationDbContext idsConfigurationDbContext)
    {
        _idsConfigurationDbContext = idsConfigurationDbContext;
    }

    public async Task<Pagination<ApiScope>> QueryApiScopePage(ApiScopePageIn pageIn)
    {
        return await _idsConfigurationDbContext.ApiScopes
            .Include(x => x.UserClaims)
            .Include(x => x.Properties)
            .OrderBy(x => x.Name)
            .ToPagination(pageIn);
    }

    public async Task<ApiScope> QueryApiScope(int id)
    {
        return await _idsConfigurationDbContext.ApiScopes
            .Include(x => x.UserClaims)
            .Include(x => x.Properties)
            .AsNoTracking()
            .FirstAsync(x => x.Id == id);
    }

    public async Task SaveApiScope(ApiScope apiScope)
    {
        if (apiScope.Id == 0)
        {
            await _idsConfigurationDbContext.ApiScopes.AddAsync(apiScope);
        }
        else
        {
            var claims = await _idsConfigurationDbContext.ApiScopeClaims.Where(x => x.ScopeId == apiScope.Id).ToListAsync();
            _idsConfigurationDbContext.ApiScopeClaims.RemoveRange(claims);

            var properties = await _idsConfigurationDbContext.ApiScopeProperties.Where(x => x.ScopeId == apiScope.Id).ToListAsync();
            _idsConfigurationDbContext.ApiScopeProperties.RemoveRange(properties);

            _idsConfigurationDbContext.ApiScopes.Update(apiScope);
        }

        await _idsConfigurationDbContext.SaveChangesAsync();
    }
}