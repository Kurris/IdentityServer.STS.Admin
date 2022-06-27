using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IdentityServer.STS.Admin.Configuration;
using IdentityServer.STS.Admin.DbContexts;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Consent;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityServer.STS.Admin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConsentController : ControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _eventService;
        private readonly ILogger<ConsentController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IdsConfigurationDbContext _idsConfigurationDbContext;
        private readonly IdentityDbContext _identityDbContext;

        public ConsentController(IIdentityServerInteractionService interactionService
            , IEventService eventService
            , ILogger<ConsentController> logger
            , IConfiguration configuration
            , IdsConfigurationDbContext idsConfigurationDbContext
            , IdentityDbContext identityDbContext)
        {
            _interaction = interactionService;
            _eventService = eventService;
            _logger = logger;
            _configuration = configuration;
            _idsConfigurationDbContext = idsConfigurationDbContext;
            _identityDbContext = identityDbContext;
        }


        private string FrontendBaseUrl => _configuration.GetSection("FrontendBaseUrl").Value;

        /// <summary>
        /// 获取同意屏幕的配置数据
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet("setting")]
        public async Task<ApiResult<object>> GetSetting(string returnUrl)
        {
            var output = await BuildConsentModelAsync(returnUrl);
            if (output != null)
            {
                return new ApiResult<object>
                {
                    Code = 200,
                    Data = output
                };
            }

            return new ApiResult<object> {Route = DefineRoute.Error};
        }

        /// <summary>
        /// 处理同意屏幕的处理
        /// </summary>
        [HttpPost("setting/process")]
        public async Task<IActionResult> GetSetting([FromForm] ConsentInput input)
        {
            var result = await ProcessConsent(input);

            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                throw new Exception(result.ValidationError);
            }

            return Redirect($"{FrontendBaseUrl}/signin?returnUrl={HttpUtility.UrlEncode(input.ReturnUrl)}");
        }


        /// <summary>
        /// 处理同意屏幕
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<ProcessConsentResult> ProcessConsent(ConsentInput input)
        {
            var result = new ProcessConsentResult();

            //验证重定向url是否正确
            var context = await _interaction.GetAuthorizationContextAsync(input.ReturnUrl);
            if (context == null) return result;

            ConsentResponse grantedConsent = null;

            //不允许授权，返回标准的"access_denied"响应
            if (!input.Allow)
            {
                grantedConsent = new ConsentResponse {Error = AuthorizationError.AccessDenied};
                await _eventService.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), context.Client.ClientId, context.ValidatedResources.RawScopeValues));
            }
            //验证数据合法性
            else if (input.Allow)
            {
                //存在授权域
                if (input.ScopesConsented != null && input.ScopesConsented.Any())
                {
                    var scopes = input.ScopesConsented;

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = input.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = input.Description
                    };

                    await _eventService.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), context.Client.ClientId, context.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
                }
                else
                {
                    result.ValidationError = "至少选择一个选项";
                }
            }
            else
            {
                result.ValidationError = "错误的选择";
            }

            if (grantedConsent != null)
            {
                //将同意结果传达回身份服务器
                await _interaction.GrantConsentAsync(context, grantedConsent);

                //指示可以重定向回授权终结点 
                result.RedirectUri = input.ReturnUrl;
                result.Client = context.Client;
            }
            else
            {
                //重新展示同意屏幕
                result.ConsentModel = await BuildConsentModelAsync(input.ReturnUrl, input);
            }

            return result;
        }

        private async Task<ConsentOutput> BuildConsentModelAsync(string returnUrl, ConsentInput model = null)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                return await CreateConsentModel(model, returnUrl, context);
            }

            _logger.LogError($"No consent request matching request: {returnUrl}");

            return null;
        }

        private async Task<ConsentOutput> CreateConsentModel(ConsentInput input, string returnUrl,
            AuthorizationRequest context)
        {
            var clientId = await _idsConfigurationDbContext.Clients
                .Where(x => x.ClientId == context.Client.ClientId)
                .Select(x => x.Id)
                .FirstAsync();

            var userId = await _idsConfigurationDbContext.ClientOwners
                .Where(x => x.ClientId == clientId)
                .Select(x => x.UserId)
                .FirstAsync();

            var user = await _identityDbContext.Users.Where(x => x.Id == userId).FirstAsync();

            var output = new ConsentOutput
            {
                ClientOwner = user,
                RememberConsent = input?.RememberConsent ?? false,
                ScopesConsented = input?.ScopesConsented ?? Enumerable.Empty<string>(),
                Description = input?.Description,

                ReturnUrl = returnUrl,

                ClientName = context.Client.ClientName ?? context.Client.ClientId,
                ClientUrl = context.Client.ClientUri,
                ClientLogoUrl = context.Client.LogoUri,
                AllowRememberConsent = context.Client.AllowRememberConsent
            };

            output.IdentityScopes = context.ValidatedResources.Resources
                .IdentityResources
                .Select(x => CreateScopeModel(x, output.ScopesConsented.Contains(x.Name) || input == null));

            var apiScopes = new List<ScopeOutput>();
            foreach (var parsedScope in context.ValidatedResources.ParsedScopes)
            {
                var apiScope = context.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateScopeModel(parsedScope, apiScope, output.ScopesConsented.Contains(parsedScope.RawValue) || input == null);
                    apiScopes.Add(scopeVm);
                }
            }

            if (context.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(GetOfflineAccessScope(output.ScopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) || input == null));
            }

            output.ApiScopes = apiScopes;

            return output;
        }

        /// <summary>
        /// 创建作用域模型
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        private static ScopeOutput CreateScopeModel(IdentityResource identity, bool check)
        {
            return new ScopeOutput
            {
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        /// <summary>
        /// 创建作用域模型
        /// </summary>
        /// <param name="parsedScopeValue"></param>
        /// <param name="apiScope"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        private static ScopeOutput CreateScopeModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            var displayName = apiScope.DisplayName ?? apiScope.Name;
            if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            {
                displayName += ":" + parsedScopeValue.ParsedParameter;
            }

            return new ScopeOutput
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }

        /// <summary>
        /// 获取离线访问作用域()
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        private static ScopeOutput GetOfflineAccessScope(bool check)
        {
            return new ScopeOutput
            {
                Value = IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = "离线访问",
                Description = "当离线时,能够访问应用和资源",
                Emphasize = true,
                Checked = check
            };
        }
    }
}