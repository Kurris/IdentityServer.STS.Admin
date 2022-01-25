using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Configuration;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Consent;
using IdentityServer.STS.Admin.Models.Device;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.STS.Admin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceFlowInteractionService _interaction;
        private readonly IEventService _events;

        public DeviceController(IDeviceFlowInteractionService interaction,
            IEventService eventService)
        {
            _interaction = interaction;
            _events = eventService;
        }


        [HttpGet("confirmation")]
        public async Task<ApiResult<DeviceAuthorizationOutputModel>> GetUserCodeConfirmationModel(string userCode)
        {
            var data = await BuildOutputModelAsync(userCode);
            data.ConfirmUserCode = true;
            return new ApiResult<DeviceAuthorizationOutputModel>
            {
                Data = data
            };
        }

        [HttpPost]
        public async Task Callback(DeviceAuthorizationInputModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var result = await ProcessConsent(model);
            if (result.HasValidationError)
            {
                throw new Exception(result.ValidationError);
            }

            //您已成功授权该设备
        }

        private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationInputModel model)
        {
            var result = new ProcessConsentResult();

            var request = await _interaction.GetAuthorizationContextAsync(model.UserCode);
            if (request == null) return result;

            ConsentResponse grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (model.Button == "no")
            {
                grantedConsent = new ConsentResponse {Error = AuthorizationError.AccessDenied};

                // emit event
                await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
            }
            // user clicked 'yes' - validate the data
            else if (model.Button == "yes")
            {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x => x != global::IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = model.Description
                    };

                    // emit event
                    await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
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

            if (grantedConsent != null)
            {
                // 将同意结果传达回身份服务器
                await _interaction.HandleRequestAsync(model.UserCode, grantedConsent);

                //指示可以重定向回授权终结点
                result.RedirectUri = model.ReturnUrl;
                result.Client = request.Client;
            }
            else
            {
                // we need to redisplay the consent UI
                result.ConsentModel = await BuildOutputModelAsync(model.UserCode, model);
            }

            return result;
        }

        private async Task<DeviceAuthorizationOutputModel> BuildOutputModelAsync(string userCode, DeviceAuthorizationInputModel model = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(userCode);
            if (request != null)
            {
                return CreateConsentOutputModel(userCode, model, request);
            }

            return null;
        }

        private DeviceAuthorizationOutputModel CreateConsentOutputModel(string userCode, DeviceAuthorizationInputModel model, DeviceFlowAuthorizationRequest request)
        {
            var vm = new DeviceAuthorizationOutputModel
            {
                UserCode = userCode,
                Description = model?.Description,

                RememberConsent = model?.RememberConsent ?? true,
                ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),

                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientUrl = request.Client.ClientUri,
                ClientLogoUrl = request.Client.LogoUri,
                AllowRememberConsent = request.Client.AllowRememberConsent
            };

            vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

            var apiScopes = new List<ScopeOutputModel>();
            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateScopeOutputModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                    apiScopes.Add(scopeVm);
                }
            }

            if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
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

        private ScopeOutputModel CreateScopeOutputModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            return new ScopeOutputModel
            {
                Value = parsedScopeValue.RawValue,
                // todo: use the parsed scope value in the display?
                DisplayName = apiScope.DisplayName ?? apiScope.Name,
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
                Value = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}