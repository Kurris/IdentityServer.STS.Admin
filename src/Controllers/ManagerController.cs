using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Entities;
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ManagerController> _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly IConfiguration _configuration;

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
            _configuration = configuration;
        }

        private string FrontendBaseUrl => _configuration.GetSection("FrontendBaseUrl").Value;


        [HttpGet("profile")]
        public async Task<ApiResult<PersonalProfileAndClaims>> GetPersonalProfileAndClaims()
        {
            var user = await _userManager.GetUserAsync(User);

            var claims = await _userManager.GetClaimsAsync(user);
            var profile = OpenIdClaimHelpers.ExtractProfileInfo(claims);

            var model = new PersonalProfileAndClaims
            {
                Username = user.UserName,
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


        [HttpPost("profile")]
        public async Task SavePersonalProfile(PersonalProfileAndClaims model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception();
            }

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


        [HttpDelete("profile")]
        public async Task DeletePersonalData(DeletePersonalDataInputModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
            }

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
                throw new Exception();
            }

            await _signInManager.SignOutAsync();
        }

        [HttpGet("profile/download")]
        public async Task<FileContentResult> DownloadPersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

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


        [HttpGet("setting/2fa/authenticator")]
        public async Task<ApiResult<EnableAuthenticatorModel>> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            var model = new EnableAuthenticatorModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);

            return new ApiResult<EnableAuthenticatorModel>
            {
                Data = model
            };
        }


        [HttpPost("setting/2fa/authenticator/verify")]
        public async Task<ApiResult<object>> EnableAuthenticator(EnableAuthenticatorModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new Exception("用户不存在");

            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            if (!await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode))
                throw new Exception("验证码无效");

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            _logger.LogInformation("User id: {0} is enable 2Fa", user.Id);

            //如果恢复码个数为0
            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

                //展示恢复码
                return new ApiResult<object>
                {
                    Route = DefineRoute.RecoveryCodes,
                    Data = recoveryCodes
                };
            }

            return new ApiResult<object>
            {
                Route = DefineRoute.TwoFactorAuthentication
            };
        }


        /// <summary>
        /// 加载二维码和share key
        /// </summary>
        /// <param name="user"></param>
        /// <param name="model"></param>
        private async Task LoadSharedKeyAndQrCodeUriAsync(User user, EnableAuthenticatorModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = string.Format(
                authenticatorUriFormat,
                _urlEncoder.Encode("Ligy.IdentityServer4.STS.Admin"),
                _urlEncoder.Encode(user.Email),
                unformattedKey);
        }

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
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
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
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

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


        [HttpDelete("setting/2fa/client")]
        public async Task ForgetTwoFactorClient()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            await _signInManager.ForgetTwoFactorClientAsync();
        }

        [HttpDelete("setting/2fa")]
        public async Task Disable2Fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
        }


        [HttpGet("setting/2fa/recoveryCodes")]
        public async Task<ApiResult<object>> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            if (!user.TwoFactorEnabled)
            {
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return new ApiResult<object> {Data = recoveryCodes};
        }


        [HttpDelete("setting/2fa/authenticator/new")]
        public async Task<ApiResult<object>> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            //_logger.LogInformation(_localizer["SuccessResetAuthenticationKey", user.Id]);

            return new ApiResult<object>();
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
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

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
        public async Task SavePassword(SavePasswordInputModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new Exception("用户不存在");

            IdentityResult result;
            if (string.IsNullOrEmpty(model.OldPassword))
                result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            else
                result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

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
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

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

        [ValidateAntiForgeryToken]
        [HttpPost("linkLogin")]
        public async Task<IActionResult> LinkLogin([FromForm] LinkLoginsInputModel model)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = "http://localhost:5000" + Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(model.Provider, redirectUrl, _userManager.GetUserId(User));

            return new ChallengeResult(model.Provider, properties);
        }

        [HttpGet("linkLoginCallback")]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id.ToString());
            if (info == null)
            {
                throw new Exception("错误的外部登录");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
            }

            //清除当前存在的外部登录cookie,确保登录无误
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var url = $"{FrontendBaseUrl}/externalLogins";
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
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            var result = await _userManager.RemoveLoginAsync(user, input.LoginProvider, input.ProviderKey);
            if (!result.Succeeded)
            {
                throw new Exception("移除外部登录失败");
            }

            await _signInManager.RefreshSignInAsync(user);
        }
    }
}