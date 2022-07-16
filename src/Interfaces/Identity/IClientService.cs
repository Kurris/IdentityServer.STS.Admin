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

        Task SaveClient(ClientInput client, int userId);

        Task RemoveClientByIdAsync(int id, int userId);

        Task<Client> QueryClientById(int id);

        Task<IEnumerable<ClientSecret>> QueryClientSecrets(int clientId);

        Task<string> AddSecret(ClientSecretInput clientSecret);

        Task DeleteSecretAsync(int id);

        Task<IEnumerable<string>> GetScopesAsync();
    }
}