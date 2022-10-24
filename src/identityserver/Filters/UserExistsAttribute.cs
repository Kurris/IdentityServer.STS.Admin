using System;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.STS.Admin.Filters
{
    /// <summary>
    /// 表示用户存在特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserExistsAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();

            var user = await userManager.GetUserAsync(context.HttpContext.User);
            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            await next();
        }
    }
}