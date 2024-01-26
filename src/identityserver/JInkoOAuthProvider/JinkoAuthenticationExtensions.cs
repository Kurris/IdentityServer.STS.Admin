/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using System.Diagnostics.CodeAnalysis;
using IdentityServer.STS.Admin.JInkoOAuthProvider;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to add Alipay authentication capabilities to an HTTP application pipeline.
/// </summary>
public static class JinkoAuthenticationExtensions
{
    /// <summary>
    /// Adds <see cref="JinkoAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Alipay authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddJinko([NotNull] this AuthenticationBuilder builder)
    {
        return builder.AddJinko(JinkoAuthenticationDefaults.AuthenticationScheme, options => { });
    }

    /// <summary>
    /// Adds <see cref="JinkoAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Alipay authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configuration">The delegate used to configure the OpenID 2.0 options.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddJinko(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] Action<JinkoAuthenticationOptions> configuration)
    {
        return builder.AddJinko(JinkoAuthenticationDefaults.AuthenticationScheme, configuration);
    }

    /// <summary>
    /// Adds <see cref="JinkoAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Alipay authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the Alipay options.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddJinko(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] string scheme,
        [NotNull] Action<JinkoAuthenticationOptions> configuration)
    {
        return builder.AddJinko(scheme, JinkoAuthenticationDefaults.DisplayName, configuration);
    }

    /// <summary>
    /// Adds <see cref="JinkoAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables Alipay authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme associated with this instance.</param>
    /// <param name="caption">The optional display name associated with this instance.</param>
    /// <param name="configuration">The delegate used to configure the Alipay options.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddJinko(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] string scheme,
        string caption,
        [NotNull] Action<JinkoAuthenticationOptions> configuration)
    {
        return builder.AddOAuth<JinkoAuthenticationOptions, JinkoAuthenticationHandler>(scheme, caption, configuration);
    }
}
