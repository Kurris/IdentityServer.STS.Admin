using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
                                context.ModelState[key].Errors.Where(x => !string.IsNullOrEmpty(key)).Select(x =>
                                {
                                    return new
                                    {
                                        Field = key,
                                        Message = x.ErrorMessage
                                    };
                                }));

                context.Result = new ObjectResult(new ApiResult<IEnumerable<object>>()
                {
                    Code = 400,
                    Data = errorResult,
                    Msg = "实体验证失败"
                });
            }
            else
            {
                object successResult = (context.Result as ObjectResult)?.Value;

                if (successResult != null)
                {
                    if (successResult.GetType().Name.StartsWith("ApiResult"))
                        context.Result = new ObjectResult(successResult);
                    else
                        context.Result = new ObjectResult(new ApiResult<object>
                        {
                            Code = 200,
                            Msg = "操作成功",
                            Data = successResult
                        });
                }
            }

            await base.OnResultExecutionAsync(context, next);
        }
    }
}
