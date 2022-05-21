using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    /// <summary>
    /// 2fa登录入参
    /// </summary>
    public class LoginWith2faInput
    {
        /// <summary>
        /// 双重验证码
        /// </summary>
        [Required]
        public string TwoFactorCode { get; set; }


        /// <summary>
        /// 记住当前设备
        /// </summary>
        public bool RememberMachine { get; set; }

        /// <summary>
        /// 记住我(来源登录界面)
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// 重定向地址
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}