using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Helpers;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Manager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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
        private readonly string _recoveryCodesKey = nameof(_recoveryCodesKey).Replace("_", "");
        private readonly string _authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ManagerController> _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly IConfiguration _configuration;

        public ManagerController(UserManager<User> userManager
            , SignInManager<User> signInManager
            , ILogger<ManagerController> logger
            , UrlEncoder urlEncoder
            ,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _configuration = configuration;
        }

        public string FrontendBaseUrl => _configuration.GetSection("FrontendBaseUrl").Value;


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
                // return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
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
            {
                // return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                //return View(model);
                return new ApiResult<object> {Data = model};
            }

            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("代码", "验证码无效");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return new ApiResult<object> {Data = model};
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var userId = await _userManager.GetUserIdAsync(user);

            _logger.LogInformation($"ID 为 {userId} 的用户已使用身份验证器应用启用了 2FA。");

            //   StatusMessage = _localizer["AuthenticatorVerified"];

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

                //return RedirectToAction(nameof(ShowRecoveryCodes));
                return new ApiResult<object>
                {
                    Route = DefineRoute.RecoveryCodes,
                    Data = recoveryCodes
                };
            }

            //return RedirectToAction(nameof(TwoFactorAuthentication));

            return new ApiResult<object>
            {
                Route = DefineRoute.TwoFactorAuthentication
            };
        }


        private async Task LoadSharedKeyAndQrCodeUriAsync(User user, EnableAuthenticatorModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = string.Format(
                _authenticatorUriFormat,
                _urlEncoder.Encode("Ligy.IdentityServer4.STS.Admin"),
                _urlEncoder.Encode(user.Email),
                unformattedKey);
        }

        private string FormatKey(string unformattedKey)
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


        [HttpGet("setting/2fa")]
        public async Task<ApiResult<TwoFactorAuthenticationOuputModel>> GetTwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }

            var model = new TwoFactorAuthenticationOuputModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user)
            };

            return new ApiResult<TwoFactorAuthenticationOuputModel>
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
                // return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }

            await _signInManager.ForgetTwoFactorClientAsync();
        }

        [HttpDelete("setting/2fa")]
        public async Task Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
        }


        [HttpGet("setting/2fa/recoveryCodes")]
        public async Task<ApiResult<object>> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }

            if (!user.TwoFactorEnabled)
            {
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return new ApiResult<object>() {Data = recoveryCodes};
        }


        [HttpDelete("setting/2fa/authenticator/new")]
        public async Task<ApiResult<object>> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                //return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
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
                throw new Exception();
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            return new ApiResult<bool>() {Data = hasPassword};
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
                throw new Exception();

            IdentityResult result;
            if (string.IsNullOrEmpty(model.OldPassword))
                result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            else
                result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));

            await _signInManager.RefreshSignInAsync(user);
        }


        [HttpGet("externalLogins")]
        public async Task<ApiResult<object>> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }

            var model = new ExternalLoginsOutputModel
            {
                CurrentLogins = await _userManager.GetLoginsAsync(user)
            };

            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;

            return new ApiResult<object>() {Data = model};
        }

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
                //  return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id.ToString());
            if (info == null)
            {
                //  throw new ApplicationException(_localizer["ErrorLoadingExternalLogin", user.Id]);
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                // return new Exception(string.Join(",", result.Errors.Select(x=>x.Description)));
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var url = $"{FrontendBaseUrl}/externalLogins";
            return Redirect(url);
        }

        [HttpDelete("externalLogin")]
        public async Task RemoveLogin(RemoveLoginInputModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                //  return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                // throw new ApplicationException(_localizer["ErrorRemovingExternalLogin", user.Id]);
            }

            await _signInManager.RefreshSignInAsync(user);
        }
    }
}