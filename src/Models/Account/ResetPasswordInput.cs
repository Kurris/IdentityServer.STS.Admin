using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    /// <summary>
    /// 重置密码入参
    /// </summary>
    public class ResetPasswordInput
    {
        /// <summary>
        /// 邮件
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// 二次确认密码
        /// </summary>
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 邮件验证code
        /// </summary>
        public string Code { get; set; }
    }
}