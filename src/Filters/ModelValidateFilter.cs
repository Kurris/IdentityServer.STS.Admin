using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.STS.Admin.Filters
{
    public class ModelValidateFilter : ActionFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorResult = context.ModelState
                    .Keys
                    .SelectMany(key =>
                        context.ModelState[key].Errors.Where(x => !string.IsNullOrEmpty(key)).Select(x => new
                        {
                            Field = key,
                            Message = x.ErrorMessage
                        }));
                var injectApiResult = context.HttpContext.RequestServices.GetService<IApiResult>();
                context.Result = new ObjectResult(injectApiResult.GetDefaultValidateApiResult(errorResult));
            }
            else
            {
                if (context.Result is ObjectResult objectResult)
                {
                    var result = objectResult.Value;
                    var type = result?.GetType();

                    if (type != null)
                    {
                        if (type.IsGenericType && typeof(IApiResult).IsAssignableFrom(type))
                            context.Result = new ObjectResult(result);
                        else
                        {
                            
                        }
                        {
                            var injectApiResult = context.HttpContext.RequestServices.GetService<IApiResult>();
                            context.Result = new ObjectResult(injectApiResult.GetDefaultSuccessApiResult(result));
                        }
                    }
                    else
                    {
                        var injectApiResult = context.HttpContext.RequestServices.GetService<IApiResult>();
                        context.Result = new ObjectResult(injectApiResult.GetDefaultSuccessApiResult(result));
                    }
                }
            }

            await base.OnResultExecutionAsync(context, next);
        }
    }
}