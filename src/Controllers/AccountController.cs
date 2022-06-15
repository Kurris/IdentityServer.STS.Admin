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
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Helpers;
using IdentityServer.STS.Admin.Models;
using IdentityServer.STS.Admin.Models.Account;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;

namespace IdentityServer.STS.Admin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private const double ExpiredTime = 30;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEventService _eventService;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly EmailGenerateService _emailGenerateService;
        private readonly ITimeLimitedDataProtector _timeLimitedDataProtector;
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
            , EmailGenerateService emailGenerateService
            , IDataProtectionProvider protectionProvider
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
            _emailGenerateService = emailGenerateService;
            _timeLimitedDataProtector = protectionProvider.CreateProtector("email").ToTimeLimitedDataProtector();
            _logger = logger;
        }

        private string FrontendBaseUrl => _configuration.GetSection("FrontendBaseUrl").Value;
        private string BackendBaseUrl => _configuration.GetSection("BackendBaseUrl").Value;


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
                if (user == null)
                {
                    await _signInManager.SignOutAsync();
                    isLogin = false;
                }
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
            _logger.LogInformation("Enabled external login providers: {Providers}", string.Join(",", output.ExternalProviders.Select(x => x.DisplayName)));

            return new ApiResult<object>
            {
                Route = DefineRoute.Login,
                Data = output
            };
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ApiResult<object>> Login(LoginInput request)
        {
            var context = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);
            var tenant = context?.Tenant;
            if (!string.IsNullOrEmpty(tenant))
            {
                _logger.LogInformation("current tenant is {Tenant}", tenant);
            }

            //取消登录

            #region cancel login

            // if (request.RequestType != "login")
            // {
            //     if (context != null)
            //     {
            //         //如果用户取消，发送一个取消认证的结果到ids，甚至可以说这个客户端没有请求"许可";
            //         //并且返回一个令牌 取消 OIDC的错误结果到客户端; 
            //         await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);
            //
            //         //如果GetAuthorizationContextAsync不是返回null值，那么这个返回地址就是可用的
            //         if (context.IsNativeClient())
            //         {
            //             //本地客户端的话,这个会让终端用户有更好的交互体验
            //             return new ApiResult<object>
            //             {
            //                 Route = DefineRoute.LoadingPage,
            //                 Data = request.ReturnUrl,
            //             };
            //         }
            //
            //         return new ApiResult<object>
            //         {
            //             Route = DefineRoute.Redirect,
            //             Data = request.ReturnUrl,
            //         };
            //     }
            //
            //     //返回主页
            //     return new ApiResult<object>
            //     {
            //         Route = DefineRoute.HomePage,
            //     };
            // }

            #endregion

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                var signResult = await _signInManager.PasswordSignInAsync(
                    user.UserName
                    , request.Password
                    , request.RememberLogin
                    , true);

                //账号密码验证成功
                if (signResult.Succeeded)
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

                    throw new Exception("非法的重定向地址");
                }

                if (signResult.RequiresTwoFactor)
                {
                    return new ApiResult<object>
                    {
                        Route = DefineRoute.LoginWith2Fa,
                        Data = new
                        {
                            rememberLogin = request.RememberLogin,
                            returnUrl = request.ReturnUrl,
                        }
                    };
                }

                if (signResult.IsLockedOut)
                {
                    throw new Exception("账号已被锁定");
                }

                //邮件地址/手机号/账号
                if (signResult.IsNotAllowed)
                {
                    throw new Exception("账号不允许登录");
                }
            }

            await _eventService.RaiseAsync(new UserLoginFailureEvent(request.UserName, "账号登录失败", clientId: context?.Client.ClientId));
            throw new Exception("账号或者密码错误");
        }


        /// <summary>
        /// 外部登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("externalLogin")]
        public IActionResult ExternalLogin([FromForm] ExternalLoginInput input)
        {
            var callbackUrl = BackendBaseUrl + Url.Action("ExternalLoginCallback", new
            {
                isLocal = input.IsLocal,
                returnUrl = HttpUtility.UrlEncode(input.ReturnUrl)
            });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(input.Provider, callbackUrl);

            return Challenge(properties, input.Provider);
        }

        /// <summary>
        /// 注册账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPost("accout/register")]
        public async Task Register(RegisterInput model)
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
                code = _timeLimitedDataProtector.Protect(code, TimeSpan.FromMinutes(ExpiredTime));
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = BackendBaseUrl + Url.Action("ConfirmEmail", new
                {
                    userId = user.Id.ToString(),
                    code
                });

                var content = await _emailGenerateService.GetEmailConfirmHtml(user.UserName, callbackUrl, ExpiredTime.ToString());
                await _emailService.SendEmailAsync("验证邮箱用户", content, new[] {new MailboxAddress(model.UserName, model.Email)});
            }
            else
            {
                //DuplicateUserName
                //DuplicateEmail
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
            }
        }

        /// <summary>
        /// 外部登录回调
        /// </summary>
        /// <param name="isLocal"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpGet("externalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(bool isLocal, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = HttpUtility.UrlDecode(returnUrl);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return await RedirectHelper.Go($"{FrontendBaseUrl}/signIn");
            }

            // 如果用户已登录，请使用此外部登录提供程序登录用户。
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                if (isLocal)
                {
                    var obj = await GetIsAuthenticated();
                    var dynamicInfo = obj.Data as dynamic;
                    var currentUsername = dynamicInfo.user.UserName;
                    return await RedirectHelper.Go($"{FrontendBaseUrl}/zone/{currentUsername}");
                }

                return await RedirectHelper.Go(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                var ps = new Dictionary<string, string>
                {
                    ["rememberMe"] = "false",
                    ["returnUrl"] = returnUrl
                };

                return await RedirectHelper.Go($"{FrontendBaseUrl}/signinWith2fa", ps);
            }

            if (result.IsLockedOut)
            {
                return await RedirectHelper.Go($"{FrontendBaseUrl}/error", new Dictionary<string, string>
                {
                    ["error"] = "账号已被锁定"
                });
            }

            if (result.IsNotAllowed)
            {
                return await RedirectHelper.Go($"{FrontendBaseUrl}/error", new Dictionary<string, string>
                {
                    ["error"] = "账号不允许登录"
                });
            }

            // 如果用户没有账号，请求用户创建
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var userName = info.Principal.Identity.Name;

            return await RedirectHelper.Go($"{FrontendBaseUrl}/externalLoginConfirmation", new Dictionary<string, string>
            {
                ["email"] = email,
                ["userName"] = userName,
                ["returnUrl"] = returnUrl,
                ["loginProvider"] = info.LoginProvider
            });
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
            //从外部登录提供器中获取用户的信息
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new Exception("外部登录关联已失效");
            }

            var user = new User
            {
                UserName = input.UserName,
                Email = input.Email
            };

            var result = await _userManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                //关联外部登录
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (input.ReturnUrl.IsLocal())
                    {
                        return new ApiResult<object>
                        {
                            Route = DefineRoute.HomePage
                        };
                    }

                    return new ApiResult<object>
                    {
                        Route = DefineRoute.Redirect,
                        Data = input.ReturnUrl
                    };
                }
            }

            throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
        }

        /// <summary>
        /// 外部登录使用本地账号登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPost("externalLoginWithLocalLogin")]
        public async Task<ApiResult<object>> ExternalLoginWithLocalLogin(ExternalLoginWithLocalInput input)
        {
            //从外部登录提供器中获取用户的信息
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new Exception("外部登录关联已失效");
            }

            var user = await _userManager.FindByNameAsync(input.UserName);
            if (user != null)
            {
                var signResult = await _signInManager.PasswordSignInAsync(user.UserName, input.Password, false, true);
                //账号密码验证成功
                if (signResult.Succeeded)
                {
                    await _userManager.RemoveLoginAsync(user, info.LoginProvider, info.ProviderKey);
                    var loginResult = await _userManager.AddLoginAsync(user, info);

                    if (loginResult.Succeeded)
                    {
                        if (input.ReturnUrl.IsLocal())
                        {
                            return new ApiResult<object>
                            {
                                Route = DefineRoute.Redirect,
                                Data = input.ReturnUrl,
                            };
                        }

                        return new ApiResult<object>
                        {
                            Route = DefineRoute.HomePage
                        };
                    }

                    throw new Exception("非法的重定向地址");
                }

                if (signResult.RequiresTwoFactor)
                {
                    return new ApiResult<object>
                    {
                        Route = DefineRoute.LoginWith2Fa,
                        Data = new
                        {
                            rememberLogin = false,
                            returnUrl = input.ReturnUrl,
                            withExternalLogin = true
                        }
                    };
                }

                if (signResult.IsLockedOut)
                {
                    throw new Exception("账号已被锁定");
                }
            }

            throw new Exception("账号或者密码错误");
        }

        /// <summary>
        /// 检查外部登录
        /// </summary>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpGet("externalRegister")]
        public async Task ExternalLoginConfirmation()
        {
            //从外部登录提供器中获取用户的信息
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new Exception("外部登录关联已失效");
            }
        }

        /// <summary>
        /// 使用双重验证登录
        /// </summary>
        /// <param name="rememberMe"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [AllowAnonymous]
        [HttpGet("twoFactorAuthenticationUser")]
        public async Task<ApiResult<object>> LoginWith2Fa(bool rememberMe, string returnUrl = null)
        {
            //确保用户账号和密码正确
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                throw new InvalidOperationException("无法加载双因素身份验证用户");

            var model = new LoginWith2FaOutput
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


        /// <summary>
        /// 双重验证登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [AllowAnonymous]
        [HttpPost("twoFactorAuthenticationUser/signIn")]
        public async Task<ApiResult<object>> LoginWith2Fa(LoginWith2FaInput input)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                throw new InvalidOperationException("用户凭证已过期,请重新登录!");

            //处理空和分隔符
            var authenticatorCode = input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, input.RememberMe, input.RememberMachine);

            if (result.Succeeded)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();

                await _userManager.RemoveLoginAsync(user, info.LoginProvider, info.ProviderKey);
                var loginResult = await _userManager.AddLoginAsync(user, info);

                if (loginResult.Succeeded)
                {
                    return new ApiResult<object>
                    {
                        Route = DefineRoute.Redirect,
                        Data = input.ReturnUrl,
                    };
                }

                throw new Exception("非法的重定向地址");
            }

            //正常情况下在登录后就会提醒被锁定
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


        /// <summary>
        /// 恢复码登录检查是否存在登录凭证
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [AllowAnonymous]
        [HttpGet("2fa/signInWithCode")]
        public async Task LoginWithRecoveryCode()
        {
            //确保用户首先通过用户名和密码
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new Exception("用户凭证失效");
            }
        }


        /// <summary>
        /// 恢复码登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPost("2fa/signInWithCode")]
        public async Task<ApiResult<string>> LoginWithRecoveryCode(LoginWithRecoveryCodeInput input)
        {
            await LoginWithRecoveryCode();

            ExternalLoginInfo info = null;
            User user = null;

            //使用外部登录关联本地登录
            if (input.WithExternalLogin)
            {
                //从外部登录提供器中获取用户的信息
                info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                    throw new Exception("外部登录关联已失效");

                user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                    throw new InvalidOperationException("用户凭证已过期,请重新登录!");

                //处理关联
                await _userManager.RemoveLoginAsync(user, info.LoginProvider, info.ProviderKey);
                await _userManager.AddLoginAsync(user, info);
            }

            var recoveryCode = input.RecoveryCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                return new ApiResult<string>
                {
                    Route = string.IsNullOrEmpty(input.ReturnUrl)
                        ? DefineRoute.HomePage
                        : DefineRoute.Redirect,
                    Data = input.ReturnUrl
                };
            }

            if (input.WithExternalLogin)
                await _userManager.RemoveLoginAsync(user, info.LoginProvider, info.ProviderKey);

            throw new Exception(result.IsLockedOut ? "账号已被锁定" : "异常错误");
        }

        /// <summary>
        /// 确定/验证邮件地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpGet("validation/{userId}/email/{code}")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var redirectErrorUrl = $"{FrontendBaseUrl}/error";

            if (userId == null || code == null)
            {
                _logger.LogInformation("userId and core mybe empty or null: userId-{UserId} code-{Code}", userId, code);
                return await RedirectHelper.Go(redirectErrorUrl, new Dictionary<string, string>
                {
                    ["error"] = "验证失败"
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogInformation("user id : {Id}  not exists", userId);
                return await RedirectHelper.Go(redirectErrorUrl, new Dictionary<string, string>
                {
                    ["error"] = "验证失败"
                });
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogInformation("user id : {Id}  already confirmed email", userId);
                return await RedirectHelper.
                    Go(redirectErrorUrl, new Dictionary<string, string>
                {
                    ["error"] = "验证失败"
                });
            }

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                code = _timeLimitedDataProtector.Unprotect(code);
            }
            catch
            {
                code = string.Empty;
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            _logger.LogInformation("confirm email result is : {Result}", JsonConvert.SerializeObject(result));

            var rtnUrl = result.Succeeded
                ? await RedirectHelper.Get($"{FrontendBaseUrl}/successed", new Dictionary<string, string>()
                {
                    ["title"] = "您已成功验证邮件",
                    ["returnUrl"] = "/signIn"
                })
                : await RedirectHelper.Get(redirectErrorUrl, new Dictionary<string, string>
                {
                    ["error"] = "邮件验证已过期请,重新发送邮件进行验证"
                });

            return await RedirectHelper.Go(rtnUrl);
        }


        /// <summary>
        /// 忘记密码，发送邮件验证
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="InvalidOperationException"></exception>
        [AllowAnonymous]
        [HttpPost("password/email")]
        public async Task ForgotPassword(ForgotPasswordInput input)
        {
            var user = await _userManager.FindByNameAsync(input.Content) ?? await _userManager.FindByEmailAsync(input.Content);

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
                ["code"] = code,
                ["email"] = user.Email,
            };

            using (var content = new FormUrlEncodedContent(dic))
            {
                var queries = await content.ReadAsStringAsync();
                //前端地址
                var callbackUrl = $"{FrontendBaseUrl}/resetPassword?" + queries;
                await _emailService.SendEmailAsync("密码找回", callbackUrl, new[]
                {
                    new MailboxAddress(user.UserName, user.Email)
                });
            }
        }

        /// <summary>
        /// 通过邮件重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPut("password/email/found")]
        public async Task ResetPasswordFromEmailSide(ResetPasswordInput input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);

            //不能透露用户不存在,默认完成
            if (user == null) return;

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(input.Code));
            var result = await _userManager.ResetPasswordAsync(user, code, input.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
        }


        /// <summary>
        /// 退出
        /// </summary>
        [AllowAnonymous]
        [HttpGet("logout")]
        public async Task<ApiResult<object>> Logout(string logoutId)
        {
            //处理如何显示退出登录提醒界面
            var output = await BuildLogoutModelAsync(logoutId);

            if (!output.ShowLogoutPrompt)
            {
                //如果注销请求已从身份服务器正确进行身份验证，则不需要显示提示，只需直接将用户注销即可。
                return await Logout(output);
            }

            return new ApiResult<object>
            {
                Route = DefineRoute.LoginOut,
                Data = output
            };
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

        /// <summary>
        /// 获取错误内容
        /// </summary>
        /// <param name="errorId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("error")]
        public async Task<ApiResult<object>> GetError(string errorId)
        {
            //获取错误的上下文
            var message = await _interaction.GetErrorContextAsync(errorId);

            //存在错误,在开发环境显示错误内容
            if (message != null && !_environment.IsDevelopment())
            {
                message.ErrorDescription = null;
            }

            return new ApiResult<object>
            {
                Code = 200,
                Data = message,
            };
        }


        /*================================================帮助方法================================================*/

        #region help methods

        /// <summary>
        /// 获取已退出后的配置对象
        /// </summary>
        /// <param name="logoutId"></param>
        /// <returns></returns>
        private async Task<LoggedOutOutput> BuildLoggedOutModelAsync(string logoutId)
        {
            //获取退出登录上下文信息，包括应用名称，注销后重定向地址或者集成退出的iframe
            var context = await _interaction.GetLogoutContextAsync(logoutId);

            var output = new LoggedOutOutput
            {
                AutomaticRedirectAfterSignOut = false,
                PostLogoutRedirectUri = context?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(context?.ClientName) ? context?.ClientId : context.ClientName,
                SignOutIframeUrl = context?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            //如果已经登录
            if (User?.Identity.IsAuthenticated == true)
            {
                //获取登录提供器
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignOut = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignOut)
                    {
                        //如果没有当前注销上下文,我们需要创建一个从当前登录用户捕获必要信息的上下文
                        //在我们注销之前,需重定向到外部 IdP 进行注销
                        output.LogoutId ??= await _interaction.CreateLogoutContextAsync();
                        output.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return output;
        }


        /// <summary>
        /// 获取登录的配置对象
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private async Task<LoginOutput> BuildLoginResultAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var schemes = await _schemeProvider.GetAllSchemesAsync();

            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var isLocal = context.IdP == IdentityServerConstants.LocalIdentityProvider;
                var output = new LoginOutput
                {
                    EnableLocalLogin = isLocal,
                    ReturnUrl = returnUrl,
                    UserName = context.LoginHint,
                };

                if (!isLocal)
                {
                    output.ExternalProviders = schemes.Where(x => x.Name == context.IdP)
                        .Select(x => new ExternalProvider
                        {
                            DisplayName = x.DisplayName ?? x.Name,
                            AuthenticationScheme = x.Name
                        });
                }

                return output;
            }

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

                    //过滤不允许的登录提供器
                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme));
                    }
                }
            }

            return new LoginOutput
            {
                EnableLocalLogin = allowLocal,
                ReturnUrl = returnUrl,
                UserName = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }


        /// <summary>
        /// 获取退出的配置对象
        /// </summary>
        /// <param name="logoutId"></param>
        /// <returns></returns>
        private async Task<LogoutOutput> BuildLogoutModelAsync(string logoutId)
        {
            var output = new LogoutOutput
            {
                LogoutId = logoutId,
                ShowLogoutPrompt = true
            };

            //如果用户没有登录，那么直接显示注销页面
            if (!User.Identity.IsAuthenticated)
            {
                output.ShowLogoutPrompt = false;
            }
            else
            {
                var context = await _interaction.GetLogoutContextAsync(logoutId);
                //ShowSignoutPrompt显示注销提醒，防止其他恶意的页面自动退出用户登录的攻击
                if (context?.ShowSignoutPrompt == false)
                {
                    //安全并且自动退出
                    output.ShowLogoutPrompt = false;
                }
            }

            return output;
        }

        #endregion
    }
}