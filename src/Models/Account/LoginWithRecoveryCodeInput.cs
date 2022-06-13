using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    /// <summary>
    /// 恢复码登录入参
    /// </summary>
    public class LoginWithRecoveryCodeInput
    {
        /// <summary>
        /// 恢复码
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        public string RecoveryCode { get; set; }

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