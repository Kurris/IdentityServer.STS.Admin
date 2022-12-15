using System;
using System.Collections.Generic;
using IdentityServer.STS.Admin.Helpers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer.STS.Admin.Seeds.Configuration;

public class ClientSeed : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasData(new List<Client>
        {
            new()
            {
                Id = 1,
                Enabled = true,
                ClientId = Guid.NewGuid().GetShortId(),
                ProtocolType = "oidc",
                ClientSecrets = null,
                RequireClientSecret = false,
                ClientName = "Dashboard",
                Description = "Dashboard",
                ClientUri = "http://localhost:5005",
                LogoUri = null,
                RequireConsent = true,
                AllowRememberConsent = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                // AllowedGrantTypes = new List<ClientGrantType>
                // {
                //     new()
                //     {
                //         Id = 1,
                //         GrantType = GrantTypes.Code.First(),
                //         ClientId = 1,
                //     }
                // },
                RequirePkce = true,
                AllowPlainTextPkce = true,
                RequireRequestObject = false,
                AllowAccessTokensViaBrowser = false,
                // RedirectUris = new List<ClientRedirectUri>
                // {
                //     new()
                //     {
                //         Id = 1,
                //         RedirectUri = "http://localhost:5005/oauth/callback",
                //         ClientId = 1,
                //     }
                // },
                // PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>
                // {
                //     new()
                //     {
                //         Id = 1,
                //         PostLogoutRedirectUri = "http://localhost:5005/oauth/logout",
                //         ClientId = 1,
                //     }
                // },
                FrontChannelLogoutUri = null,
                FrontChannelLogoutSessionRequired = false,
                BackChannelLogoutUri = null,
                BackChannelLogoutSessionRequired = false,
                AllowOfflineAccess = true,
                // AllowedScopes = new List<ClientScope>
                // {
                //     new()
                //     {
                //         Id = 1,
                //         Scope = "openid",
                //         ClientId = 1,
                //     },
                //     new()
                //     {
                //         Id = 2,
                //         Scope = "profile",
                //         ClientId = 1,
                //     }
                // },
                IdentityTokenLifetime = 300,
                AllowedIdentityTokenSigningAlgorithms = null,
                AccessTokenLifetime = 3600,
                AuthorizationCodeLifetime = 300,
                ConsentLifetime = 300,
                AbsoluteRefreshTokenLifetime = 3600,
                SlidingRefreshTokenLifetime = 3600,
                RefreshTokenUsage = (int) TokenUsage.OneTimeOnly,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenExpiration = (int) TokenExpiration.Absolute,
                AccessTokenType = (int) AccessTokenType.Jwt,
                EnableLocalLogin = true,
                IdentityProviderRestrictions = null,
                IncludeJwtId = true,
                Claims = null,
                AlwaysSendClientClaims = false,
                ClientClaimsPrefix = "client_",
                PairWiseSubjectSalt = null,
                // AllowedCorsOrigins = new List<ClientCorsOrigin>
                // {
                //     new ()
                //     {
                //         Id = 1,
                //         Origin = "http://localhost:5005",
                //     }
                // },
                Properties = null,
                Created = DateTime.Now,
                Updated = null,
                LastAccessed = null,
                UserSsoLifetime = null,
                UserCodeType = null,
                DeviceCodeLifetime = 300,
                NonEditable = false
            }
        });
    }
}