﻿using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Resolvers;
using IdentityServer4;
using IdentityServer.STS.Admin;
using IdentityServer.STS.Admin.Entities;
using IdentityServer.STS.Admin.Resolvers;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticateController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly UserResolver<User> _userResolver;
        private readonly UserManager<User> _userManager;

        public AuthenticateController(IIdentityServerInteractionService interaction,
            IWebHostEnvironment environment
            , UserResolver<User> userResolver
            , UserManager<User> userManager)
        {
            _interaction = interaction;
            _environment = environment;
            _userResolver = userResolver;
            _userManager = userManager;
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string ReturnUrl { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var context = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);
            // var user = Config.GetTestUsers()
            //     .FirstOrDefault(usr => usr.Password == request.Password && usr.Username == request.Username);
            var user = await _userResolver.GetUserAsync(request.Username);
            if (user == null || context == null) return Unauthorized();

            await HttpContext.SignInAsync(new IdentityServerUser(user.Id));
            return new JsonResult(new
            {
                RedirectUrl = request.ReturnUrl,
                IsOk = true
            });
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