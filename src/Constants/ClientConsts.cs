using System.Collections.Generic;

namespace IdentityServer.STS.Admin.Constants
{
    /// <summary>
    /// 客户端相关常量
    /// </summary>
    public class ClientConst
    {
        /// <summary>
        /// 密钥类型
        /// </summary>
        public static List<string> SecretTypes =>
            new List<string>
            {
                "SharedSecret",
                "X509Thumbprint",
                "X509Name",
                "X509CertificateBase64"
            };

        /// <summary>
        /// 标准claims
        /// <remarks>
        /// http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> StandardClaims =>
            new List<string>
            {
                "name",
                "given_name",
                "family_name",
                "middle_name",
                "nickname",
                "preferred_username",
                "profile",
                "picture",
                "website",
                "gender",
                "birthdate",
                "zoneinfo",
                "locale",
                "address",
                "updated_at"
            };

        /// <summary>
        /// 授权类型
        /// </summary>
        public static IEnumerable<string> GrantTypes =>
            new List<string>
            {
                "implicit",
                "client_credentials",
                "authorization_code",
                "hybrid",
                "password",
                "urn:ietf:params:oauth:grant-type:device_code",
                "delegation"
            };

        /// <summary>
        /// 签名算法
        /// </summary>
        public static List<string> SigningAlgorithms =>
            new List<string>
            {
                "RS256",
                "RS384",
                "RS512",
                "PS256",
                "PS384",
                "PS512",
                "ES256",
                "ES384",
                "ES512"
            };

        /// <summary>
        /// 协议类型
        /// </summary>
        public static IEnumerable<SelectItem<string, string>> ProtocolTypes =>
            new List<SelectItem<string, string>>
            {
                new SelectItem<string, string>("oidc", "OpenID Connect")
            };
    }
}