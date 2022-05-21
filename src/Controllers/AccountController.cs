﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.STS.Admin.Configuration;
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
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Web;
using IdentityServer.STS.Admin.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace IdentityServer.STS.Admin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEventService _eventService;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;


        public AccountController(IIdentityServerInteractionService interaction,
            IWebHostEnvironment environment
            , SignInManager<User> signInManager
            , UserManager<User> userManager
            , IEventService eventService
            , IClientStore clientStore
            , IAuthenticationSchemeProvider schemeProvider
            , EmailService emailService
            , IConfiguration configuration
            , ILogger<AccountController> logger)
        {
            _interaction = interaction;
            _environment = environment;
            _signInManager = signInManager;
            _userManager = userManager;
            _eventService = eventService;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        public string FrontendBaseUrl => _configuration.GetSection("FrontendBaseUrl").Value;


        /// <summary>
        /// 获取登录状态
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("status")]
        public async Task<ApiResult<object>> GetIsAuthenticated()
        {
            var isLogin = User.IsAuthenticated();

            User user = null;

            if (isLogin)
            {
                var subId = User.GetSubjectId();
                user = await _userManager.FindByIdAsync(subId);
            }

            return new ApiResult<object>
            {
                Data = new
                {
                    isLogin,
                    user
                }
            };
        }

        [HttpGet("user")]
        public async Task<User> GetUserByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }


        /// <summary>
        /// 检查并获取登录页面状态
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet("loginUiSetting")]
        [AllowAnonymous]
        public async Task<ApiResult<object>> CheckLoginAndGetUiSetting(string returnUrl)
        {
            var output = await BuildLoginResultAsync(returnUrl);
            if (!output.EnableLocalLogin && output.ExternalProviders.Count() == 1)
            {
                //只有一个外部登录可用
                output.ExternalProviders = new[]
                {
                    new ExternalProvider
                    {
                        DisplayName = output.ExternalProviders.FirstOrDefault()?.DisplayName,
                        AuthenticationScheme = output.ExternalProviders.FirstOrDefault()?.AuthenticationScheme
                    }
                };
            }

            _logger.LogInformation("Enabled external login providers: {Providers}", string.Join(",", output.ExternalProviders.Select(x => x.DisplayName)));

            return new ApiResult<object>
            {
                Route = DefineRoute.Login,
                Data = output,
            };
        }


        /// <summary>
        /// 外部登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("externalLogin")]
        [AllowAnonymous]
        public IActionResult ExternalLogin([FromForm] ExternalLoginInput input)
        {
            var redirectUrl = $"http://localhost:5000/api/authenticate/externalLoginCallback?ReturnUrl={HttpUtility.UrlEncode(input.ReturnUrl)}";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(input.Provider, redirectUrl);

            return Challenge(properties, input.Provider);
        }

        /// <summary>
        /// 外部登录回调
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="remoteError"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("externalLoginCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (!string.IsNullOrEmpty(remoteError))
            {
                throw new Exception($"外部提供程序出错{remoteError}");
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = HttpUtility.UrlDecode(returnUrl);
                // if (returnUrl.Contains("*"))
                // {
                //     returnUrl = returnUrl.Replace('*', '&');
                // }
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Redirect($"{FrontendBaseUrl}/signIn");
            }

            // 如果用户已登录，请使用此外部登录提供程序登录用户。
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl) || returnUrl.IsLocal(HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString()))
                {
                    return Redirect(returnUrl);
                }

                return Redirect(returnUrl + "/home");
            }

            if (result.RequiresTwoFactor)
            {
                // return new ApiResult<object>
                // {
                //     Route = DefineRoute.LoginWith2Fa,
                //     Data = returnUrl
                // };
            }

            if (result.IsLockedOut)
            {
                throw new Exception("账号已被锁定");
            }

            // 如果用户没有账号，请求用户创建
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var userName = info.Principal.Identity.Name;

            var urlParams = new Dictionary<string, string>()
            {
                ["email"] = email,
                ["userName"] = userName,
                ["returnUrl"] = returnUrl,
                ["loginProvider"] = info.LoginProvider
            };

            using (var urlEncodedContent = new FormUrlEncodedContent(urlParams))
            {
                var urlParamsString = await urlEncodedContent.ReadAsStringAsync();
                return Redirect("http://localhost:8080/externalLoginConfirmation" + "?" + urlParamsString);
            }
        }

        /// <summary>
        /// 外部登录注册当前用户体系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPost("externalRegister")]
        public async Task<ApiResult<object>> ExternalLoginConfirmation(ExternalLoginConfirmationInput input)
        {
            input.ReturnUrl ??= "/home";

            //从外部登录提供器中获取用户的信息
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return new ApiResult<object>()
                {
                    Route = DefineRoute.ExternalLoginFailure
                };
            }

            var user = new User
            {
                UserName = input.UserName,
                Email = input.Email
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (input.ReturnUrl.IsLocal())
                    {
                        return new ApiResult<object>
                        {
                            Route = DefineRoute.Redirect,
                            Data = input.ReturnUrl
                        };
                    }

                    return new ApiResult<object>
                    {
                        Route = DefineRoute.HomePage
                    };
                }
            }

            throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
        }


        /// <summary>
        /// 注册账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPost("accout/register")]
        public async Task<ApiResult<object>> Register(RegisterInputModel model)
        {
            model.ReturnUrl ??= $"{FrontendBaseUrl}/siginIn";

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var dic = new Dictionary<string, string>()
                {
                    ["userId"] = user.Id.ToString(),
                    ["code"] = code
                };

                using (var content = new FormUrlEncodedContent(dic))
                {
                    var callbackUrl = $"{FrontendBaseUrl}/confirmEmail?" + await content.ReadAsStringAsync();

                    await _emailService.SendEmailAsync("注册", callbackUrl, new[] {new MailboxAddress(model.UserName, model.Email)});
                    return new ApiResult<object>()
                    {
                        Route = DefineRoute.ConfirmEmail
                    };
                }
            }
            else
            {
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
            }
        }


        [HttpPost("accout/{userId}/email/{code}/validation")]
        [AllowAnonymous]
        public async Task ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                //用户id和code不存在
                throw new Exception("非法的用户信息");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("非法的用户信息");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
            }
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ApiResult<object>> Login([FromBody] LoginInputModel request)
        {
            var context = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);
            // var tenant = context?.Tenant;

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

            var user = await _userManager.FindByNameAsync(request.Username);
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
                    await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));

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

                    if (request.ReturnUrl.IsLocal())
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
                    return new ApiResult<object>()
                    {
                        Route = DefineRoute.LoginWith2Fa,
                        Data = new
                        {
                            rememberLogin = request.RememberLogin,
                            returnUrl = request.ReturnUrl,
                        }
                    };
                }

                if (result.IsLockedOut)
                {
                    throw new Exception("账号已被锁定");
                }
            }

            await _eventService.RaiseAsync(new UserLoginFailureEvent(request.Username, "错误的凭证",
                clientId: context?.Client.ClientId));

            throw new Exception("账号或者密码错误");
        }


        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<ApiResult<object>> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var output = await BuildLogoutViewModelAsync(logoutId);

            if (output.ShowLogoutPrompt == false)
            {
                //如果注销请求已从身份服务器正确进行身份验证，则
                //我们不需要显示提示，只需直接将用户注销即可。
                return await Logout(output);
            }

            return new ApiResult<object>
            {
                Route = DefineRoute.LoginOut,
                Data = output
            };
        }


        private async Task<LogoutOutputModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var output = new LogoutOutputModel
            {
                LogoutId = logoutId,
                ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt
            };

            //如果用户没有登录，那么直接显示注销页面
            if (User?.Identity.IsAuthenticated != true)
            {
                output.ShowLogoutPrompt = false;
                return output;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                //安全并且自动退出
                output.ShowLogoutPrompt = false;
                return output;
            }

            //显示注销提醒，防止其他恶意的页面自动退出用户登录的攻击
            return output;
        }


        /// <summary>
        /// 处理注销后的操作
        /// </summary>
        [AllowAnonymous]
        [HttpPost("loggedOut")]
        public async Task<ApiResult<object>> Logout(LogoutInput model)
        {
            var output = await BuildLoggedOutModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                //删除本地cookie
                await _signInManager.SignOutAsync();

                //唤起用户注销成功事件
                await _eventService.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            //检查是否需要在上游的认证中心触发注销
            if (output.TriggerExternalSignOut)
            {
                //身份认证服务器的前端需要使用表单提交的方式请求当前路由
                //方法返回值需要返回IActionResult才能让浏览器302重定向
                //创建一个返回链接，在用户成功注销后这样上游的提供器会重定向到这
                //这里处理完成的单点登出处理
                //var url = "当前方法路由的地址";

                //触发到第三方登录来退出
                // SignOut(new AuthenticationProperties
                // {
                //     RedirectUri = url
                // }, output.ExternalAuthenticationScheme);
            }

            return new ApiResult<object>
            {
                Route = DefineRoute.LoggedOut,
                Data = output,
            };
        }


        private async Task<LoggedOutOutput> BuildLoggedOutModelAsync(string logoutId)
        {
            //获取退出登录上下文信息，包括应用名称，注销后重定向地址或者集成退出的iframe
            var logoutContext = await _interaction.GetLogoutContextAsync(logoutId);

            var output = new LoggedOutOutput
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logoutContext?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logoutContext?.ClientName) ? logoutContext?.ClientId : logoutContext.ClientName,
                SignOutIframeUrl = logoutContext?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignOut = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignOut)
                    {
                        //如果没有当前注销上下文，我们需要创建一个从当前登录用户捕获必要信息的上下文
                        //在我们注销之前，请重定向到外部 IdP 进行注销
                        output.LogoutId ??= await _interaction.CreateLogoutContextAsync();
                        output.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return output;
        }


        private async Task<LoginOutputModel> BuildLoginResultAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var isLocal = context.IdP == IdentityServerConstants.LocalIdentityProvider;
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

        [AllowAnonymous]
        [HttpGet("twoFactorAuthenticationUser")]
        public async Task<ApiResult<object>> LoginWith2Fa(bool rememberMe, string returnUrl = null)
        {
            //确保用户账号和密码正确
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                throw new InvalidOperationException("无法加载双因素身份验证用户");

            var model = new LoginWith2faOutputModel
            {
                ReturnUrl = returnUrl,
                RememberMe = rememberMe
            };

            return new ApiResult<object>
            {
                Route = DefineRoute.LoginWith2Fa,
                Data = model
            };
        }


        [AllowAnonymous]
        [HttpPost("twoFactorAuthenticationUser/signIn")]
        public async Task<ApiResult<object>> LoginWith2Fa(LoginWith2faInputModel input)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                throw new InvalidOperationException("无法加载双因素身份验证用户");

            var authenticatorCode = input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, input.RememberMe, input.RememberMachine);

            if (result.Succeeded)
            {
                return new ApiResult<object>
                {
                    Route = string.IsNullOrEmpty(input.ReturnUrl) ? DefineRoute.HomePage : DefineRoute.Redirect,
                    Data = input.ReturnUrl,
                };
            }

            if (result.IsLockedOut)
            {
                return new ApiResult<object>
                {
                    Code = 0,
                    Route = DefineRoute.LoginWith2Fa,
                    Data = input,
                    Msg = "账号已被锁定",
                };
            }

            return new ApiResult<object>
            {
                Route = DefineRoute.LoginWith2Fa,
                Data = input,
                Msg = "验证码无效"
            };
        }


        [HttpGet("error")]
        public async Task<ApiResult<object>> GetError(string errorId)
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

            return new ApiResult<object>
            {
                Code = 200,
                Data = message,
            };
        }

        [AllowAnonymous]
        [HttpGet("2fa/signInWithCode")]
        public async Task<ApiResult<string>> LoginWithRecoveryCode(string returnUrl = null)
        {
            //确保用户首先通过用户名和密码
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("用户尚未启用双重验证");
            }

            //var model = new LoginWithRecoveryCodeViewModel()
            //{
            //    ReturnUrl = returnUrl
            //};

            return new ApiResult<string>
            {
                Data = returnUrl,
            };
        }


        [HttpPost("2fa/signInWithCode")]
        [AllowAnonymous]
        public async Task<ApiResult<object>> LoginWithRecoveryCode(LoginWithRecoveryCodeInputModel model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                //throw new InvalidOperationException(_localizer["Unable2FA"]);
                throw new Exception("用户尚未开启双重验证");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                return new ApiResult<object>()
                {
                    Route = string.IsNullOrEmpty(model.ReturnUrl)
                        ? DefineRoute.HomePage
                        : DefineRoute.Redirect,
                    Data = model.ReturnUrl,
                };
            }

            if (result.IsLockedOut)
                throw new Exception("账号已被锁定");

            throw new Exception("异常错误");
        }

        /// <summary>
        /// 忘记密码，发送邮件验证
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPost("password/email")]
        [AllowAnonymous]
        public async Task ForgotPassword(ForgotPasswordInput model)
        {
            var user = model.Policy switch
            {
                LoginResolutionPolicyType.Email => await _userManager.FindByEmailAsync(model.Content),
                LoginResolutionPolicyType.Username => await _userManager.FindByNameAsync(model.Content),
                _ => throw new InvalidOperationException()
            };

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // 不能透露用户不存在，只能提醒已发送邮件
                if (user == null)
                    return;
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //url query
            var dic = new Dictionary<string, string>
            {
                ["userId"] = user.Id.ToString(),
                ["code"] = code,
                ["email"] = user.Email,
            };

            using (var content = new FormUrlEncodedContent(dic))
            {
                var queries = await content.ReadAsStringAsync();
                //前端地址
                var callbackUrl = $"{FrontendBaseUrl}/resetPassword?" + queries;
                await _emailService.SendEmailAsync("密码找回", callbackUrl, new[] {new MailboxAddress(user.UserName, user.Email)});
            }
        }

        /// <summary>
        /// 通过邮件重置密码
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPut("password/email/found")]
        public async Task ResetPasswordFromEmailSide(ResetPasswordInput model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            //不能透露用户不存在,默认完成
            if (user == null)
                return;

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
        }
    }
}