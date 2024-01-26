/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System.Security.Claims;
using AspNet.Security.OAuth.Alipay;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace IdentityServer.STS.Admin.JInkoOAuthProvider;

/// <summary>
/// Defines a set of options used by <see cref="JinkoAuthenticationHandler"/>.
/// </summary>
public class JinkoAuthenticationOptions : OAuthOptions
{
    public JinkoAuthenticationOptions()
    {
        ClaimsIssuer = JinkoAuthenticationDefaults.Issuer;
        CallbackPath = JinkoAuthenticationDefaults.CallbackPath;

        AuthorizationEndpoint = JinkoAuthenticationDefaults.AuthorizationEndpoint;
        TokenEndpoint = JinkoAuthenticationDefaults.TokenEndpoint;
        UserInformationEndpoint = JinkoAuthenticationDefaults.UserInformationEndpoint;

        Scope.Add("read");

        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "uid");
    }
}
