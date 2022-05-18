using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IdentityServer.STS.Admin.Configuration;
using IdentityServer.STS.Admin.Helpers;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Consent;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public ConsentController(IIdentityServerInteractionService interactionService
            , IEventService eventService
            , ILogger<ConsentController> logger
            , IConfiguration configuration)
        {
            _interaction = interactionService;
            _eventService = eventService;
            _logger = logger;
            _configuration = configuration;
        }


        public string FrontendBaseUrl => _configuration.GetSection("FrontendBaseUrl").Value;

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
        public async Task<IActionResult> GetSetting([FromForm] ConsentInputModel model)
        {
            var result = await ProcessConsent(model);

            if (result.IsRedirect)
            {
                var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                if (context?.IsNativeClient() == true)
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    // return this.LoadingPage("Redirect", result.RedirectUri);
                }

                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                throw new Exception(result.ValidationError);
            }

            if (result.ShowView)
            {
                //return Redirect();
                //return View("Index", result.ConsentModel);
            }

            return Redirect($"{FrontendBaseUrl}/signin?returnUrl={HttpUtility.UrlEncode(model.ReturnUrl)}");
        }


        private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
        {
            var result = new ProcessConsentResult();

            //验证重定向url是否正确
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (context == null) return result;

            ConsentResponse grantedConsent = null;

            //不允许授权，返回标准的"access_denied"响应
            if (model.Button == "no")
            {
                grantedConsent = new ConsentResponse {Error = AuthorizationError.AccessDenied};
                await _eventService.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), context.Client.ClientId, context.ValidatedResources.RawScopeValues));
            }
            //验证数据合法性
            else if (model.Button == "yes")
            {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = model.Description
                    };

                    await _eventService.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), context.Client.ClientId, context.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
                }
                else
                {
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null && grantedConsent.Error == null)
            {
                //将同意结果传达回身份服务器
                await _interaction.GrantConsentAsync(context, grantedConsent);

                //指示可以重定向回授权终结点 
                result.RedirectUri = model.ReturnUrl;
                result.Client = context.Client;
            }
            else
            {
                //重新展示同意屏幕
                result.ConsentModel = await BuildConsentModelAsync(model.ReturnUrl, model);
            }

            return result;
        }

        private async Task<ConsentOutputModel> BuildConsentModelAsync(string returnUrl, ConsentInputModel model = null)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                return CreateConsentViewModel(model, returnUrl, context);
            }

            _logger.LogError($"No consent request matching request: {returnUrl}");

            return null;
        }

        private ConsentOutputModel CreateConsentViewModel(ConsentInputModel model, string returnUrl,
            AuthorizationRequest context)
        {
            var vm = new ConsentOutputModel
            {
                RememberConsent = model?.RememberConsent ?? false,
                ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
                Description = model?.Description,

                ReturnUrl = returnUrl,

                ClientName = context.Client.ClientName ?? context.Client.ClientId,
                ClientUrl = context.Client.ClientUri,
                ClientLogoUrl = context.Client.LogoUri,
                AllowRememberConsent = context.Client.AllowRememberConsent
            };

            vm.IdentityScopes = context.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

            var apiScopes = new List<ScopeOutputModel>();
            foreach (var parsedScope in context.ValidatedResources.ParsedScopes)
            {
                var apiScope = context.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                    apiScopes.Add(scopeVm);
                }
            }

            if (ConsentOptions.EnableOfflineAccess && context.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
            }

            vm.ApiScopes = apiScopes;

            return vm;
        }

        private ScopeOutputModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeOutputModel
            {
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        private ScopeOutputModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            var displayName = apiScope.DisplayName ?? apiScope.Name;
            if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            {
                displayName += ":" + parsedScopeValue.ParsedParameter;
            }

            return new ScopeOutputModel
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }

        private ScopeOutputModel GetOfflineAccessScope(bool check)
        {
            return new ScopeOutputModel
            {
                Value = IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}