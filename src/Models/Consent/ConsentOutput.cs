using System.Collections.Generic;
using IdentityServer.STS.Admin.Models.Account;

namespace IdentityServer.STS.Admin.Models.Consent
{
    public class ConsentOutput : ConsentInput
    {
        /// <summary>
        /// 拥有者
        /// </summary>
        public UserOutput ClientOwner { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 应用链接
        /// </summary>
        public string ClientUrl { get; set; }

        /// <summary>
        /// 应用logo地址
        /// </summary>
        public string ClientLogoUrl { get; set; }

        /// <summary>
        /// 是否允许记住同意屏幕
        /// </summary>
        public bool AllowRememberConsent { get; set; } = true;

        /// <summary>
        /// 身份作用域
        /// </summary>
        public IEnumerable<ScopeOutput> IdentityScopes { get; set; }

        /// <summary>
        /// api作用域
        /// </summary>
        public IEnumerable<ScopeOutput> ApiScopes { get; set; }
    }
}