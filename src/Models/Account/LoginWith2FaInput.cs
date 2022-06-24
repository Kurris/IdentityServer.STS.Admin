using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace IdentityServer.STS.Admin.Models.Account
{
    /// <summary>
    /// 2fa登录入参
    /// </summary>
    public class LoginWith2FaInput
    {
        /// <summary>
        /// 双重验证码
        /// </summary>
        [Required(ErrorMessage = "验证码不能为空")]
        public string TwoFactorCode { get; set; }

        /// <summary>
        /// 是否记住机器
        /// </summary>
        public bool RememberMachine { get; set; }

        /// <summary>
        /// 是否记住我
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// 重定向地址
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 是否关联外部登录
        /// </summary>
        public bool WithExternalLogin { get; set; }
    }
}