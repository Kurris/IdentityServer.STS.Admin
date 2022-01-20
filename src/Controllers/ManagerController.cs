using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Helpers;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

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

        public ManagerController(UserManager<User> userManager
            , SignInManager<User> signInManager
            , ILogger<ManagerController> logger
            , UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }


        [HttpGet]
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

        // [HttpPost]
        // public async Task<IActionResult> GenerateRecoveryCodes()
        // {
        //     var user = await _userManager.GetUserAsync(User);
        //     if (user == null)
        //     {
        //        // return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
        //     }
        //
        //     if (!user.TwoFactorEnabled)
        //     {
        //         AddError(_localizer["ErrorGenerateCodesWithout2FA"]);
        //         return View();
        //     }
        //
        //     var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        //
        //     _logger.LogInformation(_localizer["UserGenerated2FACodes", user.Id]);
        //
        //     var model = new ShowRecoveryCodesViewModel {RecoveryCodes = recoveryCodes.ToArray()};
        //
        //     return View(nameof(ShowRecoveryCodes), model);
        // }


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
                //return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);

            //  _logger.LogInformation(_localizer["SuccessDisabled2FA", user.Id]);
        }
    }
}