using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer.STS.Admin.Filters;

public class ExceptionFilter : ExceptionFilterAttribute
{
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        context.Result = new ObjectResult(new ApiResult<string>
        {
            Code = 500,
            Msg = context.Exception.GetBaseException().Message,
        });
        context.ExceptionHandled = true;

        await Task.CompletedTask;
    }
}