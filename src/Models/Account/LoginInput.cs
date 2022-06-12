using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    public class LoginInput
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool RememberLogin { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}