using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Helpers;
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
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<IActionResult> Callback([FromForm] DeviceAuthorizationInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var result = await ProcessConsent(input);
            if (result.HasValidationError)
            {
                return await RedirectHelper.Error(new Dictionary<string, string>()
                {
                    ["error"] = result.ValidationError
                });
            }

            if (input.Allow)
            {
                return await RedirectHelper.Success(new Dictionary<string, string>()
                {
                    ["title"] = "您已经成功授权"
                });
            }
            else
            {
                return await RedirectHelper.Error(new Dictionary<string, string>()
                {
                    ["error"] = "您对" + result.Client.ClientName + "的授权已拒绝"
                });
            }
        }

        /// <summary>
        /// 处理同意屏幕
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationInput input)
        {
            var result = new ProcessConsentResult();

            var context = await _interaction.GetAuthorizationContextAsync(input.UserCode);
            if (context == null) return result;

            ConsentResponse grantedConsent = null;

            //返回标准的access_denied响应
            if (!input.Allow)
            {
                grantedConsent = new ConsentResponse {Error = AuthorizationError.AccessDenied};
                await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), context.Client.ClientId, context.ValidatedResources.RawScopeValues));
            }
            //验证数据
            else if (input.Allow)
            {
                // if the user consented to some scope, build the response model
                if (input.ScopesConsented != null && input.ScopesConsented.Any())
                {
                    var scopes = input.ScopesConsented;

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = input.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = input.Description
                    };

                    await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), context.Client.ClientId, context.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
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
                await _interaction.HandleRequestAsync(input.UserCode, grantedConsent);

                //指示可以重定向回授权终结点
                result.RedirectUri = input.ReturnUrl;
                result.Client = context.Client;
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

            output.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScope(x, output.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

            var apiScopes = new List<ScopeOutput>();
            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateScopeOutput(parsedScope, apiScope, output.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                    apiScopes.Add(scopeVm);
                }
            }

            //添加refresh token
            if (request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(GetOfflineAccessScope(output.ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
            }

            output.ApiScopes = apiScopes;

            return output;
        }

        private static ScopeOutput CreateScope(IdentityResource identity, bool check)
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

        private static ScopeOutput CreateScopeOutput(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
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