using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Interfaces.Identity;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Admin.Identity;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.STS.Admin.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;
        private readonly IIdentityResourceService _identityResourceService;

        public ConfigurationController(IConfigurationService configurationService
            , IIdentityResourceService identityResourceService)
        {
            _configurationService = configurationService;
            _identityResourceService = identityResourceService;
        }

        [HttpGet("claims")]
        public IEnumerable<string> GetStandardClaims()
        {
            return _configurationService.GetStandardClaims();
        }


        [HttpGet("identityResource/page")]
        public async Task<Pagination<IdentityResource>> QueryIdentityResourcePage([FromQuery] IdentityResourcePageIn pageIn)
        {
            return await _identityResourceService.QueryIdentityResourcePage(pageIn);
        }

        [HttpGet("identityResource")]
        public async Task<IdentityResource> QueryIdentityResource(int id)
        {
            return await _identityResourceService.QueryIdentityResource(id);
        }

        [HttpPost("identityResource")]
        public async Task SaveIdentityResource(IdentityResource identityResource)
        {
            await _identityResourceService.SaveIdentityResource(identityResource);
        }
    }
}