using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Microsoft.Extensions.Configuration;

namespace IdentityServer.STS.Admin.Controllers
{
    /// <summary>
    /// 设备授权
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceFlowInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IConfiguration _configuration;

        public DeviceController(IDeviceFlowInteractionService interaction,
            IEventService eventService
            , IConfiguration configuration)
        {
            _interaction = interaction;
            _events = eventService;
            _configuration = configuration;
        }

        public string FrontendBaseUrl => _configuration.GetSection("FrontendBaseUrl").Value;


        /// <summary>
        /// 确认设备授权码
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("confirmation")]
        public async Task<ApiResult<DeviceAuthorizationOutput>> GetUserCodeConfirmationInfo(string userCode)
        {
            if (string.IsNullOrEmpty(userCode))
            {
                throw new Exception("用户code不能为空");
            }

            var data = await BuildOutputModelAsync(userCode);
            if (data == null)
            {
                throw new Exception($"无效的用户验证码:{userCode}");
            }

            data.ConfirmUserCode = true;
            return new ApiResult<DeviceAuthorizationOutput>
            {
                Data = data
            };
        }

        /// <summary>
        /// 设备授权回调处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<IActionResult> Callback([FromForm] DeviceAuthorizationInput model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var result = await ProcessConsent(model);
            if (result.HasValidationError)
            {
                throw new Exception(result.ValidationError);
            }

            return Redirect($"{FrontendBaseUrl}/successed");
        }

        /// <summary>
        /// 处理同意屏幕
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationInput model)
        {
            var result = new ProcessConsentResult();

            var request = await _interaction.GetAuthorizationContextAsync(model.UserCode);
            if (request == null) return result;

            ConsentResponse grantedConsent = null;

            //返回标准的access_denied响应
            if (!model.Allow)
            {
                grantedConsent = new ConsentResponse {Error = AuthorizationError.AccessDenied};
                await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
            }
            //验证数据
            else if (model.Allow)
            {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = model.Description
                    };

                    await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
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

        private async Task<DeviceAuthorizationOutput> BuildOutputModelAsync(string userCode, DeviceAuthorizationInput model = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(userCode);
            return request != null ? CreateConsentOutputModel(userCode, model, request) : null;
        }

        private static DeviceAuthorizationOutput CreateConsentOutputModel(string userCode, DeviceAuthorizationInput model, DeviceFlowAuthorizationRequest request)
        {
            var output = new DeviceAuthorizationOutput
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

            output.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeModel(x, output.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

            var apiScopes = new List<ScopeOutput>();
            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateScopeOutputModel(parsedScope, apiScope, output.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                    apiScopes.Add(scopeVm);
                }
            }

            if (request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(GetOfflineAccessScope(output.ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
            }

            output.ApiScopes = apiScopes;

            return output;
        }

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

        private static ScopeOutput CreateScopeOutputModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            return new ScopeOutput
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

        private static ScopeOutput GetOfflineAccessScope(bool check)
        {
            return new ScopeOutput
            {
                Value = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = "离线访问",
                Description = "当离线时,能够访问应用和资源",
                Emphasize = true,
                Checked = check
            };
        }
    }
}