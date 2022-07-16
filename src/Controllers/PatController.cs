using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.STS.Admin.Helpers;
using IdentityServer.STS.Admin.Models.Pat;
using IdentityServer.STS.Admin.Services;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IdentityServer.STS.Admin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatController : ControllerBase
    {
        private readonly ReferenceTokenToolService _referenceTokenTools;
        private readonly IPersistedGrantStore _persistedGrantStore;

        public PatController(ReferenceTokenToolService referenceTokenTools, IPersistedGrantStore persistedGrantStore)
        {
            _referenceTokenTools = referenceTokenTools;
            _persistedGrantStore = persistedGrantStore;
        }

        /// <summary>
        /// 创建person access token
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> CreatePat(PatInput input)
        {
            double lifeTime = input.LifeTime ?? int.MaxValue;

            if (lifeTime > int.MaxValue)
            {
                // int.MaxValue;  68years
                lifeTime = int.MaxValue;
            }

            var issuer = "ligy.identity";
            var token = await _referenceTokenTools.IssueReferenceToken((int) lifeTime, issuer, input.Description, new List<Claim>
            {
                new(JwtClaimTypes.Subject, User.GetSubjectId()),
                new(JwtClaimTypes.Scope, "openid"),
                new(JwtClaimTypes.Scope, "ref"),
                new(JwtClaimTypes.Issuer, issuer)
            }, new List<string>
            {
                "weather_api"
            });

            return token;
        }


        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IEnumerable<PatOutput>> GetAllItems()
        {
            var sub = User.GetSubjectId();

            var grants = await _persistedGrantStore.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = sub,
                SessionId = null,
                ClientId = "reference",
                Type = "reference_token"
            });

            return grants.Select(x =>
            {
                var token = JsonConvert.DeserializeObject<Token>(x.Data, new ClaimConverter());
                var createTime = token.CreationTime.ToLocalTime();
                var expiredTime = createTime.AddSeconds(token.Lifetime);

                return new PatOutput
                {
                    Key = x.Key,
                    Description = x.Description,
                    CreateTime = createTime,
                    ExpiredTime = expiredTime
                };
            }).OrderByDescending(x => x.CreateTime);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        [HttpDelete]
        public async Task Delete(PatDeleteIInput input)
        {
            await _persistedGrantStore.RemoveAsync(input.Key);
        }
    }
}