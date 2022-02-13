using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.STS.Admin.Interfaces.Identity
{
    public interface IClientService
    {
        Task<Pagination<Client>> QueryClientPage(ClientSearchPageIn pageIn);
    }
}
