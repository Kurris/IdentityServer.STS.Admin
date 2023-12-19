using System;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IdentityServer.STS.Admin.Filters;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (ex.Source != "IdentityServer4")
            {
                if (!context.Response.HasStarted && context.Response.StatusCode != 302)
                {
                    string msg = ex.GetBaseException().Message;

                    context.Response.StatusCode = 200;
                    byte[] content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ApiResult<object>()
                    {
                        Code = 500,
                        Msg = msg
                    }, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));

                    context.Response.ContentType = "application/json";
                    context.Response.ContentLength = content.Length;
                    await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(content));
                }
            }
        }
    }
}