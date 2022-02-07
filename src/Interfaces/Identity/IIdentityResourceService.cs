using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.STS.Admin.Interfaces.Identity
{
    public interface IIdentityResourceService
    {
        Task<Pagination<IdentityResource>> QueryIdentityResourcePage(IdentityResourcePageIn pageIn);

        Task<IdentityResource> QueryIdentityResource(int id);

        Task SaveIdentityResource(IdentityResource identityResource);

    }
}