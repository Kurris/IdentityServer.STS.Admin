using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer.STS.Admin.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            context.Result = new ObjectResult(new
            {
                code = 500,
                msg = context.Exception.Message
            });
            context.ExceptionHandled = true;

            await Task.CompletedTask;
        }
    }
}
