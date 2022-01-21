using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Manager;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.STS.Admin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GrantsController : ControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _client;
        private readonly IResourceStore _resource;
        private readonly IEventService _events;

        public GrantsController(IIdentityServerInteractionService interaction
            , IClientStore client
            , IResourceStore resource
            , IEventService events)
        {
            _interaction = interaction;
            _client = client;
            _resource = resource;
            _events = events;
        }


        [HttpGet("grants")]
        public async Task<ApiResult<IEnumerable<GrantOutputModel>>> GetGrants()
        {
            var grants = await _interaction.GetAllUserGrantsAsync();

            var list = new List<GrantOutputModel>();
            foreach (var grant in grants)
            {
                var client = await _client.FindClientByIdAsync(grant.ClientId);
                if (client != null)
                {
                    var resources = await _resource.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantOutputModel()
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Description = grant.Description,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                        ApiGrantNames = resources.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray()
                    };

                    list.Add(item);
                }
            }

            return new ApiResult<IEnumerable<GrantOutputModel>>(){Data = list};
        }


        [HttpDelete("client/{clientId}")]
        public async Task Revoke(string clientId)
        {
            await _interaction.RevokeUserConsentAsync(clientId);
            await _events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), clientId));
        }
    }
}