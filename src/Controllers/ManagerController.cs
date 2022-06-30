using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Filters;
using IdentityServer.STS.Admin.Helpers;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Manager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IdentityServer.STS.Admin.Controllers
{
    [UserExists]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ManagerController> _logger;
        private readonly UrlEncoder _urlEncoder;

        public ManagerController(UserManager<User> userManager
            , SignInManager<User> signInManager
            , ILogger<ManagerController> logger
            , UrlEncoder urlEncoder
            , IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _urlEncoder = urlEncoder;

            FrontendBaseUrl = configuration.GetSection("FrontendBaseUrl").Value;
            BackendBaseUrl = configuration.GetSection("BackendBaseUrl").Value;

            _logger.LogInformation(this.Request.Scheme + "://" + this.Request.Host);
        }

        private string FrontendBaseUrl { get; }
        private string BackendBaseUrl { get; }

        /// <summary>
        /// 获取个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile")]
        public async Task<ApiResult<PersonalProfileAndClaims>> GetPersonalProfileAndClaims()
        {
            var user = await _userManager.GetUserAsync(User);

            var claims = await _userManager.GetClaimsAsync(user);
            var profile = OpenIdClaimHelpers.ExtractProfileInfo(claims);

            var model = new PersonalProfileAndClaims
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                Name = profile.FullName,
                Website = profile.Website,
                Profile = profile.Profile,
                Country = profile.Country,
                Region = profile.Region,
                PostalCode = profile.PostalCode,
                Locality = profile.Locality,
                StreetAddress = profile.StreetAddress
            };
            return new ApiResult<PersonalProfileAndClaims>
            {
                Route = DefineRoute.None,
                Data = model,
            };
        }

        /// <summary>
        /// 保存个人信息
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="ApplicationException"></exception>
        [HttpPost("profile")]
        public async Task SavePersonalProfile(PersonalProfileAndClaims model)
        {
            var user = await _userManager.GetUserAsync(User);

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException("错误的邮件设置");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException("错误的电话号码设置");
                }
            }

            await UpdateUserClaimsAsync(model, user);
        }

        /// <summary>
        /// 删除个人信息
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="Exception"></exception>
        [HttpDelete("profile")]
        public async Task DeletePersonalData(DeletePersonalDataInputModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var requirePassword = await _userManager.HasPasswordAsync(user);
            if (requirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    throw new Exception("密码错误");
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("删除用户信息");
            }

            await _signInManager.SignOutAsync();
        }

        /// <summary>
        /// 下载个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile/download")]
        public async Task<FileContentResult> DownloadPersonalData()
        {
            var user = await _userManager.GetUserAsync(User);

            var personalDataProps = typeof(User).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            var personalData = personalDataProps.ToDictionary(p => p.Name, p => p.GetValue(user)?.ToString() ?? "null");

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "application/octet-stream")
            {
                FileDownloadName = "PersonalData.json",
            };
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        private async Task UpdateUserClaimsAsync(PersonalProfileAndClaims model, User user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var oldProfile = OpenIdClaimHelpers.ExtractProfileInfo(claims);
            var newProfile = new OpenIdProfile
            {
                Website = model.Website,
                StreetAddress = model.StreetAddress,
                Locality = model.Locality,
                PostalCode = model.PostalCode,
                Region = model.Region,
                Country = model.Country,
                FullName = model.Name,
                Profile = model.Profile
            };

            var claimsToRemove = OpenIdClaimHelpers.ExtractClaimsToRemove(oldProfile, newProfile);
            var claimsToAdd = OpenIdClaimHelpers.ExtractClaimsToAdd(oldProfile, newProfile);
            var claimsToReplace = OpenIdClaimHelpers.ExtractClaimsToReplace(claims, newProfile);

            await _userManager.RemoveClaimsAsync(user, claimsToRemove);
            await _userManager.AddClaimsAsync(user, claimsToAdd);

            foreach (var pair in claimsToReplace)
            {
                await _userManager.ReplaceClaimAsync(user, pair.Item1, pair.Item2);
            }
        }


        /// <summary>
        /// 获取身份验证器信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("setting/2fa/authenticator")]
        public async Task<ApiResult<AuthenticatorOutput>> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);

            var output = await LoadSharedKeyAndQrCodeUriAsync(user);

            return new ApiResult<AuthenticatorOutput>
            {
                Data = output
            };
        }


        /// <summary>
        /// 验证2fa code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost("setting/2fa/authenticator/verify")]
        public async Task<IEnumerable<string>> VerifyAuthenticatorCode(AuthenticatorOutput model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new Exception("用户不存在");

            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            if (!await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode))
                throw new Exception("验证码无效");

            _logger.LogInformation("User id: {0} is enable 2Fa", user.Id);

            return await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 12);
        }


        /// <summary>
        /// 加载二维码和share key
        /// </summary>
        /// <param name="user"></param>
        private async Task<AuthenticatorOutput> LoadSharedKeyAndQrCodeUriAsync(User user)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                //重置验证器key
                await _userManager.ResetAuthenticatorKeyAsync(user);
                //获取key
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

            var result = new AuthenticatorOutput
            {
                Code = null,
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = string.Format(
                    authenticatorUriFormat,
                    _urlEncoder.Encode("Ligy.IdentityServer4.STS.Admin"),
                    _urlEncoder.Encode(user.Email),
                    unformattedKey)
            };

            return result;
        }


        /// <summary>
        /// 获取双重验证信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("setting/2fa")]
        public async Task<ApiResult<TwoFactorAuthenticationOutput>> GetTwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new TwoFactorAuthenticationOutput
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2FaEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user)
            };

            return new ApiResult<TwoFactorAuthenticationOutput>
            {
                Data = model
            };
        }

        /// <summary>
        /// 2fa忘记当前设置
        /// </summary>
        [HttpDelete("setting/2fa/client")]
        public async Task ForgetTwoFactorClient()
        {
            await _signInManager.ForgetTwoFactorClientAsync();
        }

        /// <summary>
        /// 设置用户双重验证是否开启
        /// </summary>
        /// <param name="enable"></param>
        [HttpPut("setting/2fa/{enable:bool}")]
        public async Task Enable2Fa(bool enable)
        {
            var user = await _userManager.GetUserAsync(User);
            await _userManager.SetTwoFactorEnabledAsync(user, enable);
        }

        /// <summary>
        /// 获取新的恢复码
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("setting/2fa/recoveryCodes")]
        public async Task<ApiResult<IEnumerable<string>>> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);

            if (!user.TwoFactorEnabled)
            {
                throw new Exception("尚未开启双重验证,无法生成恢复码");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 12);
            return new ApiResult<IEnumerable<string>> {Data = recoveryCodes};
        }


        /// <summary>
        /// 重置双重验证器
        /// </summary>
        [HttpPut("setting/2fa/authenticator/reset")]
        public async Task ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);

            //停用
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            //重置
            await _userManager.ResetAuthenticatorKeyAsync(user);
        }


        /// <summary>
        /// 检查是否存在密码
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("password/status")]
        public async Task<ApiResult<bool>> CheckPassword()
        {
            var user = await _userManager.GetUserAsync(User);

            var hasPassword = await _userManager.HasPasswordAsync(user);

            return new ApiResult<bool> {Data = hasPassword};
        }

        /// <summary>
        /// 保存密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost("password")]
        public async Task SavePassword(SavePasswordInput model)
        {
            var user = await _userManager.GetUserAsync(User);

            var result = string.IsNullOrEmpty(model.OldPassword)
                ? await _userManager.AddPasswordAsync(user, model.NewPassword)
                : await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));

            await _signInManager.RefreshSignInAsync(user);
        }

        /// <summary>
        /// 获取关联的外部登录
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("externalLogins")]
        public async Task<ApiResult<object>> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new ExternalLoginsOutput
            {
                CurrentLogins = await _userManager.GetLoginsAsync(user)
            };

            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            model.AbleRemove = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;

            return new ApiResult<object> {Data = model};
        }

        /// <summary>
        /// 关联外部登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("linkLogin")]
        public async Task<IActionResult> LinkLogin([FromForm] LinkLoginsInput model)
        {
            //清除当前存在的外部登录cookie,确保登录无误
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var redirectUrl = BackendBaseUrl + Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(model.Provider, redirectUrl, _userManager.GetUserId(User));

            return Challenge(properties, model.Provider);
        }


        /// <summary>
        /// 关联登录callback
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("linkExternalLoginCallback")]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id.ToString());
            if (info == null)
            {
                return await RedirectHelper.Error(new Dictionary<string, string>()
                {
                    ["error"] = "外部关联已失效,请重新操作"
                });
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                return await RedirectHelper.Error(new Dictionary<string, string>
                {
                    ["error"] = string.Join(",", result.Errors.Select(x => x.Description))
                });
            }

            //清除当前存在的外部登录cookie,确保登录无误
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var url = $"{FrontendBaseUrl}/setting/externalLogins";
            return Redirect(url);
        }

        /// <summary>
        /// 移除外部登录
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="Exception"></exception>
        [HttpDelete("externalLogin")]
        public async Task RemoveLogin(RemoveExternalLoginInput input)
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _userManager.RemoveLoginAsync(user, input.LoginProvider, input.ProviderKey);
            if (!result.Succeeded)
            {
                var error = string.Join(",", result.Errors.Select(x => x.Description));
                throw new Exception("移除外部登录失败:" + error);
            }

            await _signInManager.RefreshSignInAsync(user);
        }

        /*================================================帮助方法================================================*/


        /// <summary>
        /// 格式化key
        /// </summary>
        /// <param name="unformattedKey"></param>
        /// <returns></returns>
        private static string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;

            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey[currentPosition..]);
            }

            return result.ToString().ToLowerInvariant();
        }
    }
}