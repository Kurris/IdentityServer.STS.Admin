using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.STS.Admin.Interfaces.Identity
{
    public interface IClientService
    {
        Task<Pagination<Client>> QueryClientPage(ClientSearchPageIn pageIn);

        Task SaveClient(ClientInput client);

        Task<Client> QueryClientById(int id);

        Task AddSecret(ClientSecretInput clientSecret);

        Task DeleteSecretAsync(int id);

        Task<IEnumerable<string>> GetScopesAsync();
    }
}