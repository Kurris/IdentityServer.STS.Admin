using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.DbContexts;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Account;
using IdentityServer.STS.Admin.Models.Manager;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.STS.Admin.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GrantsController : ControllerBase
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _client;
    private readonly IResourceStore _resource;
    private readonly IEventService _events;
    private readonly Id4ConfigurationDbContext _idsConfigurationDbContext;
    private readonly IdentityDbContext _identityDbContext;

    public GrantsController(IIdentityServerInteractionService interaction
        , IClientStore client
        , IResourceStore resource
        , IEventService events
        , Id4ConfigurationDbContext idsConfigurationDbContext
        , IdentityDbContext identityDbContext)
    {
        _interaction = interaction;
        _client = client;
        _resource = resource;
        _events = events;
        _idsConfigurationDbContext = idsConfigurationDbContext;
        _identityDbContext = identityDbContext;
    }


    /// <summary>
    /// 查询用户所有授权
    /// </summary>
    /// <returns></returns>
    [HttpGet("grants")]
    public async Task<ApiResult<IEnumerable<GrantOutput>>> GetGrants()
    {
        var grants = await _interaction.GetAllUserGrantsAsync();
        //reference client
        grants = grants.Where(x => x.ClientId != "reference");

        var list = new List<GrantOutput>();
        foreach (var grant in grants)
        {
            var client = await _client.FindClientByIdAsync(grant.ClientId);
            if (client != null)
            {
                var resources = await _resource.FindResourcesByScopeAsync(grant.Scopes);

                var clientId = await _idsConfigurationDbContext.Clients
                    .Where(x => x.ClientId == client.ClientId)
                    .Select(x => x.Id)
                    .FirstAsync();

                var userId = await _idsConfigurationDbContext.ClientOwners
                    .Where(x => x.ClientId == clientId)
                    .Select(x => x.UserId)
                    .FirstAsync();

                var user = await _identityDbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

                var item = new GrantOutput
                {
                    ClientOwner = new UserOutput
                    {
                        UserName = user?.UserName
                    },
                    ClientId = client.ClientId,
                    ClientName = client.ClientName ?? client.ClientId,
                    ClientLogoUrl = client.LogoUri,
                    ClientUrl = client.ClientUri,
                    Description = client.Description,
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