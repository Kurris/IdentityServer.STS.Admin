using System;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.STS.Admin.Models;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Extensions;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains(".well-known/openid-configuration"))
            {
                var baseUrl = context.RequestServices.GetService<IConfiguration>().GetSection("BackendBaseUrl").Value;
                var issuerUri = context.GetIdentityServerIssuerUri();

                var responseGenerator = context.RequestServices.GetService<IDiscoveryResponseGenerator>();
                var options = context.RequestServices.GetService<IdentityServerOptions>();

                // generate response
                var response = await responseGenerator.CreateDiscoveryDocumentAsync(baseUrl, issuerUri);

                var result = new DiscoveryDocumentResult(response, options.Discovery.ResponseCacheInterval).Entries;

                context.Response.StatusCode = 200;
                byte[] content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                context.Response.ContentType = "application/json";
                context.Response.ContentLength = content.Length;
                await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(content));
            }
            else
            {
                await _next(context);
            }
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

//{
//    "issuer": "identity.isawesome.cn",
//    "jwks_uri": "http://localhost:5000/.well-known/openid-configuration/jwks",
//    "authorization_endpoint": "http://localhost:5000/connect/authorize",
//    "token_endpoint": "http://localhost:5000/connect/token",
//    "userinfo_endpoint": "http://localhost:5000/connect/userinfo",
//    "end_session_endpoint": "http://localhost:5000/connect/endsession",
//    "check_session_iframe": "http://localhost:5000/connect/checksession",
//    "revocation_endpoint": "http://localhost:5000/connect/revocation",
//    "introspection_endpoint": "http://localhost:5000/connect/introspect",
//    "device_authorization_endpoint": "http://localhost:5000/connect/deviceauthorization",
//    "frontchannel_logout_supported": true,
//    "frontchannel_logout_session_supported": true,
//    "backchannel_logout_supported": true,
//    "backchannel_logout_session_supported": true,
//    "scopes_supported": [
//        "openid",
//        "email",
//        "address",
//        "roles",
//        "profile",
//        "offline_access"
//    ],
//    "claims_supported": [
//        "sub",
//        "email",
//        "address",
//        "role",
//        "name",
//        "family_name",
//        "given_name",
//        "middle_name",
//        "nickname",
//        "preferred_username",
//        "profile",
//        "picture",
//        "website",
//        "gender",
//        "birthdate",
//        "zoneinfo",
//        "locale",
//        "updated_at",
//        "tenant"
//    ],
//    "grant_types_supported": [
//        "authorization_code",
//        "client_credentials",
//        "refresh_token",
//        "implicit",
//        "password",
//        "urn:ietf:params:oauth:grant-type:device_code",
//        "delegation"
//    ],
//    "response_types_supported": [
//        "code",
//        "token",
//        "id_token",
//        "id_token token",
//        "code id_token",
//        "code token",
//        "code id_token token"
//    ],
//    "response_modes_supported": [
//        "form_post",
//        "query",
//        "fragment"
//    ],
//    "token_endpoint_auth_methods_supported": [
//        "client_secret_basic",
//        "client_secret_post"
//    ],
//    "id_token_signing_alg_values_supported": [
//        "RS256"
//    ],
//    "subject_types_supported": [
//        "public"
//    ],
//    "code_challenge_methods_supported": [
//        "plain",
//        "S256"
//    ],
//    "request_parameter_supported": true
//}