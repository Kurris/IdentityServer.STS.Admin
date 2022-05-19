using System.Collections.Generic;

namespace IdentityServer.STS.Admin.Models.Consent
{
    public class ConsentOutput : ConsentInput
    {
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
        public IEnumerable<ScopeOutputModel> IdentityScopes { get; set; }

        /// <summary>
        /// api作用域
        /// </summary>
        public IEnumerable<ScopeOutputModel> ApiScopes { get; set; }
    }
}