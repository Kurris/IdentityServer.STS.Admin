using System.Collections.Generic;

namespace IdentityServer.STS.Admin.Models.Consent
{
    /// <summary>
    /// 同意屏幕入参
    /// </summary>
    public class ConsentInput
    {
        /// <summary>
        /// 是否允许
        /// </summary>
        public bool Allow { get; set; }

        /// <summary>
        /// 授权的作用域
        /// </summary>
        public IEnumerable<string> ScopesConsented { get; set; }

        /// <summary>
        /// 是否记住同意屏幕的操作
        /// </summary>
        public bool RememberConsent { get; set; } = true;

        /// <summary>
        /// 返回的url
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}