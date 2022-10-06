using System;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Models;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.IdentityServerExtension
{
    /// <summary>
    /// 使用后端密码验证
    /// </summary>
    public class BackendPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEventService _eventService;

        public BackendPasswordValidator(UserManager<User> userManager
            , SignInManager<User> signInManager
            , IEventService eventService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _eventService = eventService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var tempUserName = context.UserName;
            var password = context.Password;

            var user = await _userManager.FindByNameAsync(tempUserName) ?? await _userManager.FindByEmailAsync(tempUserName);

            if (user != null)
            {
                var signResult = await _signInManager.PasswordSignInAsync(
                    user.UserName
                    , password
                    , false
                    , true);

                //账号密码验证成功
                if (signResult.Succeeded)
                {
                    await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));
                    context.Result = new GrantValidationResult(user.Id.ToString(), GrantType.ResourceOwnerPassword);
                }

                if (signResult.RequiresTwoFactor)
                {
                }

                if (signResult.IsLockedOut)
                {
                    throw new Exception("账号已被锁定");
                }

                if (signResult.IsNotAllowed)
                {
                    throw new Exception("账号不允许登录");
                }
            }
        }
    }
}