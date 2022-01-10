using System;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Configuration;
using IdentityServer.STS.Admin.Resolvers;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Helpers;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Account;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Extensions;

namespace IdentityServer.STS.Admin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly UserResolver<User> _userResolver;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEventService _eventService;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public AuthenticateController(IIdentityServerInteractionService interaction,
            IWebHostEnvironment environment
            , UserResolver<User> userResolver
            , SignInManager<User> signInManager
            , UserManager<User> userManager
            , IEventService eventService
            , IClientStore clientStore
            , IAuthenticationSchemeProvider schemeProvider)
        {
            _interaction = interaction;
            _environment = environment;
            _userResolver = userResolver;
            _signInManager = signInManager;
            _userManager = userManager;
            _eventService = eventService;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
        }


        /// <summary>
        /// 获取登录状态
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("status")]
        public async Task<ApiResult<object>> GetIsAuthenticated()
        {
            var result = User.IsAuthenticated();

            var apiResult = new ApiResult<object>()
            {
                Code = result ? 200 : 401,
                Data = result,
            };

            return await Task.FromResult(apiResult);
        }


        /// <summary>
        /// 检查并获取登录页面状态
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet("checkLogin")]
        [AllowAnonymous]
        public async Task<ApiResult<object>> GetLogin(string returnUrl)
        {
            var output = await BuildLoginResultAsync(returnUrl);
            if (!output.EnableLocalLogin && output.ExternalProviders.Count() == 1)
            {
                //只有一个外部登录可用
                return await ExternalLogin(output.ExternalProviders.First().AuthenticationScheme, returnUrl);
            }

            return new ApiResult<object>
            {
                Route = DefineRoute.Login,
                Data = output,
            };
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<object>> ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = await ExternalLoginCallback(returnUrl);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, "redirectUrl");

            return new ApiResult<object>
            {
                Code = 200,
                Data = Challenge(properties, provider)
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<object>> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"外部提供程序出错{remoteError}");

                return new ApiResult<object> {Route = DefineRoute.Login};
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return await GetLogin(null);
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return new ApiResult<object> {Route = DefineRoute.Redirect, Data = returnUrl};
                }

                return new ApiResult<object> {Route = DefineRoute.HomePage};
            }

            if (result.RequiresTwoFactor)
            {
                return new ApiResult<object>
                {
                    Route = DefineRoute.LoginWith2Fa,
                    Data = returnUrl
                };
            }

            if (result.IsLockedOut)
            {
                return new ApiResult<object>
                {
                    Route = DefineRoute.Lockout
                };
            }

            // 如果用户没有账号，请求用户创建
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var userName = info.Principal.Identity.Name;

            return await ExternalLoginConfirmation(new ExternalLoginConfirmationOutputModel
            {
                Email = email,
                UserName = userName,
                ReturnUrl = returnUrl,
                LoginProvider = info.LoginProvider,
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ApiResult<object>> ExternalLoginConfirmation(ExternalLoginConfirmationOutputModel model)
        {
            model.ReturnUrl ??= "/home";

            //从外部登录提供器中获取用户的信息
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return new ApiResult<object>()
                {
                    Route = DefineRoute.ExternalLoginFailure
                };
            }

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return new ApiResult<object> {Route = DefineRoute.Redirect, Data = model.ReturnUrl};
                        }

                        return new ApiResult<object>()
                        {
                            Route = DefineRoute.HomePage
                        };
                    }
                }

                AddErrors(result);
            }

            model.LoginProvider = info.LoginProvider;

            return new ApiResult<object>()
            {
                Route = DefineRoute.ExternalLoginConfirmation,
                Data = model
            };
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ApiResult<object>> Login([FromBody] LoginInputModel request)
        {
            var context = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);

            // the user clicked the "cancel" button
            if (request.RequestType != "login")
            {
                if (context != null)
                {
                    //如果用户取消，发送一个取消认证许可的结果到ids，甚至可以说这个客户端没有请求"许可";
                    //并且返回一个令牌 取消 OIDC的错误结果到客户端; 
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    //如果GetAuthorizationContextAsync不是返回null值，那么这个返回地址就是可用的
                    if (context.IsNativeClient())
                    {
                        //本地客户端的话,这个会让终端用户有更好的交互体验
                        return new ApiResult<object>
                        {
                            Route = DefineRoute.LoadingPage,
                            Data = request.ReturnUrl,
                        };
                    }

                    return new ApiResult<object>
                    {
                        Route = DefineRoute.Redirect,
                        Data = request.ReturnUrl,
                    };
                }

                //返回主页
                return new ApiResult<object>
                {
                    Route = DefineRoute.HomePage,
                };
            }

            //实体验证
            if (ModelState.IsValid)
            {
                var user = await _userResolver.GetUserAsync(request.Username);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName
                        , request.Password
                        , request.RememberLogin
                        , true);

                    //账号密码验证成功
                    if (result.Succeeded)
                    {
                        await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                return new ApiResult<object>
                                {
                                    Route = DefineRoute.LoadingPage,
                                    Data = request.ReturnUrl
                                };
                            }

                            return new ApiResult<object>
                            {
                                Route = DefineRoute.Redirect,
                                Data = request.ReturnUrl,
                            };
                        }

                        if (Url.IsLocalUrl(request.ReturnUrl))
                        {
                            return new ApiResult<object>
                            {
                                Route = DefineRoute.Redirect,
                                Data = request.ReturnUrl,
                            };
                        }

                        if (string.IsNullOrEmpty(request.ReturnUrl))
                        {
                            return new ApiResult<object>
                            {
                                Route = DefineRoute.HomePage,
                            };
                        }

                        throw new Exception("invalid return URL");
                    }

                    if (result.RequiresTwoFactor)
                    {
                        return await LoginWith2fa(new LoginWith2faInputModel
                        {
                            ReturnUrl = request.ReturnUrl,
                            RememberMe = request.RememberLogin,
                        });
                    }

                    if (result.IsLockedOut)
                    {
                        return new ApiResult<object>()
                        {
                            Route = DefineRoute.Lockout,
                        };
                    }
                }

                await _eventService.RaiseAsync(new UserLoginFailureEvent(request.Username, "invalid credentials",
                    clientId: context?.Client.ClientId));

                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var loginResult = await BuildLoginResultAsync(request);
            return loginResult;
        }


        private async Task<ApiResult<object>> BuildLoginResultAsync(LoginInputModel model)
        {
            var output = await BuildLoginResultAsync(model.ReturnUrl);
            output.Username = model.Username;
            output.RememberLogin = model.RememberLogin;
            return new ApiResult<object>()
            {
                Route = DefineRoute.None,
                Data = output,
            };
        }

        private async Task<LoginOutputModel> BuildLoginResultAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var isLocal = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var output = new LoginOutputModel
                {
                    EnableLocalLogin = isLocal,
                    ReturnUrl = returnUrl,
                    Username = context.LoginHint,
                };

                if (!isLocal)
                {
                    output.ExternalProviders = new[]
                    {
                        new ExternalProvider
                        {
                            AuthenticationScheme = context.IdP
                        }
                    };
                }

                return output;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                });

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme));
                    }
                }
            }

            return new LoginOutputModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }


        public async Task<ApiResult<object>> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException("无法加载双因素身份验证用户");
            }

            var model = new LoginWith2faOutputModel()
            {
                ReturnUrl = returnUrl,
                RememberMe = rememberMe
            };

            return new ApiResult<object>()
            {
                Route = DefineRoute.None,
                Data = model
            };
        }

        public async Task<ApiResult<object>> LoginWith2fa(LoginWith2faInputModel request)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<object>()
                {
                    Route = DefineRoute.LoginWith2Fa,
                    Data = request,
                };
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("无法加载双因素身份验证用户");
            }

            var authenticatorCode = request.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, request.RememberMe, request.RememberMachine);

            if (result.Succeeded)
            {
                return new ApiResult<object>()
                {
                    Route = string.IsNullOrEmpty(request.ReturnUrl) ? DefineRoute.HomePage : DefineRoute.Redirect,
                    Data = request,
                };
            }

            if (result.IsLockedOut)
            {
                return new ApiResult<object>
                {
                    Route = DefineRoute.LoginWith2Fa,
                    Data = request,
                };
            }

            ModelState.AddModelError(string.Empty, "验证码无效");

            return new ApiResult<object>()
            {
                Route = DefineRoute.LoginWith2Fa,
                Data = request,
            };
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var context = await _interaction.GetLogoutContextAsync(logoutId);
            var showSignoutPrompt = true;

            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                showSignoutPrompt = false;
            }

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await HttpContext.SignOutAsync();
            }

            // no external signout supported for now (see \Quickstart\Account\AccountController.cs TriggerExternalSignout)
            return Ok(new
            {
                showSignoutPrompt,
                ClientName = string.IsNullOrEmpty(context?.ClientName) ? context?.ClientId : context?.ClientName,
                context?.PostLogoutRedirectUri,
                context?.SignOutIFrameUrl,
                logoutId
            });
        }

        [HttpGet]
        [Route("Error")]
        public async Task<IActionResult> Error(string errorId)
        {
            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);

            if (message != null)
            {
                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return Ok(message);
        }
    }
}