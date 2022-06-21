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


        /// <summary>
        /// 查询用户所有授权
        /// </summary>
        /// <returns></returns>
        [HttpGet("grants")]
        public async Task<ApiResult<IEnumerable<GrantOutput>>> GetGrants()
        {
            var grants = await _interaction.GetAllUserGrantsAsync();
            var list = new List<GrantOutput>();
            foreach (var grant in grants)
            {
                var client = await _client.FindClientByIdAsync(grant.ClientId);
                if (client != null)
                {
                    var resources = await _resource.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantOutput
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Description = grant.Description,
                        Created = grant.CreationTime.ToLocalTime(),
                        Expires = grant.Expiration?.ToLocalTime() ?? default,
                        IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name),
                        ApiGrantNames = resources.ApiScopes.Select(x => resources.ApiResources.FirstOrDefault(resource => resource.Scopes.Contains(x.Name)).DisplayName + ":" + x.DisplayName)
                    };

                    list.Add(item);
                }
            }

            return new ApiResult<IEnumerable<GrantOutput>>
            {
                Data = list
            };
        }


        /// <summary>
        /// 移除授权
        /// </summary>
        /// <param name="clientId"></param>
        [HttpDelete("client/{clientId}")]
        public async Task Revoke(string clientId)
        {
            await _interaction.RevokeUserConsentAsync(clientId);
            await _events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), clientId));
        }
    }
}