using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.STS.Admin.Services.Interfaces.Identity;

public interface IApiScopeService
{
    Task<Pagination<ApiScope>> QueryApiScopePage(ApiScopePageIn pageIn);

    Task<ApiScope> QueryApiScope(int id);

    Task SaveApiScope(ApiScope apiScope);
}
